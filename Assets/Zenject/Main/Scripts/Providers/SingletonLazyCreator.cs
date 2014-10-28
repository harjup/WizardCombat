using System;

namespace ModestTree.Zenject
{
    internal class SingletonLazyCreator
    {
        int _referenceCount;
        object _instance;
        Type _instanceType;
        SingletonProviderMap _owner;
        DiContainer _container;
        Instantiator _instantiator;
        bool _hasInstance;

        public SingletonLazyCreator(
            DiContainer container, SingletonProviderMap owner, Type instanceType)
        {
            _container = container;
            _owner = owner;
            _instanceType = instanceType;
        }

        public void IncRefCount()
        {
            _referenceCount += 1;
        }

        public void DecRefCount()
        {
            _referenceCount -= 1;

            if (_referenceCount <= 0)
            {
                _owner.RemoveCreator(_instanceType);
            }
        }

        public void SetInstance(object instance)
        {
            Assert.IsNull(_instance);
            Assert.That(instance != null || _container.AllowNullBindings);

            _instance = instance;
            // We need this flag for validation
            _hasInstance = true;
        }

        public bool HasInstance()
        {
            if (_hasInstance)
            {
                Assert.That(_container.AllowNullBindings || _instance != null);
            }

            return _hasInstance;
        }

        public Type GetInstanceType()
        {
            return _instanceType;
        }

        public object GetInstance(Type contractType)
        {
            if (!_hasInstance)
            {
                if (_instantiator == null)
                {
                    _instantiator = _container.Resolve<Instantiator>();
                }

                _instance = _instantiator.Instantiate(GetTypeToInstantiate(contractType));
                Assert.That(_instance != null);
                _hasInstance = true;
            }

            return _instance;
        }

        Type GetTypeToInstantiate(Type contractType)
        {
            if (_instanceType.IsOpenGenericType())
            {
                Assert.That(!contractType.IsAbstract);
                Assert.That(contractType.GetGenericTypeDefinition() == _instanceType);
                return contractType;
            }

            Assert.That(_instanceType.DerivesFromOrEqual(contractType));
            return _instanceType;
        }
    }
}
