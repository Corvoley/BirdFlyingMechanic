using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public bool IsAimingButtonHeld()
    {
        return Input.GetKey(KeyCode.Mouse1);
    }
    public bool IsFlyingForwardHeld()
    {
        return Input.GetKey(KeyCode.W);
    }
    public bool IsFlyingBrakeHeld()
    {
        return Input.GetKey(KeyCode.S);
    }

    public float GetHorizontalRawInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }
    public float GetVerticallRawInput()
    {
        return Input.GetAxisRaw("Vertical");
    }
    public float GetHorizontalInput()
    {
        return Input.GetAxis("Horizontal");
    }
    public float GetVerticallInput()
    {
        return Input.GetAxis("Vertical");
    }

    public bool IsJumpButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
    public bool IsJumpButtonHeld()
    {
        return Input.GetKey(KeyCode.Space);
    }

}
