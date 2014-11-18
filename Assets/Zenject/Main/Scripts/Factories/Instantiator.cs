using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ModestTree.Zenject
{
    public class Instantiator
    {
        readonly DiContainer _container;

        public Instantiator(DiContainer container)
        {
            _container = container;
        }

        public T Instantiate<T>(
            params object[] extraArgs)
        {
            return (T)Instantiate(typeof(T), extraArgs);
        }

        public object Instantiate(
            Type concreteType, params object[] extraArgs)
        {
            Assert.That(!extraArgs.Contains(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiateExplicit", concreteType);

            return InstantiateExplicit(
                concreteType, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // This is used instead of Instantiate to support specifying null values
        public T InstantiateExplicit<T>(List<TypeValuePair> extraArgMap)
        {
            return (T)InstantiateExplicit(typeof(T), extraArgMap);
        }

        public object InstantiateExplicit(
            Type concreteType, List<TypeValuePair> extraArgMap)
        {
            using (ProfileBlock.Start("Zenject.Instantiate({0})", concreteType))
            {
                using (_container.PushLookup(concreteType))
                {
                    return InstantiateInternal(concreteType, extraArgMap);
                }
            }
        }

        object InstantiateInternal(
            Type concreteType, IEnumerable<TypeValuePair> extraArgMapParam)
        {
            Assert.That(!concreteType.DerivesFrom<UnityEngine.Component>(),
                "Error occurred while instantiating object of type '{0}'. Instantiator should not be used to create new mono behaviours.  Must use GameObjectInstantiator, GameObjectFactory, or GameObject.Instantiate.", concreteType.Name());

            var typeInfo = TypeAnalyzer.GetInfo(concreteType);

            if (typeInfo.InjectConstructor == null)
            {
                throw new ZenjectResolveException(
                    "More than one or zero constructors found for type '{0}' when creating dependencies.  Use one [Inject] attribute to specify which to use.".With(concreteType));
            }

            // Make a copy since we remove from it below
            var extraArgMap = extraArgMapParam.ToList();
            var paramValues = new List<object>();

            foreach (var injectInfo in typeInfo.ConstructorInjectables)
            {
                object value;

                if (!InstantiateUtil.PopValueWithType(extraArgMap, injectInfo.ContractType, out value))
                {
                    value = _container.Resolve(injectInfo);
                }

                paramValues.Add(value);
            }

            object newObj;

            try
            {
                using (ProfileBlock.Start("{0}.{0}()", concreteType))
                {
                    newObj = typeInfo.InjectConstructor.Invoke(paramValues.ToArray());
                }
            }
            catch (Exception e)
            {
                throw new ZenjectResolveException(
                    "Error occurred while instantiating object with type '{0}'".With(concreteType.Name()), e);
            }

            FieldsInjecter.Inject(_container, newObj, extraArgMap, true, typeInfo);

            return newObj;
        }
    }
}
