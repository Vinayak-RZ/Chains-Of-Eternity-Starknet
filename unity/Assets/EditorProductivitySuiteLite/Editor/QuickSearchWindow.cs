
// SPDX-License-Identifier: MIT
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace EditorProductivitySuiteLite
{
    public class QuickSearchWindow : EditorWindow
    {
        private string _query = "";
        private Vector2 _scroll;
        private readonly List<Object> _results = new();
        private double _nextSearchTime = 0;
        private const double SEARCH_DELAY = 0.15f;

        [MenuItem("Tools/EPS Lite/Quick Search")]
        [Shortcut("epslite.quicksearch", KeyCode.Quote, ShortcutModifiers.Action)]
        public static void Open()
        {
            var w = GetWindow<QuickSearchWindow>("Quick Search");
            w.position = new Rect(Screen.width/2f, Screen.height/2f, 600, 420);
            w.Focus();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(4);
            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("QSField");
            _query = EditorGUILayout.TextField("Search", _query);
            if (EditorGUI.EndChangeCheck())
                _nextSearchTime = EditorApplication.timeSinceStartup + SEARCH_DELAY;

            if (Event.current.type == EventType.Repaint && EditorApplication.timeSinceStartup > _nextSearchTime)
            {
                DoSearch(_query);
                _nextSearchTime = double.MaxValue;
                Repaint();
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var obj in _results)
            {
                if (GUILayout.Button(new GUIContent(obj.name, EditorGUIUtility.ObjectContent(obj, obj.GetType()).image)))
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    if (!string.IsNullOrEmpty(path))
                    {
                        var t = AssetDatabase.GetMainAssetTypeAtPath(path);
                        if (t == typeof(SceneAsset))
                        {
                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                                EditorSceneManager.OpenScene(path);
                        }
                        else
                        {
                            Selection.activeObject = obj;
                            EditorGUIUtility.PingObject(obj);
                        }
                    }
                    else
                    {
                        Selection.activeObject = obj; EditorGUIUtility.PingObject(obj);
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            if (Event.current.type == EventType.Layout) EditorGUI.FocusTextInControl("QSField");
        }

        private void DoSearch(string query)
        {
            _results.Clear();
            if (string.IsNullOrWhiteSpace(query)) return;
            var lower = query.ToLower();

            // Scene objects
            foreach (var go in Object.FindObjectsOfType<GameObject>())
                if (go.name.ToLower().Contains(lower)) _results.Add(go);

            // Assets
            foreach (var guid in AssetDatabase.FindAssets(query))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset) _results.Add(asset);
            }
        }
    }
}
