using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectTransientProviderFromPrefab<T> : ProviderBase where T : Component
    {
        DiContainer _container;
        GameObject _template;
        GameObjectInstantiator _instantiator;

        public GameObjectTransientProviderFromPrefab(DiContainer container, GameObject template)
        {
            _container = container;
            _template = template;
        }

        GameObjectInstantiator Instantiator
        {
            get
            {
                return _instantiator ?? (_instantiator = _container.Resolve<GameObjectInstantiator>());
            }
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override bool HasInstance(Type contractType)
        {
            Assert.That(typeof(T).DerivesFromOrEqual(contractType));
            return false;
        }

        public override object GetInstance(Type contractType, InjectContext context)
        {
            Assert.That(typeof(T).DerivesFromOrEqual(contractType));
            return Instantiator.Instantiate<T>(_template);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(Type contractType, InjectContext context)
        {
            return BindingValidator.ValidateObjectGraph(_container, typeof(T));
        }
    }
}
