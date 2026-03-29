using UnityEngine;
public static class BoundsHelper
{
    public static FenceZone FindZoneContaining(Vector3 position, FenceZone[] zones)
    {
        if (zones == null) return null;
        foreach (var zone in zones)
        {
            if (TerrainHelper.IsInsideZone(position, zone.startXZ, zone.endXZ))
                return zone;
        }
        return null;
    }
    public static FenceZone FindClosestZone(Vector3 position, FenceZone[] zones)
    {
        if (zones == null || zones.Length == 0) return null;
        FenceZone closest = null;
        float minDist = float.MaxValue;
        foreach (var zone in zones)
        {
            Vector3 center = new Vector3((zone.startXZ.x + zone.endXZ.x) / 2f, position.y, (zone.startXZ.y + zone.endXZ.y) / 2f);
            float dist = Vector3.Distance(position, center);
            if (dist < minDist)
            {
                minDist = dist;
                closest = zone;
            }
        }
        return closest;
    }
    public static Rect GetZoneBounds(FenceZone zone, float margin)
    {
        float minX = zone.startXZ.x + margin;
        float maxX = zone.endXZ.x - margin;
        float minZ = zone.startXZ.y + margin;
        float maxZ = zone.endXZ.y - margin;
        return Rect.MinMaxRect(minX, minZ, maxX, maxZ);
    }

    public static Rect GetTerrainBounds(Terrain terrain, float margin)
    {
        if (terrain == null)
            return Rect.MinMaxRect(0, 0, 0, 0);

        Vector3 size = terrain.terrainData.size;
        float minX = margin;
        float maxX = size.x - margin;
        float minZ = margin;
        float maxZ = size.z - margin;
        return Rect.MinMaxRect(minX, minZ, maxX, maxZ);
    }

    public static Vector3 ClampPosition(Vector3 position, Rect bounds)
    {
        position.x = Mathf.Clamp(position.x, bounds.xMin, bounds.xMax);
        position.z = Mathf.Clamp(position.z, bounds.yMin, bounds.yMax);
        return position;
    }
}

