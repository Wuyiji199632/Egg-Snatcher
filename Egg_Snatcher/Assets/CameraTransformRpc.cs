using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraTransformRpc : NetworkBehaviour
{
    [SerializeField] private Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().transform;
        }
        transform.position = new Vector3(target.position.x, target.position.y, -2.69f);
    }
}
