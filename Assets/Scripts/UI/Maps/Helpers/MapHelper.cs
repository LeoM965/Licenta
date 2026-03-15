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

    public static void DrawBox(Rect targetRect, Color drawColor) 
    {
        DrawRectWithSolidColor(targetRect, drawColor);
    }

    public static void DrawShadow(Rect targetRect, float shadowOffset)
    {
        DrawRectWithSolidColor(new Rect(targetRect.x + shadowOffset, targetRect.y + shadowOffset, targetRect.width, targetRect.height), new Color(0, 0, 0, 0.4f));
    }

    public static void DrawBorder(Rect targetRect, Color drawColor, int borderThickness)
    {
        DrawRectWithSolidColor(new Rect(targetRect.x, targetRect.y, targetRect.width, borderThickness), drawColor);
        DrawRectWithSolidColor(new Rect(targetRect.x, targetRect.yMax - borderThickness, targetRect.width, borderThickness), drawColor);
        DrawRectWithSolidColor(new Rect(targetRect.x, targetRect.y, borderThickness, targetRect.height), drawColor);
        DrawRectWithSolidColor(new Rect(targetRect.xMax - borderThickness, targetRect.y, borderThickness, targetRect.height), drawColor);
    }

    public static Vector2 WorldToMap(Vector3 worldPosition, Vector3 terrainPosition, float terrainInverseWidth, float terrainInverseHeight, Rect minimapRect)
    {
        float x = minimapRect.x + (worldPosition.x - terrainPosition.x) * terrainInverseWidth * minimapRect.width;
        float y = minimapRect.y + (worldPosition.z - terrainPosition.z) * terrainInverseHeight * minimapRect.height;
        return new Vector2(x, y);
    }

    public static void DrawDot(Vector2 position, float size, Color drawColor)
    {
        DrawRectWithSolidColor(new Rect(position.x - size * 0.5f, position.y - size * 0.5f, size, size), drawColor);
    }

    public static void DrawPulse(Vector2 position, float size, Color drawColor, float pulseTime)
    {
        float alpha = (Mathf.Sin(pulseTime) + 1f) * 0.15f;
        DrawRectWithSolidColor(new Rect(position.x - size, position.y - size, size * 2, size * 2), new Color(drawColor.r, drawColor.g, drawColor.b, alpha));
    }

    public static bool ClickedIn(Rect clickArea, Event guiEvent)
    {
        return guiEvent.type == EventType.MouseDown && clickArea.Contains(guiEvent.mousePosition);
    }

    private static void DrawRectWithSolidColor(Rect targetRect, Color drawColor)
    {
        GUI.color = drawColor;
        GUI.DrawTexture(targetRect, WhiteTexture);
        GUI.color = Color.white;
    }
}
