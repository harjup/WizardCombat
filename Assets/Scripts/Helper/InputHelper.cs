using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Managers;

public class InputHelper
{
    private readonly InputManager _inputManager;
    private readonly Camera _camera;

    public InputHelper(InputManager inputManager, Camera camera)
    {
        _inputManager = inputManager;
        _camera = camera;
    }


    public Vector3 GetInputDirection()
    {
        var forward = Vector3.forward;
        //Create the reference axis based on the camera rotation, ignoring y rotation
        //We're getting the main camera, which should be the one that's enabled. It's null if disabled so don't do anything if so
        if (_camera != null)
        {
            forward = _camera.transform.TransformDirection(Vector3.forward).SetY(0).normalized;
        }

        var right = new Vector3(forward.z, 0.0f, -forward.x);

        var horizontalInputVector = _inputManager.HoritzontalAxis * right;
        var verticalInputVector = _inputManager.VerticalAxis * forward;

        //Set the player's walk direction
        var walkVector = horizontalInputVector + verticalInputVector;

        //prevent the player from moving faster when walking diagonally
        if (walkVector.sqrMagnitude > 1f)
        {
            walkVector = walkVector.normalized;
        }
        
        return walkVector;
    }
}