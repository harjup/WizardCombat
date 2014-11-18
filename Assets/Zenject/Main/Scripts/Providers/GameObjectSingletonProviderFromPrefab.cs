using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectSingletonProviderFromPrefab<T> : ProviderBase where T : Component
    {
        T _instance;
        DiContainer _container;
        GameObject _template;

        public GameObjectSingletonProviderFromPrefab(DiContainer container, GameObject template)
        {
            _container = container;
            _template = template;
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override bool HasInstance(Type contractType)
        {
            Assert.That(typeof(T).DerivesFromOrEqual(contractType));
            return _instance != null;
        }

        public override object GetInstance(Type contractType, InjectContext context)
        {
            Assert.That(typeof(T).DerivesFromOrEqual(contractType));

            if (_instance == null)
            {
                _instance = _container.Resolve<GameObjectInstantiator>().Instantiate<T>(_template);
                Assert.That(_instance != null);
            }

            return _instance;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(Type contractType, InjectContext context)
        {
            return BindingValidator.ValidateObjectGraph(_container, typeof(T));
        }
    }
}
