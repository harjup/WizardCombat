using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;
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
        _playerGuyHooks.PlayerHandsCollider.TriggerEnter += EvaluateTriggerCollision;
    }

    public void Tick()
    {
        if (_dashRoutine == null)
        {
            Walk();
        }
        else
        {
            DashTick();
        }
    }

    public void Walk()
    {
        var movementDirection = _inputHelper.GetInputDirection();
        LookTowardsDirection(movementDirection);

        var velocity = movementDirection * 10f;
        SetRigidbodyVelocity(velocity);

        SetSimpleAnimation(velocity);

        //TODO: Use events for inputs
        ReadInput(velocity);
    }

    private IEnumerator _dashRoutine;
    public void StartDash()
    {
        _dashRoutine = _timerFactory.CreateTimer(.5f);
        _asyncTaskProcessor.Process(_dashRoutine, () =>
        {
            _dashRoutine = null;
        });
    }

    public void DashTick()
    {
        var movementDirection = Transform.forward;
        LookTowardsDirection(movementDirection);

        var velocity = movementDirection * 20f;
        SetRigidbodyVelocity(velocity);

        SetSimpleAnimation(velocity);
    }

    private void EvaluateTriggerCollision(Collider collider)
    {
        // We only care about this when dashing
        if (_dashRoutine == null)
        {
            return;
        }

        var boxTransform = collider.transform;
        var movableBox = collider.GetComponent<MovableBox>();

        if (movableBox != null && boxTransform.parent != Transform)
        {
            movableBox.DisablePhysics();
            boxTransform.SetParent(Transform);

            boxTransform
                .DOLocalMove(Vector3.zero.SetY(1f), .5f)
                .SetEase(Ease.OutCirc);
        }
    }

    public void ReadInput(Vector3 velocity)
    {
        if (InputManager.Instance.InteractAction)
        {
            var box = Transform.GetComponentInChildren<MovableBox>();
            if (box != null)
            {
                DisablePickUpColliderForHalfSecond();
                ThrowBox(box, velocity);
            }
            else
            {
                StartDash();
            }
        }
    }

    public void ThrowBox(MovableBox box, Vector3 velocity)
    {
        var throwPower = 5f;
        var throwVector = (Transform.forward + Transform.up) * throwPower;
        var boxVelocity = throwVector + velocity;

        box.ResetParent();
        box.EnablePhysics(boxVelocity);
    }

    //TODO: Persist better
    private IEnumerator _timer;
    public void DisablePickUpColliderForHalfSecond()
    {
        if (_timer != null)
        {
            _asyncTaskProcessor.Cancel(_timer);
        }

        _playerGuyHooks.PlayerHandsCollider.collider.enabled = false;
        _timer = _timerFactory.CreateTimer(.5f);
        _asyncTaskProcessor.Process(_timer, () =>
        {
            _playerGuyHooks.PlayerHandsCollider.collider.enabled = true;
            _timer = null;
        });
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
