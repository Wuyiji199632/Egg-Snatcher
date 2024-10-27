using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerDetection : MonoBehaviour
{
    [Header("Components")]
    private PlayerController playerController;


    [Header("Elements")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask trampolineMask;
    [SerializeField] private BoxCollider boxCollider;
    public CapsuleCollider capsuleCollider;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        DetectTrampolines();
    }
    /*With box collider*/
    //public bool CanGoThere(Vector3 targetPosition)
    //{

    //    Vector3 center = targetPosition + boxCollider.center;

    //    Collider[] colliders = Physics.OverlapBox(center, boxCollider.bounds.extents / 2,Quaternion.identity,groundMask);

    //    foreach(Collider col in colliders)
    //    {
    //        Debug.Log(col.name);
    //    }

    //    return colliders.Length <= 0;


    //}

    public bool CanGoThere(Vector3 targetPosition/*, out Collider firstCollider*/)
    {
        Vector3 center = targetPosition + capsuleCollider.center;

        float halfHeight = (capsuleCollider.height / 2)-capsuleCollider.radius;
        Vector3 offset = transform.up * halfHeight;
        Vector3 point0 = center + offset;
        Vector3 point1 = center - offset;

       

        Collider[] colliders = Physics.OverlapCapsule(point0,point1,capsuleCollider.radius,groundMask);

     
        //if (colliders.Length > 0)
        //    firstCollider = colliders[0];
        //else
        //    firstCollider = null;


        return colliders.Length <= 0 || playerController.hitCeiling;

    }

    public bool IsGrounded()
    {
        return Physics.OverlapBox(boxCollider.transform.position, boxCollider.size / 2, Quaternion.identity, groundMask).Length>0;
    }
   
    private void DetectTrampolines()
    {
        if (!IsGrounded()) return;

        if(Physics.OverlapBox(boxCollider.transform.position, boxCollider.size / 2, Quaternion.identity, trampolineMask).Length > 0)
        {
            playerController.Jump();
        }


    }
}
