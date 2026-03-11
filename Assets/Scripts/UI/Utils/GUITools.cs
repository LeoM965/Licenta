using UnityEngine;

public static class GUITools
{
    public static void DrawRow(float x, ref float y, string label, string value, GUIStyle valueStyle, UITheme t, float labelWidth = 160f, float valueWidth = 120f)
    {
        GUI.Label(new Rect(x, y, labelWidth, 20), label, t.Label);
        GUI.Label(new Rect(x + labelWidth + 5, y, valueWidth, 20), value, valueStyle ?? t.Value);
        y += 24;
    }

    public static void DrawBar(Rect rect, float fill, Color color)
    {
        Color oldColor = GUI.color;
        GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        
        GUI.color = color;
        Rect fillRect = new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(fill), rect.height);
        GUI.DrawTexture(fillRect, Texture2D.whiteTexture);
        
        GUI.color = oldColor;
    }

    public static Color GetScoreColor(float score)
    {
        if (score >= 70) return new Color(0.2f, 0.8f, 0.3f);
        if (score >= 40) return new Color(0.9f, 0.7f, 0.2f);
        return new Color(0.9f, 0.3f, 0.2f);
    }
}
