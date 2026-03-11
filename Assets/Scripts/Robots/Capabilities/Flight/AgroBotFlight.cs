using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sensors.Components;
using Robots.Models;

public class AgroBotFlight : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private FlightSettings settings = new FlightSettings();
    [SerializeField] private Transform flightBody;

    private List<EnvironmentalSensor> trackedParcels = new List<EnvironmentalSensor>();
    private EnvironmentalSensor currentTargetParcel;
    private OperationRegion region;
    private FlightState state = FlightState.Initializing;
    
    private Vector3 currentMoveTarget;
    private int currentParcelIndex;
    private float waitTimer;

    private Vector3 lastPosition;

    private void Start()
    {
        if (flightBody == null) flightBody = transform;
        lastPosition = flightBody.position;
        StartCoroutine(InitializationRoutine());
    }

    private IEnumerator InitializationRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        
        SetupOperationRegion();
        PopulateTrackedParcels();

        if (trackedParcels.Count > 0)
        {
            trackedParcels.Sort((a, b) => a.LatestAnalysis.qualityScore.CompareTo(b.LatestAnalysis.qualityScore));
            SetNextTarget();
            state = FlightState.Navigating;
        }
        else StartCoroutine(InitializationRoutine());
    }

    private void SetupOperationRegion()
    {
        var fence = FindFirstObjectByType<FenceGenerator>();
        if (fence != null && fence.zones != null && fence.zones.Length > 0)
        {
            region = OperationRegion.FromZone(GetNearestZone(fence.zones));
        }
        else
        {
            var terrain = Terrain.activeTerrain;
            Rect bounds = Rect.zero;
            if (terrain != null)
            {
                bounds = new Rect(0, 0, terrain.terrainData.size.x, terrain.terrainData.size.z);
            }
            region = new OperationRegion(bounds);
        }
    }

    private FenceZone GetNearestZone(FenceZone[] zones)
    {
        float minSqrDist = float.MaxValue;
        int bestIndex = 0;
        Vector3 pos = transform.position;

        for (int i = 0; i < zones.Length; i++)
        {
            Vector2 center = (zones[i].startXZ + zones[i].endXZ) * 0.5f;
            float sqrDist = (pos.x - center.x) * (pos.x - center.x) + (pos.z - center.y) * (pos.z - center.y);
            if (sqrDist < minSqrDist) { minSqrDist = sqrDist; bestIndex = i; }
        }
        return zones[bestIndex];
    }

    private void PopulateTrackedParcels()
    {
        trackedParcels = region.FilterParcels(ParcelCache.Parcels);
        if (trackedParcels.Count == 0) trackedParcels.AddRange(ParcelCache.Parcels);
    }

    private void Update()
    {
        if (state == FlightState.Initializing) return;

        UpdateHoverPhysics();

        // Track distance for simulation time passage
        float distMoved = Vector3.Distance(flightBody.position, lastPosition);
        if (distMoved > 0.001f && TimeManager.Instance != null)
        {
            TimeManager.Instance.AddDistanceTraveled(distMoved);
        }
        lastPosition = flightBody.position;

        if (state == FlightState.Navigating) ExecuteNavigation();
        else ExecuteHoverWait();
    }

    private void ExecuteNavigation()
    {
        Vector3 pos = flightBody.position;
        Vector3 dir = currentMoveTarget - pos;
        dir.y = 0;

        if (dir.magnitude > 0.1f)
        {
            Vector3 moveDir = dir.normalized;
            flightBody.position += moveDir * settings.speed * Time.deltaTime;
            flightBody.rotation = Quaternion.Slerp(flightBody.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 3f);
        }

        if (dir.sqrMagnitude < settings.ArrivalThresholdSqr)
        {
            waitTimer = settings.waitTimePerParcel;
            state = FlightState.HoveringAtTarget;
        }
    }

    private void ExecuteHoverWait()
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
        {
            SetNextTarget();
            state = FlightState.Navigating;
        }
    }

    private void UpdateHoverPhysics()
    {
        Vector3 pos = flightBody.position;
        pos.y = settings.altitude + Mathf.Sin(Time.time * settings.AngularHoverFrequency) * settings.hoverAmplitude;
        
        pos.x = Mathf.Clamp(pos.x, region.Bounds.xMin - 5f, region.Bounds.xMax + 5f);
        pos.z = Mathf.Clamp(pos.z, region.Bounds.yMin - 5f, region.Bounds.yMax + 5f);
        
        flightBody.position = pos;
    }

    private void SetNextTarget()
    {
        if (trackedParcels.Count == 0) return;
        currentParcelIndex = (currentParcelIndex + 1) % trackedParcels.Count;
        currentTargetParcel = trackedParcels[currentParcelIndex];
        currentMoveTarget = currentTargetParcel.transform.position;
        currentMoveTarget.y = settings.altitude;
    }

    public string GetStatus()
    {
        if (currentTargetParcel != null)
        {
            return $"{currentTargetParcel.name} [{state}]";
        }
        return "Idle";
    }
}
