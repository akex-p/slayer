using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("General Settings")]
    [Range(0, 1000)]
    [SerializeField] private float sensX;
    [Range(0, 1000)]
    [SerializeField] private float sensY;

    [Header("FOV Settings")]
    [SerializeField] private float maxFOVIncrease = 10f;
    [SerializeField] private float speedAtMaxFOV = 10f;
    [SerializeField] private float fovLerpSpeed = 5f;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private PlayerMovement playerMovement;

    private Camera cam;
    private float defaultFOV;
    private float rotationX;
    private float rotationY;

    #region Init
    void Start()
    {
        InitCursor();
        InitFOV();
    }

    private void InitCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void InitFOV()
    {
        cam = GetComponent<Camera>();
        defaultFOV = cam.fieldOfView;
    }
    #endregion

    #region Run
    void Update()
    {
        HandleInput();
        HandleRotation();
        HandlePosition();
        HandleFOV();
    }

    private void HandleInput()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
    }

    private void HandleRotation()
    {
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientation.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    private void HandlePosition()
    {
        transform.position = cameraPosition.position;
    }

    private void HandleFOV()
    {
        if (playerMovement == null) return;

        float speed01 = Mathf.Clamp01(playerMovement.currentVelocity.magnitude / speedAtMaxFOV); // Normalize speed
        float targetFOV = defaultFOV + (speed01 * maxFOVIncrease);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
    }
    #endregion
}