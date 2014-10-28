using UnityEngine;
using System.Collections;

//This should probably exist separate from the player and get spawned in by bootstrapped
public class CameraController : Singleton<CameraController>
{

    private Transform _cameraRig;
    private Transform _targetTransform;

    // ReSharper disable once UnusedMember.Local
    void Start()
    {
        _cameraRig = transform.parent.transform;
        _targetTransform = GameObject.Find("PlayerRig").transform;
    }

    // ReSharper disable once UnusedMember.Local
    void FixedUpdate()
    {
        //Move the camera on the corresponding input axis
        //TODO: Rotate the camera based on the direction the player is facing
        //TODO: Center the camera behind the player if the press the center camera button
        //TODO: Remove manual camera axis stuff here

        MoveCamera();
    }

    void MoveCamera()
    {
        _cameraRig.position = _cameraRig.position.SetY(iTween.FloatUpdate(_cameraRig.position.y, _targetTransform.position.y, 5f));

        //TODO: Have the camera contextually recenter or some shit
        var positionDifference = _targetTransform.position - _cameraRig.position;
        float xSpeed = Mathf.Abs(positionDifference.x) * 5f;
        float zSpeed = Mathf.Abs(positionDifference.z) * 5f;

        //Cap the camera's speed so it doesn't go fucking nuts and start overshooting the player
        if (xSpeed > 40) { xSpeed = 40; }
        if (zSpeed > 40) { zSpeed = 40; }

        if (Mathf.Abs(positionDifference.x) >= .5f)
        {
            _cameraRig.position = _cameraRig.position.SetX(iTween.FloatUpdate(_cameraRig.position.x, _targetTransform.position.x, xSpeed));
        }
        if (Mathf.Abs(positionDifference.z) >= .5f)
        {
            _cameraRig.position = _cameraRig.position.SetZ(iTween.FloatUpdate(_cameraRig.position.z, _targetTransform.position.z, zSpeed));
        }

        var rotationDifference = Mathf.Abs(_cameraRig.gameObject.transform.eulerAngles.y
                                      - _targetTransform.rotation.eulerAngles.y);


        /*if ((rotationDifference < 150 || rotationDifference > 220)
            && (cameraRig.gameObject.GetComponent<iTween>() == null 
                || !cameraRig.gameObject.GetComponent<iTween>().isRunning))
        {

            var rotationMagnitude = 400f / (Mathf.Abs(rotationDifference - 180) + 40);
            iTween.RotateUpdate(cameraRig.gameObject,
                cameraRig.rotation.eulerAngles.SetY(targetTransform.rotation.eulerAngles.y), rotationMagnitude);
        }*/
    }
}
