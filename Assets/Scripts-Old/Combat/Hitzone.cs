using UnityEngine;
using System.Collections;

public class Hitzone : MonoBehaviour
{
    /*[SerializeField]
    private Vector3 _direction = Vector3.zero;*/

    public Vector3 Direction
    {
        get { return transform.forward;  }
    }


    [SerializeField]
    private float _power = 0;

    public float Power
    {
        get { return _power; } 
    }
}
