using UnityEngine;

public class RobotLifecycle : MonoBehaviour
{
    private void OnEnable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.RegisterRobot();
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.UnregisterRobot();
    }
}
