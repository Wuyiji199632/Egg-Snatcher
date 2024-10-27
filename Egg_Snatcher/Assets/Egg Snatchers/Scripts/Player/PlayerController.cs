using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    enum PlayerState
    {
       Grounded,
       Air,
    }
    [SerializeField]
    private PlayerState playerState = PlayerState.Grounded;
    [Header("Components")]
    [SerializeField] private PlayerDetection playerDetection;

    [Header("Elements")]
    [SerializeField] private MobileJoystick joystick;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f,jumpSpeed;

    [SerializeField] private BoxCollider groundDetector;

    [SerializeField] private LayerMask groundMask;

    

    private float ySpeed;
    // Start is called before the first frame update
    void Start()
    {
        playerState = PlayerState.Grounded;
    }

    // Update is called once per frame
    void Update()
    {
        MoveHorizontal();

        MoveVertical();
    }

    private void MoveVertical()
    {
        switch (playerState)
        {
            case PlayerState.Grounded:
                MoveVerticalGrounded(); break;

            case PlayerState.Air:
                MoveVerticalAir();break;   
        }
    }

    private void MoveVerticalAir()
    {
        float targetY = transform.position.y + ySpeed * Time.deltaTime;

        Vector3 targetPosition = transform.position.With(y: targetY);


        if(!playerDetection.CanGoThere(targetPosition, out Collider firstCollider))
        {

            float minY = firstCollider.ClosestPoint(transform.position).y;
            //Physics.Raycast(groundDetector.transform.position, Vector3.down, out RaycastHit hit, 1, groundMask);

            if(firstCollider != null)           
                targetPosition.y = minY;
            else
            {
                float maxY = firstCollider.ClosestPoint(transform.position).y;


                //Physics.Raycast(transform.position,Vector3.up,out hit,3f,groundMask);

                //float maxY = hit.point.y;
                targetPosition.y = maxY - playerDetection.capsuleCollider.height*1.89f;
                ySpeed = 0;
                Debug.Log("hit ceiling");
            }
            Debug.Log("unable to traverse");
            //transform.position = targetPosition;
            //Land(); return;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
            Debug.Log("falling");
        }


        transform.position = targetPosition;
        
        if (playerDetection.IsGrounded())        
            Land();
        
    }

    private void Land()
    {
        playerState = PlayerState.Grounded;
        ySpeed = 0;
    }

    private void StartFalling()
    {
        playerState = PlayerState.Air;
    }

    private void MoveVerticalGrounded()
    {
        if (!playerDetection.IsGrounded())
        {
            StartFalling(); return;
        }
    }

    private void MoveHorizontal()
    {
        Vector2 moveVector = joystick.GetMoveVector();
        moveVector.x *= moveSpeed;

        float targetX = transform.position.x + moveVector.x * Time.deltaTime;
        Vector2 targetPosition = transform.position.With(x: targetX);

        if (playerDetection.CanGoThere(targetPosition, out Collider firstCollider))
        {
            transform.position = targetPosition;
        }
        
    }

    public void Jump()
    {
        playerState = PlayerState.Air;
        ySpeed = jumpSpeed;
    }
}
