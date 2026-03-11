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
    }
}
