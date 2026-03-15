using UnityEngine;
using Sensors.Components;
using TMPro;

namespace UI.Canvas
{
    public class ParcelInfo : InteractivePanel
    {
        private TextMeshProUGUI idTxt, qualityTxt, moistureTxt, phTxt, nTxt, pTxt, kTxt, cropTxt, stageTxt, progressTxt;

        protected override void OnInitialize()
        {
            idTxt = GetText("Visuals/Row_Name/Val");
            qualityTxt = GetText("Visuals/Row_Quality/Val");
            moistureTxt = GetText("Visuals/Row_Moisture/Val");
            phTxt = GetText("Visuals/Row_PH/Val");
            nTxt = GetText("Visuals/Row_N/Val");
            pTxt = GetText("Visuals/Row_P/Val");
            kTxt = GetText("Visuals/Row_K/Val");
            cropTxt = GetText("Visuals/Row_Crops/Val");
            stageTxt = GetText("Visuals/Row_Stage/Val");
            progressTxt = GetText("Visuals/Row_Progress/Val");
        }

        protected override void OnRefresh()
        {
            var sel = selectedTarget.GetComponent<EnvironmentalSensor>();
            if (sel == null) return;

            if (idTxt) idTxt.text = sel.name;
            if (qualityTxt) qualityTxt.text = $"{sel.soilQuality:F1}%";
            if (moistureTxt) moistureTxt.text = $"{sel.soilMoisture:F1}%";
            if (phTxt) phTxt.text = $"{sel.soilPH:F1}";
            if (nTxt) nTxt.text = $"{sel.nitrogen:F1}";
            if (pTxt) pTxt.text = $"{sel.phosphorus:F1}";
            if (kTxt) kTxt.text = $"{sel.potassium:F1}";

            bool isEmpty = string.IsNullOrEmpty(sel.plantedVarietyName) || sel.plantedVarietyName == "None";
            if (cropTxt) cropTxt.text = isEmpty ? "Niciuna" : sel.plantedVarietyName;
            if (stageTxt) stageTxt.text = isEmpty ? "-" : GetStage(sel.currentGrowthStage);
            if (progressTxt) progressTxt.text = isEmpty ? "-" : $"{sel.growthProgress:F1}%";
        }

        protected override Transform FindTarget(Transform t)
        {
            var s = t.GetComponentInParent<EnvironmentalSensor>();
            return s != null ? s.transform : null;
        }

        private string GetStage(CropStage s) => s switch
        {
            CropStage.Seed => "Sămânță",
            CropStage.Seedling => "Răsad",
            CropStage.Growing => "Crestere",
            CropStage.Mature => "Matur",
            _ => "N/A"
        };


    }
}
