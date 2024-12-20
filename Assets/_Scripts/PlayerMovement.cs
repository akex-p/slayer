using System;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float groundDamping;
    [SerializeField] private float wallDamping;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;

    [Header("Other Settings")]
    [SerializeField] private float playerHeight;
    [SerializeField] private float maxSlopeAngle;

    [Header("Input Settings")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform playerCamera;

    [Header("Debug")]
    [SerializeField] private bool showGrounded = false;
    [SerializeField] private bool showPlayerState = false;
    [SerializeField] private bool showMoveDirection = false;
    [SerializeField] private bool showJumpReady = false;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    private float moveSpeed;
    private float startYScale;

    private RaycastHit slobHit;

    private bool grounded;
    private bool jumpReady = true;

    private MovementState movementState = MovementState.IDLE;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    #region Init
    void Start()
    {
        // Init RB
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Init height
        startYScale = transform.localScale.y;

        // Init Collider
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    #endregion

    #region Run
    void Update()
    {
        // Handle Ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight + 0.2f, groundLayer);

        // Apply drag
        if (grounded) rb.linearDamping = groundDamping;
        else rb.linearDamping = 0f;

        DebugAll();
    }

    void FixedUpdate()
    {
        StateHandler();
        GetInput();
        PlayerMove();
        CheckPlayerSpeed();
    }

    private void StateHandler()
    {
        if (grounded)
        {
            if (Input.GetKey(sprintKey))
            {
                movementState = MovementState.SPRINTING;
                moveSpeed = sprintSpeed;
            }
            else if (Input.GetKey(crouchKey))
            {
                movementState = MovementState.CROUCHING;
                moveSpeed = crouchSpeed;
            }
            else if (rb.linearVelocity.magnitude > 0f)
            {
                movementState = MovementState.WALKING;
                moveSpeed = walkSpeed;
            }
            else
            {
                movementState = MovementState.IDLE;
            }
        }
        else
        {
            movementState = MovementState.AIRBORNE;
        }
    }

    private void GetInput()
    {
        // Move
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump
        if (Input.GetKey(jumpKey) && jumpReady && grounded) PlayerJump();

        // Crouch
        if (Input.GetKey(crouchKey)) PlayerCrouch(true);
        else PlayerCrouch(false);
    }

    private void PlayerMove()
    {
        moveDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;

        if (OnSlob()) rb.AddForce(GetSlobMoveDirection() * moveSpeed * 10f, ForceMode.Force);
        if (grounded) rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded) rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void CheckPlayerSpeed()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void PlayerJump()
    {
        jumpReady = false;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        jumpReady = true;
    }

    private void PlayerCrouch(bool crouching)
    {
        if (crouching)
        {
            capsuleCollider.height = 1f;
        }
        else
        {
            capsuleCollider.height = 2f;
        }
    }

    private bool OnSlob()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slobHit, playerHeight + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slobHit.normal);
            return angle < maxSlopeAngle && angle != 0f;
        }
        return false;
    }

    private Vector3 GetSlobMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slobHit.normal).normalized;
    }
    #endregion

    #region Debug
    private void DebugAll()
    {
        if (showGrounded) Debug.Log("Player grounded: " + grounded);
        if (showJumpReady) Debug.Log("Player Jump ready: " + jumpReady);
        if (showPlayerState) Debug.Log("Current PlayerState: " + movementState);
        if (showMoveDirection) Debug.Log("Player Direction: " + moveDirection);
    }
    #endregion
}

