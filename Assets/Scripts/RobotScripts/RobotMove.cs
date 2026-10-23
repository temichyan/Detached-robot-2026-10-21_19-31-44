using UnityEngine;
using UnityEngine.InputSystem;

public class RobotMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Robot")]
    [SerializeField] private Transform robotModel;

    [Header("Camera")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float mouseSensitivity = 200f;
    private InputAction moveAction;

    private Rigidbody rb;

    private float verticalVelocity;
    private float cameraRotationX;

    private bool isGrounded;

    private void Awake()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Update()
    {
        HandleCamera();

        // Проверка земли
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            2f
        );

        // Прыжок
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Получаем ввод WASD из Input Actions
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        // Преобразуем ввод в направление относительно робота
        Vector3 movement =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        // Не учитываем вертикальное направление объекта
        movement.y = 0f;
        movement.Normalize();

        // Обработка гравитации
        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity +=
            Physics.gravity.y * Time.fixedDeltaTime;

        // Ограничиваем скорость падения
        verticalVelocity = Mathf.Max(
            verticalVelocity,
            -maxFallSpeed
        );

        // Движение Rigidbody
        rb.linearVelocity = new Vector3(
            movement.x * speed,
            verticalVelocity,
            movement.z * speed
        );

        // Поворачиваем модель в направлении движения
        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(movement);

            robotModel.rotation = Quaternion.Slerp(
                robotModel.rotation,
                targetRotation,
                10f * Time.fixedDeltaTime
            );
        }
    }

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
}