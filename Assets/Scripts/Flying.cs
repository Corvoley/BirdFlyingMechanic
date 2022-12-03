using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying : MonoBehaviour
{
    [SerializeField] private float throttleIncrement;
    [SerializeField] private float maxThrottle;
    [SerializeField] private float responsiveness;
    
    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;

    private Rigidbody rb;
    private float responseModifier => (rb.mass / 10f) * responsiveness;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void InputsCheck()
    {
        roll = Input.GetAxis("Roll");
        pitch = -Input.GetAxis("Mouse Y");
        yaw = Input.GetAxis("Mouse X");

        if (Input.GetKey(KeyCode.Space))
        {
            throttle += throttleIncrement;
        }
        else
        {
            throttle -= throttleIncrement/10;
        }
        throttle = Mathf.Clamp(throttle, 0, 100);
    }
    private void Update()
    {
        InputsCheck();
    }


    private void FixedUpdate()
    {
        rb.AddForce(transform.forward * maxThrottle * throttle);
        rb.AddTorque(transform.up * yaw * responseModifier);
        rb.AddTorque(transform.right * pitch * responseModifier);
        rb.AddTorque(-transform.forward * roll * responseModifier);
       
    }


}
