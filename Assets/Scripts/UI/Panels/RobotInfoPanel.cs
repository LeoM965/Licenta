using UnityEngine;

public class RobotInfoPanel : MonoBehaviour
{
    [SerializeField] private UITheme theme;
    public UITheme Theme => theme;
    [SerializeField] private RobotSelector selector;
    
    private void Start()
    {
        var spawner = FindFirstObjectByType<MultiRobotSpawner>();
        if (selector == null)
            selector = GetComponent<RobotSelector>() ?? gameObject.AddComponent<RobotSelector>();
        
        if (GetComponent<DecisionPanel>() == null && FindFirstObjectByType<DecisionPanel>() == null)
            gameObject.AddComponent<DecisionPanel>();
    }
}
