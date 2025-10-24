using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Look Settings")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    public float maxLookAngle = 85f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    public float currentMoveSpeed = 0f;        
    public float currentRotationSpeed = 0f;

    private Vector3 previousPosition;
    private float previousYaw;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
        MeasureSpeeds();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded)
        {
            velocity.y = -2f; // keep grounded
            if (Input.GetButtonDown("Jump"))
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    void MeasureSpeeds()
    {
        Vector3 currentPosition = transform.position;

        // Ignore Y for horizontal movement speed
        Vector3 flatCurrent = new Vector3(currentPosition.x, 0f, currentPosition.z);
        Vector3 flatPrevious = new Vector3(previousPosition.x, 0f, previousPosition.z);
        float distance = Vector3.Distance(flatCurrent, flatPrevious);
        currentMoveSpeed = distance / Time.deltaTime;

        // Small dead-zone for jitter at rest
        if (currentMoveSpeed < 0.05f)
            currentMoveSpeed = 0f;

        previousPosition = currentPosition;

        // --- ROTATION SPEED ---
        float currentYaw = transform.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(previousYaw, currentYaw);
        currentRotationSpeed = Mathf.Abs(deltaYaw) / Time.deltaTime;

        previousYaw = currentYaw;
    }
}
