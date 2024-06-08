using TMPro;

using UnityEngine;
using UnityEngine.Rendering;

namespace VerticalLevel;

public class AircraftStatHelper : MonoBehaviour
{
    private TMP_Text m_Text;
    public Aircraft m_Aircraft;
    bool inited = false;

    void Start()
    {
        GameObject obj = new GameObject("Text");
        m_Text = obj.AddComponent<TextMeshPro>();
        m_Text.fontSize = 2;
        m_Text.horizontalAlignment = HorizontalAlignmentOptions.Left;
        m_Text.verticalAlignment = VerticalAlignmentOptions.Top;
        m_Text.rectTransform.sizeDelta = new Vector2(2, 1);
        obj.transform.SetParent(m_Aircraft.transform);
        obj.transform.localPosition = new Vector3(1, -3f, 5);

        // make sorting layer of obj "Text"
        SortingGroup sg = obj.AddComponent<SortingGroup>();
        sg.sortingLayerName = "Text";
        sg.sortingOrder = 1;

        inited = true;
    }

    void Update()
    {
        if (m_Aircraft == null)
            Destroy(gameObject);

        if (!inited || m_Text == null)
        {
            return;
        }

        m_Text.text = $@"{m_Aircraft.callsign}
{Mathf.RoundToInt(m_Aircraft.heading):000} | {Mathf.RoundToInt(m_Aircraft.speed)}  -  A340/H";

        AircraftHeight ah = m_Aircraft.GetComponent<AircraftHeight>();
        if (ah != null)
        {
            m_Text.text += $"\nFL{ah.height / 100f:000}";

            // if (ah.targetHeight != ah.height)
            if (true)
            {
                m_Text.text += $"\nFL{ah.targetHeight / 100f:000}";
            }
        }
    }
}