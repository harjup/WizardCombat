using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public abstract class Installer : IInstaller
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

        public abstract void InstallBindings();
    }
}
