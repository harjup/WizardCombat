using System;
using UnityEngine;
using System.Collections;

public class PickupHooks : MonoBehaviour
{
    public event Action<Collider> TriggerEnter = delegate { };
    public SpriteRenderer SpriteRenderer;
    
    public void OnTriggerEnter(Collider other)
    {
        TriggerEnter(other);
    }

    public void KillUrSelf()
    {
        Destroy(gameObject);
    }
}
