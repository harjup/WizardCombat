using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

public class StageHooks : MonoBehaviour
{
    public BoxBasket BoxBasket { get; set; }
    public SpawnPoint PlayerSpawnPoint { get; set; }

    public List<SpawnPoint> BoxSpawnPoints { get; set; }

    public void ResolveDependencies()
    {
        BoxBasket = GetComponentInChildren<BoxBasket>();
        PlayerSpawnPoint = GetComponentsInChildren<SpawnPoint>().FirstOrDefault(s => s.Type == SpawnPoint.SpawnType.Player);
        BoxSpawnPoints = GetComponentsInChildren<SpawnPoint>().Where(s => s.Type == SpawnPoint.SpawnType.Box).ToList();

        Assert.IsNotNull(BoxBasket);
        Assert.IsNotNull(PlayerSpawnPoint);
    }
}
