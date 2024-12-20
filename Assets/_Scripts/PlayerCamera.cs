using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Settings")]
    [Range(0, 1000)]
    [SerializeField] private float sensX;
    [Range(0, 1000)]
    [SerializeField] private float sensY;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform transformation;

    private float rotationX;
    private float rotationY;

    #region Init
    void Start()
    {
        InitCursor();
    }

    private void InitCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    #region Run
    void Update()
    {
        GetRotation();
        ApplyRotation();
        ApplyPosition();
    }

    private void GetRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
    }

    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientation.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    private void ApplyPosition()
    {
        transform.position = transformation.position;
    }
    #endregion
}