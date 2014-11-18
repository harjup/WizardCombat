using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Helper class to manually fill in dependencies on given objects
    public static class InjectionHelper
    {
        // Inject dependencies into child game objects
        public static void InjectChildGameObjects(
            DiContainer container, GameObject gameObject)
        {
            InjectChildGameObjects(container, gameObject, Enumerable.Empty<object>());
        }

        public static void InjectChildGameObjects(
            DiContainer container, GameObject gameObject, IEnumerable<object> extraArgs)
        {
            foreach (var monoBehaviour in gameObject.GetComponentsInChildren<MonoBehaviour>())
            {
                InjectMonoBehaviour(container, monoBehaviour, extraArgs);
            }
        }

        public static void InjectGameObject(DiContainer container, GameObject gameObj)
        {
            foreach (var component in gameObj.GetComponents<Component>())
            {
                InjectMonoBehaviour(container, component);
            }
        }

        public static void InjectMonoBehaviour(DiContainer container, Component component)
        {
            InjectMonoBehaviour(container, component, Enumerable.Empty<object>());
        }

        public static void InjectMonoBehaviour(
            DiContainer container, Component component, IEnumerable<object> extraArgs)
        {
            // null if monobehaviour link is broken
            if (component != null)
            {
                using (container.PushLookup(component.GetType()))
                {
                    FieldsInjecter.Inject(container, component, extraArgs);
                }
            }
        }
    }
}
