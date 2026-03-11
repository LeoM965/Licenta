using UnityEngine;

[System.Serializable]
public class FenceZone
{
    public Vector2 startXZ;
    public Vector2 endXZ;
    public Vector2 gapStart = new Vector2(220, 0);
    public Vector2 gapEnd = new Vector2(270, 0);
    public bool hasGap;
}
