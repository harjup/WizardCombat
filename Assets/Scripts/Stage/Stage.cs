using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

public class Stage
{
    private SpawnPoint _playerSpawn;
    private List<SpawnPoint> _boxSpawns;
    private List<MovableBox> _boxes;
    private MovableBox.Factory _boxFactory;

    public Stage(StageHooks stageHooks, MovableBox.Factory boxFactory)
    {
        Assert.IsNotNull(stageHooks);
        stageHooks.ResolveDependencies();

        _playerSpawn = stageHooks.PlayerSpawnPoint;
        _boxSpawns = stageHooks.BoxSpawnPoints;
        _boxFactory = boxFactory;

        _boxes = new List<MovableBox>();
    }

    public void StartLevel(IPlayerGuy player)
    {
        player.Transform.position = _playerSpawn.transform.position;

        //_stageRoot.AddStage(this);

        // Spawn boxes at their spawn points
        foreach (var spawnPoint in _boxSpawns)
        {
            var box = _boxFactory.Create();
            box.transform.position = spawnPoint.transform.position;
            _boxes.Add(box);
        }
        
        // Start a timer
    }

    public void Finish()
    {
        // End timer

        // Display things to gui

        // Do an effect

        //_stageRoot.SetNextStageAsActive();
        //_stageRoot.SendPlayerToActiveStageSpawnPoint();
    }
}
