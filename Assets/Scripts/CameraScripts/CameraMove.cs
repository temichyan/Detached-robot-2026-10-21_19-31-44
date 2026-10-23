using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 200f;
    private float verticalRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x;
        float mouseY = mouseDelta.y;

        // Вверх / вниз
        verticalRotation -= mouseY * mouseSensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(
            verticalRotation,
            -80f,
            80f
        );

        transform.localRotation = Quaternion.Euler(
            verticalRotation,
            0f,
            0f
        );
    }
}
