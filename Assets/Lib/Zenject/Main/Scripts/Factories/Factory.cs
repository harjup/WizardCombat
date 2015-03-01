using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    // Zero parameters
    public class Factory<T> : IValidatableFactory, IFactory<T>
    {
        [Inject]
        DiContainer _container = null;

        public Type ConstructedType
        {
            get { return typeof(T); }
        }

        public Type[] ProvidedTypes
        {
            get { return new Type[0]; }
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public T Create()
        {
            return _container.Instantiate<T>();
        }
    }

    // One parameter
    public class Factory<TParam1, TValue> : IValidatableFactory, IFactory<TParam1, TValue>
    {
        [Inject]
        DiContainer _container = null;

        public Type ConstructedType
        {
            get { return typeof(TValue); }
        }

        public Type[] ProvidedTypes
        {
            get { return new Type[] { typeof(TParam1) }; }
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public TValue Create(TParam1 param)
        {
            return _container.InstantiateExplicit<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param),
                });
        }
    }

    // Two parameters
    public class Factory<TParam1, TParam2, TValue> : IValidatableFactory, IFactory<TParam1, TParam2, TValue>
    {
        [Inject]
        DiContainer _container = null;

        public Type ConstructedType
        {
            get { return typeof(TValue); }
        }

        public Type[] ProvidedTypes
        {
            get { return new Type[] { typeof(TParam1), typeof(TParam2) }; }
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public TValue Create(TParam1 param1, TParam2 param2)
        {
            return _container.InstantiateExplicit<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                });
        }
    }

    // Three parameters
    public class Factory<TParam1, TParam2, TParam3, TValue> : IValidatableFactory, IFactory<TParam1, TParam2, TParam3, TValue>
    {
        [Inject]
        DiContainer _container = null;

        public Type ConstructedType
        {
            get { return typeof(TValue); }
        }

        public Type[] ProvidedTypes
        {
            get { return new Type[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) }; }
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return _container.InstantiateExplicit<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                    InstantiateUtil.CreateTypePair(param3),
                });
        }
    }

    // Four parameters
    public class Factory<TParam1, TParam2, TParam3, TParam4, TValue> : IValidatableFactory, IFactory<TParam1, TParam2, TParam3, TParam4, TValue>
    {
        [Inject]
        DiContainer _container = null;

        public Type ConstructedType
        {
            get { return typeof(TValue); }
        }

        public Type[] ProvidedTypes
        {
            get { return new Type[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4) }; }
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public TValue Create(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return _container.InstantiateExplicit<TValue>(
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
