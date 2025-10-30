
// SPDX-License-Identifier: MIT
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using System.Collections.Generic;

namespace EditorProductivitySuiteLite
{
    public class QuickActionsConfigLite : ScriptableObject
    {
        public List<string> favoriteScenes = new();
        public float gridSize = 1f;
        public static string AssetPath = "Assets/EPS_Lite/QuickActions.asset";
        public static QuickActionsConfigLite GetOrCreate()
        {
            var a = AssetDatabase.LoadAssetAtPath<QuickActionsConfigLite>(AssetPath);
            if (!a)
            {
                System.IO.Directory.CreateDirectory("Assets/EPS_Lite");
                a = CreateInstance<QuickActionsConfigLite>();
                AssetDatabase.CreateAsset(a, AssetPath);
                AssetDatabase.SaveAssets();
            }
            return a;
        }
    }

    public class QuickActionsToolbarLite : EditorWindow
    {
        [MenuItem("Tools/EPS Lite/Quick Actions (Lite)")]
        [Shortcut("epslite.quick_actions", KeyCode.Q, ShortcutModifiers.Alt | ShortcutModifiers.Action)]
        public static void Open() => GetWindow<QuickActionsToolbarLite>("Quick Actions (Lite)");

        private Vector2 _scroll;
        void OnGUI()
        {
            var cfg = QuickActionsConfigLite.GetOrCreate();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            GUILayout.Label("Play / Pause", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Play")) EditorApplication.isPlaying = true;
            if (GUILayout.Button("Pause")) EditorApplication.isPaused = !EditorApplication.isPaused;
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(6);
            GUILayout.Label("Selection", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Duplicate"))
                foreach (var o in Selection.gameObjects) PrefabUtility.InstantiatePrefab(o);
            if (GUILayout.Button("Align To Grid"))
            {
                foreach (var o in Selection.gameObjects)
                {
                    Undo.RecordObject(o.transform, "Align To Grid");
                    o.transform.position = new Vector3(
                        Mathf.Round(o.transform.position.x / cfg.gridSize) * cfg.gridSize,
                        Mathf.Round(o.transform.position.y / cfg.gridSize) * cfg.gridSize,
                        Mathf.Round(o.transform.position.z / cfg.gridSize) * cfg.gridSize
                    );
                }
            }
            EditorGUILayout.EndHorizontal();
            cfg.gridSize = EditorGUILayout.FloatField("Grid Size", cfg.gridSize);

            GUILayout.Space(6);
            GUILayout.Label("Favorite Scenes", EditorStyles.boldLabel);
            int remove = -1;
            for (int i=0;i<cfg.favoriteScenes.Count;i++)
            {
                EditorGUILayout.BeginHorizontal();
                cfg.favoriteScenes[i] = EditorGUILayout.TextField(cfg.favoriteScenes[i]);
                if (GUILayout.Button("Open"))
                {
                    if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(cfg.favoriteScenes[i]);
                }
                if (GUILayout.Button("X")) remove = i;
                EditorGUILayout.EndHorizontal();
            }
            if (remove >= 0) cfg.favoriteScenes.RemoveAt(remove);
            if (GUILayout.Button("Add Scene Path")) cfg.favoriteScenes.Add("");

            if (GUI.changed) { EditorUtility.SetDirty(cfg); AssetDatabase.SaveAssets(); }

            EditorGUILayout.EndScrollView();
        }
    }
}
