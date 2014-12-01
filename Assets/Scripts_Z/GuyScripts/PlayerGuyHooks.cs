using System;
using UnityEngine;
using System.Collections;

public class PlayerGuyHooks : MonoBehaviour {

    public event Action PickupEvent = delegate { };
    public Rigidbody Rigidbody;

    private PlayerHandsCollider _playerHandsCollider;
    public PlayerHandsCollider PlayerHandsCollider 
    {
        get
        {
            return _playerHandsCollider ?? (_playerHandsCollider = GetComponentInChildren<PlayerHandsCollider>()); 
        }
    }

    public void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        
        PlayerHandsCollider.PickupEvent += PickupEvent;
    }

}
