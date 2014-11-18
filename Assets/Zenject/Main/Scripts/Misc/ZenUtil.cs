using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class ZenUtil
    {
        public static void LoadScene(string levelName)
        {
            ZenUtil.LoadScene(levelName, null);
        }

        public static void LoadScene(
            string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup += extraBindings;
            Application.LoadLevel(levelName);
        }

        public static void LoadSceneAdditive(string levelName)
        {
            LoadSceneAdditive(levelName, null);
        }

        public static void LoadSceneAdditive(
            string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup += extraBindings;
            Application.LoadLevelAdditive(levelName);
        }

        // This method can be used to load the given scene and perform injection on its contents
        // Note that the scene we're loading can have [Inject] flags however it should not have
        // its own composition root
        public static IEnumerator LoadSceneAdditiveWithContainer(
            string levelName, DiContainer parentContainer)
        {
            var rootObjectsBeforeLoad = GameObject.FindObjectsOfType<Transform>().Where(x => x.parent == null).ToList();

            Application.LoadLevelAdditive(levelName);

            // Wait one frame for objects to be added to the scene heirarchy
            yield return null;

            var rootObjectsAfterLoad = GameObject.FindObjectsOfType<Transform>().Where(x => x.parent == null).ToList();

            foreach (var newObject in rootObjectsAfterLoad.Except(rootObjectsBeforeLoad).Select(x => x.gameObject))
            {
                Assert.That(newObject.GetComponent<CompositionRoot>() == null,
                    "LoadSceneAdditiveWithContainer does not expect a container to exist in the loaded scene");

                InjectionHelper.InjectChildGameObjects(parentContainer, newObject);
            }
        }
    }
}
