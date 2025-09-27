
// SPDX-License-Identifier: MIT
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace EditorProductivitySuiteLite
{
    public static class HierarchyGroups
    {
        [MenuItem("Tools/EPS Lite/Groups/Create Group")]
        [Shortcut("epslite.create_group", KeyCode.G, ShortcutModifiers.Shift | ShortcutModifiers.Action)]
        public static void CreateGroup()
        {
            var group = new GameObject("— Group");
            Undo.RegisterCreatedObjectUndo(group, "Create Group");
            Selection.activeGameObject = group;
        }

        [MenuItem("Tools/EPS Lite/Groups/Move Selection To New Group")]
        [Shortcut("epslite.move_to_group", KeyCode.M, ShortcutModifiers.Shift | ShortcutModifiers.Action)]
        public static void MoveSelectionToGroup()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("EPS Lite", "Select at least one GameObject.", "OK");
                return;
            }
            var group = new GameObject("— Group");
            Undo.RegisterCreatedObjectUndo(group, "Create Group");
            Transform parent = Selection.gameObjects[0].transform.parent;
            group.transform.SetParent(parent, true);
            foreach (var go in Selection.gameObjects)
                Undo.SetTransformParent(go.transform, group.transform, "Move To Group");
            Selection.activeGameObject = group;
        }
    }
}
