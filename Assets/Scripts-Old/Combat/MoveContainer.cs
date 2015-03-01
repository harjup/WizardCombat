using UnityEngine;
using System.Collections;

public class MoveContainer : MonoBehaviour
{
    //speed
    //power
    //hitzones in correct order

    private Hitzone _dashCollider;

    void Start()
    {
        _dashCollider = GetComponentInChildren<Hitzone>();
    }

    public void SetDashAttack(bool value)
    {
        _dashCollider.gameObject.SetActive(value);
    }
}
