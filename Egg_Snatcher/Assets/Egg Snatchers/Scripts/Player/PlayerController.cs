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

    [SerializeField] private BoxCollider groundDetector,ceilingDetector;

    [SerializeField] private LayerMask groundMask;

    public bool hitCeiling = false;

    RaycastHit ceilingHit;
    public  float xSpeed { get; private set; }
    private float ySpeed;

    [Header("Actions")]
    public Action OnJumpStarted,OnFallStarted,OnLandStarted;
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
        
       

        if (!playerDetection.CanGoThere(targetPosition/*out Collider firstCollider)*/))
        {

           
            Physics.Raycast(groundDetector.transform.position, Vector3.down, out RaycastHit hit, 1, groundMask);

            if (hit.collider != null)           
                targetPosition.y = hit.point.y;


            Physics.Raycast(transform.position + new Vector3(0, 2, 0), Vector3.up, out ceilingHit, 3f, groundMask);
            Debug.DrawRay(transform.position + new Vector3(0, 2, 0), Vector3.up * 3f, Color.red);
            hitCeiling = ceilingHit.collider != null;
            transform.position = targetPosition;
            Land();

            return;
        }
        else
        {
            if (hitCeiling)         
                StartCoroutine(ResetBool());
            
            transform.position = targetPosition;
            ySpeed += Physics.gravity.y * Time.deltaTime;

            if (playerDetection.IsGrounded())
                Land();

        }



    }
    IEnumerator ResetBool()
    {
        yield return new WaitForSeconds(1f);
        hitCeiling = false;
    }
    private void Land()
    {
        playerState = PlayerState.Grounded;
        ySpeed = 0;
        OnLandStarted?.Invoke();
    }

    private void StartFalling()
    {
        playerState = PlayerState.Air;
        
        OnFallStarted?.Invoke();
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
        xSpeed = Mathf.Abs(moveVector.x);
        ManageFacing(moveVector.x);
        moveVector.x *= moveSpeed;
        
        float targetX = transform.position.x + moveVector.x * Time.deltaTime;
        Vector2 targetPosition = transform.position.With(x: targetX);

        if (playerDetection.CanGoThere(targetPosition))
        {
            transform.position = targetPosition;
        }
        
    }
    private void ManageFacing(float xSpeed)
    {

        float facing = xSpeed != 0? Mathf.Sign(xSpeed): transform.localScale.x;

        transform.localScale = transform.localScale.With(x: facing);
    }
    public void Jump()
    {
        playerState = PlayerState.Air;
        ySpeed = jumpSpeed;
        OnJumpStarted?.Invoke();
    }
}
