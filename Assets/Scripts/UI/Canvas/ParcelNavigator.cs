using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sensors.Components;
using System.Collections;

namespace UI.Canvas
{
    public class ParcelNavigator : MonoBehaviour
    {
        private TMP_InputField input;
        private TextMeshProUGUI msg;
        private RobotCamera robotCam;
        private GameObject visuals;

        void Start()
        {
            robotCam = FindFirstObjectByType<RobotCamera>();
            visuals = transform.Find("Visuals")?.gameObject;
            if (visuals)
            {
                input = visuals.transform.Find("Input")?.GetComponent<TMP_InputField>();
                msg = visuals.transform.Find("Msg")?.GetComponent<TextMeshProUGUI>();
                visuals.transform.Find("Btn")?.GetComponent<Button>()?.onClick.AddListener(Navigate);
                visuals.SetActive(false);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (visuals && visuals.activeSelf)
                {
                    if (string.IsNullOrWhiteSpace(input.text)) visuals.SetActive(false);
                    else Navigate();
                }
                else if (visuals)
                {
                    visuals.SetActive(true);
                    input.text = "";
                    input.Select();
                    input.ActivateInputField();
                    if (msg) msg.text = "";
                }
            }

            if (visuals && visuals.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Return)) Navigate();
                if (Input.GetKeyDown(KeyCode.Escape)) visuals.SetActive(false);
            }
        }

        private void Navigate()
        {
            string query = input.text.Trim().ToUpper();
            if (query.EndsWith("G")) query = query.Substring(0, query.Length - 1).Trim(); // Strip 'G' if pressed while input active
            if (string.IsNullOrEmpty(query)) return;

            EnvironmentalSensor target = null;
            foreach (var p in ParcelCache.Parcels)
            {
                if (p == null) continue;
                string n = p.name.ToUpper();
                if (n.EndsWith("_" + query) || n.EndsWith(" " + query) || n == query || n == "PARCEL_" + query)
                {
                    target = p;
                    break;
                }
            }

            if (target != null)
            {
                robotCam?.SetTarget(target.transform);
                visuals.SetActive(false);
            }
            else if (msg)
            {
                msg.text = $"{query} : NEGĂSIT";
                msg.color = CanvasHelper.Bad;
                StopAllCoroutines();
                StartCoroutine(ClearMessage());
            }
        }

        private IEnumerator ClearMessage() { yield return new WaitForSeconds(2); if (msg) msg.text = ""; }
    }
}
