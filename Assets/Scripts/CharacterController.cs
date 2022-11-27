using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController))]
public class CharacterController : MonoBehaviour
{
    #region Character Parameters
    [SerializeField] private float acceleration;
    [SerializeField] private float maxGroundVelocity;
    [SerializeField] private float maxAirVelocity;
    [SerializeField] private float maxDescendVelocity;
    [SerializeField] private float airHorizontalSlow;
    [SerializeField] private float airVerticalSlow;
    [SerializeField] private float airVerticalSlowMult;
    [SerializeField] private float airHorizontalVelocityMult;
    [SerializeField] private float airVerticalVelocityMult;
    [SerializeField] private float gravityForce;


    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float playerHeight;



    [SerializeField] private float flyingForwardForce;
    [SerializeField] private float flyingUpwardForce;
    [SerializeField] private float flyingUpwardMultiplier;
    [SerializeField] private float rotationSpeed;
    #endregion

    #region References
    private Rigidbody rb;
    private InputController inputController;
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
    private float currentFlyingForwardForce;
    private float currentFlyingUpwardForce;
    private float currenAirSlowMult;
    private float currentVertMult;


    public float Velocity => rb.velocity.magnitude;
    public bool IsFlying => isFlying;
    public float CurrentFlyingForwardForce => currentFlyingForwardForce;
    public float CurrentFlyingUpwardForce => currentFlyingUpwardForce;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        inputController = GetComponent<InputController>();
    }

    private void Start()
    {
        currenAirSlowMult = airVerticalSlowMult;
    }

    void Update()
    {
        InputCheck();
        SpeedControl();
        RotationCheck();
        FlyingMultiplierCheck();


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
            currentFlyingUpwardForce = 0f;
        }
        if (inputController.IsFlyingForwardHeld())
        {
            if (currentFlyingForwardForce < maxAirVelocity)
            {
                currentFlyingForwardForce += flyingForwardForce * Time.deltaTime;

            }
            if (currentFlyingUpwardForce <= maxAirVelocity && currentFlyingUpwardForce >= -maxAirVelocity)
            {
               
                if (currentVertMult < 0)
                {
                    currentFlyingUpwardForce += flyingUpwardForce * Time.deltaTime * 10;
                }
                else if( currentVertMult > 0)
                {
                    currentFlyingUpwardForce -= flyingUpwardForce * Time.deltaTime * 10;
                }
            }

        }
        else
        {
            if (currentVertMult > 0)
            {
                if (currentFlyingForwardForce < maxDescendVelocity)
                {
                    currentFlyingForwardForce += currentVertMult * Time.deltaTime * 10;

                }
                if (currentFlyingUpwardForce > -maxDescendVelocity)
                {
                    currentFlyingUpwardForce -= currentVertMult * Time.deltaTime * 10;
                }

            }
            else if (currentVertMult < 0)
            {
                if (currentFlyingForwardForce < 1)
                {
                    currentFlyingForwardForce -= -currentVertMult * Time.deltaTime * 10;

                }
                if (currentFlyingForwardForce > 1 && currentFlyingUpwardForce <= maxAirVelocity)
                {
                    currentFlyingUpwardForce -= -currentVertMult * Time.deltaTime * 10;
                }

            }



        }

    }

    private void FlyingCheck()
    {
        if (isFlying && inputController.IsFlyingForwardHeld())
        {
            moveDirection = mainCamera.forward;
            Vector3 flyingForce = new Vector3(transform.forward.x * currentFlyingForwardForce, currentFlyingUpwardForce, transform.forward.z * currentFlyingForwardForce);

            //criar uma velocidade constante enquanto boa que aumenta ou diminue de acordo com o angulo em relacao ao chao 

            rb.velocity = flyingForce;

        }
        else if (isFlying)
        {
            moveDirection = mainCamera.forward;
            Vector3 flyingForce = new Vector3(transform.forward.x * currentFlyingForwardForce, currentFlyingUpwardForce, transform.forward.z * currentFlyingForwardForce);
            rb.velocity = flyingForce;

        }


    }
    private void FlyingMultiplierCheck()
    {
        var a = mainCamera.rotation.eulerAngles.x;
        if (a > 180)
        {
            a -= 360;
        }
        var percent = Mathf.InverseLerp(-50f, 60f, a);
        currentVertMult = Mathf.Lerp(-airVerticalVelocityMult, airVerticalVelocityMult, percent);
        Debug.Log(currentVertMult);
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


            // rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        }
    }
    private void RotationCheck()
    {
        if (moveDirection != Vector3.zero)
        {
            // var toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
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
