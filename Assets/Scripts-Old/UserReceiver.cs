using UnityEngine;
using System.Collections;

public class UserReceiver : MonoBehaviour
{
    public void GetPickup()
    {
        StartCoroutine(PickupSpawn());
    }

    IEnumerator PickupSpawn()
    {
        var prefab = Resources.Load<GameObject>("Prefabs/BillboardSprite");
        var spawnedSprite = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        spawnedSprite.transform.parent = transform;
        spawnedSprite.transform.localPosition = Vector3.zero;

        //TODO: Get iTween to respect local positioning
        iTween.MoveTo(spawnedSprite, spawnedSprite.transform.position.SetY(spawnedSprite.transform.position.y + 1f), 1f);
        Destroy(spawnedSprite, 1.5f);
        yield break;
    }

}
