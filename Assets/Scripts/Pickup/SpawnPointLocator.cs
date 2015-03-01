using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class SpawnPointLocator : MonoBehaviour
{
    public List<SpawnPoint> Find(SpawnPoint.SpawnType spawnType)
    {
        return FindObjectsOfType<SpawnPoint>().Where(s => s.Type == spawnType).ToList();
    }
}
