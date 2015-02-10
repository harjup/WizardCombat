using System;
using System.Threading;
using Assets.Scripts.Managers;
using ModestTree.Zenject;
using UnityEngine;
using System.Collections;

public class PlayerGuy : ITickable, IInitializable
{
    enum PlayerState
    {
        Undefined,
        Ground,
        Climbing,
        Jumping,
        Airborne
    }

    private PlayerState _playerState = PlayerState.Ground;


    private readonly PlayerGuyHooks _playerGuyHooks;
    private readonly ParallelAsyncTaskProcessor _asyncTaskProcessor;
    private readonly TimerFactory _timerFactory;
    private readonly Camera _camera;

    private readonly DebugGuiHooks _debugGuiHooks;

    private const float InitialSpeed = 5f;
    private const float SpeedMultiplier = 5f;
    
    private IEnumerator _timerRoutine;
    private IEnumerator _walkingTimeout = null;
    private IEnumerator _climbingRoutine = null;
    private IEnumerator _applyGravityRoutine;
    private int _speedLevel = 1;
    private const int MaxSpeedLevel = 3;

    private const float PlayerHeight = 1f;
    private Vector3? _climbTarget;

    public PlayerGuy(PlayerGuyHooks playerGuyHooks, ParallelAsyncTaskProcessor asyncTaskProcessor,
        CameraManager cameraManager, DebugGuiHooks debugGuiHooks)
    {
        _playerGuyHooks = playerGuyHooks;
        _asyncTaskProcessor = asyncTaskProcessor;
        _camera = cameraManager.Main;

        _playerGuyHooks.PlayerHandsCollider.PickupEvent += () => Log.Debug("Got a pickup");

        _debugGuiHooks = debugGuiHooks;

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
        if (!_asyncTaskProcessor.IsProcessing(_climbingRoutine))
        {
            MovePlayer();
            
            if (_playerState != PlayerState.Jumping)
            {
                CheckForFloor();
            }
            if (_playerState == PlayerState.Ground)
            {
                //JumpCheck();
            }
            
            Vector3? climbTarget = CheckForClimbableSurfaces();
            if (climbTarget != null)
            {
                _climbingRoutine = ClimbSurface(climbTarget.Value);
                _asyncTaskProcessor.Process(_climbingRoutine, () => { _climbingRoutine = null; });
            }


            // TODO: Maybe let this go if our guy is too heavy
            /*if (!_asyncTaskProcessor.IsProcessing(_applyGravityRoutine))
            {
                _applyGravityRoutine = ApplyGravity();
                _asyncTaskProcessor.Process(_applyGravityRoutine, () =>
                {
                    _applyGravityRoutine = null;
                });
            }*/
        }
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
        Vector3 movementDirection = GetMovementDirection();

        if (_timerRoutine == null && _speedLevel < MaxSpeedLevel && movementDirection != Vector3.zero)
        {
            _timerRoutine = _timerFactory.CreateTimer(_speedLevel + 1);
            _asyncTaskProcessor.Process(_timerRoutine, () =>
            {
                _speedLevel++;
                Log.Debug("Speedlevel: {0}", _speedLevel);
                _timerRoutine = null;
            });
        }

        if (movementDirection == Vector3.zero)
        {
            if (_walkingTimeout == null)
            {
                _walkingTimeout = _timerFactory.CreateTimer(.25f);
                _asyncTaskProcessor.Process(_walkingTimeout, () =>
                {
                    _speedLevel = 1;

                    if (_asyncTaskProcessor.IsProcessing(_timerRoutine))
                    {
                        _asyncTaskProcessor.Cancel(_timerRoutine);
                        _timerRoutine = null;
                        _walkingTimeout = null;
                    }
                });
            }
        }
        else
        {
            if (_asyncTaskProcessor.IsProcessing(_walkingTimeout))
            {
                _asyncTaskProcessor.Cancel(_walkingTimeout);
                _walkingTimeout = null;
            }
        }

        //Also if we have a nonzero velocity lets have the mesh kinda rotated forward
        //and bobbing as a little placeholder, more rotated per speed level

        var velocity = movementDirection * (InitialSpeed + (_speedLevel * SpeedMultiplier));

        //Set direction and speed
        Rigidbody.velocity = Rigidbody.velocity
                                    .SetX(velocity.x)
                                    .SetZ(velocity.z);
    }

    private IEnumerator ClimbSurface(Vector3 target)
    {
        /*iTween.MoveTo(
            _playerGuyHooks.gameObject, 
            iTween.Hash(
                "y", 
                target.y + PlayerHeight / 2f, 
                "time", .2f, 
                "easetype", 
                iTween.EaseType.easeInBack));*/

        _playerGuyHooks.gameObject.transform.position =
            _playerGuyHooks.gameObject.transform.position.SetY(target.y + PlayerHeight/2f);

        yield return null;

        //yield return _timerFactory.CreateTimer(.25f / _speedLevel);

        Debug.DrawLine(target, target.SetY(target.y + 2), Color.red);
    }

    private Vector3? CheckForClimbableSurfaces()
    {
        RaycastHit hit;
        var distanceTraveledLastFrame = (Rigidbody.velocity * Time.fixedDeltaTime).magnitude * 2.5;
        if (Physics.Raycast(Transform.position, Forward, out hit))
        {
            //TODO: Figure out when we should ledge-climb, probably based on speed/direction/distance
            if (hit.distance < distanceTraveledLastFrame)
            {
                var playerTop = Transform.position.y + PlayerHeight / 2f;
                
                Debug.DrawLine(Transform.position, hit.point, Color.cyan);
                Debug.DrawLine(hit.point, hit.point.SetY(hit.point.y + 2), Color.grey);
                
                RaycastHit[] hits;
                hits = Physics.RaycastAll(hit.point.SetY(hit.point.y + 10f) + Forward, Vector3.down, 100);
                foreach (var raycastHit in hits)
                {
                    var distance = Mathf.Abs(raycastHit.point.y - playerTop);
                    if (!(distance < .5f)) continue;

                    return raycastHit.point;
                }
            }
        }

        return null;
    }

    private void CheckForFloor()
    {
        bool hit = Physics.Raycast(
            Transform.position,
            Vector3.down,
            PlayerHeight/2f);

        Debug.DrawLine(Transform.position,
            Transform.position + (Vector3.down * (PlayerHeight / 2f)), 
            Color.red);

        if (!hit)
        {
            Rigidbody.velocity = Rigidbody.velocity.SetY(-5);
            _playerState = PlayerState.Airborne;
            _debugGuiHooks.TextDebug = "In Air";
        }
        else
        {
            Rigidbody.velocity = Rigidbody.velocity.SetY(0);
            _playerState = PlayerState.Ground;
            _debugGuiHooks.TextDebug = "On Ground";
        }
    }

    //TODO: Determine in what context this should be called, have some basic falling logic in here too
    public void JumpCheck()
    {
        if (_playerGuyHooks.gameObject.GetComponent<iTween>() != null) return;

        Rigidbody.velocity = Rigidbody.velocity.SetY(0f);

        var predictedDistanceForward = (Rigidbody.velocity * Time.fixedDeltaTime * 2.5f).SetY(0);

        var distanceToFloorForward = GetDistanceToFloor(predictedDistanceForward);
        if (distanceToFloorForward > .5f)
        {
            _playerState = PlayerState.Jumping;
            Rigidbody.velocity = Rigidbody.velocity.SetY(5f);

            IEnumerator timer = _timerFactory.CreateTimer(.5f);
            _asyncTaskProcessor.Process(timer, () => {_playerState = PlayerState.Airborne;});
        }
    }

    public IEnumerator ApplyGravity()
    {
        if (_playerGuyHooks.gameObject.GetComponent<iTween>() != null) yield break;

        var forwardEdge = Transform.forward * PlayerHeight / 2f;
        var backwardEdge = -1 * forwardEdge;

        var distanceToFloorForward = GetDistanceToFloor(forwardEdge);
        var distanceToFloorBehind = GetDistanceToFloor(backwardEdge);

        if (distanceToFloorForward > .01f && distanceToFloorBehind > .01f)
        {
            if (distanceToFloorForward < PlayerHeight / 2f
                && distanceToFloorBehind < PlayerHeight / 2f)
            {
                Transform.position = Transform.position.SetY(Transform.position.y - distanceToFloorForward);
            }
            else if (distanceToFloorForward < 90 &&
                      distanceToFloorBehind < 90)
            {
                iTween.MoveTo(_playerGuyHooks.gameObject, iTween.Hash(
                    "y", (Transform.position.y - distanceToFloorForward),
                    "time", .5f,
                    "easetype", iTween.EaseType.easeInBack));

                yield return _timerFactory.CreateTimer(0.5f);
            }
        }
    }

    float GetDistanceToFloor(Vector3 directionToLook)
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(
            Transform.position + directionToLook + new Vector3(0f, 10f, 0f),
            Vector3.down,
            100);


        float leastDistance = 100;
        foreach (var raycastHit in hits)
        {
            Debug.DrawLine(Transform.position + directionToLook, raycastHit.point, Color.cyan);

            var edgePosition = (Transform.position.y - ((Transform.localScale.y / 2f)));
            var pointDifference = edgePosition - (raycastHit.point.y);
            if (pointDifference < leastDistance)
            {
                leastDistance = pointDifference;
            }
        }

        return leastDistance;
    }

}
