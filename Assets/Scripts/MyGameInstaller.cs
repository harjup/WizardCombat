using System;
using ModestTree.Asteroids;
using ModestTree.Zenject;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

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

        Container.Bind<PlayerGuyHooks>().ToTransientFromPrefab<PlayerGuyHooks>(MySettings.PlayerGuy.Prefab).WhenInjectedInto<SimplePlayer>();

        Container.Bind<ZenPickup>().ToSingle();
        Container.Bind<ZenPickup.Factory>().ToSingle();
        Container.Bind<IInitializable>().ToSingle<ZenPickup>();
        Container.Bind<PickupHooks>().ToTransientFromPrefab<PickupHooks>(MySettings.Pickup.Prefab).WhenInjectedInto<ZenPickup>();

        Container.Bind<SpawnPointLocator>().ToSingleGameObject();

        Container.Bind<IFixedTickable>().ToSingle<CameraFollow>();

        Container.Bind<SimplePlayer>().ToSingle();
        Container.Bind<IPlayerGuy>().ToSingle<SimplePlayer>();
        Container.Bind<ITickable>().ToSingle<SimplePlayer>();
        Container.Bind<IInitializable>().ToSingle<SimplePlayer>();

        Container.Bind<GuiManager>().ToSingle();
        Container.Bind<DebugGuiHooks>().ToSingleFromPrefab<DebugGuiHooks>(MySettings.DebugGuiHooksPrefab);

        Container.Bind<Stage>().ToSingle();
        Container.Bind<ITickable>().ToSingle<Stage>();
        Container.Bind<StageHooks>().ToTransientFromPrefab<StageHooks>(MySettings.Stage.FirstLevelPrefab).WhenInjectedInto<Stage>();

        Container.BindGameObjectFactory<MovableBox.Factory>(MySettings.Stage.MovableBoxPrefab);

        Container.Bind<Timer>().ToTransient();
        Container.Bind<IInitializable>().ToTransient<Timer>();
    }
}

[Serializable]        
public class MySettings
{
    public Camera MainCamera;
    public PlayerGuySettings PlayerGuy;
    public PickupSettings Pickup;
    public StageSettings Stage;

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

    [Serializable]
    public class StageSettings
    {
        public GameObject FirstLevelPrefab;

        public GameObject MovableBoxPrefab;

    }

    public GameObject DebugGuiHooksPrefab;
}

public class MyGameRunner : ITickable, IInitializable
{
    private SimplePlayer _playerGuy;
    private Stage _stage;
    private ZenPickup.Factory _zenPickupFactory;

    public MyGameRunner(SimplePlayer playerGuy, ZenPickup.Factory zenPickupFactory, Stage stage, SpawnPointLocator spawnPointLocator)
    {
        _playerGuy = playerGuy;
        _stage = stage;
        _zenPickupFactory = zenPickupFactory;

        /*foreach (var powerUpSpawnPoint in spawnPointLocator.Find(SpawnPoint.SpawnType.ZenPowerup))
        {
            ZenPickup zenPickup = _zenPickupFactory.Create();
            zenPickup.Position = powerUpSpawnPoint.transform.position;
        }*/

        _stage.StartLevel(_playerGuy);
    }

    public void Initialize()
    {
        
    }

    public void Tick()
    {

    }
}