using EventDirector;

using HarmonyLib;

using System.Runtime.CompilerServices;

using UnityEngine;

namespace TransparentHandledAircraft;

[HarmonyPatch]
public class AircraftPatch
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UpdateTransparentStatusSafe(Aircraft instance)
    {
        if (instance.GetComponent<AircraftTransparent>() is not { } aircraftTransparent)
            return;  // component not found;
        aircraftTransparent.UpdateEnableStatus();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Aircraft), "Start")]
    public static void StartPrefix(ref Aircraft __instance)
    {
        __instance.gameObject.AddComponent<AircraftTransparent>();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.EnableVisualWarning))]
    public static void UpdateStatusWhenEnableVisualWarning(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.DisableVisualWarning))]
    public static void UpdateStatusWhenDisableVisualWarning(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.SetFlyHeading), [])]
    public static void UpdateStatusWhenSetFlyHeading1(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.SetFlyHeading), [typeof(float)])]
    public static void UpdateStatusWhenSetFlyHeading2(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.SetVectorTo), [typeof(Transform)])]
    public static void UpdateStatusWhenSetVectorTo1(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.SetVectorTo), [typeof(WaypointAutoHover)])]
    public static void UpdateStatusWhenSetVectorTo2(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.SetVectorTo), [typeof(PlaceableWaypoint)])]
    public static void UpdateStatusWhenSetVectorTo3(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.SetWaypoint), [typeof(Waypoint)])]
    public static void UpdateStatusWhenSetWaypoint(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.SetupRegularEvent), [typeof(RegularEventManager.RegularEventType), typeof(bool)])]
    public static void UpdateStatusWhenSetupRegularEvent(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.Land), [typeof(bool)])]
    public static void UpdateStatusWhenLand1(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.Land), [typeof(Runway), typeof(bool)])]
    public static void UpdateStatusWhenLand2(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.TrySetupLanding), [typeof(Runway), typeof(bool)])]
    public static void UpdateStatusWhenTrySetupLanding(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.ClearWaypoint))]
    public static void UpdateStatusWhenClearWaypoint(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), "set_" + nameof(Aircraft.HARWCurWP))]
    public static void UpdateStatusWhenSetHARWCurWP(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Aircraft), "set_" + nameof(Aircraft.NextWayPoint))]
    public static void UpdateStatusWhenSetNextWayPoint(ref Aircraft __instance)
        => UpdateTransparentStatusSafe(__instance);
}
