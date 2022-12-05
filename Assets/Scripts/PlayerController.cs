using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerController : MonoBehaviour
{
    #region Character Parameters
    [Header("Movement")]
    [SerializeField] private float acceleration;
    [SerializeField] private float maxGroundVelocity;
    [SerializeField] private float maxAirVelocity;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float playerHeight;
    [SerializeField] private float rotationSpeed;
    private Vector3 moveDirection;
    private bool canJump;


    [Header("Flying")]
    [SerializeField] private float throttleIncrement;
    [SerializeField] private float throttleMax;
    [SerializeField] private float maxThrust;
    [SerializeField] private float responsiveness;
    private bool isFlying;
    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;
    private float slide;
    private float upThrottle;
    private float responseModifier => (rb.mass / 10f) * responsiveness;

    [Header("Combat")]
    private bool isInCombatMode;


    #endregion

    #region References
    [Header("References")]
    private Rigidbody rb;
    private PlayerInputController inputController;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private LayerMask groundLayer;

    #endregion
    #region Inputs
    private float horizontalInput;
    private float verticalInput;
    private bool jumpInput;
    #endregion


    public float Velocity => rb.velocity.magnitude;
    public bool IsFlying => isFlying;
    public bool IsInCombatMode => isInCombatMode;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputController = GetComponent<PlayerInputController>();
    }

    void Update()
    {
        InputCheck();
        SpeedControl();
        RotationCheck();
    }
    private void FixedUpdate()
    {
        MoveCheck();
        JumpCheck();
        FlyingCheck();

    }

    private void InputCheck()
    {
        horizontalInput = inputController.GetHorizontalRawInput();
        verticalInput = inputController.GetVerticallRawInput();
        jumpInput = inputController.IsJumpButtonPressed();
        roll = Input.GetAxis("Roll");
        pitch = -Input.GetAxis("Mouse Y");
        yaw = Input.GetAxis("Mouse X");
        slide = Input.GetAxis("Yaw");
       




        if (inputController.IsFlyingForwardHeld() && isFlying)
        {
            throttle += throttleIncrement;
        }
        else if(inputController.IsFlyingBrakeHeld() && isFlying)
        {
            throttle -= throttleIncrement;
        }
        else
        {
            throttle -= throttleIncrement / 10;
        }
        throttle = Mathf.Clamp(throttle, 0, throttleMax);

        if (inputController.IsJumpButtonHeld() && isFlying)
        {
            upThrottle += throttleIncrement * 10;
        }
        else
        {
            upThrottle -= throttleIncrement * 10;
        }
        upThrottle = Mathf.Clamp(upThrottle, 0, throttleMax);



        if (jumpInput && IsGrounded())
        {
            canJump = true;
        }

        if (jumpInput && !IsGrounded())
        {
            isFlying = true;

        }
        else if (IsGrounded())
        {
            isFlying = false;
            throttle = 0;
        }

    }

    private void FlyingCheck()
    {
        if (isFlying)
        {
            rb.AddForce((transform.forward * maxThrust * throttle * Time.deltaTime) * 50);
            rb.AddTorque((transform.up * yaw * responseModifier * Time.deltaTime) * 30);
            rb.AddTorque((transform.right * pitch * responseModifier * Time.deltaTime) * 30);
            rb.AddTorque((-transform.forward * roll * responseModifier * Time.deltaTime) * 50);

            rb.AddForce((transform.right * slide * 5000 )* Time.deltaTime);
            rb.AddForce((transform.up * upThrottle * maxThrust * Time.deltaTime) * 20);

        }
    }


    private void SpeedControl()
    {
        if (IsGrounded())
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 1;
        }
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
    }
    private void MoveCheck()
    {
        if (IsGrounded())
        {
            moveDirection = new Vector3(horizontalInput, 0, verticalInput);
            moveDirection = Quaternion.AngleAxis(mainCamera.rotation.eulerAngles.y, Vector3.up) * moveDirection;

            if (rb.velocity.magnitude <= maxGroundVelocity)
            {
                //rb.velocity += moveDirection.normalized * acceleration * Time.deltaTime;
                rb.AddForce(moveDirection.normalized * acceleration, ForceMode.Force);
            }

        }
    }
    private void RotationCheck()
    {
        if (moveDirection != Vector3.zero && !isFlying)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDirection.normalized, Time.deltaTime * rotationSpeed);
        }
    }
    private void JumpCheck()
    {
        if (canJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            canJump = false;
        }
    }

}
