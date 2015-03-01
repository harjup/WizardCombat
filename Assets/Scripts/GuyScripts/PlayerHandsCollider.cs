using System;
using UnityEngine;
using System.Collections;

public class PlayerHandsCollider : MonoBehaviour {

    public event Action<Collider> TriggerEnter = delegate { };
    public event Action PickupEvent = delegate { };

    public void OnTriggerEnter(Collider other)
    {
        TriggerEnter(other);
    }

    public void GetPickup()
    {
        PickupEvent();
    }
}
