using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class BinderGeneric<TContract> : Binder
    {
        public BinderGeneric(DiContainer container)
            : base(container, typeof(TContract))
        {
        }

        public BindingConditionSetter ToLookup<TConcrete>() where TConcrete : TContract
        {
            return ToMethod(c => c.Resolve<TConcrete>());
        }

        public BindingConditionSetter ToLookup<TConcrete>(object identifier) where TConcrete : TContract
        {
            return ToMethod(c => c.Resolve<TConcrete>(
                new InjectContext(typeof(TConcrete))
                {
                Identifier = identifier,
                }));
        }

        public BindingConditionSetter ToMethod(Func<DiContainer, TContract> method)
        {
            return ToProvider(new MethodProvider<TContract>(method, _container));
        }

        public BindingConditionSetter ToGetter<TObj>(Func<TObj, TContract> method)
        {
            return ToMethod(c => method(c.Resolve<TObj>()));
        }
    }
}
