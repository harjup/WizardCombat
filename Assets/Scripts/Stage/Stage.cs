using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Zenject;

public class Stage : ITickable
{
    private SpawnPoint _playerSpawn;
    private List<SpawnPoint> _boxSpawns;
    private List<MovableBox> _boxes;
    private MovableBox.Factory _boxFactory;
    private Timer _timer;

    private DebugGuiHooks _debugGuiHooks;

    private int _boxesRemaining;

    private int _seconds;

    public Stage(StageHooks stageHooks, 
        MovableBox.Factory boxFactory, 
        Timer timer,
        DebugGuiHooks debugGuiHooks)
    {
        Assert.IsNotNull(stageHooks);
        stageHooks.ResolveDependencies();

        _playerSpawn = stageHooks.PlayerSpawnPoint;
        _boxSpawns = stageHooks.BoxSpawnPoints;
        _boxFactory = boxFactory;

        _boxes = new List<MovableBox>();
        _timer = timer;

        _debugGuiHooks = debugGuiHooks;   
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
            box.OnDestroyed += OnBoxDestroyed;
        }

        _debugGuiHooks.BoxAmount = _boxes.Count.ToString();

        // Start a timer
        _timer.Start();
    }

    private void OnBoxDestroyed(MovableBox box)
    {
        _boxes.Remove(box);
        _debugGuiHooks.BoxAmount = _boxes.Count.ToString();
    }

    public void Finish()
    {
        // End timer
        _timer.Stop();

        // Display things to gui

        // Do an effect

        //_stageRoot.SetNextStageAsActive();
        //_stageRoot.SendPlayerToActiveStageSpawnPoint();
    }

    public void Tick()
    {
        _timer.Tick();
        _debugGuiHooks.TimeElapsed = _timer.GetSeconds().ToString("F2");
    }
}
