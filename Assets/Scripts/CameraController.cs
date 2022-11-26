using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;    
    [SerializeField] private Transform player;    
    [SerializeField] private InputController inputController;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float combatRotationSpeed;


    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private CinemachineVirtualCamera combatCamera;

    Vector3 viewDir;
    Vector3 combatViewDir;
    Vector3 inputDir;
    private CameraStyle currentCameraStyle;
    private enum CameraStyle
    {
        Basic,
        Combat,
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }


    private void Update()
    {
        if (inputController.IsAimingButtonHeld())
        {
            currentCameraStyle = CameraStyle.Combat;
            freeLookCamera.enabled = false;
        }
        else
        {
            currentCameraStyle = CameraStyle.Basic;
            freeLookCamera.enabled = true;
        }

        

    }




}
