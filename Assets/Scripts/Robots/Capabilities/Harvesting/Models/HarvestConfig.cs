using UnityEngine;

[System.Serializable]
public class HarvestConfig
{
    [Header("Movement")]
    public float arriveDistance = 4f;
    public float harvestRadius = 1.8f;

    [Header("Timing")]
    public float harvestDelay = 0.5f;
    public float rescanInterval = 10f;

    [Header("Quality")]
    public float minSoilQuality = 0.1f;
}
