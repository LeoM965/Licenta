using UnityEngine;

namespace UI.Utils
{
    public static class UIDrawUtils
    {
        public static void DrawHorizontalLine(float x, float y, float width)
        {
            Color oldColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.15f);
            GUI.DrawTexture(new Rect(x, y, width, 1), Texture2D.whiteTexture);
            GUI.color = oldColor;
        }

        public static void DrawRow(float x, float y, float[] offsets, string[] values, GUIStyle style, float height = 16f, float colWidth = 100f)
        {
            if (values == null || offsets == null) return;
            int count = Mathf.Min(values.Length, offsets.Length);
            for (int i = 0; i < count; i++)
            {
                float w = (i < offsets.Length - 1) ? (offsets[i+1] - offsets[i]) : colWidth;
                GUI.Label(new Rect(x + offsets[i], y, w, height), values[i], style);
            }
        }

        public static void DrawRow(float x, float y, float[] offsets, string[] values, GUIStyle[] styles, float height = 16f, float colWidth = 100f)
        {
            if (values == null || offsets == null || styles == null) return;
            int count = Mathf.Min(values.Length, Mathf.Min(offsets.Length, styles.Length));
            for (int i = 0; i < count; i++)
            {
                float w = (i < offsets.Length - 1) ? (offsets[i+1] - offsets[i]) : colWidth;
                GUI.Label(new Rect(x + offsets[i], y, w, height), values[i], styles[i]);
            }
        }
    }
}
