using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using System.Linq;

namespace ModestTree.Zenject
{
    public class ListFactory<T>
    {
        readonly Instantiator _instantiator;
        List<Type> _implTypes;

        public ListFactory(
            List<Type> implTypes,
            Instantiator instantiator)
        {
            foreach (var type in implTypes)
            {
                Assert.That(type.DerivesFromOrEqual<T>());
            }

            _implTypes = implTypes;
            _instantiator = instantiator;
        }

        public static void Bind()
        {
        }

        public List<T> Create(params object[] constructorArgs)
        {
            var list = new List<T>();

            foreach (var type in _implTypes)
            {
                list.Add(_instantiator.Instantiate<T>(type, constructorArgs));
            }

            return list;
        }
    }
}

