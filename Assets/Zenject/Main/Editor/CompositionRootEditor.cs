using System.Collections.Generic;
using System.Linq;
using ModestTree.Zenject;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModestTree
{
    [CustomEditor(typeof(CompositionRoot))]
    internal sealed class CompositionRootEditor : UnityEditor.Editor
    {
        static readonly GUIContent INSTALLERS_HEADER = new GUIContent("Installers", "Sorted array of custom installers for your scene");

        ReorderableList _installersList;
        SerializedProperty _installersProperty;

        void OnEnable()
        {
            _installersProperty = serializedObject.FindProperty("Installers");

            _installersList = new ReorderableList(serializedObject, _installersProperty, true, true, true, true);

            _installersList.drawHeaderCallback += rect =>
            {
                GUI.Label(rect, INSTALLERS_HEADER);
            };
            _installersList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.width -= 40;
                rect.x += 20;
                EditorGUI.PropertyField(rect, _installersProperty.GetArrayElementAtIndex(index), GUIContent.none, true);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (Application.isPlaying)
            {
                GUI.enabled = false;
            }

            _installersList.DoLayoutList();

            GUI.enabled = true;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
