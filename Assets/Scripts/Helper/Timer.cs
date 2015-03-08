using UnityEngine;
using ModestTree.Zenject;

public class Timer : IInitializable
{
    private bool _running;
    private float _seconds;

    public void Initialize()
    {
        _running = false;
    }

    public void Start()
    {
        _running = true;
    }

    public void Stop()
    {
        _running = false;
    }

    public void Tick()
    {
        if (_running)
        {
            _seconds += Time.smoothDeltaTime;
        }
    }

    public float GetSeconds()
    {
        return _seconds;
    }
}