using UnityEngine;

namespace Settings.Utils
{
    public static class SettingsUIHelper
    {
        public static void DrawLabeledSlider(ref float sy, string label, ref float value, float min, float max, string format, UITheme theme)
        {
            GUIStyle small = new GUIStyle(theme.Label) { fontSize = 11 };
            GUI.Label(new Rect(0, sy, 170, 20), label, small); // fits long names like "Consum Mișcare"
            value = GUI.HorizontalSlider(new Rect(175, sy + 5, 250, 20), value, min, max);
            
            string input = GUI.TextField(new Rect(430, sy, 60, 20), value.ToString(format), theme.Input);
            if (float.TryParse(input, out float result)) value = Mathf.Clamp(result, min, max);
            
            sy += 24;
        }

        public static void DrawCompactSlider(ref float sy, string label, ref float value, float offsetX, float min, float max, UITheme theme)
        {
            GUIStyle small = new GUIStyle(theme.Label) { fontSize = 10 };
            GUI.Label(new Rect(offsetX, sy, 35, 18), label, small);
            value = GUI.HorizontalSlider(new Rect(offsetX + 35, sy + 4, 60, 18), value, min, max);
            
            string input = GUI.TextField(new Rect(offsetX + 100, sy, 40, 18), value.ToString("F0"), theme.Input);
            if (float.TryParse(input, out float result)) value = Mathf.Clamp(result, min, max);
        }
    }
}
