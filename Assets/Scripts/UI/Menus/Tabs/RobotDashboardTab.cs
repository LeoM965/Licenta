using UnityEngine;
using System.Collections.Generic;
using Economics.Managers;
using UI.Utils;
using Economics.Models;

namespace UI.Menus.Tabs
{
    public class RobotDashboardTab
    {
        private string filterZone = "Toate";
        private readonly float[] robotColOffsets = { 0, 45, 115, 250, 310, 370, 435, 505, 575 };

        private struct CachedRobotData
        {
            public string Zone;
            public string Type;
            public string StatusText;
            public float BatteryPct;
            public float WorkHours;
            public float DistKm;
            public float TotalCost;
            public float RevenueGenerated;
            public float ROI;
        }
        private List<CachedRobotData> cachedRobots = new List<CachedRobotData>();

        public void CacheRobotData()
        {
            cachedRobots.Clear();
            if (RobotEconomicsManager.Instance == null) return;

            var robotStatsMap = RobotEconomicsManager.Instance.RobotStatsMap;
            var sortedRobots = new List<KeyValuePair<Transform, RobotStats>>(robotStatsMap);
            sortedRobots.Sort((a, b) => {
                int zoneCompare = a.Value.zone.CompareTo(b.Value.zone);
                if (zoneCompare != 0) return zoneCompare;
                return a.Value.type.CompareTo(b.Value.type);
            });

            foreach (var kvp in sortedRobots)
            {
                var stats = kvp.Value;
                var robotTransform = kvp.Key;
                
                string statusText = "Idle";
                float batteryPct = 100f;

                if (robotTransform != null)
                {
                    var op = robotTransform.GetComponent<RobotOperator>();
                    if (op != null)
                        statusText = op.CurrentState.ToString();

                    var energy = robotTransform.GetComponent<RobotEnergy>();
                    if (energy != null) batteryPct = energy.BatteryPercent * 100f;
                }

                cachedRobots.Add(new CachedRobotData
                {
                    Zone = stats.zone,
                    Type = stats.type,
                    StatusText = statusText,
                    BatteryPct = batteryPct,
                    WorkHours = stats.time / 3600f,
                    DistKm = stats.distance / 1000f,
                    TotalCost = stats.TotalCost,
                    RevenueGenerated = stats.revenueGenerated,
                    ROI = stats.ROI
                });
            }
        }

        public void DrawTab(float x, float y, UITheme theme)
        {
            GUI.Label(new Rect(x, y, 300, 20), "TABEL ANALIZĂ FLOTĂ ROBOȚI", theme.Value);
            y += 30;

            float bx = x + 340;
            string[] zones = { "Toate", "A", "B", "C", "D" };
            foreach (var z in zones)
            {
                if (GUI.Button(new Rect(bx, y, 45, 18), z, filterZone == z ? theme.Value : theme.Label))
                    filterZone = z;
                bx += 48;
            }
            y += 25;

            string[] headers = { "Zone", "Model", "Activity", "Bat.", "Hours", "Dist(km)", "Cost(€)", "Venit(€)", "ROI %" };
            for (int i = 0; i < headers.Length; i++)
                GUI.Label(new Rect(x + robotColOffsets[i], y, 100, 16), headers[i], theme.Value);
            
            y += 22;
            UIDrawUtils.DrawHorizontalLine(x, y, 610);
            y += 8;

            foreach (var data in cachedRobots)
            {
                if (filterZone != "Toate" && data.Zone != filterZone) continue;

                GUIStyle batStyle = data.BatteryPct > 60 ? theme.Good : (data.BatteryPct > 25 ? theme.Warn : theme.Bad);
                
                // Resolve style based on activity text
                GUIStyle statusStyle = theme.Label;
                if (data.StatusText == "Working" || data.StatusText == "MovingToParcel") statusStyle = theme.Good;
                else if (data.StatusText == "Charging") statusStyle = theme.Bad;
                else if (data.StatusText != "Idle") statusStyle = theme.Warn;

                GUI.Label(new Rect(x + robotColOffsets[0], y, 50, 15), $"[{data.Zone}]", theme.Good);
                GUI.Label(new Rect(x + robotColOffsets[1], y, 70, 15), data.Type, theme.Label);
                GUI.Label(new Rect(x + robotColOffsets[2], y, 130, 15), data.StatusText, statusStyle);
                GUI.Label(new Rect(x + robotColOffsets[3], y, 50, 15), $"{data.BatteryPct:F0}%", batStyle);
                GUI.Label(new Rect(x + robotColOffsets[4], y, 60, 15), data.WorkHours.ToString("F2") + "h", theme.Label);
                GUI.Label(new Rect(x + robotColOffsets[5], y, 60, 15), data.DistKm.ToString("F2") + "km", theme.Label);
                GUI.Label(new Rect(x + robotColOffsets[6], y, 60, 15), data.TotalCost.ToString("F1") + "€", theme.Label);
                GUI.Label(new Rect(x + robotColOffsets[7], y, 60, 15), data.RevenueGenerated.ToString("F1") + "€", theme.Good);
                GUI.Label(new Rect(x + robotColOffsets[8], y, 80, 15), data.ROI.ToString("F2") + "%", theme.GetProfitStyle(data.ROI));
                
                y += 18;
            }
        }
    }
}
