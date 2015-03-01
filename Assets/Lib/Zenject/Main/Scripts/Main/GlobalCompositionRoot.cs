// Ignore the fact that we don't use _dependencyRoot
#pragma warning disable 414

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModestTree.Zenject
{
    public sealed class GlobalCompositionRoot : MonoBehaviour
    {
        static GlobalCompositionRoot _instance;
        DiContainer _container;
        IDependencyRoot _dependencyRoot;

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public static GlobalCompositionRoot Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("Global Composition Root")
                        .AddComponent<GlobalCompositionRoot>();
                }
                return _instance;
            }
        }

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            // Is this a good idea?
            //go.hideFlags = HideFlags.HideInHierarchy;

            _container = CreateContainer(false, gameObject);
            _dependencyRoot = _container.Resolve<IDependencyRoot>();
        }

        // If we're destroyed manually somehow handle that
        public void OnDestroy()
        {
            _instance = null;
            _dependencyRoot = null;
        }

        public static DiContainer CreateContainer(bool allowNullBindings, GameObject gameObj)
        {
            Assert.That(allowNullBindings || gameObj != null);

            var container = new DiContainer();
            container.AllowNullBindings = allowNullBindings;

            container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();
            container.Bind<GameObject>().To(gameObj)
                .WhenInjectedInto<StandardUnityInstaller>();
            container.InstallInstallers();
            Assert.That(!container.HasBinding<IInstaller>());

            foreach (var installer in GetGlobalInstallers())
            {
                FieldsInjecter.Inject(container, installer);
                container.Bind<IInstaller>().To(installer);
                container.InstallInstallers();
                Assert.That(!container.HasBinding<IInstaller>());
            }

            return container;
        }

        static IEnumerable<IInstaller> GetGlobalInstallers()
        {
            var installerConfig = (GlobalInstallerConfig)Resources.Load("ZenjectGlobalCompositionRoot", typeof(GlobalInstallerConfig));

            if (installerConfig == null)
            {
                return Enumerable.Empty<IInstaller>();
            }

            return installerConfig.Installers;
        }
    }
}
