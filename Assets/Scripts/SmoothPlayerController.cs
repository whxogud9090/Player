using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public sealed class SmoothPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5.5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float deceleration = 22f;
    [SerializeField] private float rotationSmoothTime = 0.08f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2.1f;
    [SerializeField] private float gravity = -24f;
    [SerializeField] private float groundedStickForce = -3f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.24f;
    [SerializeField] private LayerMask groundLayers = ~0;

    [Header("References")]
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float rotationVelocity;
    private bool isGrounded;

    public Vector3 Velocity => horizontalVelocity + Vector3.up * verticalVelocity;
    public bool IsGrounded => isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraRoot == null && Camera.main != null)
        {
            cameraRoot = Camera.main.transform;
        }

        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        if (groundCheck == null)
        {
            var check = new GameObject("GroundCheck");
            check.transform.SetParent(transform);
            check.transform.localPosition = new Vector3(0f, -0.9f, 0f);
            groundCheck = check.transform;
        }
    }

    private void Update()
    {
        UpdateGroundedState();
        Move();
        UpdateAnimator();
    }

    private void UpdateGroundedState()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayers,
            QueryTriggerInteraction.Ignore);

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedStickForce;
        }
    }

    private void Move()
    {
        Vector2 input = ReadMoveInput();
        Vector3 desiredDirection = GetCameraRelativeDirection(input);
        bool sprinting = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        float targetSpeed = sprinting ? sprintSpeed : walkSpeed;
        Vector3 targetVelocity = desiredDirection * targetSpeed;

        float rate = targetVelocity.sqrMagnitude > 0.01f ? acceleration : deceleration;
        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity,
            targetVelocity,
            rate * Time.deltaTime);

        if (desiredDirection.sqrMagnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.z) * Mathf.Rad2Deg;
            float smoothedAngle = Mathf.SmoothDampAngle(
                visualRoot.eulerAngles.y,
                targetAngle,
                ref rotationVelocity,
                rotationSmoothTime);

            visualRoot.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
        }

        if (WantsJump() && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 motion = (horizontalVelocity + Vector3.up * verticalVelocity) * Time.deltaTime;
        controller.Move(motion);
    }

    private Vector2 ReadMoveInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                input.y += 1f;
            }
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                input.y -= 1f;
            }
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                input.x += 1f;
            }
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                input.x -= 1f;
            }
        }

        if (Gamepad.current != null)
        {
            input += Gamepad.current.leftStick.ReadValue();
        }

        return Vector2.ClampMagnitude(input, 1f);
    }

    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        if (input.sqrMagnitude < 0.001f)
        {
            return Vector3.zero;
        }

        Vector3 forward = cameraRoot != null ? cameraRoot.forward : Vector3.forward;
        Vector3 right = cameraRoot != null ? cameraRoot.right : Vector3.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return (forward * input.y + right * input.x).normalized;
    }

    private static bool WantsJump()
    {
        bool keyboardJump = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool gamepadJump = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;
        return keyboardJump || gamepadJump;
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        Vector3 flatVelocity = horizontalVelocity;
        flatVelocity.y = 0f;
        animator.SetFloat("Speed", flatVelocity.magnitude);
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
