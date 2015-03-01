using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public class InjectContext
    {
        // Null for root-level lookups
        public Type EnclosingType;
        // Null for constructor params
        public object EnclosingInstance;
        // Null for root-level lookups
        public string SourceName;
        // Null most of the time
        public object Identifier;
        public List<Type> ParentTypes;
        public bool Optional;
        public DiContainer Container;

        public InjectContext(DiContainer container)
        {
            Container = container;
            ParentTypes = new List<Type>();
        }

        public InjectContext(DiContainer container, Type targetType)
        {
            Container = container;
            ParentTypes = new List<Type>();
            EnclosingType = targetType;
        }

        internal InjectContext(
            DiContainer container,
            InjectableInfo injectInfo, List<Type> parents, object targetInstance)
        {
            Container = container;
            Optional = injectInfo.Optional;
            Identifier = injectInfo.Identifier;
            SourceName = injectInfo.SourceName;
            EnclosingType = injectInfo.EnclosingType;
            EnclosingInstance = targetInstance;
            ParentTypes = parents;
        }
    }
}
