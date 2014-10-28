using System.Security.Cryptography;
using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
    private bool _pickupAllowed = false;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        StartCoroutine(SpawnCooldown());
    }

    IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        _pickupAllowed = true;
    }


    void OnTriggerEnter(Collider other)
    {
        if (!_pickupAllowed) return;


        var userReceiver = other.GetComponent<UserReceiver>();
        if (userReceiver != null)
        {
            userReceiver.GetPickup();

            StartCoroutine(Despawn());
            _pickupAllowed = false;
        }
    }


    IEnumerator Despawn()
    {
        _spriteRenderer.enabled = false;
        _pickupAllowed = false;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

}
