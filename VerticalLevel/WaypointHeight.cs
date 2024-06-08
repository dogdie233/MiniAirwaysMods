using System.ComponentModel;
using System.Linq;
using TMPro;

using UnityEngine;
using UnityEngine.Rendering;

namespace VerticalLevel;

public class WaypointHeight : MonoBehaviour
{
    private TMP_Text m_Text;
    private float height;
    private Waypoint bindingWaypoint;

    public float Height
    {
        get => height;
        set
        {
            value = Mathf.Clamp(value, 1000f, 10000f);
            if (height == value)
                return;

            height = value;
            foreach (var aircraft in AircraftManager.Instance.inboundAircraft.Concat(AircraftManager.Instance.outboundAircraft))
            {
                if ((aircraft.state == Aircraft.State.AutoHover && aircraft.NextWayPoint == transform) ||
                    (aircraft.state == Aircraft.State.HeadingAfterReachingWaypoint && FastMemberHelper<Aircraft, PlaceableWaypoint>.Get("HARWNxtWP", aircraft) == bindingWaypoint))
                    aircraft.GetComponent<AircraftHeight>().targetHeight = height;
            }

            if (m_Text != null)
                m_Text.text = $"FL{height / 100f:000}";
        }
    }

    private void Start()
    {
        bindingWaypoint = GetComponent<Waypoint>();
        GameObject obj = new GameObject("Text");
        m_Text = obj.AddComponent<TextMeshPro>();
        m_Text.fontSize = 2;
        m_Text.horizontalAlignment = HorizontalAlignmentOptions.Left;
        m_Text.verticalAlignment = VerticalAlignmentOptions.Top;
        m_Text.rectTransform.sizeDelta = new Vector2(2, 1);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(1, -3f, 5);

        // make sorting layer of obj "Text"
        SortingGroup sg = obj.AddComponent<SortingGroup>();
        sg.sortingLayerName = "Text";
        sg.sortingOrder = 1;

        Height = 3000f;
    }

    private void Update()
    {
        var bound = Aircraft.MagnetDist / 6.5f * Camera.main.orthographicSize;
        if (Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) > bound)
            return;

        if (Input.mouseScrollDelta.y * Time.deltaTime >= 0.01f)
            Height += 1000f;
        else if (Input.mouseScrollDelta.y * Time.deltaTime <= -0.01f)
            Height -= 1000f;
    }
}
