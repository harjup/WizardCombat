using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class MonoBehaviourSingletonProvider : ProviderBase
    {
        Component _instance;
        GameObjectInstantiator _instantiator;
        DiContainer _container;
        Type _componentType;
        GameObject _gameObject;

        public MonoBehaviourSingletonProvider(
            Type componentType, DiContainer container, GameObject gameObject)
        {
            Assert.That(componentType.DerivesFrom<Component>());

            _gameObject = gameObject;
            _componentType = componentType;
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return _componentType;
        }

        public override bool HasInstance(Type contractType)
        {
            Assert.That(_componentType.DerivesFromOrEqual(contractType));
            return _instance != null;
        }

        public override object GetInstance(Type contractType, InjectContext context)
        {
            Assert.That(_componentType.DerivesFromOrEqual(contractType));

            if (_instance == null)
            {
                Assert.That(!_container.AllowNullBindings,
                    "Tried to instantiate a MonoBehaviour with type '{0}' during validation. Object graph: {1}", _componentType, DiContainer.GetCurrentObjectGraph());

                _instance = _gameObject.AddComponent(_componentType);
                Assert.That(_instance != null);

                InjectionHelper.InjectMonoBehaviour(_container, _instance);
            }

            return _instance;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(
            Type contractType, InjectContext context)
        {
            return BindingValidator.ValidateObjectGraph(_container, _componentType);
        }
    }
}
