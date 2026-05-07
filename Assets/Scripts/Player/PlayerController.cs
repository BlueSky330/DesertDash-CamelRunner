using UnityEngine;
using System.Collections;

/// <summary>
/// Controls camel movement: lane changes, jump, slide.
/// Supports mobile swipe input with keyboard fallback.
///
/// Spec:
///   - 3 lanes at X = -laneWidth, 0, +laneWidth
///   - Swipe threshold: 30 px
///   - Lane-switch cooldown: 200 ms
///   - Jump cooldown: 400 ms
///   - Slow-down applied when HealthSystem < 25%
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    // ── Inspector ──────────────────────────────────────────────────────────
    [Header("Lane Settings")]
    public float laneWidth      = 2f;   // distance between lane centres
    public float laneChangeSpeed = 12f; // lateral lerp speed

    [Header("Jump Settings")]
    public float jumpForce   = 12f;
    public float gravity     = 22f;

    [Header("Slide Settings")]
    public float slideDuration = 0.8f;

    [Header("Speed")]
    public float forwardSpeed = 10f; // driven externally by DifficultyManager

    [Header("Low-health slow")]
    [Range(0f, 1f)]
    public float lowHealthSpeedMultiplier = 0.6f; // applied below 25% health

    // ── Cooldowns (ms → s) ─────────────────────────────────────────────────
    private const float LANE_COOLDOWN = 0.2f;
    private const float JUMP_COOLDOWN = 0.4f;

    // ── State ──────────────────────────────────────────────────────────────
    private CharacterController       cc;
    private Animator                  anim;
    private CamelAnimationController  _camelAnim;

    private int   currentLane   = 1; // 0 left | 1 centre | 2 right
    private float targetX;
    private float verticalVelocity;
    private bool  isJumping;
    private bool  isSliding;

    private float laneCooldownTimer;
    private float jumpCooldownTimer;

    // ── Touch tracking ─────────────────────────────────────────────────────
    private Vector2 touchStart;
    private bool    trackingTouch;
    private const float SWIPE_THRESHOLD_PX = 30f;

    // ── Animator hashes ────────────────────────────────────────────────────
    private static readonly int AnimIsRunning = Animator.StringToHash("IsRunning");
    private static readonly int AnimJump      = Animator.StringToHash("Jump");
    private static readonly int AnimSlide     = Animator.StringToHash("Slide");
    private static readonly int AnimHit       = Animator.StringToHash("Hit");

    // ── Unity lifecycle ───────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        cc        = GetComponent<CharacterController>();
        anim      = GetComponent<Animator>();
        _camelAnim = GetComponentInChildren<CamelAnimationController>();

        currentLane = 1;
        targetX     = LaneX(currentLane);
        Vector3 pos = transform.position;
        pos.x = targetX;
        transform.position = pos;

        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOver    += OnGameStopped;
        GameManager.OnGameReset   += OnGameStopped;

        // Subscribe to skin changes
        SkinManager.onSkinEquipped += OnSkinEquipped;

        anim.SetBool(AnimIsRunning, false);
    }

    void OnDestroy()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOver    -= OnGameStopped;
        GameManager.OnGameReset   -= OnGameStopped;
        SkinManager.onSkinEquipped -= OnSkinEquipped;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameManager.GameState.Running)
            return;

        laneCooldownTimer -= Time.deltaTime;
        jumpCooldownTimer -= Time.deltaTime;

        ProcessInput();
        MoveCharacter();
    }

    // ── Input ──────────────────────────────────────────────────────────────

    private void ProcessInput()
    {
        // Touch input (primary on mobile)
        ProcessTouchInput();

        // Keyboard fallback for editor / development
        if (Input.GetKeyDown(KeyCode.LeftArrow))  TryChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TryChangeLane(+1);
        if (Input.GetKeyDown(KeyCode.UpArrow))    TryJump();
        if (Input.GetKeyDown(KeyCode.DownArrow))  TrySlide();
    }

    private void ProcessTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStart    = touch.position;
                trackingTouch = true;
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (!trackingTouch) break;
                trackingTouch = false;

                Vector2 delta = touch.position - touchStart;

                // Only register if displacement exceeds threshold
                if (delta.magnitude < SWIPE_THRESHOLD_PX) break;

                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    // Horizontal swipe
                    TryChangeLane(delta.x > 0 ? +1 : -1);
                }
                else
                {
                    // Vertical swipe
                    if (delta.y > 0) TryJump();
                    else             TrySlide();
                }
                break;
        }
    }

    // ── Actions ────────────────────────────────────────────────────────────

    public void TryChangeLane(int direction)
    {
        if (laneCooldownTimer > 0f) return;
        int target = currentLane + direction;
        if (target < 0 || target > 2) return;

        currentLane       = target;
        targetX           = LaneX(currentLane);
        laneCooldownTimer = LANE_COOLDOWN;
    }

    public void TryJump()
    {
        if (isSliding || isJumping) return;
        if (jumpCooldownTimer > 0f) return;
        if (!cc.isGrounded) return;

        isJumping         = true;
        verticalVelocity  = jumpForce;
        jumpCooldownTimer = JUMP_COOLDOWN;
        anim.SetTrigger(AnimJump);
        GameAudioEvents.OnPlayJump?.Invoke();
    }

    public void TrySlide()
    {
        if (isJumping || isSliding) return;
        StartCoroutine(SlideRoutine());
    }

    // Public aliases used by GameplayTester simulation
    public void SwipeLeft()  => TryChangeLane(-1);
    public void SwipeRight() => TryChangeLane(+1);
    public void Jump()  => TryJump();
    public void Slide() => TrySlide();

    // ── Movement ───────────────────────────────────────────────────────────

    private void MoveCharacter()
    {
        float speedMult = (HealthSystem.Instance != null && HealthSystem.Instance.IsLowHealth())
            ? lowHealthSpeedMultiplier
            : 1f;

        // Forward movement — speed driven by DifficultyManager when available
        float fwd = (DifficultyManager.Instance != null)
            ? DifficultyManager.Instance.GetCurrentSpeed()
            : forwardSpeed;
        fwd *= speedMult;

        // Lateral smoothing
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * laneChangeSpeed);
        transform.position = pos;

        // Vertical physics
        if (cc.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1f; // keep grounded
            if (isJumping) isJumping = false;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 motion = new Vector3(0f, verticalVelocity, fwd);
        cc.Move(motion * Time.deltaTime);
    }

    // ── Slide coroutine ───────────────────────────────────────────────────

    private IEnumerator SlideRoutine()
    {
        isSliding = true;
        anim.SetTrigger(AnimSlide);
        GameAudioEvents.OnPlaySlide?.Invoke();

        // Shrink collider centre/height so slide clears low obstacles
        if (cc != null)
        {
            cc.height = 0.9f;
            cc.center = new Vector3(0f, 0.45f, 0f);
        }

        yield return new WaitForSeconds(slideDuration);

        // Restore collider
        if (cc != null)
        {
            cc.height = 1.8f;
            cc.center = new Vector3(0f, 0.9f, 0f);
        }
        isSliding = false;
    }

    // ── Collision ─────────────────────────────────────────────────────────

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.gameObject.CompareTag("Obstacle")) return;

        // If Scarab Shell (one-collision shield) is active, absorb and return
        if (PowerUpManager.Instance != null && PowerUpManager.Instance.HasScarabShield())
        {
            PowerUpManager.Instance.ConsumeScarabShield();
            return;
        }

        // If Magic Carpet (invincibility) active, ignore
        if (PowerUpManager.Instance != null && PowerUpManager.Instance.isMagicCarpetActive)
            return;

        // Route through CamelAnimationController when present so hit-flash fires.
        // CamelAnimationController.OnHit() sets the Animator trigger internally.
        if (_camelAnim != null)
            _camelAnim.OnHit();
        else
            anim.SetTrigger(AnimHit);

        if (HealthSystem.Instance != null)
            HealthSystem.Instance.TakeDamage(25f);

        GameAudioEvents.OnPlayCollision?.Invoke();
    }

    // ── Reset ─────────────────────────────────────────────────────────────

    public void ResetPosition()
    {
        StopAllCoroutines();
        currentLane      = 1;
        targetX          = LaneX(1);
        verticalVelocity = 0f;
        isJumping        = false;
        isSliding        = false;
        laneCooldownTimer = 0f;
        jumpCooldownTimer = 0f;

        // Restore collider defaults
        if (cc != null) { cc.height = 1.8f; cc.center = new Vector3(0f, 0.9f, 0f); }

        Vector3 pos = transform.position;
        pos.x = targetX;
        transform.position = pos;

        anim.SetBool(AnimIsRunning, false);
    }

    // ── Skin Swapping ──────────────────────────────────────────────────────

    private void OnSkinEquipped(string skinName)
    {
        ApplySkin(skinName);
    }

    /// <summary>Apply a skin to the player model by updating renderers with skin materials.</summary>
    public void ApplySkin(string skinName)
    {
        // Get all child renderers and update their materials
        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning($"[PlayerController] No renderers found on player model for skin '{skinName}'");
            return;
        }

        // Try to load skin template from resources
        string templateName = SanitizeSkinName(skinName);
        SkinMaterialTemplate template = Resources.Load<SkinMaterialTemplate>($"Skins/SkinTemplate_{templateName}");

        if (template != null)
        {
            Material skinMat = template.GetDefaultMaterial();
            if (skinMat != null)
            {
                // Apply material to all renderers
                foreach (var renderer in renderers)
                {
                    if (renderer != null)
                        renderer.material = skinMat;
                }
                Debug.Log($"[PlayerController] Applied skin '{skinName}' with material");
            }
            else
            {
                Debug.LogWarning($"[PlayerController] No default material found in template for '{skinName}'");
            }
        }
        else
        {
            Debug.LogWarning($"[PlayerController] Skin template not found for '{skinName}'. Run Tools > Camel Runner > Setup Skin Materials");
        }
    }

    /// <summary>Sanitize skin name to match template file naming.</summary>
    private string SanitizeSkinName(string name)
    {
        return name.Replace(" ", "_").Replace("(", "").Replace(")", "");
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private float LaneX(int lane)
    {
        // Lane 0 = -laneWidth, Lane 1 = 0, Lane 2 = +laneWidth
        return (lane - 1) * laneWidth;
    }

    private void OnGameStarted() => anim.SetBool(AnimIsRunning, true);
    private void OnGameStopped() => anim.SetBool(AnimIsRunning, false);
}
