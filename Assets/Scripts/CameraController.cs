using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;    
    [SerializeField] private Transform playerTransform;    
    [SerializeField] private PlayerInputController playerInputController;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float combatRotationSpeed;


    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private CinemachineVirtualCamera combatCamera;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }


    private void Update()
    {
        if (playerInputController.IsAimingButtonHeld())
        {         
            freeLookCamera.enabled = false;
        }
        else
        {           
            freeLookCamera.enabled = true;
        }

        

    }




}
