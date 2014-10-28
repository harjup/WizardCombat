using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Define this class as a component of a top-level game object of your scene heirarchy
    // Then any children will get injected during resolve stage
    public class CompositionRoot : MonoBehaviour
    {
        DiContainer _container;
        IDependencyRoot _dependencyRoot;

        public MonoInstaller[] Installers;

        static Action<DiContainer> _extraBindingLookup;

        internal static Action<DiContainer> ExtraBindingsLookup
        {
            set
            {
                Assert.IsNull(_extraBindingLookup);
                _extraBindingLookup = value;
            }
        }

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        void Register()
        {
            if (Installers.Where(x => x != null).IsEmpty())
            {
                Log.Warn("No installers found while initializing CompositionRoot");
                return;
            }

            foreach (var installer in Installers)
            {
                if (installer == null)
                {
                    Log.Warn("Found null installer hooked up to CompositionRoot");
                    continue;
                }

                if (installer.enabled)
                {
                    // The installers that are part of the scene are monobehaviours
                    // and therefore were not created via Zenject and therefore do
                    // not have their members injected
                    // At the very least they will need the container injected but
                    // they might also have some configuration passed from another
                    // scene as well
                    FieldsInjecter.Inject(_container, installer);
                    _container.Bind<IInstaller>().To(installer);

                    // Install this installer and also any other installers that it installs
                    _container.InstallInstallers();

                    Assert.That(!_container.HasBinding<IInstaller>());
                }
            }
        }

        void InitContainer()
        {
            _container = new DiContainer();

            // Note: This has to go first
            _container.Bind<CompositionRoot>().To(this);

            if (_extraBindingLookup != null)
            {
                _extraBindingLookup(_container);
                _extraBindingLookup = null;
            }
        }

        void Awake()
        {
            Log.Debug("Zenject Started");

            InitContainer();
            Register();
            Resolve();
        }

        void OnDestroy()
        {
            if (_dependencyRoot != null)
            {
                _dependencyRoot.Dispose();
            }
        }

        void Resolve()
        {
            InjectionHelper.InjectChildGameObjects(_container, gameObject);

            if (_container.HasBinding<IDependencyRoot>())
            {
                _dependencyRoot = _container.Resolve<IDependencyRoot>();
                _dependencyRoot.Start();
            }
            else
            {
                Log.Warn("No dependency root found");
            }
        }
    }
}
