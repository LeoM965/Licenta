using UnityEngine;
using Economics.Models;
using Economics.Managers;
using UI.Utils;
using System.Collections.Generic;

namespace UI.Menus.Tabs
{
    public class HistoryDashboardTab : BaseDashboardTab
    {
        private readonly float[] colOffsets = { 0, 80, 180, 280, 380, 480, 580 };

        public override void DrawTab(float x, float y, UITheme theme)
        {
            if (EconomicsHistoryManager.Instance == null)
            {
                GUI.Label(new Rect(x, y, 400, 20), "Managerul de istoric nu a fost găsit în scenă.", theme.Bad);
                return;
            }

            var history = EconomicsHistoryManager.Instance.History;

            if (history.Count == 0)
            {
                GUI.Label(new Rect(x, y, 400, 20), "Încă nu există date istorice. Așteaptă să treacă prima zi!", theme.Warn);
                return;
            }

            DrawComparisonSummary(x, y, history, theme);
            y += 70;

            DrawSectionTitle(x, ref y, "ISTORIC ZILNIC DETALIAT", theme);

            DrawTableHeader(x, y, theme);
            y += 20;

            DrawScrollableArea(x, ref y, 610, 200, history.Count, 20, (rowY) => {
                for (int i = history.Count - 1; i >= 0; i--)
                {
                    DrawHistoryRow(0, rowY, history[i], theme);
                    rowY += 20;
                }
            });
        }

        private void DrawComparisonSummary(float x, float y, List<DailySnapshot> history, UITheme theme)
        {
            var latest = history[history.Count - 1];
            
            GUI.Label(new Rect(x, y, 400, 18), "ANALIZĂ EVOLUȚIE (IERI vs AZI)", theme.Value);
            
            // Adaugă butonul de Export în dreapta titlului
            if (GUI.Button(new Rect(x + 450, y, 150, 25), "EXPORT EXCEL (CSV)"))
            {
                EconomicsHistoryManager.Instance.ExportToCSV();
            }

            y += 22;

            float profitDelta = latest.ProfitDelta;
            string deltaSign = profitDelta >= 0 ? "+" : "";
            GUIStyle deltaStyle = profitDelta >= 0 ? theme.Good : theme.Bad;

            GUI.Label(new Rect(x, y, 200, 16), "Evoluție Profit Net:", theme.Label);
            GUI.Label(new Rect(x + 150, y, 150, 16), $"{deltaSign}{profitDelta:F2} €", deltaStyle);

            y += 18;
            float revDelta = latest.RevenueDelta;
            string revSign = revDelta >= 0 ? "+" : "";
            GUIStyle revStyle = revDelta >= 0 ? theme.Good : theme.Bad;

            GUI.Label(new Rect(x, y, 200, 16), "Evoluție Venituri:", theme.Label);
            GUI.Label(new Rect(x + 150, y, 150, 16), $"{revSign}{revDelta:F2} €", revStyle);
        }

        private void DrawTableHeader(float x, float y, UITheme theme)
        {
            string[] headers = { "Ziua", "Profit €", "Venit €", "Costuri €", "Kg Total", "Plante" };
            UIDrawUtils.DrawRow(x, y, colOffsets, headers, theme.Value, colWidth: 90f);
            UIDrawUtils.DrawHorizontalLine(x, y + 18, 610);
        }

        private void DrawHistoryRow(float x, float y, DailySnapshot s, UITheme theme)
        {
            string[] values = {
                $"Ziua {s.Day}",
                s.NetProfit.ToString("F1") + " €",
                s.TotalRevenue.ToString("F1") + " €",
                s.TotalCosts.ToString("F1") + " €",
                s.TotalWeightKg.ToString("F1"),
                s.TotalPlants.ToString()
            };

            GUIStyle[] styles = {
                theme.Label,
                theme.GetProfitStyle(s.NetProfit),
                theme.Label,
                theme.Label,
                theme.Label,
                theme.Label
            };

            UIDrawUtils.DrawRow(x, y, colOffsets, values, styles, colWidth: 90f);
        }
    }
}
