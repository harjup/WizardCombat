using System.Collections.Generic;
using UnityEngine;

namespace ModestTree.Zenject
{
    public sealed class UnityDependencyRoot : MonoBehaviour, IDependencyRoot
    {
        [Inject]
        public TickableManager _tickableMgr;

        [Inject]
        public InitializableManager _initializableMgr;

        [Inject]
        public DisposableManager _disposablesMgr;

        [PostInject]
        public void Initialize()
        {
            _initializableMgr.Initialize();
        }

        public void OnDestroy()
        {
            _disposablesMgr.Dispose();
        }

        public void Update()
        {
            _tickableMgr.Update();
        }

        public void FixedUpdate()
        {
            _tickableMgr.FixedUpdate();
        }
    }
}
