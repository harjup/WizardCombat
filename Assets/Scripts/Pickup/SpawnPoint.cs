using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
    public enum SpawnType
    {
        Undefined,
        ZenPowerup,
        Player,
        Box
    }

    public SpawnType Type;

    public void Start()
    {
        gameObject.SetActive(false);
    }
}
