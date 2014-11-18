using System;
using ModestTree.Asteroids;
using ModestTree.Zenject;
using UnityEngine;
using System.Collections;

public class MyGameInstaller : MonoInstaller
{
    public enum Cameras
    {
        Main,
    }

    public MySettings MySettings;

    public override void InstallBindings()
    {
        Container.Bind<IDependencyRoot>().ToSingle<DependencyRootStandard>();

        Container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();

        Container.Bind<ParallelAsyncTaskProcessor>().ToSingle();
        Container.Bind<ITickable>().ToSingle<ParallelAsyncTaskProcessor>();

        Container.Bind<ITickable>().ToSingle<MyGameRunner>();
        Container.Bind<IInitializable>().ToSingle<MyGameRunner>();

        Container.Bind<Camera>().ToSingle(MySettings.MainCamera).As(Cameras.Main);
        Container.Bind<CameraManager>().ToSingle<CameraManager>();

        //~~~Time to attempt to spawn a player wheee
        Container.Bind<PlayerGuyHooks>().ToTransientFromPrefab<PlayerGuyHooks>(MySettings.PlayerGuy.Prefab).WhenInjectedInto<PlayerGuy>();

        Container.Bind<IFixedTickable>().ToSingle<CameraFollow>();

        Container.Bind<PlayerGuy>().ToSingle();
        Container.Bind<ITickable>().ToSingle<PlayerGuy>();
        Container.Bind<IInitializable>().ToSingle<PlayerGuy>();
    }
}

[Serializable]        
public class MySettings
{
    public Camera MainCamera;
    public PlayerGuySettings PlayerGuy;
    
    [Serializable]
    public class PlayerGuySettings
    {
        public GameObject Prefab;
    }
}

public class MyGameRunner : ITickable, IInitializable
{
    private PlayerGuy _playerGuy;

    public MyGameRunner(PlayerGuy playerGuy)
    {
        _playerGuy = playerGuy;
    }

    public void Initialize()
    {
        Debug.Log("Hello World");
    }

    public void Tick()
    {

    }
}