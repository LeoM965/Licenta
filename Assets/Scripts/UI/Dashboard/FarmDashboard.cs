using UnityEngine;
using System.Collections.Generic;
using Sensors.Components;

public class FarmDashboard : MonoBehaviour
{
    [SerializeField] KeyCode toggleKey = KeyCode.F1;
    [SerializeField] bool showOnStart = true;
    [SerializeField] UITheme theme;

    MultiRobotSpawner spawner;
    List<EnvironmentalSensor> parcels = new List<EnvironmentalSensor>();
    float avgQuality, avgHumidity, avgPH;
    int good, poor, critical, robots;
    float timer;
    bool show;

    void Start()
    {
        show = showOnStart;
        spawner = FindFirstObjectByType<MultiRobotSpawner>();
        Invoke(nameof(Init), 0.5f);
    }

    void Init()
    {
        parcels.AddRange(ParcelCache.Parcels);
        Refresh();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) show = !show;
        timer -= Time.deltaTime;
        if (timer <= 0f) { Refresh(); timer = 2f; }
    }

    void Refresh()
    {
        if (parcels.Count == 0) return;
        float totalQ = 0f, totalH = 0f, totalP = 0f;
        good = poor = critical = 0;

        foreach (var p in parcels)
        {
            if (p?.composition == null) continue;
            float q = p.LatestAnalysis.qualityScore;
            totalQ += q;
            totalH += p.composition.moisture;
            totalP += p.composition.pH;
            if (q >= 65f) good++;
            else if (q >= 35f) poor++;
            else critical++;
        }

        int n = parcels.Count;
        avgQuality = totalQ / n;
        avgHumidity = totalH / n;
        avgPH = totalP / n;
        robots = spawner != null ? (spawner.GetRobots()?.Count ?? 0) : 0;
    }

    void OnGUI()
    {
        if (!show) return;
        Rect panel = new Rect(Screen.width - 195, 150, 180, 200);
        theme.DrawPanel(panel);
        float y = panel.y + 10, x = panel.x + 12;

        GUI.Label(new Rect(x, y, 160, 18), "Statistici Ferma", theme.Title);
        y += 22;
        Row(x, ref y, "Parcele", parcels.Count.ToString());
        Row(x, ref y, "Roboti", robots.ToString());
        y += 5;
        Row(x, ref y, "Calitate", $"{avgQuality:F0}%", theme.GetQualityStyle(avgQuality));
        Row(x, ref y, "pH", $"{avgPH:F1}");
        Row(x, ref y, "Umiditate", $"{avgHumidity:F0}%");
        y += 5;
        Row(x, ref y, "Bune", good.ToString(), theme.Good);
        Row(x, ref y, "Slabe", poor.ToString(), theme.Warn);
        Row(x, ref y, "Critice", critical.ToString(), theme.Bad);
    }

    void Row(float x, ref float y, string label, string value, GUIStyle style = null)
    {
        GUI.Label(new Rect(x, y, 90, 16), label, theme.Label);
        GUI.Label(new Rect(x + 92, y, 70, 16), value, style ?? theme.Label);
        y += 17;
    }
}
