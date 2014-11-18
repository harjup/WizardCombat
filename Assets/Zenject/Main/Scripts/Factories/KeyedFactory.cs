using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using System.Linq;

namespace ModestTree.Zenject
{
    public abstract class KeyedFactoryBase<TBase, TKey> : IValidatable
    {
        [Inject]
        readonly DiContainer _container;

        [Inject]
        [InjectOptional]
        readonly List<Tuple<TKey, Type>> _typePairs;

        Dictionary<TKey, Type> _typeMap;

        [Inject]
        [InjectOptional]
        readonly Type _fallbackType;

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        protected abstract Type[] ProvidedTypes
        {
            get;
        }

        [PostInject]
        public void Initialize()
        {
            Assert.That(_fallbackType == null || _fallbackType.DerivesFromOrEqual<TBase>(),
                "Expected fallback type '{0}' to derive from '{1}'", _fallbackType, typeof(TBase));

            _typeMap = _typePairs.ToDictionary(x => x.First, x => x.Second);
            _typePairs.Clear();
        }

        public Type GetMapping(TKey key)
        {
            return _typeMap[key];
        }

        protected Type GetTypeForKey(TKey key)
        {
            Type keyedType;

            if (!_typeMap.TryGetValue(key, out keyedType))
            {
                Assert.IsNotNull(_fallbackType, "Could not find instance for key '{0}'", key);
                return _fallbackType;
            }

            return keyedType;
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            foreach (var constructType in _typeMap.Values)
            {
                foreach (var error in _container.ValidateObjectGraph(constructType, ProvidedTypes))
                {
                    yield return error;
                }
            }
        }

        protected static BindingConditionSetter AddBindingInternal<TDerived>(DiContainer container, TKey key)
            where TDerived : TBase
        {
            return container.Bind<Tuple<TKey, Type>>().To(Tuple.New(key, typeof(TDerived)));
        }
    }

    // Zero parameters
    public class KeyedFactory<TBase, TKey> : KeyedFactoryBase<TBase, TKey>
    {
        protected override Type[] ProvidedTypes
        {
            get
            {
                return new Type[0];
            }
        }

        public TBase Create(TKey key)
        {
            var type = GetTypeForKey(key);
            return (TBase)Container.Instantiate(type);
        }
    }

    // One parameter
    public class KeyedFactory<TBase, TKey, TParam1> : KeyedFactoryBase<TBase, TKey>
    {
        protected override Type[] ProvidedTypes
        {
            get
            {
                return new Type[] { typeof(TParam1) };
            }
        }

        public TBase Create(TKey key, TParam1 param1)
        {
            return (TBase)Container.InstantiateExplicit(
                GetTypeForKey(key),
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                });
        }
    }

    // Two parameters
    public class KeyedFactory<TBase, TKey, TParam1, TParam2> : KeyedFactoryBase<TBase, TKey>
    {
        protected override Type[] ProvidedTypes
        {
            get
            {
                return new Type[] { typeof(TParam1), typeof(TParam2) };
            }
        }

        public TBase Create(TKey key, TParam1 param1, TParam2 param2)
        {
            return (TBase)Container.InstantiateExplicit(
                GetTypeForKey(key),
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                });
        }
    }

    // Three parameters
    public class KeyedFactory<TBase, TKey, TParam1, TParam2, TParam3> : KeyedFactoryBase<TBase, TKey>
    {
        protected override Type[] ProvidedTypes
        {
            get
            {
                return new Type[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) };
            }
        }

        public TBase Create(TKey key, TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return (TBase)Container.InstantiateExplicit(
                GetTypeForKey(key),
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                    InstantiateUtil.CreateTypePair(param3),
                });
        }
    }

    // Four parameters
    public class KeyedFactory<TBase, TKey, TParam1, TParam2, TParam3, TParam4> : KeyedFactoryBase<TBase, TKey>
    {
        protected override Type[] ProvidedTypes
        {
            get
            {
                return new Type[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4) };
            }
        }

        public TBase Create(TKey key, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return (TBase)Container.InstantiateExplicit(
                GetTypeForKey(key),
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                    InstantiateUtil.CreateTypePair(param3),
                    InstantiateUtil.CreateTypePair(param4),
                });
        }
    }
}
