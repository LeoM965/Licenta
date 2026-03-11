using UnityEngine;
using AI.Analytics;
using AI.Models.Decisions;

public class DecisionPanel : MonoBehaviour
{
    [SerializeField] private UITheme theme;
    [SerializeField] private KeyCode toggleKey = KeyCode.D;
    [SerializeField] private RobotSelector selector;

    private bool show = true;

    private void Start()
    {
        if (selector == null)
            selector = FindFirstObjectByType<RobotSelector>();
        if (selector == null)
            selector = gameObject.AddComponent<RobotSelector>();
        if (theme == null)
        {
            var infoPanel = GetComponent<RobotInfoPanel>();
            if (infoPanel != null) theme = infoPanel.Theme;
        }
        if (DecisionTracker.Instance == null)
        {
            GameObject trackerGo = new GameObject("DecisionTracker");
            trackerGo.AddComponent<DecisionTracker>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey)) show = !show;
    }

    private void OnGUI()
    {
        if (!show || theme == null || selector == null || selector.Selected == null) return;
        DrawDecisionPanel(selector.Selected);
    }

    private void DrawDecisionPanel(Transform selected)
    {
        DecisionRecord decision = null;
        if (DecisionTracker.Instance != null)
            decision = DecisionTracker.Instance.GetLastDecision(selected);

        float panelWidth = 300f;
        float panelHeight = decision != null ? 300f : 60f;
        
        // Position to the right of screen center
        Rect r = new Rect((Screen.width - 220) / 2 + 230, 12, panelWidth, panelHeight);

        theme.DrawPanel(r);
        float y = r.y + 8, x = r.x + 10;
        GUI.Label(new Rect(x, y, 200, 18), "OPTIMIZARE DECIZII", theme.Title);
        y += 22;

        if (decision == null)
        {
            GUI.Label(new Rect(x, y, 220, 16), "Aștept prima decizie...", theme.Label);
            return;
        }

        GUI.Label(new Rect(x, y, 240, 16), $"Parcel: {decision.parcelName}", theme.Label);
        y += 18;

        GUI.Label(new Rect(x, y, 140, 16), $"✓ {decision.chosenOption}", theme.Good);
        GUI.Label(new Rect(x + 145, y, 80, 16), $"{decision.chosenScore:F1} pts", theme.Value);
        y += 20;

        GUI.Box(new Rect(x, y, panelWidth - 20, 1), "");
        y += 5;

        GUI.Label(new Rect(x, y, 200, 16), "Alternative evaluate:", theme.Label);
        y += 16;

        int maxAlternatives = Mathf.Min(5, decision.alternatives.Count);
        for (int i = 0; i < maxAlternatives; i++)
        {
            DecisionAlternative alt = decision.alternatives[i];
            string prefix = alt.isChosen ? "✓" : "  ";
            GUIStyle style = alt.isChosen ? theme.Good : theme.Label;

            GUI.Label(new Rect(x, y, 150, 15), $"{prefix} {alt.name}", style);
            GUI.Label(new Rect(x + 155, y, 60, 15), $"{alt.score:F1}", style);

            GUITools.DrawBar(new Rect(x + 210, y + 3, 40, 10), alt.score / 100f, GUITools.GetScoreColor(alt.score));
            y += 15;
        }

        y += 15;
        GUI.Label(new Rect(x, y, 200, 16), "Factori decizie:", theme.Label);
        y += 16;

        foreach (var f in new[] { ("pH", decision.factors.phScore), ("Umiditate", decision.factors.humidityScore), ("N", decision.factors.nitrogenScore), ("P", decision.factors.phosphorusScore), ("K", decision.factors.potassiumScore) })
        {
            GUI.Label(new Rect(x, y, 80, 14), f.Item1, theme.Label);
            GUITools.DrawBar(new Rect(x + 85, y + 2, 100, 10), f.Item2 / 100f, GUITools.GetScoreColor(f.Item2));
            GUI.Label(new Rect(x + 190, y, 40, 14), $"{f.Item2:F0}%", theme.Value);
            y += 15;
        }

        y += 8;
        int totalDecisions = DecisionTracker.Instance.GetTotalDecisions(selected);
        float avgScore = DecisionTracker.Instance.GetAverageScore(selected);
        GUI.Label(new Rect(x, y, 240, 16), $"Decizii: {totalDecisions} | Scor: {avgScore:F1}", theme.Label);
    }
}
