using System.Collections;
using UnityEngine;

public class PartyBox : MonoBehaviour
{
    [SerializeField]
    private GameObject _boxBreakEffect;
    [SerializeField]
    private GameObject _powerToSpawn;

    private int _spawnCount = 3;

    private float _healthPoints = 10f;


    void OnTriggerEnter(Collider other)
    {
        var hitzone = other.GetComponent<Hitzone>();
        if (hitzone != null && GetComponent(typeof(iTween)) == null)
        {
            _healthPoints -= hitzone.Power;
            rigidbody.velocity = hitzone.Direction * hitzone.Power;
            if (_healthPoints <= 0)
            {
                //spawn explosion effect
                Instantiate(_boxBreakEffect, transform.position, Quaternion.identity);
                for (int i = 0; i < _spawnCount; i++)
                {
                    var power = Instantiate(_powerToSpawn, transform.position, Quaternion.identity) as GameObject;
                    power.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(0f, 1f), 1f, Random.Range(0f, 1f)) * 5f;
                }

                iTween.ScaleTo(gameObject,
                    iTween.Hash(
                    "scale",
                    gameObject.transform.localScale / 2f,
                    "easetype",
                    iTween.EaseType.easeOutBounce,
                    "time",
                    .3f));

                rigidbody.angularVelocity = Vector3.left;

                rigidbody.isKinematic = true;
                GetComponent<Collider>().enabled = false;
                Destroy(gameObject, 1f);
            }
            else
            {
                iTween.ScaleFrom(gameObject,
                    iTween.Hash(
                    "scale",
                    (gameObject.transform.localScale.magnitude * ((hitzone.Direction) / (Mathf.Sqrt(hitzone.Power))))
                        .SetY(gameObject.transform.localScale.z),
                    "easetype",
                    iTween.EaseType.easeOutElastic,
                    "time",
                    .5f));
            }

        }
    }
}
