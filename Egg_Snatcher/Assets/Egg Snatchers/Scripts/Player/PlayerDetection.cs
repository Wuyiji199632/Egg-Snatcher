using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerDetection : NetworkBehaviour
{
    [Header("Components")]
    private PlayerController playerController;


    [Header("Elements")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask trampolineMask;
    [SerializeField] private LayerMask eggMask;
    [SerializeField] private BoxCollider boxCollider;
    public CapsuleCollider capsuleCollider;
    public bool IsHoldingEgg { get; private set; }

    [Header("Settings")]
    public float eggGrabbedHeight = 2.5f;
    public  bool canStealEgg;

    // Start is called before the first frame update
    void Start()
    {
        canStealEgg = true;
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        DetectTrampolines(); if (IsServer) DetectEggs();
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
   
    private void DetectEggs()
    {

        if(!canStealEgg||IsHoldingEgg)
            return;

        Collider[] detectedEggs = DetectColliders(transform.position, eggMask, out Collider egg);

        if (egg == null)
            return;
        Debug.Log("Egg detected");

        if (egg.transform.parent == null)
            GrabEgg(egg);
        else if (egg.transform.parent.TryGetComponent(out PlayerDetection playerDetection))
            StealEggFrom(playerDetection,egg);
        
    }

    private void StealEggFrom(PlayerDetection otherPlayer, Collider egg)
    {
        GrabEgg(egg);
        otherPlayer.LoseEgg();
    }
    private void LoseEgg()
    {
        IsHoldingEgg = false;
        canStealEgg = false;

        StartCoroutine(LoseEggCoroutine());
    }
    IEnumerator LoseEggCoroutine()
    {
        yield return new WaitForSecondsRealtime(1);
        canStealEgg = true;
    }
    private void GrabEgg(Collider egg)
    {
        egg.transform.SetParent(transform);
        egg.transform.localPosition = Vector3.up * eggGrabbedHeight;
        IsHoldingEgg = true;

    }

    private Collider[] DetectColliders(Vector3 position,LayerMask mask,out Collider firstCollider)
    {
        Vector3 center = position + capsuleCollider.center;

        float halfHeight = (capsuleCollider.height / 2) - capsuleCollider.radius;
        Vector3 offset = transform.up * halfHeight;
        Vector3 point0 = center + offset;
        Vector3 point1 = center - offset;



        Collider[] colliders = Physics.OverlapCapsule(point0, point1, capsuleCollider.radius, mask);


        if (colliders.Length > 0)
            firstCollider = colliders[0];
        else
            firstCollider = null;


        return colliders;
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
