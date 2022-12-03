using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{      
    [SerializeField] private PlayerController playerControllerController;

    [SerializeField] private float groundRotationSpeed;
    [SerializeField] private float flyingRotationSpeed;


    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private CinemachineVirtualCamera flyingCamera;
    

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }


    private void Update()
    {
        if (playerControllerController.IsFlying)
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = flyingRotationSpeed;
            freeLookCamera.enabled = false;
            flyingCamera.enabled = true;
        }
        else
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = groundRotationSpeed;
            freeLookCamera.enabled = true;
            flyingCamera.enabled = false;
        }

        

    }




}
