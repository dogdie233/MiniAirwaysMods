using HarmonyLib;

using System;
using System.Linq;

using Tutorial;

using UnityEngine;

namespace GoodlifeShortcut;

[HarmonyPatch]
public class RightClickToDestPatch
{
    [HarmonyPatch(typeof(AircraftManager), "Update", new Type[0])]
    [HarmonyPostfix]
    public static void Update(ref AircraftManager __instance, ref Camera ____camera)
    {
        if (!Input.GetMouseButtonDown(1) || (TutorialWindowManager.Instance != null && TutorialWindowManager.Instance.HasShowingTutorial) || UpgradeManager.Instance.HasShowingUpgradePanel || LevelManager.Instance.ShowingOptions || LevelManager.Instance.ShowingPauseMenu || GameOverManager.Instance.GameOverFlag || TakeoffTask.CurrentCommandingTakeoffTask != null || WaypointPropsManager.Instance.HasPlacingProps)
        {
            return;
        }
        var vector = ____camera.ScreenToWorldPoint(Input.mousePosition);
        float num = float.PositiveInfinity;
        Aircraft aircraft = null;
        float num2 = 0.15384616f * Camera.main.orthographicSize;
        foreach (Aircraft aircraft3 in AircraftManager.GetAircraft())
        {
            if (aircraft3.direction != Aircraft.Direction.Outbound || aircraft3.state == Aircraft.State.TakingOff )
                continue;

            float num3 = Vector2.Distance(aircraft3.gameObject.transform.position, vector);
            if (num3 < num && num3 <= num2)
            {
                num = num3;
                aircraft = aircraft3;
            }
        }

        if (aircraft == null)
            return;

        var dest = WaypointManager.Instance.GetAllWaypoints().FirstOrDefault(waypoint => waypoint.colorCode == aircraft.colorCode && waypoint.shapeCode == aircraft.shapeCode);
        if (dest != null)
            aircraft.SetVectorTo(dest.transform);
    }
}
