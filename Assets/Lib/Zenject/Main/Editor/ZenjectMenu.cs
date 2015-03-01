using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Debug=UnityEngine.Debug;

namespace ModestTree.Zenject
{
    public static class ZenjectMenu
    {
        public static void ValidateCurrentSceneThenPlay()
        {
            if (ValidateCurrentScene())
            {
                EditorApplication.isPlaying = true;
            }
        }

        [MenuItem("Edit/Zenject/Create Global Composition Root")]
        public static void CreateProjectConfig()
        {
            var asset = ScriptableObject.CreateInstance<GlobalInstallerConfig>();

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Assets/Resources")))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            string assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/ZenjectGlobalCompositionRoot.asset");
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.Refresh();
        }

        // Note that you can also use ZenEditorUtil.ValidateAllActiveScenes if you want the errors back
        [MenuItem("Edit/Zenject/Validate All Active Scenes")]
        public static bool ValidateAllActiveScenes()
        {
            var startScene = EditorApplication.currentScene;

            var activeScenes = UnityEditor.EditorBuildSettings.scenes
                .Select(x => new { Name = Path.GetFileNameWithoutExtension(x.path), Path = x.path }).ToList();

            var failedScenes = new List<string>();

            foreach (var sceneInfo in activeScenes)
            {
                Log.Info("Validating scene '{0}'...", sceneInfo.Name);

                EditorApplication.OpenScene(sceneInfo.Path);

                var compRoot = GameObject.FindObjectsOfType<CompositionRoot>().OnlyOrDefault();

                // Do not validate if there is no comp root
                if (compRoot != null)
                {
                    if (!ValidateCurrentScene())
                    {
                        Log.Error("Failed to validate scene '{0}'", sceneInfo.Name);
                        failedScenes.Add(sceneInfo.Name);
                    }
                }
            }

            EditorApplication.OpenScene(startScene);

            if (failedScenes.IsEmpty())
            {
                Log.Info("Successfully validated all {0} scenes", activeScenes.Count);
                return true;
            }
            else
            {
                Log.Error("Validated {0}/{1} scenes. Failed to validate the following: {2}",
                    activeScenes.Count-failedScenes.Count, activeScenes.Count, failedScenes.Join(", "));
                return false;
            }
        }

        [MenuItem("Edit/Zenject/Validate Current Scene #%v")]
        public static bool ValidateCurrentScene()
        {
            var compRoot = GameObject.FindObjectsOfType<CompositionRoot>().OnlyOrDefault();

            if (compRoot == null)
            {
                Log.Error("Unable to find unique composition root in current scene");
                return false;
            }

            if (compRoot.Installers.IsEmpty())
            {
                Log.Warn("Could not find installers while validating current scene");
                // Return true to allow playing in this case
                return true;
            }

            // Only show a few to avoid spamming the log too much
            var resolveErrors = ZenEditorUtil.ValidateInstallers(compRoot).Take(10).ToList();

            foreach (var error in resolveErrors)
            {
                Log.ErrorException(error);
            }

            if (resolveErrors.Any())
            {
                Log.Error("Validation Completed With Errors");
                return false;
            }

            Log.Info("Validation Completed Successfully");
            return true;
        }

        [MenuItem("Edit/Zenject/Output Object Graph For Current Scene")]
        public static void OutputObjectGraphForScene()
        {
            if (!EditorApplication.isPlaying)
            {
                Log.Error("Zenject error: Must be in play mode to generate object graph.  Hit Play button and try again.");
                return;
            }

            DiContainer container;
            try
            {
                container = ZenEditorUtil.GetContainerForCurrentScene();
            }
            catch (ZenjectException e)
            {
                Log.Error("Unable to find container in current scene. " + e.Message);
                return;
            }

            var ignoreTypes = Enumerable.Empty<Type>();
            var types = container.AllConcreteTypes;

            ZenEditorUtil.OutputObjectGraphForCurrentScene(container, ignoreTypes, types);
        }
    }
}

