using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;
using System.Linq;

namespace ModestTree.Asteroids
{
    public enum Cameras
    {
        Main,
    }

    public class AsteroidsInstaller : MonoInstaller
    {
        public Settings SceneSettings;

        public override void InstallBindings()
        {
            // Install any other re-usable installers
            InstallIncludes();
            // Install the main game
            InstallAsteroids();
            InstallSettings();
            InitPriorities();
        }

        // In this example there is only one 'installer' but in larger projects you
        // will likely end up with many different re-usable installers
        // that you'll want to use in several different scenes
        // To re-use an existing installer you can simply bind it to IInstaller like below
        // Note that this will only work if your installer is just a normal C# class
        // If it's a monobehaviour (that is, derived from MonoInstaller) then you would be
        // better off making it a prefab and then just including it in your scene (and adding
        // it to the list of installers in the inspector of CompositionRoot) to re-use it
        void InstallIncludes()
        {
            //Container.Bind<IInstaller>().ToSingle<MyCustomInstaller>();
        }

        void InstallAsteroids()
        {
            Container.Bind<LevelHelper>().ToSingle();

            // Any time you use ToSingle<>, what that means is that the DiContainer will only ever instantiate
            // one instance of the type given inside the ToSingle<>. So in this case, any classes that take ITickable,
            // IFixedTickable, or AsteroidManager as inputs will receive the same instance of AsteroidManager
            Container.Bind<ITickable>().ToSingle<AsteroidManager>();
            Container.Bind<IFixedTickable>().ToSingle<AsteroidManager>();
            Container.Bind<AsteroidManager>().ToSingle();

            // Here, we're defining a generic factory to create asteroid objects using the given prefab
            // There's several different ways of instantiating new game objects in zenject, this is
            // only one of them
            // So any classes that want to create new asteroid objects can simply include a injected field
            // or constructor parameter of type Asteroid.Factory, then call create on that
            Container.BindGameObjectFactory<Asteroid.Factory>(SceneSettings.Asteroid.Prefab);

            Container.Bind<IInitializable>().ToSingle<GameController>();
            Container.Bind<ITickable>().ToSingle<GameController>();
            Container.Bind<GameController>().ToSingle();

            Container.Bind<ShipStateFactory>().ToSingle();

            // Here's another way to create game objects dynamically, by using ToTransientFromPrefab
            // We prefer to use ITickable / IInitializable in favour of the Monobehaviour methods
            // so we just use a monobehaviour wrapper class here to pass in asset data
            Container.Bind<ShipHooks>().ToTransientFromPrefab<ShipHooks>(SceneSettings.Ship.Prefab).WhenInjectedInto<Ship>();

            // In this game there is only one camera so an enum isn't necessary
            // but used here to show how it would work if there were multiple
            Container.Bind<Camera>().ToSingle(SceneSettings.MainCamera).As(Cameras.Main);

            Container.Bind<Ship>().ToSingle();
            Container.Bind<ITickable>().ToSingle<Ship>();
            Container.Bind<IInitializable>().ToSingle<Ship>();
        }

        void InstallSettings()
        {
            Container.Bind<ShipStateMoving.Settings>().ToSingle(SceneSettings.Ship.StateMoving);
            Container.Bind<ShipStateDead.Settings>().ToSingle(SceneSettings.Ship.StateDead);
            Container.Bind<ShipStateWaitingToStart.Settings>().ToSingle(SceneSettings.Ship.StateStarting);

            Container.Bind<AsteroidManager.Settings>().ToSingle(SceneSettings.Asteroid.Spawner);
            Container.Bind<Asteroid.Settings>().ToSingle(SceneSettings.Asteroid.General);
        }

        // We don't need to include these bindings but often its nice to have
        // control over initialization-order and update-order
        void InitPriorities()
        {
            Container.Bind<IInstaller>().ToSingle<InitializablePrioritiesInstaller>();
            Container.Bind<List<Type>>().To(InitializablesOrder)
                .WhenInjectedInto<InitializablePrioritiesInstaller>();

            Container.Bind<IInstaller>().ToSingle<TickablePrioritiesInstaller>();
            Container.Bind<List<Type>>().To(TickablesOrder).WhenInjectedInto<TickablePrioritiesInstaller>();

            Container.Bind<IInstaller>().ToSingle<FixedTickablePrioritiesInstaller>();
            Container.Bind<List<Type>>().To(FixedTickablesOrder).WhenInjectedInto<FixedTickablePrioritiesInstaller>();
        }

        [Serializable]
        public class Settings
        {
            public Camera MainCamera;
            public ShipSettings Ship;
            public AsteroidSettings Asteroid;

            [Serializable]
            public class ShipSettings
            {
                public GameObject Prefab;
                public ShipStateMoving.Settings StateMoving;
                public ShipStateDead.Settings StateDead;
                public ShipStateWaitingToStart.Settings StateStarting;
            }

            [Serializable]
            public class AsteroidSettings
            {
                public GameObject Prefab;
                public AsteroidManager.Settings Spawner;
                public Asteroid.Settings General;
            }
        }

        static List<Type> InitializablesOrder = new List<Type>()
        {
            // Re-arrange this list to control init order
            typeof(GameController),
        };

        static List<Type> TickablesOrder = new List<Type>()
        {
            // Re-arrange this list to control update order
            typeof(AsteroidManager),
            typeof(GameController),
        };

        static List<Type> FixedTickablesOrder = new List<Type>()
        {
            // Re-arrange this list to control update order
            typeof(AsteroidManager),
        };
    }
}
