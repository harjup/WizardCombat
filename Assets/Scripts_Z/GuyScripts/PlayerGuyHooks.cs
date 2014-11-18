using System;
using UnityEngine;
using System.Collections;

public class PlayerGuyHooks : MonoBehaviour {

    public event Action<Collider> TriggerEnter = delegate { };

    public Rigidbody Rigidbody;

    public void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void OnTriggerEnter(Collider other)
    {
        TriggerEnter(other);
    }
}
