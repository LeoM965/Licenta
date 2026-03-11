using UnityEngine;
using Economics.Models;
using UI.Utils;

namespace UI.Menus.Tabs
{
    public class CropDashboardTab
    {
        private readonly float[] colOffsets = { 0, 110, 160, 230, 305, 380, 455, 525, 595 };

        public void DrawTab(float x, float y, EconomicReport activeReport, CropData[] crops, UITheme theme)
        {
            DrawTableHeader(x, y, theme);
            y += 20;

            foreach (var crop in crops)
            {
                activeReport.AnalysisByVariety.TryGetValue(crop.name, out var stats);
                DrawDataRow(x, y, crop.name, stats, theme, isTotalRow: false);
                y += 17;
            }

            UIDrawUtils.DrawHorizontalLine(x, y + 4, 610);
            
            float cropOnlyProfit = activeReport.FarmTotals.TotalRevenue - activeReport.FarmTotals.TotalSeedCost;
            DrawDataRowCustom(x, y + 12, "TOTAL CULTURI", activeReport.FarmTotals, cropOnlyProfit, theme, isTotalRow: true);
            
            y += 55;
            DrawOperationalBreakdown(x, y, activeReport.FarmTotals, theme);
        }

        private void DrawTableHeader(float x, float y, UITheme theme)
        {
            string[] headers = { "Cultură", "Plant", "Cost €", "Venit €", "Kg", "Profit €", "ROI %", "Fit %" };
            for (int i = 0; i < headers.Length; i++)
                GUI.Label(new Rect(x + colOffsets[i], y, 65, 16), headers[i], theme.Value);
        }

        private void DrawDataRow(float x, float y, string label, CropStats s, UITheme theme, bool isTotalRow)
        {
            DrawDataRowCustom(x, y, label, s, s.NetProfit, theme, isTotalRow);
        }

        private void DrawDataRowCustom(float x, float y, string label, CropStats s, float profitToShow, UITheme theme, bool isTotalRow)
        {
            GUIStyle labelStyle = isTotalRow ? theme.Value : ((s.TotalPlants + s.HarvestedPlants) > 0 ? theme.Good : theme.Label);
            GUIStyle dataStyle = isTotalRow ? theme.Value : theme.Label;

            string plantText = s.HarvestedPlants > 0 
                ? $"{s.TotalPlants}+{s.HarvestedPlants}" 
                : s.TotalPlants.ToString();

            GUI.Label(new Rect(x + colOffsets[0], y, 105, 15), label, labelStyle);
            GUI.Label(new Rect(x + colOffsets[1], y, 45, 15), plantText, dataStyle);
            GUI.Label(new Rect(x + colOffsets[2], y, 50, 15), s.TotalSeedCost.ToString("F1"), dataStyle);
            GUI.Label(new Rect(x + colOffsets[3], y, 55, 15), s.TotalRevenue.ToString("F1"), dataStyle);
            GUI.Label(new Rect(x + colOffsets[4], y, 55, 15), s.TotalWeightKg.ToString("F1"), dataStyle);
            
            GUI.Label(new Rect(x + colOffsets[5], y, 50, 15), profitToShow.ToString("F1"), theme.GetProfitStyle(profitToShow));
            
            float roi = (s.TotalSeedCost > 0) ? (profitToShow / s.TotalSeedCost) * 100f : 0f;
            GUI.Label(new Rect(x + colOffsets[6], y, 45, 15), roi.ToString("F0") + "%", theme.GetProfitStyle(roi));
            GUI.Label(new Rect(x + colOffsets[7], y, 40, 15), s.AvgSoilCompatibility.ToString("F0") + "%", theme.GetProfitStyle(s.AvgSoilCompatibility - 50f));
        }

        private void DrawOperationalBreakdown(float x, float y, CropStats totals, UITheme theme)
        {
            GUI.Label(new Rect(x, y, 320, 18), "DETALII COSTURI OPERAȚIONALE (LOGISTICĂ)", theme.Value);
            y += 26;
            
            float labelWidth = 160;
            float indent = 12;
            float valueX = x + indent + labelWidth;

            DrawSmallRow(x + indent, valueX, ref y, "Venit Recoltat:", totals.HarvestedRevenue.ToString("F2") + " €", theme.Label, theme.Good);
            DrawSmallRow(x + indent, valueX, ref y, "Venit în Câmp:", totals.FieldRevenue.ToString("F2") + " €", theme.Label, theme.Value);
            y += 4;
            DrawSmallRow(x + indent, valueX, ref y, "Consum Energie:", totals.TotalEnergyCost.ToString("F2") + " €", theme.Label, theme.Value);
            DrawSmallRow(x + indent, valueX, ref y, "Mentenanță Utilitaje:", totals.TotalMaintenanceCost.ToString("F2") + " €", theme.Label, theme.Value);
            DrawSmallRow(x + indent, valueX, ref y, "Amortizare Active:", totals.TotalDepreciationCost.ToString("F2") + " €", theme.Label, theme.Value);
            
            y += 8;
            UIDrawUtils.DrawHorizontalLine(x + indent, y, 400);
            y += 10;
            DrawSmallRow(x + indent, valueX, ref y, "PROFIT NET FINAL:", totals.NetProfit.ToString("F2") + " €", theme.Label, theme.GetProfitStyle(totals.NetProfit));
        }

        private void DrawSmallRow(float x, float valueX, ref float y, string label, string value, GUIStyle labelStyle, GUIStyle valueStyle)
        {
            GUI.Label(new Rect(x, y, 160, 16), label, labelStyle);
            GUI.Label(new Rect(valueX, y, 120, 16), value, valueStyle);
            y += 18;
        }
    }
}
