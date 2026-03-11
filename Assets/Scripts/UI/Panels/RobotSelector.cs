using UnityEngine;

public class RobotSelector : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;
    
    private Camera cam;
    private MultiRobotSpawner spawner;
    private bool enabled_ = true;

    public Transform Selected { get; private set; }
    public bool IsEnabled => enabled_;

    private void Start()
    {
        cam = Camera.main;
        spawner = FindFirstObjectByType<MultiRobotSpawner>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey)) enabled_ = !enabled_;
        if (!enabled_) return;
        HandleClick();
    }

    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500f))
            Selected = FindRobot(hit.transform);
    }

    private Transform FindRobot(Transform tr)
    {
        if (spawner == null) return null;
        while (tr != null)
        {
            foreach (var robot in spawner.GetRobots())
                if (robot != null && robot.transform == tr) return tr;
            tr = tr.parent;
        }
        return null;
    }

    public void ClearSelection() => Selected = null;
}
