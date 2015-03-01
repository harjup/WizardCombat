using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public abstract class MonoInstaller : MonoBehaviour, IInstaller
    {
        [Inject]
        DiContainer _container;

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public virtual void Start()
        {
            // Define this method so we expose the enabled check box
        }

        public abstract void InstallBindings();
    }
}
