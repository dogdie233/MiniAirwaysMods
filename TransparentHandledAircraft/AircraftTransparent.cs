using DG.Tweening;

using System.Collections.Generic;

using UnityEngine;

namespace TransparentHandledAircraft;

public class AircraftTransparent : MonoBehaviour
{
    public static float fadeDuration = 0.8f;
    public static float initTransparent = 0.3f;

    private Aircraft aircraft;
    private float _alpha = initTransparent;
    private List<Tweener> alphaTweeners = new();

    public float Alpha
    {
        get => _alpha;
        set
        {
            _alpha = value;
            if (enabled)
                Animate(_alpha);
        }
    }

    public bool IsTransparentConditionMet
    {
        get => !Utils.IsTCASWarning(aircraft) && // no TCAS warning
            !aircraft.manualTurn &&  // not manual turn
            !Utils.IsTCASWarning(aircraft) &&  // not boundary warning
            (aircraft.LandingRunway != null || aircraft.NextWayPoint != null || (aircraft.HARWNxtWP != null && aircraft.state == Aircraft.State.HeadingAfterReachingWaypoint));  // has next waypoint
    }

    public void UpdateEnableStatus()
    {
        if (IsTransparentConditionMet == enabled)
            return;
        enabled = !enabled;
    }

    private void Awake()
    {
        aircraft = GetComponent<Aircraft>();
        if (aircraft == null)
        {
            TransparentHandledAircraftPlugin.MyLogger.LogError($"{nameof(AircraftTransparent)} component was added to a object which not a aircraft wrongly, destroy self");
            Destroy(this);
        }
    }

    private void Start()
    {
        UpdateEnableStatus();
    }

    private void OnEnable()
    {
        Animate(_alpha);
    }

    private void OnDisable()
    {
        Animate(1f);
    }

    private void Animate(float alpha)
    {
        foreach (var tweener in alphaTweeners)
            tweener.Kill();

        alphaTweeners.Clear();
        foreach (var renderer in aircraft.colorReceivers)
            alphaTweeners.Add(renderer.DOFade(alpha, fadeDuration));
        if (aircraft.AP.GetComponent<SpriteRenderer>() is { } apRenderer)
            alphaTweeners.Add(apRenderer.DOFade(alpha, fadeDuration));
        alphaTweeners.Add(aircraft.APStatTri.DOFade(alpha, fadeDuration));
    }
}
