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

    private GuiManager _guiManager;

    private int _boxesRemaining;

    private int _seconds;

    private IPlayerGuy _player;

    public Stage(StageHooks stageHooks, 
        MovableBox.Factory boxFactory, 
        Timer timer,
        GuiManager guiManager)
    {
        Assert.IsNotNull(stageHooks);
        stageHooks.ResolveDependencies();

        _playerSpawn = stageHooks.PlayerSpawnPoint;
        _boxSpawns = stageHooks.BoxSpawnPoints;
        _boxFactory = boxFactory;

        _boxes = new List<MovableBox>();
        _timer = timer;

        _guiManager = guiManager;

        _guiManager.GuiHooks.AgainButtonPressed += () => { StartLevel(_player); };
    }

    public void StartLevel(IPlayerGuy player)
    {
        _player = player;

        _guiManager.ShowMain();
        
        player.Transform.position = _playerSpawn.transform.position;

        foreach (var spawnPoint in _boxSpawns)
        {
            var box = _boxFactory.Create();
            box.transform.position = spawnPoint.transform.position;
            _boxes.Add(box);
            box.OnDestroyed += OnBoxDestroyed;
        }

        _guiManager.GuiHooks.BoxAmount = _boxes.Count.ToString();

        // Start a timer
        _timer.Reset();
        _timer.Start();
    }

    private void OnBoxDestroyed(MovableBox box)
    {
        _boxes.Remove(box);
        var count = _boxes.Count;

        _guiManager.GuiHooks.BoxAmount = count.ToString();

        if (count <= 0)
        {
            Finish();
        }
    }

    public void Finish()
    {
        // End timer
        _timer.Stop();

        // Display things to gui
        _guiManager.ShowResults();

        // Do an effect

        //_stageRoot.SetNextStageAsActive();
        //_stageRoot.SendPlayerToActiveStageSpawnPoint();
    }

    public void Tick()
    {
        _timer.Tick();
        _guiManager.GuiHooks.TimeElapsed = _timer.GetSeconds().ToString("F2");
    }
}
