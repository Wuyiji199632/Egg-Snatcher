using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    [Header("Elements")]
    [SerializeField] private Animator animator;

    [Header("Components")]
    private PlayerController playerController;


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        playerController.OnJumpStarted += Jump;

        playerController.OnFallStarted += Fall;

        playerController.OnLandStarted += Land;
    }

    private void OnDestroy()
    {
        playerController.OnJumpStarted -= Jump;

        playerController.OnFallStarted -= Fall;

        playerController.OnLandStarted -= Land;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateBlendTree();
    }

    private void UpdateBlendTree()
    {
        animator.SetFloat("xSpeed", playerController.xSpeed);
    }

    public void Jump()
    {
        animator.Play("Jump");
    }

    public void Fall()
    {
        animator.Play("Fall");
    }

    public void Land()
    {
        animator.Play("Land");
    }
}