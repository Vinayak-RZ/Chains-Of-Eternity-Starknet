
// SPDX-License-Identifier: MIT
using UnityEngine;

namespace EditorProductivitySuiteLite
{
    public enum TagPreset { None, Red, Orange, Yellow, Green, Cyan, Blue, Purple, Pink, Gray }

    [DisallowMultipleComponent]
    public class ColorTag : MonoBehaviour
    {
        public TagPreset preset = TagPreset.Green;
        [ColorUsage(false, true)] public Color customColor = new Color(0.2f, 0.8f, 0.4f, 0.35f);
        public string label = "";
        public Color GetColor()
        {
            switch (preset)
            {
                case TagPreset.Red:    return new Color(1f, 0.35f, 0.35f, 0.35f);
                case TagPreset.Orange: return new Color(1f, 0.6f, 0.3f, 0.35f);
                case TagPreset.Yellow: return new Color(1f, 0.95f, 0.4f, 0.35f);
                case TagPreset.Green:  return new Color(0.5f, 1f, 0.5f, 0.35f);
                case TagPreset.Cyan:   return new Color(0.4f, 1f, 1f, 0.35f);
                case TagPreset.Blue:   return new Color(0.6f, 0.8f, 1f, 0.35f);
                case TagPreset.Purple: return new Color(0.8f, 0.6f, 1f, 0.35f);
                case TagPreset.Pink:   return new Color(1f, 0.7f, 0.9f, 0.35f);
                case TagPreset.Gray:   return new Color(0.8f, 0.8f, 0.8f, 0.35f);
                default: return customColor;
            }
        }
    }
}
