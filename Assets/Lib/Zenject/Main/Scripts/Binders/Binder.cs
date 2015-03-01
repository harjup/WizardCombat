using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class Binder
    {
        readonly protected Type _contractType;
        readonly protected DiContainer _container;

        public Binder(
            DiContainer container,
            Type contractType)
        {
            _container = container;
            _contractType = contractType;
        }

        public virtual BindingConditionSetter ToProvider(ProviderBase provider)
        {
            _container.RegisterProvider(provider, _contractType);
            return new BindingConditionSetter(provider);
        }
    }
}
