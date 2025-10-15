
// SPDX-License-Identifier: MIT
using System;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace EditorProductivitySuiteLite
{
    public static class FullscreenGameView
    {
        [MenuItem("Tools/EPS Lite/Toggle Fullscreen Game View")]
        [Shortcut("epslite.toggle_fullscreen_gv", KeyCode.F, ShortcutModifiers.Alt | ShortcutModifiers.Action)]
        public static void Toggle()
        {
            var t = Type.GetType("UnityEditor.GameView,UnityEditor");
            if (t == null) { Debug.LogWarning("GameView type not found."); return; }
            var gv = EditorWindow.GetWindow(t);
            if (gv == null) { Debug.LogWarning("GameView not found."); return; }
            gv.maximized = !gv.maximized;
            gv.Focus();
        }
    }
}
