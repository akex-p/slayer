using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float dashSpeed;

    [Header("Dash Settings")]
    [SerializeField] private float dashStopDistance = 0.05f;
    [SerializeField] private float launchBackForce;

    [Header("Lerp Settings")]
    [SerializeField] private float sprintSmoothing;
    [SerializeField] private float movementSmoothing;

    [Header("Jump Settings")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpHeight;

    [Header("References")]
    [SerializeField] private Transform orientation;

    private CharacterController controller;

    private float horizontalInput;
    private float verticalInput;
    private float verticalVelocity;
    private float currentSpeed;
    private Vector3 currentMoveDirection;
    private Vector3? moveTarget = null;
    private bool isDashing = false;
    private Vector3 externalForce = Vector3.zero;

    public Vector3 currentVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!isDashing)
        {
            HandleInput();
            HandleMovement();
        }
        else
        {
            HandleDash();
        }
    }

    private void HandleInput()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) & controller.isGrounded)
        {
            ApplyForce(Vector3.up * Mathf.Sqrt(jumpHeight * gravity * 2f));
        }
    }

    private void HandleMovement()
    {
        // get direction and lerp
        Vector3 targetDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;
        currentMoveDirection = Vector3.Lerp(currentMoveDirection, targetDirection, movementSmoothing * Time.deltaTime);

        // sprinting
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, sprintSmoothing * Time.deltaTime);

        // horizontal movement
        currentVelocity = currentMoveDirection * currentSpeed;

        // vertical movement
        verticalVelocity = CalculateGravity();
        currentVelocity.y = verticalVelocity;

        // Apply external force
        currentVelocity += externalForce;
        externalForce = Vector3.zero; // reset after applying

        // move
        controller.Move(currentVelocity * Time.deltaTime);
    }

    private float CalculateGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0) verticalVelocity = -1f;
        else verticalVelocity -= gravity * Time.deltaTime;

        return verticalVelocity;
    }

    public void ApplyForce(Vector3 force)
    {
        externalForce += new Vector3(force.x, 0f, force.z);
        verticalVelocity = force.y;
    }

    private void HandleDash()
    {
        if (!moveTarget.HasValue)
        {
            isDashing = false;
            return;
        }

        Vector3 toTarget = moveTarget.Value - transform.position;

        if (toTarget.magnitude <= dashStopDistance)
        {
            Vector3 bounce = (transform.position - moveTarget.Value).normalized * 5f;
            bounce.y = 4f;
            bounce *= launchBackForce;
            ApplyForce(bounce);

            isDashing = false;
            moveTarget = null;

            return;
        }

        Vector3 dashDirection = toTarget.normalized;
        Vector3 velocity = dashDirection * dashSpeed;

        controller.Move(velocity * Time.deltaTime);
    }

    public void MoveToPosition(Vector3 targetPosition)
    {
        moveTarget = targetPosition;
        isDashing = true;
    }
}
