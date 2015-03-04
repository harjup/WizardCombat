﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using ModestTree;

public class MovableBox : MonoBehaviourBase
{
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Transform _initialParent;
    private GameObject _boxConfetti;

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _initialParent = transform.parent;

        // Forgive me, o great lords of dependency injection
        // TODO: Do the DI 
        _boxConfetti = Resources.Load<GameObject>("OrbCollect/BoxBreak");
        Assert.IsNotNull(_boxConfetti);
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

    public void PoofAway()
    {
        transform.DOScale(Vector3.one * .1f, 1f).SetEase(Ease.OutExpo);
        Instantiate(_boxConfetti, transform.position, Quaternion.identity);
        Destroy(gameObject, 1.5f);
    }
}