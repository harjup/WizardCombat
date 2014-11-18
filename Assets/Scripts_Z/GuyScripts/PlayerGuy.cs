using System;
using Assets.Scripts.Managers;
using ModestTree.Zenject;
using UnityEngine;
using System.Collections;

public class PlayerGuy : ITickable, IInitializable
{
    private readonly PlayerGuyHooks _playerGuyHooks;
    private readonly ParallelAsyncTaskProcessor _asyncTaskProcessor;
    private readonly TimerFactory _timerFactory;
    private readonly Camera _camera;

    private const float MaxSpeed = 10f;
    private IEnumerator _timerRoutine = null;
    private int _speedLevel = 1;
    private const int MaxSpeedLevel = 5;
    
    public PlayerGuy(PlayerGuyHooks playerGuyHooks, ParallelAsyncTaskProcessor asyncTaskProcessor,
        CameraManager cameraManager)
    {
        _playerGuyHooks = playerGuyHooks;
        _asyncTaskProcessor = asyncTaskProcessor;
        _camera = cameraManager.Main;

        _playerGuyHooks.PickupEvent += () => Log.Debug("Got a pickup");
        _timerFactory = new TimerFactory();
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
        
    }

    public void Tick()
    {
        MovePlayer();
    }

    public void RotateTo(Vector3 rotation)
    {
        Transform.rotation = Quaternion.Euler(Transform.eulerAngles.SetY(rotation.y));
    }
    public void LookAt(Vector3 vector3)
    {
        Transform.LookAt(vector3.SetY(Transform.position.y));
    }
    public Vector3 Forward
    {
        get { return Transform.forward; }
    }

    Vector3 GetMovementDirection()
    {
        var forward = Vector3.forward;
        //Create the reference axis based on the camera rotation, ignoring y rotation
        //We're getting the main camera, which should be the one that's enabled. It's null if disabled so don't do anything if so
        if (_camera != null)
        {
            forward = _camera.transform.TransformDirection(Vector3.forward).SetY(0).normalized;
        }

        var right = new Vector3(forward.z, 0.0f, -forward.x);

        //Set the player's walk direction
        Vector3 walkVector = (
            InputManager.Instance.HoritzontalAxis * right
            + InputManager.Instance.VerticalAxis * forward
            );

        //prevent the player from moving faster when walking diagonally
        if (walkVector.sqrMagnitude > 1f)
            walkVector = walkVector.normalized;

        //Rotate the player to face direction of movement only when input keys are pressed
        if (Math.Abs(InputManager.Instance.RawHoritzontalAxis) >= 1
            || Math.Abs(InputManager.Instance.RawVerticalAxis) >= 1)
            Transform.rotation = Quaternion.LookRotation(walkVector.SetY(0), Vector3.up);

        return walkVector;
    }

    void MovePlayer()
    {
        if (_timerRoutine == null && _speedLevel < MaxSpeedLevel)
        {
            _timerRoutine = _timerFactory.CreateTimer(_speedLevel + 1);
            _asyncTaskProcessor.Process(_timerRoutine, () =>
            {
                _speedLevel++;
                //Camera.main.fieldOfView = 59 + _speedLevel;
                _timerRoutine = null;
            });
        }

        Vector3 movementDirection = GetMovementDirection();

        if (movementDirection == Vector3.zero)
        {
            _speedLevel = 1;
            //Camera.main.fieldOfView = 59 + _speedLevel;
        }

        //If the user is inputting a direction increment our accelerate timer
        //If they aren't for a second, stop
        //When our accelerate timer hits 0 increment our speed level.

        //Also if we have a nonzero velocity lets have the mesh kinda rotated forward
        //and bobbing as a little placeholder, more rotated per speed level

        //This should eventually be a linear increase
        var velocity = movementDirection * (_speedLevel * MaxSpeed);

        //Set direction and speed
        Rigidbody.velocity = Rigidbody.velocity
                                    .SetX(velocity.x)
                                    .SetY(0f)
                                    .SetZ(velocity.z);
    }

    public IEnumerator CreateTimer(float seconds)
    {
        var timeRemaining = seconds;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
    }
}
