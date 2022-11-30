using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerController : MonoBehaviour
{
    #region Character Parameters
    [SerializeField] private float acceleration;
    [SerializeField] private float maxGroundVelocity;
    [SerializeField] private float maxAirVelocity;
    [SerializeField] private float maxDescendVelocity;
    [SerializeField] private float airHorizontalSlow;
    [SerializeField] private float airVerticalSlow;    
    [SerializeField] private float airHorizontalVelocityMult;
    [SerializeField] private float airVerticalVelocityMult;
    [SerializeField] private float gravityForce;


    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float playerHeight;



    [SerializeField] private float flyingForwardAccelerationForce;
    [SerializeField] private float flyingGravityAccelerationForce;
    [SerializeField] private float flyingUpwardMultiplier;
    [SerializeField] private float rotationSpeed;
    #endregion

    #region References
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

    private Vector3 moveDirection;
    private bool canJump;
    private bool isFlying;
    private bool isInCombatMode;
    private bool isAccelerating;
    private float currentFlyingForwardForce;
    private float currentFlyingGravityForce;
    private float currentVertMult;
    private float currentHorizontalMult;

    public float Velocity => rb.velocity.magnitude;
    public bool IsFlying => isFlying;
    public bool IsInCombatMode => isInCombatMode;
    public float CurrentFlyingForwardForce => currentFlyingForwardForce;
    public float CurrentFlyingUpwardForce => currentFlyingGravityForce;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        inputController = GetComponent<PlayerInputController>();
    }

    void Update()
    {
        InputCheck();
        SpeedControl();
        RotationCheck();
        FlyingMultiplierByCameraXAngleCheck();
        FlyingAccelerationCheck();
    }
    private void FixedUpdate()
    {
        MoveCheck();
        JumpCheck();
        FlyingCheck();
        if (!isFlying)
        {
            GravityCheck();
        }
    }

    private void InputCheck()
    {
        horizontalInput = inputController.GetHorizontalRawInput();
        verticalInput = inputController.GetVerticallRawInput();
        jumpInput = inputController.IsButtonPressed();

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
            currentFlyingForwardForce = 0f;
        }

        if (inputController.IsFlyingForwardHeld())
        {
            isAccelerating = true;
        }
        else
        {
            isAccelerating = false;
        }
    }
    private void FlyingAccelerationCheck()
    {
        
        if (isAccelerating)
        {
            if (currentFlyingForwardForce < maxAirVelocity)
            {
                currentFlyingForwardForce += (flyingForwardAccelerationForce - currentHorizontalMult) * Time.deltaTime;
                
            }
            if (currentFlyingGravityForce < 10f)
            {

                currentFlyingGravityForce += (flyingGravityAccelerationForce + currentVertMult * 2) * Time.deltaTime;
            }
        }
        else
        {
            if (currentFlyingForwardForce > 0 && currentFlyingForwardForce <= maxDescendVelocity)
            {
                currentFlyingForwardForce -= (airHorizontalSlow + currentHorizontalMult) * Time.deltaTime;
            }
            else if (currentFlyingForwardForce > 0)
            {
                currentFlyingForwardForce -= airHorizontalSlow * Time.deltaTime;
            }
            if (currentFlyingGravityForce > -maxDescendVelocity)
            {
                currentFlyingGravityForce = (AirVerticalSlowValueByForwardSpeed(airVerticalSlow) + currentVertMult );
                Debug.Log(AirVerticalSlowValueByForwardSpeed(airVerticalSlow));
            }
        }

        
    }

    private void FlyingCheck()
    {
        if (isFlying && inputController.IsFlyingForwardHeld())
        {
            moveDirection = mainCamera.forward;
            Vector3 flyingForce = new Vector3(transform.forward.x * currentFlyingForwardForce, currentFlyingGravityForce, transform.forward.z * currentFlyingForwardForce);

            rb.velocity = flyingForce;

        }
        else if (isFlying)
        {
            moveDirection = mainCamera.forward;
            Vector3 flyingForce = new Vector3(transform.forward.x * currentFlyingForwardForce, currentFlyingGravityForce, transform.forward.z * currentFlyingForwardForce);
            rb.velocity = flyingForce;
        }

    }

    private float AirVerticalSlowValueByForwardSpeed(float airVerticalSlow)
    {
        var percent = Mathf.InverseLerp(0, maxDescendVelocity, currentFlyingForwardForce);
        return Mathf.Lerp(-airVerticalSlow, 0, percent);
    }
    private void FlyingMultiplierByCameraXAngleCheck()
    {
        var a = mainCamera.rotation.eulerAngles.x;
        if (a > 180)
        {
            a -= 360;
        }
        var percent = Mathf.InverseLerp(-50f, 60f, a);
       
        currentVertMult = Mathf.Lerp(airVerticalVelocityMult, -airVerticalVelocityMult, percent);
        currentHorizontalMult = Mathf.Lerp(airHorizontalVelocityMult, -airHorizontalVelocityMult, percent);
                
    }
    private void SpeedControl()
    {
        if (IsGrounded())
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
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
                rb.velocity += moveDirection.normalized * acceleration * Time.deltaTime;
            }

        }
    }
    private void RotationCheck()
    {
        if (moveDirection != Vector3.zero)
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

    private void GravityCheck()
    {
        float gravity = -gravityForce * Time.deltaTime;
        rb.velocity += new Vector3(0, gravity, 0);
    }

}
