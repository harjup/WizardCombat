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
        Container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();

        Container.Bind<ParallelAsyncTaskProcessor>().ToSingle();
        Container.Bind<ITickable>().ToSingle<ParallelAsyncTaskProcessor>();

        Container.Bind<ITickable>().ToSingle<MyGameRunner>();
        Container.Bind<IInitializable>().ToSingle<MyGameRunner>();

        Container.Bind<Camera>().ToSingle(MySettings.MainCamera).As(Cameras.Main);
        Container.Bind<CameraManager>().ToSingle<CameraManager>();

        //~~~Time to attempt to spawn a player wheee
        Container.Bind<PlayerGuyHooks>().ToTransientFromPrefab<PlayerGuyHooks>(MySettings.PlayerGuy.Prefab).WhenInjectedInto<PlayerGuy>();

        Container.Bind<ZenPickup>().ToSingle();
        Container.Bind<ZenPickup.Factory>().ToSingle();
        Container.Bind<IInitializable>().ToSingle<ZenPickup>();
        Container.Bind<PickupHooks>().ToTransientFromPrefab<PickupHooks>(MySettings.Pickup.Prefab).WhenInjectedInto<ZenPickup>();

        Container.Bind<SpawnPointLocator>().ToSingleGameObject();

        Container.Bind<IFixedTickable>().ToSingle<CameraFollow>();

        Container.Bind<PlayerGuy>().ToSingle();
        Container.Bind<ITickable>().ToSingle<PlayerGuy>();
        Container.Bind<IInitializable>().ToSingle<PlayerGuy>();

        Container.Bind<DebugGuiHooks>().ToSingleFromPrefab<DebugGuiHooks>(MySettings.DebugGuiHooksPrefab);
    }
}

[Serializable]        
public class MySettings
{
    public Camera MainCamera;
    public PlayerGuySettings PlayerGuy;
    public PickupSettings Pickup;

    [Serializable]
    public class PickupSettings
    {
        public GameObject Prefab;
    }
    
    [Serializable]
    public class PlayerGuySettings
    {
        public GameObject Prefab;
    }

    public GameObject DebugGuiHooksPrefab;
}

public class MyGameRunner : ITickable, IInitializable
{
    private PlayerGuy _playerGuy;
    private ZenPickup.Factory _zenPickupFactory;

    public MyGameRunner(PlayerGuy playerGuy, ZenPickup.Factory zenPickupFactory, SpawnPointLocator spawnPointLocator)
    {
        _playerGuy = playerGuy;
        _zenPickupFactory = zenPickupFactory;

        foreach (var powerUpSpawnPoint in spawnPointLocator.Find(SpawnPoint.SpawnType.ZenPowerup))
        {
            ZenPickup zenPickup = _zenPickupFactory.Create();
            zenPickup.Position = powerUpSpawnPoint.transform.position;
        }
        
    }

    public void Initialize()
    {
        
    }

    public void Tick()
    {

    }
}