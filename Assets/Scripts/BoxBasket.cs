using UnityEngine;
using System.Collections;

public class BoxBasket : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        var movableBox = collision.gameObject.GetComponent<MovableBox>();
        if (movableBox != null)
        {
            movableBox.PoofAway();
        }
    }


}
