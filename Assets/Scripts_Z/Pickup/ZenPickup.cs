using ModestTree.Zenject;
using UnityEngine;
using System.Collections;

public class ZenPickup : IInitializable
{
    private PickupHooks _pickupHooks;
    private ParallelAsyncTaskProcessor _parallelAsyncTaskProcessor;
    private bool _pickupAllowed = true;
    private TimerFactory _timerFactory = new TimerFactory();

    public ZenPickup(PickupHooks pickupHooks, ParallelAsyncTaskProcessor parallelAsyncTaskProcessor)
    {
        _pickupHooks = pickupHooks;
        _parallelAsyncTaskProcessor = parallelAsyncTaskProcessor;
        pickupHooks.TriggerEnter += OnTriggerEnter;
        
    }

    public void Initialize()
    {
        //_parallelAsyncTaskProcessor.Process(SpawnCooldown());
    }


    IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        _pickupAllowed = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_pickupAllowed) return;


        var userReceiver = other.GetComponent<PlayerGuyHooks>();
        if (userReceiver != null)
        {
            userReceiver.GetPickup();

            _parallelAsyncTaskProcessor.Process(Despawn());
            _pickupAllowed = false;
        }
    }

    IEnumerator Despawn()
    {
        _pickupHooks.SpriteRenderer.enabled = false;
        _pickupAllowed = false;
        yield return _timerFactory.CreateTimer(1f);
        _pickupHooks.KillUrSelf();
    }
}
