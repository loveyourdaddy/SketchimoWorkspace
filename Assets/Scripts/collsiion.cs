using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collsiion : MonoBehaviour
{
    public BoxCollider box;
    // Start is called before the first frame update
    void Start()
    {
        // var box = GetComponent<BoxCollider>();
        // Vector3 center = box.center;
        // Vector3 size = box.size;

        // Vector3 maxBound = box.bounds.max;
        // Vector3 minBound = box.bounds.min;

        //find vertex index 

        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("start");
    }

    void OnCollisionEnter(Collision collision)
    {
        var a = collision.contacts;
        Debug.Log("collision detection");
    }

    void OnTriggerEnter(Collider collider)
    {
        // var a = collision.contacts;
        Debug.Log("collision detection");
    }
}
