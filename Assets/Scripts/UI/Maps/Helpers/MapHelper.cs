using UnityEngine;

public static class MapHelper
{
    private static Texture2D whiteTextureInstance;
    public static Texture2D WhiteTexture
    {
        get 
        { 
            if (whiteTextureInstance == null) whiteTextureInstance = Texture2D.whiteTexture;
            return whiteTextureInstance; 
        }
    }

    public static void DrawBox(Rect rect, Color color) 
    {
        DrawRectWithSolidColor(rect, color);
    }

    public static void DrawShadow(Rect rect, float offset)
    {
        DrawRectWithSolidColor(new Rect(rect.x + offset, rect.y + offset, rect.width, rect.height), new Color(0, 0, 0, 0.4f));
    }

    public static void DrawBorder(Rect rect, Color color, int thickness)
    {
        DrawRectWithSolidColor(new Rect(rect.x, rect.y, rect.width, thickness), color);
        DrawRectWithSolidColor(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
        DrawRectWithSolidColor(new Rect(rect.x, rect.y, thickness, rect.height), color);
        DrawRectWithSolidColor(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
    }

    public static Vector2 WorldToMap(Vector3 worldPosition, Vector3 terrainPosition, float inverseWidth, float inverseHeight, Rect mapRect)
    {
        float x = mapRect.x + (worldPosition.x - terrainPosition.x) * inverseWidth * mapRect.width;
        float y = mapRect.y + (worldPosition.z - terrainPosition.z) * inverseHeight * mapRect.height;
        return new Vector2(x, y);
    }

    public static void DrawDot(Vector2 position, float size, Color color)
    {
        DrawRectWithSolidColor(new Rect(position.x - size * 0.5f, position.y - size * 0.5f, size, size), color);
    }

    public static void DrawPulse(Vector2 position, float size, Color color, float time)
    {
        float alpha = (Mathf.Sin(time) + 1f) * 0.15f;
        DrawRectWithSolidColor(new Rect(position.x - size, position.y - size, size * 2, size * 2), new Color(color.r, color.g, color.b, alpha));
    }

    public static bool ClickedIn(Rect area, Event inputEvent)
    {
        return inputEvent.type == EventType.MouseDown && area.Contains(inputEvent.mousePosition);
    }

    private static void DrawRectWithSolidColor(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }
}




