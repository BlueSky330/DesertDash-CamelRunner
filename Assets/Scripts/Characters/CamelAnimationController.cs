using UnityEngine;

/// <summary>
/// Drives procedural animations for the Camel character.
///
/// Works with CamelAnimatorController to apply runtime transformations:
///   - Running bob: gentle Y sine wave
///   - Slide: Y scale to 0.5, restore on exit
///   - Hit: brief red material flash
///   - Jump: physics handled by PlayerController; anim just signals pose
///
/// No keyframe animations needed — all motion via script + Animator state machine.
/// </summary>
[RequireComponent(typeof(Animator))]
public class CamelAnimationController : MonoBehaviour
{
    [Header("Running Animation")]
    [SerializeField] private float bobAmplitude = 0.05f;  // Y wobble range (m)
    [SerializeField] private float bobFrequency = 4f;     // cycles per second

    [Header("Slide/Duck")]
    [SerializeField] private float slideScaleY = 0.5f;    // Y compression when sliding
    [SerializeField] private float slideTransitionSpeed = 5f;

    [Header("Hit Reaction")]
    [SerializeField] private Color hitFlashColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.2f;

    private Animator _animator;
    private Vector3 _baseScale;
    private Vector3 _basePosition;
    private Material _material;
    private Color _originalColor;
    private float _hitFlashTimer;

    // Cached animator parameter hashes
    private int _isRunningHash;
    private int _jumpHash;
    private int _slideHash;
    private int _hitHash;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _baseScale = transform.localScale;
        _basePosition = transform.localPosition;

        // Cache parameter hashes for efficiency
        _isRunningHash = Animator.StringToHash("IsRunning");
        _jumpHash = Animator.StringToHash("Jump");
        _slideHash = Animator.StringToHash("Slide");
        _hitHash = Animator.StringToHash("Hit");

        // Get material for hit flash
        var meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            _material = meshRenderer.material;
            _originalColor = _material.color;
        }
    }

    void Update()
    {
        bool isRunning = _animator.GetBool(_isRunningHash);

        // ── Running Bob ────────────────────────────────────────────────────────
        if (isRunning)
        {
            float bobOffset = Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f) * bobAmplitude;
            var pos = _basePosition;
            pos.y += bobOffset;
            transform.localPosition = pos;
        }
        else
        {
            // Idle: return to base position
            transform.localPosition = Vector3.Lerp(transform.localPosition, _basePosition, Time.deltaTime * 5f);
        }

        // ── Slide/Duck ─────────────────────────────────────────────────────────
        // Check animator's current state; if sliding, compress Y
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        bool isSliding = stateInfo.shortNameHash == Animator.StringToHash("Slide");

        float targetScaleY = isSliding ? slideScaleY : 1f;
        var newScale = _baseScale;
        newScale.y = Mathf.Lerp(_baseScale.y, targetScaleY * _baseScale.y, Time.deltaTime * slideTransitionSpeed);
        transform.localScale = newScale;

        // ── Hit Flash ──────────────────────────────────────────────────────────
        if (_hitFlashTimer > 0f)
        {
            _hitFlashTimer -= Time.deltaTime;
            float lerpFactor = 1f - (_hitFlashTimer / hitFlashDuration);
            _material.color = Color.Lerp(hitFlashColor, _originalColor, lerpFactor);
        }
        else if (_material.color != _originalColor)
        {
            _material.color = _originalColor;
        }
    }

    /// <summary>
    /// Called by external systems (e.g., PlayerController) to trigger hit reaction.
    /// </summary>
    public void OnHit()
    {
        _hitFlashTimer = hitFlashDuration;
        _animator.SetTrigger(_hitHash);
    }

    /// <summary>
    /// Called by external systems to set running state.
    /// </summary>
    public void SetRunning(bool running)
    {
        _animator.SetBool(_isRunningHash, running);
    }

    /// <summary>
    /// Called by external systems to trigger jump.
    /// </summary>
    public void Jump()
    {
        _animator.SetTrigger(_jumpHash);
    }

    /// <summary>
    /// Called by external systems to trigger slide.
    /// </summary>
    public void Slide()
    {
        _animator.SetTrigger(_slideHash);
    }
}
