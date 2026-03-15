using UnityEngine;
using Sensors.Components;
using TMPro;

namespace UI.Canvas
{
    public class FarmDashboard : MonoBehaviour
    {
        private TextMeshProUGUI pTxt, rTxt, qTxt, phTxt, hTxt, gTxt, wTxt, bTxt;
        private MultiRobotSpawner spawner;
        private float timer;

        void Start()
        {
            spawner = FindFirstObjectByType<MultiRobotSpawner>();
            pTxt = CanvasHelper.GetText(transform, "Visuals/Row_Parcels/Val"); 
            rTxt = CanvasHelper.GetText(transform, "Visuals/Row_Robots/Val"); 
            qTxt = CanvasHelper.GetText(transform, "Visuals/Row_Quality/Val");
            phTxt = CanvasHelper.GetText(transform, "Visuals/Row_PH/Val"); 
            hTxt = CanvasHelper.GetText(transform, "Visuals/Row_Humidity/Val"); 
            gTxt = CanvasHelper.GetText(transform, "Visuals/Row_Good/Val");
            wTxt = CanvasHelper.GetText(transform, "Visuals/Row_Warn/Val"); 
            bTxt = CanvasHelper.GetText(transform, "Visuals/Row_Bad/Val");
            Refresh();
        }

        void Update()
        {
            if ((timer -= Time.deltaTime) <= 0f) { Refresh(); timer = 2f; }
        }

        private void Refresh()
        {
            var ps = ParcelCache.Parcels;
            if (ps == null || ps.Count == 0) return;

            float q = 0, h = 0, ph = 0; int g = 0, w = 0, b = 0;
            foreach (var p in ps)
            {
                if (p == null) continue;
                q += p.soilQuality; h += p.soilMoisture; ph += p.soilPH;
                if (p.soilQuality >= 65f) g++; else if (p.soilQuality >= 35f) w++; else b++;
            }

            int count = ps.Count;
            if (pTxt) pTxt.text = count.ToString();
            if (rTxt) rTxt.text = (spawner?.GetRobots()?.Count ?? 0).ToString();
            if (qTxt) qTxt.text = $"{(q / count):F0}%";
            if (phTxt) phTxt.text = $"{(ph / count):F1}";
            if (hTxt) hTxt.text = $"{(h / count):F0}%";
            if (gTxt) gTxt.text = g.ToString();
            if (wTxt) wTxt.text = w.ToString();
            if (bTxt) bTxt.text = b.ToString();
        }


    }
}
