using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class RobotMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 1.1f; // от центра до ног + небольшой запас
    [SerializeField] private LayerMask groundMask = ~0;        // выбери слой земли, не включай слой робота

    [Header("Robot")]
    [SerializeField] private Transform robotModel;
    [SerializeField] private float modelTurnSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float mouseSensitivity = 200f;

    private InputAction moveAction;
    private Rigidbody rb;

    private Vector3 moveDirection;
    private float cameraRotationX;
    private bool isGrounded;
    private bool jumpRequested;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        moveAction = GetComponent<PlayerInput>().actions["Move"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleCamera();
        RotateModel();

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
        HandleMovement();
        HandleJump();
        LimitFallSpeed();
    }

    private void CheckGround()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void HandleMovement()
    {
        Vector2 input = Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1f);

        // Направление относительно поворота тела
        moveDirection = transform.right * input.x + transform.forward * input.y;
        moveDirection.y = 0f;

        // Меняем только X и Z, вертикаль (гравитацию) оставляем физике
        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection.x * speed;
        velocity.z = moveDirection.z * speed;
        rb.linearVelocity = velocity;
    }

    private void HandleJump()
    {
        if (jumpRequested && isGrounded)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = jumpForce;
            rb.linearVelocity = velocity;
        }

        jumpRequested = false;
    }

    private void LimitFallSpeed()
    {
        Vector3 velocity = rb.linearVelocity;

        if (velocity.y < -maxFallSpeed)
        {
            velocity.y = -maxFallSpeed;
            rb.linearVelocity = velocity;
        }
    }

    // Камера: точно как в первоначальном коде
    private void HandleCamera()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x;
        float mouseY = mouseDelta.y;

        transform.Rotate(
            0f,
            mouseX * mouseSensitivity * Time.deltaTime,
            0f
        );

        cameraRotationX -=
            mouseY * mouseSensitivity * Time.deltaTime;

        cameraRotationX = Mathf.Clamp(
            cameraRotationX,
            -80f,
            80f
        );

        cameraPivot.localRotation =
            Quaternion.Euler(
                cameraRotationX,
                0f,
                0f
            );
    }

    private void RotateModel()
    {
        if (moveDirection.sqrMagnitude < 0.01f)
            return;

        Quaternion target = Quaternion.LookRotation(moveDirection);

        robotModel.rotation = Quaternion.Slerp(
            robotModel.rotation,
            target,
            modelTurnSpeed * Time.deltaTime
        );
    }
}