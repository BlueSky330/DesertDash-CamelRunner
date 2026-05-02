using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float laneChangeSpeed = 10f;
    public float jumpForce = 10f;
    public float slideDuration = 1f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private int currentLane = 1; // 0: left, 1: middle, 2: right
    private float targetLaneX;
    private bool isJumping = false;
    private bool isSliding = false;
    private float slideTimer;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        targetLaneX = transform.position.x; // Start in current lane
    }

    void Update()
    {
        // Handle lane change input
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > 0)
        {
            currentLane--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentLane < 2)
        {
            currentLane++;
        }

        // Calculate target X position for the current lane
        if (currentLane == 0) targetLaneX = -2f; // Example left lane X
        else if (currentLane == 1) targetLaneX = 0f; // Example middle lane X
        else if (currentLane == 2) targetLaneX = 2f; // Example right lane X

        // Smoothly move to the target lane
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(newPosition.x, targetLaneX, Time.deltaTime * laneChangeSpeed);
        transform.position = newPosition;

        // Handle jump input
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isJumping && !isSliding)
        {
            isJumping = true;
            moveDirection.y = jumpForce;
        }

        // Handle slide input
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isSliding && !isJumping)
        {
            isSliding = true;
            slideTimer = slideDuration;
            // Optionally, reduce collider height here
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                isSliding = false;
                // Optionally, restore collider height here
            }
        }

        // Apply gravity
        if (characterController.isGrounded)
        {
            moveDirection.y = -0.5f; // Small downward force to keep grounded
            if (isJumping) isJumping = false;
        }
        else
        {
            moveDirection.y -= 20f * Time.deltaTime; // Gravity force
        }

        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);
    }

    // Placeholder for collision detection
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Hit obstacle!");
            // Trigger game over or take damage
        }
    }
}
