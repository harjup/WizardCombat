using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModestTree.Zenject
{
    // This class is meant to be used the following way:
    //
    //  using (var scope = _container.CreateScope())
    //  {
    //      scope.Bind(playerWrapper);
    //      ...
    //      ...
    //      var bar = _container.Resolve<Foo>();
    //  }
    public class BindScope : IDisposable
    {
        DiContainer _container;
        List<ProviderBase> _scopedProviders = new List<ProviderBase>();
        SingletonProviderMap _singletonMap;

        internal BindScope(DiContainer container, SingletonProviderMap singletonMap)
        {
            _container = container;
            _singletonMap = singletonMap;
        }

        public BinderUntyped Bind(Type contractType)
        {
            return new CustomScopeUntypedBinder(this, contractType, _container, _singletonMap);
        }

        public ReferenceBinder<TContract> Bind<TContract>() where TContract : class
        {
            return new CustomScopeReferenceBinder<TContract>(this, _container, _singletonMap);
        }

        public ValueBinder<TContract> BindValue<TContract>() where TContract : struct
        {
            return new CustomScopeValueBinder<TContract>(this, _container);
        }

        // This method is just an alternative way of binding to a dependency of
        // a specific class with a specific identifier
        public void BindIdentifier<TClass, TParam>(object identifier, TParam value)
            where TParam : class
        {
            Bind(typeof(TParam)).To(value).WhenInjectedInto<TClass>().As(identifier);

            // We'd pref to do this instead but it fails on web player because Mono
            // seems to interpret TDerived : TBase to require that TDerived != TBase?
            //Bind<TParam>().To(value).WhenInjectedInto<TClass>().As(identifier);
        }

        void AddProvider(ProviderBase provider)
        {
            Assert.That(!_scopedProviders.Contains(provider));
            _scopedProviders.Add(provider);
        }

        public void Dispose()
        {
            foreach (var provider in _scopedProviders)
            {
                _container.UnregisterProvider(provider);
            }
        }

        class CustomScopeValueBinder<TContract> : ValueBinder<TContract> where TContract : struct
        {
            BindScope _owner;

            public CustomScopeValueBinder(
                BindScope owner,
                DiContainer container)
                : base(container)
            {
                _owner = owner;
            }

            public override BindingConditionSetter ToProvider(ProviderBase provider)
            {
                _owner.AddProvider(provider);
                return base.ToProvider(provider);
            }
        }

        class CustomScopeReferenceBinder<TContract> : ReferenceBinder<TContract> where TContract : class
        {
            BindScope _owner;

            public CustomScopeReferenceBinder(
                BindScope owner,
                DiContainer container, SingletonProviderMap singletonMap)
                : base(container, singletonMap)
            {
                _owner = owner;
            }

            public override BindingConditionSetter ToProvider(ProviderBase provider)
            {
                _owner.AddProvider(provider);
                return base.ToProvider(provider);
            }
        }

        class CustomScopeUntypedBinder : BinderUntyped
        {
            BindScope _owner;

            public CustomScopeUntypedBinder(
                BindScope owner, Type contractType,
                DiContainer container, SingletonProviderMap singletonMap)
                : base(container, contractType, singletonMap)
            {
                _owner = owner;
            }

            public override BindingConditionSetter ToProvider(ProviderBase provider)
            {
                _owner.AddProvider(provider);
                return base.ToProvider(provider);
            }
        }
    }
}
