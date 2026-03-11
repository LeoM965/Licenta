using UnityEngine;

namespace Settings
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] UITheme theme;
        bool isOpen;
        CropDatabase cropDB;
        string[] cropNames;
        Vector2 scroll;

        void Start()
        {
            cropDB = CropLoader.Load();
            SimulationSettings.InitFromDatabase(cropDB);
            int count = cropDB.crops.Length;
            cropNames = new string[count + 1];
            cropNames[0] = "Auto";
            for (int i = 0; i < count; i++) cropNames[i + 1] = cropDB.crops[i].name;
        }

        void Update() { if (Input.GetKeyDown(KeyCode.S)) isOpen = !isOpen; }

        void OnGUI()
        {
            if (!isOpen || theme == null) return;
            float w = 420f, h = 480f;
            Rect p = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);
            theme.DrawPanel(p);

            float x = p.x + 20, y = p.y + 15;

            // Header
            GUI.Label(new Rect(x, y, 380, 30), "CONFIGURARE PLANTE", theme.Header);
            y += 40;

            // Plante per rand
            GUI.Label(new Rect(x, y, 130, 20), "Plante per rând:", theme.Label);
            SimulationSettings.PlantsPerRow = (int)GUI.HorizontalSlider(new Rect(x + 140, y + 5, 140, 20), SimulationSettings.PlantsPerRow, 1, 20);
            GUI.Label(new Rect(x + 290, y, 30, 20), SimulationSettings.PlantsPerRow.ToString(), theme.Value);
            y += 30;

            // Tip cultura
            int idx = SimulationSettings.SelectedCropIndex + 1;
            GUI.Label(new Rect(x, y, 130, 20), "Tip cultură:", theme.Label);
            if (GUI.Button(new Rect(x + 140, y, 25, 22), "<", theme.Button)) idx = (idx - 1 + cropNames.Length) % cropNames.Length;
            GUI.Label(new Rect(x + 170, y, 110, 22), cropNames[idx], theme.Value);
            if (GUI.Button(new Rect(x + 285, y, 25, 22), ">", theme.Button)) idx = (idx + 1) % cropNames.Length;
            SimulationSettings.SelectedCropIndex = idx - 1;
            y += 35;

            // Robot Economics section
            GUI.Label(new Rect(x, y, 380, 20), "ECONOMIE ROBOT", theme.Title);
            y += 25;

            GUI.Label(new Rect(x, y, 140, 20), "Preț Energie (€/kWh):", theme.Label);
            SimulationSettings.EnergyPrice = GUI.HorizontalSlider(new Rect(x + 160, y + 5, 140, 20), SimulationSettings.EnergyPrice, 0.05f, 1.00f);
            GUI.Label(new Rect(x + 310, y, 60, 20), SimulationSettings.EnergyPrice.ToString("F2"), theme.Value);
            y += 35;

            // Separator
            GUI.Label(new Rect(x, y, 380, 20), "PREȚURI PER CULTURĂ", theme.Title);
            y += 25;

            // Definition for small font style
            GUIStyle small = new GUIStyle(theme.Label) { fontSize = 10 };

            // Crop list with scroll view
            if (cropDB?.crops != null && SimulationSettings.SeedCosts != null)
            {
                Rect scrollArea = new Rect(x, y, 380, p.yMax - y - 55);
                Rect content = new Rect(0, 0, 360, cropDB.crops.Length * 110); // Adjusted height for multi-row items
                scroll = GUI.BeginScrollView(scrollArea, scroll, content);

                float sy = 0;
                for (int i = 0; i < cropDB.crops.Length; i++)
                {
                    // Row for the crop name
                    GUI.Label(new Rect(0, sy, 350, 20), cropDB.crops[i].name, theme.Title);
                    sy += 20;

                    // Row for Seed Cost
                    GUI.Label(new Rect(0, sy, 80, 20), "Sămânță (€):", small);
                    SimulationSettings.SeedCosts[i] = GUI.HorizontalSlider(new Rect(85, sy + 5, 200, 20), SimulationSettings.SeedCosts[i], 0.001f, 0.5f);
                    GUI.Label(new Rect(290, sy, 60, 20), SimulationSettings.SeedCosts[i].ToString("F3"), theme.Value);
                    sy += 22;

                    // Row for Weight
                    GUI.Label(new Rect(0, sy, 80, 20), "Rezultat (kg):", small);
                    SimulationSettings.YieldWeights[i] = GUI.HorizontalSlider(new Rect(85, sy + 5, 200, 20), SimulationSettings.YieldWeights[i], 0.01f, 25f);
                    GUI.Label(new Rect(290, sy, 60, 20), SimulationSettings.YieldWeights[i].ToString("F2"), theme.Value);
                    sy += 22;

                    // Row for Price
                    GUI.Label(new Rect(0, sy, 80, 20), "Preț/kg (€):", small);
                    SimulationSettings.MarketPrices[i] = GUI.HorizontalSlider(new Rect(85, sy + 5, 200, 20), SimulationSettings.MarketPrices[i], 0.1f, 5f);
                    GUI.Label(new Rect(290, sy, 60, 20), SimulationSettings.MarketPrices[i].ToString("F2"), theme.Value);
                    
                    sy += 45; // Spacing between crops
                }
                GUI.EndScrollView();
            }

            // Footer
            GUI.Label(new Rect(p.x, p.yMax - 28, w, 20), "Apasă S pentru a închide", theme.Footer);
            if (GUI.Button(new Rect(p.xMax - 30, p.y + 5, 25, 25), "X", theme.Bad)) isOpen = false;
        }
    }
}
