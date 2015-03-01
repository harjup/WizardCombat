using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree.Zenject
{
    // Iterate over fields/properties on a given object and inject any with the [Inject] attribute
    public class FieldsInjecter
    {
        public static void Inject(DiContainer container, object injectable)
        {
            Inject(container, injectable, Enumerable.Empty<object>());
        }

        public static void Inject(DiContainer container, object injectable, IEnumerable<object> additional)
        {
            Inject(container, injectable, additional, false);
        }

        public static void Inject(DiContainer container, object injectable, IEnumerable<object> additional, bool shouldUseAll)
        {
            Inject(container, injectable, additional, shouldUseAll, TypeAnalyzer.GetInfo(injectable.GetType()));
        }

        internal static void Inject(
            DiContainer container, object injectable,
            IEnumerable<object> additional, bool shouldUseAll, ZenjectTypeInfo typeInfo)
        {
            Assert.That(!additional.Contains(null),
                "Null value given to injection argument list. In order to use null you must provide a List<TypeValuePair> and not just a list of objects");

            Inject(
                container, injectable,
                InstantiateUtil.CreateTypeValueList(additional), shouldUseAll, typeInfo);
        }

        internal static void Inject(
            DiContainer container, object injectable,
            IEnumerable<TypeValuePair> extraArgMapParam, bool shouldUseAll, ZenjectTypeInfo typeInfo)
        {
            Assert.IsEqual(typeInfo.TypeAnalyzed, injectable.GetType());
            Assert.That(injectable != null);

            // Make a copy since we remove from it below
            var extraArgMap = extraArgMapParam.ToList();

            foreach (var injectInfo in typeInfo.FieldInjectables.Concat(typeInfo.PropertyInjectables))
            {
                object value;

                if (InstantiateUtil.PopValueWithType(extraArgMap, injectInfo.ContractType, out value))
                {
                    injectInfo.Setter(injectable, value);
                }
                else
                {
                    value = container.Resolve(injectInfo, injectable);

                    if (injectInfo.Optional && value == null)
                    {
                        // Do not override in this case so it retains the hard-coded value
                    }
                    else
                    {
                        injectInfo.Setter(injectable, value);
                    }
                }
            }

            if (shouldUseAll && !extraArgMap.IsEmpty())
            {
                throw new ZenjectResolveException(
                    "Passed unnecessary parameters when injecting into type '{0}'. \nExtra Parameters: {1}\nObject graph:\n{2}"
                        .With(injectable.GetType().Name(), String.Join(",", extraArgMap.Select(x => x.Type.Name()).ToArray()), DiContainer.GetCurrentObjectGraph()));
            }

            foreach (var methodInfo in typeInfo.PostInjectMethods)
            {
                using (ProfileBlock.Start("{0}.{1}()", injectable.GetType(), methodInfo.Name))
                {
                    methodInfo.Invoke(injectable, new object[0]);
                }
            }
        }
    }
}

