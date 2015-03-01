using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UserMovement : MonoBehaviourBase
    {
        public const float MaxSpeed = 10;
        private const int MaxSpeedLevel = 5;
        private int _speedLevel = 1;
        private bool _runState = false;



        //private Vector3 _walkVector;
        private Rigidbody _rigidBody;
        public Transform CameraRig;

        private Transform _playerMesh;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            //This should be ok for now since there aren't multiple cameras flying around
            //TODO: There are multiple cameras flying around make sure this is alright
            //var userCamera = GetComponentInChildren<Camera>();
            //userCamera = CameraManager.Instance.GetPlayerCamera();
            //cameraRig = userCamera.transform.parent;
            
            _playerMesh = transform;
            //playerAnimator = playerMesh.GetComponentInChildren<Animator>();
            
            _rigidBody = GetComponent<Rigidbody>();
        }

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            //If in walking state then do a moveplayer
            if (InputManager.Instance.PlayerMovementEnabled)
            {
                MovePlayer();
            }
            

            /*if (InputManager.Instance.CameraAction)
            {
                iTween.Stop(CameraRig.gameObject);
                iTween.RotateTo(CameraRig.gameObject,
                    CameraRig.rotation.eulerAngles.SetY(_playerMesh.rotation.eulerAngles.y),
                    1f);
            }*/
        }

        public void RotateTo(Vector3 rotation)
        {
            _playerMesh.rotation = Quaternion.Euler(_playerMesh.eulerAngles.SetY(rotation.y));
        }
        public void LookAt(Vector3 vector3)
        {
            _playerMesh.LookAt(vector3.SetY(_playerMesh.position.y));
        }
        public Vector3 Forward
        {
            get { return _playerMesh.forward; }
        }

        Vector3 GetMovementDirection()
        {
            var forward = Vector3.forward;
            //Create the reference axis based on the camera rotation, ignoring y rotation
            //We're getting the main camera, which should be the one that's enabled. It's null if disabled so don't do anything if so
            if (Camera.main != null)
            {
                forward = Camera.main.transform.TransformDirection(Vector3.forward).SetY(0).normalized;
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
                _playerMesh.rotation = Quaternion.LookRotation(walkVector.SetY(0), Vector3.up);

            return walkVector;
        }



        private IEnumerator _timerRoutine = null;
        void MovePlayer()
        {
            FindObjectOfType<Text>().text = "Speed: " + (rigidbody.velocity.sqrMagnitude);

            if (_timerRoutine == null && _speedLevel < MaxSpeedLevel)
            {
                _timerRoutine = StartTimer(_speedLevel + 1, () =>
                {
                    _speedLevel++;
                    //Camera.main.fieldOfView = 59 + _speedLevel;
                    _timerRoutine = null;
                });
                StartCoroutine(_timerRoutine);
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
            _rigidBody.velocity = _rigidBody.velocity
                                        .SetX(velocity.x)
                                        .SetY(0f)
                                        .SetZ(velocity.z);

            if (InputManager.Instance.InteractAction)
            {
                StartCoroutine(DoCoolDiveMove(_speedLevel, movementDirection));
            }

        }

        private IEnumerator StartTimer(int time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback();
        }

        public IEnumerator DoCoolDiveMove(int level, Vector3 direction)
        {
            var dashGuyThing = GetComponentInChildren<MoveContainer>();
            
            InputManager.Instance.PlayerMovementEnabled = false;
            //try double normal speed?
            //add a trail effect
            //add some kinda "shockwave" that's bigger than the player as a hitbox/visual bit
            dashGuyThing.SetDashAttack(true);
            rigidbody.velocity = direction.normalized * (level * MaxSpeed * 2);
            yield return new WaitForSeconds(.25f);
            rigidbody.velocity = Vector3.zero;
            dashGuyThing.SetDashAttack(false);
            yield return new WaitForSeconds(.25f);
            InputManager.Instance.PlayerMovementEnabled = true;
        }

        public IEnumerator ClimbGeometry(Vector3 target, Action action)
        {
            collider.enabled = false;
            
            //SoundManager.Instance.Play(SoundManager.SoundEffect.EffortNoise);
            iTween.MoveTo(gameObject,
                target.SetY(target.y + 2.5f),
                .4f);
            action();
            yield return new WaitForSeconds(.4f);
            collider.enabled = true;
            yield break;
        }

    }
}
