using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using System.Linq;

namespace ModestTree.Zenject
{
    public class KeyedFactory<TBase, TKey>
    {
        readonly Instantiator _instantiator;
        readonly Dictionary<TKey, Type> _typeMap;

        public KeyedFactory(
            List<Tuple<TKey, Type>> typePairs,
            Instantiator instantiator)
        {
            _typeMap = typePairs.ToDictionary(x => x.First, x => x.Second);
            _instantiator = instantiator;
        }

        public Type GetMapping(TKey key)
        {
            return _typeMap[key];
        }

        public TBase Create(TKey key, params object[] args)
        {
            Assert.That(_typeMap.ContainsKey(key),
                "Could not find instance for key '{0}'", key);

            return (TBase)_instantiator.Instantiate(_typeMap[key], args);
        }

        public static void AddBinding<TDerived>(DiContainer container, TKey key)
            where TDerived : TBase
        {
            container.Bind<Tuple<TKey, Type>>().To(Tuple.New(key, typeof(TDerived))).WhenInjectedInto<KeyedFactory<TBase, TKey>>();
        }
    }
}
