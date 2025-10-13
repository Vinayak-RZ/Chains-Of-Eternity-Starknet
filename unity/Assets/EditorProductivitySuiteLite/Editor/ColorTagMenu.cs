
// SPDX-License-Identifier: MIT
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EditorProductivitySuiteLite
{
    public static class ColorTagMenu
    {
        [MenuItem("Tools/EPS Lite/Color Tags/Add ColorTag")]
        public static void AddColorTag()
        {
            if (Selection.activeGameObject != null)
                Undo.AddComponent<ColorTag>(Selection.activeGameObject);
            else
                EditorUtility.DisplayDialog("EPS Lite", "Select a GameObject in the Hierarchy.", "OK");
        }
    }
}
#endif
