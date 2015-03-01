using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Managers;
using DG.Tweening;
using ModestTree.Zenject;

public interface IPlayerGuy
{
    Transform Transform { get; }
}

public class SimplePlayer : ITickable, IInitializable, IPlayerGuy
{

    private readonly PlayerGuyHooks _playerGuyHooks;
    private readonly ParallelAsyncTaskProcessor _asyncTaskProcessor;
    private readonly TimerFactory _timerFactory;
    private readonly Camera _camera;
    private readonly DebugGuiHooks _debugGuiHooks;

    private readonly InputHelper _inputHelper;

    public SimplePlayer(
        PlayerGuyHooks playerGuyHooks, 
        ParallelAsyncTaskProcessor asyncTaskProcessor,
        CameraManager cameraManager, 
        DebugGuiHooks debugGuiHooks)
    {
        _playerGuyHooks = playerGuyHooks;
        _asyncTaskProcessor = asyncTaskProcessor;
        _camera = cameraManager.Main;

        _playerGuyHooks.PlayerHandsCollider.PickupEvent += () => Log.Debug("Got a pickup");

        _debugGuiHooks = debugGuiHooks;

        _timerFactory = new TimerFactory();

        _inputHelper = new InputHelper(InputManager.Instance, _camera);
    }

    // Use this for initialization
    public Transform Transform
    {
        get { return _playerGuyHooks.transform; }
    }

    public Rigidbody Rigidbody
    {
        get { return _playerGuyHooks.Rigidbody; }
    }

    public void Initialize()
    {

    }

    public void Tick()
    {
        var movementDirection = _inputHelper.GetInputDirection();
        LookTowardsDirection(movementDirection);

        var velocity = movementDirection * 10f;
        SetRigidbodyVelocity(velocity);

        SetSimpleAnimation(velocity);
    }

    //TODO: Store state elsewhere
    private bool direction;
    public void SetSimpleAnimation(Vector3 velocity)
    {
        var meshEulerAngles = _playerGuyHooks.CubeMesh.transform.localEulerAngles;
        
        if (velocity.magnitude > .1f)
        {
            if (direction)
            {
                _playerGuyHooks.CubeMesh.transform.DOLocalRotate(Vector3.zero.SetZ(5), .1f);
            }
            else
            {
                _playerGuyHooks.CubeMesh.transform.DOLocalRotate(Vector3.zero.SetZ(-5), .1f);
            }

            if (Math.Abs(meshEulerAngles.z - 355) < .05)
            {
                direction = true;
            }
            else if (Math.Abs(meshEulerAngles.z - 5) < .05)
            {
                direction = false;
            }
        }
        else
        {
            _playerGuyHooks.CubeMesh.transform.DOLocalRotate(Vector3.zero.SetZ(0), 0f);
        }
    }
    
    public void LookTowardsDirection(Vector3 direction)
    {
        if (Math.Abs(InputManager.Instance.RawHoritzontalAxis) >= 1
            || Math.Abs(InputManager.Instance.RawVerticalAxis) >= 1)
        {
            Transform.rotation = Quaternion.LookRotation(direction.SetY(0), Vector3.up);
        }            
    }

    public void SetRigidbodyVelocity(Vector3 velocity)
    {
        Rigidbody.velocity = Rigidbody.velocity
                                    .SetX(velocity.x)
                                    .SetZ(velocity.z);
    }
}
