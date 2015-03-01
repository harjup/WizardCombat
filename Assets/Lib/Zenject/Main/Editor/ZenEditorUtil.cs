using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ModestTree.Zenject
{
    public static class ZenEditorUtil
    {
        public static DiContainer GetContainerForCurrentScene()
        {
            var compRoot = GameObject.FindObjectsOfType<CompositionRoot>().OnlyOrDefault();

            if (compRoot == null)
            {
                throw new ZenjectException(
                    "Unable to find CompositionRoot in current scene.");
            }

            return compRoot.Container;
        }

        public static IEnumerable<ZenjectResolveException> ValidateAllActiveScenes()
        {
            var activeScenes = UnityEditor.EditorBuildSettings.scenes
                .Select(x => new { Name = Path.GetFileNameWithoutExtension(x.path), Path = x.path }).ToList();

            foreach (var sceneInfo in activeScenes)
            {
                EditorApplication.OpenScene(sceneInfo.Path);

                foreach (var error in ValidateCurrentScene())
                {
                    yield return error;
                }
            }
        }

        public static IEnumerable<ZenjectResolveException> ValidateCurrentScene()
        {
            var compRoot = GameObject.FindObjectsOfType<CompositionRoot>().OnlyOrDefault();

            if (compRoot == null || compRoot.Installers.IsEmpty())
            {
                return Enumerable.Empty<ZenjectResolveException>();
            }

            return ZenEditorUtil.ValidateInstallers(compRoot);
        }

        public static IEnumerable<ZenjectResolveException> ValidateInstallers(CompositionRoot compRoot)
        {
            var globalContainer = GlobalCompositionRoot.CreateContainer(true, null);
            var container = compRoot.CreateContainer(true, globalContainer);

            foreach (var error in container.ValidateResolve<IDependencyRoot>())
            {
                yield return error;
            }

            // Also make sure we can fill in all the dependencies in the built-in scene
            foreach (var curTransform in compRoot.GetComponentsInChildren<Transform>())
            {
                foreach (var monoBehaviour in curTransform.GetComponents<MonoBehaviour>())
                {
                    if (monoBehaviour == null)
                    {
                        Log.Warn("Found null MonoBehaviour on " + curTransform.name);
                        continue;
                    }

                    foreach (var error in container.ValidateObjectGraph(monoBehaviour.GetType()))
                    {
                        yield return error;
                    }
                }
            }

            foreach (var installer in globalContainer.InstalledInstallers.Concat(container.InstalledInstallers))
            {
                if (installer is IValidatable)
                {
                    foreach (var error in ((IValidatable)installer).Validate())
                    {
                        yield return error;
                    }
                }
            }

            foreach (var error in container.ValidateValidatables())
            {
                yield return error;
            }
        }

        public static void OutputObjectGraphForCurrentScene(
            DiContainer container, IEnumerable<Type> ignoreTypes, IEnumerable<Type> contractTypes)
        {
            string dotFilePath = EditorUtility.SaveFilePanel("Choose the path to export the object graph", "", "ObjectGraph", "dot");

            if (!dotFilePath.IsEmpty())
            {
                ObjectGraphVisualizer.OutputObjectGraphToFile(
                    container, dotFilePath, ignoreTypes, contractTypes);

                var dotExecPath = EditorPrefs.GetString("Zenject.GraphVizDotExePath", "");

                if (dotExecPath.IsEmpty() || !File.Exists(dotExecPath))
                {
                    EditorUtility.DisplayDialog(
                        "GraphViz", "Unable to locate GraphViz.  Please select the graphviz 'dot.exe' file which can be found at [GraphVizInstallDirectory]/bin/dot.exe.  If you do not have GraphViz you can download it at http://www.graphviz.org", "Ok");

                    dotExecPath = EditorUtility.OpenFilePanel("Please select dot.exe from GraphViz bin directory", "", "exe");

                    EditorPrefs.SetString("Zenject.GraphVizDotExePath", dotExecPath);
                }

                if (!dotExecPath.IsEmpty())
                {
                    RunDotExe(dotExecPath, dotFilePath);
                }
            }
        }

        static void RunDotExe(string dotExePath, string dotFileInputPath)
        {
            var outputDir = Path.GetDirectoryName(dotFileInputPath);
            var fileBaseName = Path.GetFileNameWithoutExtension(dotFileInputPath);

            var proc = new System.Diagnostics.Process();

            proc.StartInfo.FileName = dotExePath;
            proc.StartInfo.Arguments = "-Tpng {0}.dot -o{0}.png".With(fileBaseName);
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WorkingDirectory = outputDir;

            proc.Start();
            proc.WaitForExit();

            var errorMessage = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            if (errorMessage.IsEmpty())
            {
                EditorUtility.DisplayDialog(
                    "Success!", "Successfully created files {0}.dot and {0}.png".With(fileBaseName), "Ok");
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Error", "Error occurred while generating {0}.png".With(fileBaseName), "Ok");

                Log.Error("Zenject error: Failure during object graph creation: " + errorMessage);

                // Do we care about STDOUT?
                //var outputMessage = proc.StandardOutput.ReadToEnd();
                //Log.Error("outputMessage = " + outputMessage);
            }

        }
    }
}
