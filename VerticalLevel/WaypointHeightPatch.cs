using HarmonyLib;

using UnityEngine;

namespace VerticalLevel;

[HarmonyPatch]
public class WaypointHeightPatch
{
    [HarmonyPatch(typeof(WaypointPropsManager), nameof(WaypointPropsManager.SpawnWaypointProps))]
    [HarmonyPostfix]
    public static void WaypointPropsManagerSpawnWaypointPropsPostfix(GameObject waypoint, ref bool __runOriginal, ref GameObject __result)
    {
        if (!__runOriginal || __result == null)
            return;
        __result.AddComponent<WaypointHeight>();
    }

    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.SetVectorTo), [typeof(PlaceableWaypoint)])]
    [HarmonyPostfix]
    public static void AircraftSerVectorToPatch(Aircraft __instance, PlaceableWaypoint waypoint)
    {
        TryCallAircraftChaseHeight(__instance, waypoint);
    }

    [HarmonyPatch(typeof(Aircraft), nameof(Aircraft.NextWayPoint), MethodType.Setter)]
    [HarmonyPostfix]
    public static void AircraftSetNextWayPointPostfix(Aircraft __instance, Waypoint value)
    {
        TryCallAircraftChaseHeight(__instance, value);
    }

    [HarmonyPatch(typeof(Aircraft), "HARWCurWP", MethodType.Setter)]
    [HarmonyPostfix]
    public static void AircraftSetHARWCurWPPostfix(Aircraft __instance, PlaceableWaypoint value)
    {
        if (value == null || value.GetNextHop() is not { } next)
            return;

        TryCallAircraftChaseHeight(__instance, next);
    }

    private static void TryCallAircraftChaseHeight(Aircraft aircraft, Waypoint waypoint)
    {
        if (waypoint == null || aircraft == null)
            return;
        if (aircraft.GetComponent<AircraftHeight>() is not { } aircraftHeight ||
            waypoint.GetComponent<WaypointHeight>() is not { } waypointHeight)
            return;
        aircraftHeight.targetHeight = waypointHeight.Height;
    }
}
