using System;
using System.Collections.Generic;
using System.Linq;

namespace ModestTree.Zenject
{
    public delegate bool BindingCondition(InjectContext c);

    public class BindingConditionSetter
    {
        readonly ProviderBase _provider;

        public BindingConditionSetter(ProviderBase provider)
        {
            _provider = provider;
        }

        public void As(object identifier)
        {
            _provider.Identifier = identifier;
        }

        public IdentifierSetter When(BindingCondition condition)
        {
            _provider.Condition = condition;
            return new IdentifierSetter(_provider);
        }

        public IdentifierSetter WhenInjectedIntoInstance(object instance)
        {
            _provider.Condition = r => ReferenceEquals(r.EnclosingInstance, instance);
            return new IdentifierSetter(_provider);
        }

        public IdentifierSetter WhenInjectedInto(params Type[] targets)
        {
            _provider.Condition = r => targets.Where(x => r.EnclosingType != null && r.EnclosingType.DerivesFromOrEqual(x)).Any();
            return new IdentifierSetter(_provider);
        }

        public IdentifierSetter WhenInjectedInto<T>()
        {
            _provider.Condition = r => r.EnclosingType != null && r.EnclosingType.DerivesFromOrEqual(typeof(T));
            return new IdentifierSetter(_provider);
        }
    }
}
