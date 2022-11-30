using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    private Animator animator;
    private PlayerController characterController;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<PlayerController>();
    }


    private void Update()
    {
        if (characterController.IsFlying)
        {
            animator.SetBool("IsFlying", true);
        }
        else
        {
            animator.SetBool("IsFlying", false);
        }
    }



}
