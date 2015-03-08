using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ModestTree;

public class StageRoot
{
    private IPlayerGuy _player;
    private List<Stage> _stages;
    private Stage _currentStage;

    public StageRoot(IPlayerGuy player)
    {
        _player = player;
        _stages = new List<Stage>();

        Assert.That(_player != null, "StageRoot should have a valid player reference!");
    }

    public void AddStage(Stage stage)
    {
        _stages.Add(stage);
        if (_currentStage == null)
        {
            _currentStage = stage;
        }
    }

    public void SetNextStageAsActive()
    {
        var currentStageIndex = _stages.IndexOf(_currentStage);
        currentStageIndex += 1;
        _currentStage = _stages[currentStageIndex];
    }

    public void SendPlayerToActiveStageSpawnPoint()
    {
        _currentStage.StartLevel(_player);
    }

}
