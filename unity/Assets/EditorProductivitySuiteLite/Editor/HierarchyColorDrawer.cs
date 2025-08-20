
// SPDX-License-Identifier: MIT
using UnityEditor;
using UnityEngine;

namespace EditorProductivitySuiteLite
{
    [CustomEditor(typeof(ColorTag))]
    public class ColorTagEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var preset = serializedObject.FindProperty("preset");
            var color  = serializedObject.FindProperty("customColor");
            var label  = serializedObject.FindProperty("label");
            EditorGUILayout.PropertyField(preset);
            if ((TagPreset)preset.enumValueIndex == TagPreset.None)
                EditorGUILayout.PropertyField(color);
            EditorGUILayout.PropertyField(label);
            serializedObject.ApplyModifiedProperties();
        }
    }

    [InitializeOnLoad]
    public static class HierarchyColorDrawer
    {
        static HierarchyColorDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (!obj) return;
            var tag = obj.GetComponent<ColorTag>();
            if (!tag) return;

            EditorGUI.DrawRect(selectionRect, tag.GetColor());
            if (!string.IsNullOrEmpty(tag.label))
            {
                var style = new GUIStyle(EditorStyles.miniBoldLabel)
                {
                    alignment = TextAnchor.MiddleRight,
                    normal = { textColor = Color.black }
                };
                GUI.Label(selectionRect, "  " + tag.label + "  ", style);
            }
        }
    }
}
