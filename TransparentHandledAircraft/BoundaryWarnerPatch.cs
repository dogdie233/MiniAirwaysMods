using HarmonyLib;

using System.Runtime.CompilerServices;

namespace TransparentHandledAircraft;

[HarmonyPatch]
public class BoundaryWarnerPatch
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UpdateTransparentStatusSafe(BoundaryWarner instance)
    {
        if ((instance.GetComponent<AircraftTransparent>() ?? instance.GetComponentInParent<AircraftTransparent>()) is not { } aircraftTransparent)
            return;  // component not found;
        aircraftTransparent.UpdateEnableStatus();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BoundaryWarner), nameof(BoundaryWarner.EnableWarner))]
    public static void UpdateStatusWhenEnableWarner(ref BoundaryWarner __instance)
        => UpdateTransparentStatusSafe(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BoundaryWarner), nameof(BoundaryWarner.DisableWarner))]
    public static void UpdateStatusWhenDisableWarner(ref BoundaryWarner __instance)
        => UpdateTransparentStatusSafe(__instance);
}
