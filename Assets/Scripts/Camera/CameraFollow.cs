using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModestTree.Zenject;
using UnityEngine;

public class CameraFollow : IFixedTickable
{
    private readonly Transform _targetTransform;
    private readonly Transform _cameraTransform;

    public CameraFollow(CameraManager cameraManager, IPlayerGuy playerGuy)
    {
        _targetTransform = playerGuy.Transform;
        _cameraTransform = cameraManager.Main.transform.parent.transform;
    }


    public void FixedTick()
    {
        _cameraTransform.position = _cameraTransform.position.SetY(iTween.FloatUpdate(_cameraTransform.position.y, _targetTransform.position.y, 5f));
        
        var positionDifference = _targetTransform.position - _cameraTransform.position;
        float xSpeed = Mathf.Abs(positionDifference.x) * 5f;
        float zSpeed = Mathf.Abs(positionDifference.z) * 5f;

        //Cap the camera's speed so it doesn't go fucking nuts and start overshooting the player
        if (xSpeed > 40) { xSpeed = 40; }
        if (zSpeed > 40) { zSpeed = 40; }

        if (Mathf.Abs(positionDifference.x) >= .5f)
        {
            _cameraTransform.position = _cameraTransform.position.SetX(iTween.FloatUpdate(_cameraTransform.position.x, _targetTransform.position.x, xSpeed));
        }
        if (Mathf.Abs(positionDifference.z) >= .5f)
        {
            _cameraTransform.position = _cameraTransform.position.SetZ(iTween.FloatUpdate(_cameraTransform.position.z, _targetTransform.position.z, zSpeed));
        }
    }

}