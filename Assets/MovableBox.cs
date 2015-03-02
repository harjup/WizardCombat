using UnityEngine;
using System.Collections;

public class MovableBox : MonoBehaviourBase
{
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Transform _initialParent;


    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _initialParent = transform.parent;
    }

    public void ResetParent()
    {
        transform.SetParent(_initialParent);
    }

    public void EnablePhysics(Vector3 initialVelocity)
    {
        _rigidbody.isKinematic = false;
        _collider.enabled = true;
        _rigidbody.velocity = initialVelocity;
    }

    public void DisablePhysics()
    {
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
    }
}