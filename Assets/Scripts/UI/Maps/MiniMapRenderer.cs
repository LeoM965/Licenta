using UnityEngine;

public static class MiniMapRenderer
{
    public static void DrawZone(Rect mapRect, Vector2 start, Vector2 end, float inverseWidth, float inverseHeight, Color color)
    {
        float x = mapRect.x + start.x * inverseWidth * mapRect.width;
        float y = mapRect.y + start.y * inverseHeight * mapRect.height;
        float x2 = mapRect.x + end.x * inverseWidth * mapRect.width;
        float y2 = mapRect.y + end.y * inverseHeight * mapRect.height;
        Rect r = new Rect(Mathf.Min(x, x2), Mathf.Min(y, y2), Mathf.Abs(x - x2), Mathf.Abs(y - y2));
        MapHelper.DrawBox(r, color);
    }

    public static void DrawBuilding(Rect mapRect, Building building, Vector3 terrainPos, float inverseWidth, float inverseHeight, Color color, Event guiEvent)
    {
        Vector2 pos = MapHelper.WorldToMap(building.position, terrainPos, inverseWidth, inverseHeight, mapRect);
        MapHelper.DrawDot(pos, 6, color);
    }

    public static bool DrawRobot(Rect mapRect, Vector3 position, Vector3 terrainPos, float inverseWidth, float inverseHeight, Color color, float size, bool isSelected, float pulseTime, Event guiEvent)
    {
        Vector2 mapPos = MapHelper.WorldToMap(position, terrainPos, inverseWidth, inverseHeight, mapRect);
        if (isSelected)
            MapHelper.DrawPulse(mapPos, size, color, pulseTime);
        MapHelper.DrawDot(mapPos, size, color);
        return MapHelper.ClickedIn(new Rect(mapPos.x - size, mapPos.y - size, size * 2, size * 2), guiEvent);
    }
}
