
// SPDX-License-Identifier: MIT
// EPS Lite Demo - One-click setup
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using EditorProductivitySuiteLite; // requires EPS Lite namespace

namespace EPSLiteDemo
{
    public static class DemoPaths
    {
        public const string DemoRoot = "Assets/EPSLiteDemo";
        public const string ScenesDir = DemoRoot + "/Scenes";
        public const string MaterialsDir = DemoRoot + "/Materials";
        public const string PrefabsDir = DemoRoot + "/Prefabs";
    }

    public class LiteDemoMarker : ScriptableObject { public string note; }

    public static class EPSLiteDemoSetup
    {
        [MenuItem("Tools/EPS Lite/Demo/Setup All (Lite)")]
        public static void SetupAll()
        {
            AssetDatabase.DisallowAutoRefresh();
            try
            {
                CreateFolders();
                var scenePath = CreateDemoScene();

                // Configure Quick Actions (Lite)
                ConfigureQuickActions(scenePath);

                // Save all
                AssetDatabase.SaveAssets();
                EditorSceneManager.SaveOpenScenes();

                EditorUtility.DisplayDialog("EPS Lite Demo",
                    "Demo content created.\nOpen Quick Actions (Lite) and Quick Search from Tools → EPS Lite.\nEnjoy!",
                    "OK");
            }
            finally
            {
                AssetDatabase.AllowAutoRefresh();
                AssetDatabase.Refresh();
            }
        }

        static void CreateFolders()
        {
            Directory.CreateDirectory(DemoPaths.DemoRoot);
            Directory.CreateDirectory(DemoPaths.ScenesDir);
            Directory.CreateDirectory(DemoPaths.MaterialsDir);
            Directory.CreateDirectory(DemoPaths.PrefabsDir);
        }

        static string CreateDemoScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            scene.name = "DemoScene_Lite";

            // Root group
            var group = new GameObject("— Group");
            // Player
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.SetParent(group.transform);
            var ctPlayer = player.AddComponent<ColorTag>();
            ctPlayer.preset = TagPreset.Green;
            ctPlayer.label = "Player";

            // UI Root
            var ui = new GameObject("UI_Root");
            ui.transform.SetParent(group.transform);
            var ctUI = ui.AddComponent<ColorTag>();
            ctUI.preset = TagPreset.Pink;
            ctUI.label = "UI";

            // Pickup item
            var pickup = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickup.name = "Pickup_Coin";
            pickup.transform.SetParent(group.transform);
            var ctPick = pickup.AddComponent<ColorTag>();
            ctPick.preset = TagPreset.Yellow;
            ctPick.label = "Pickup";

            // Enemy
            var enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            enemy.name = "Enemy_01";
            enemy.transform.SetParent(group.transform);
            var ctEnemy = enemy.AddComponent<ColorTag>();
            ctEnemy.preset = TagPreset.Red;
            ctEnemy.label = "Enemy";

            // Arrange positions
            player.transform.position = new Vector3(-2, 1, 0);
            pickup.transform.position = new Vector3(0, 1, 0);
            enemy.transform.position = new Vector3(2, 1, 0);

            var path = DemoPaths.ScenesDir + "/DemoScene_Lite.unity";
            EditorSceneManager.SaveScene(scene, path);
            return path;
        }

        static void ConfigureQuickActions(string favoriteScenePath)
        {
            var cfg = QuickActionsConfigLite.GetOrCreate();
            cfg.gridSize = 0.5f;
            cfg.favoriteScenes.Clear();
            cfg.favoriteScenes.Add(favoriteScenePath);
            EditorUtility.SetDirty(cfg);
        }
    }
}
#endif
