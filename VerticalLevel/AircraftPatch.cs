using HarmonyLib;

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using UnityEngine;

namespace VerticalLevel;

[HarmonyPatch]
public class AircraftPatch
{
    [HarmonyPatch(typeof(Aircraft), "TrySetupLanding", [typeof(Runway), typeof(bool)])]
    [HarmonyPrefix]
    public static bool TrySetupLandingPrefix(Runway runway, bool doLand, ref Aircraft __instance)
    {
        bool CONTINUE_ORIGINAL_FUNCTION = true;
        bool IGNORE_ORIGINAL_FUNCTION = false;

        // get height
        AircraftHeight ah1 = __instance.GetComponent<AircraftHeight>();

        bool specialCondition = ah1.height > 3000f;
        if (specialCondition)
        {
            // may play a new voice line here?
            // __instance.PlayVoiceLine("altitude too high");

            return IGNORE_ORIGINAL_FUNCTION;
        }

        // not only that, we should set target height to 1000 immediately
        ah1.targetHeight = 1000f;
        return CONTINUE_ORIGINAL_FUNCTION;
    }

    [HarmonyPatch(typeof(Aircraft), "AircraftCollideGameOver", [typeof(Aircraft), typeof(Aircraft)])]
    [HarmonyPrefix]
    public static bool AircraftCollideGameOverPrefix(Aircraft aircraft1, Aircraft aircraft2)
    {
        bool CONTINUE_GAMEOVER = true;
        bool IGNORE_GAMEOVER = false;

        if (aircraft1.GetComponent<AircraftHeight>() is not { } ah1 ||
            aircraft2.GetComponent<AircraftHeight>() is not { } ah2)
            return CONTINUE_GAMEOVER;

        return (int)(ah1.height / 100f) == (int)(ah2.height / 100f) ? CONTINUE_GAMEOVER : IGNORE_GAMEOVER;
    }

    [HarmonyPatch(typeof(Aircraft), "OnTriggerEnter2D", [typeof(Collider2D)])]
    [HarmonyPatch(typeof(Aircraft), "OnTriggerStay2D", [typeof(Collider2D)])]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> TranspileAircraftTCAS(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var method = AccessTools.Method(typeof(Aircraft), nameof(Aircraft.EnableVisualWarning));
        var hijack = AccessTools.Method(typeof(AircraftPatch), nameof(HijackEnableVisualWarning));
        foreach (var instruction in instructions)
        {
            if (instruction.Calls(method))
            {
                yield return new CodeInstruction(OpCodes.Call, hijack);
                continue;
            }
            yield return instruction;
        }
        Plugin.MyLogger.LogInfo($"Hijacking method {original.Name} for Aircraft to implement TCAS");
    }

    public static void HijackEnableVisualWarning(Aircraft instance, GameObject other, bool isAircraftWarner = false)
    {
        if (!AircraftHeight.CheckTCASHeightCondition(instance, other.GetComponent<AircraftRef>().aircraft))
            return;
        instance.EnableVisualWarning(other, isAircraftWarner);
    }

    [HarmonyPatch(typeof(Aircraft), "OnTriggerEnter2D", [typeof(Collider2D)])]
    [HarmonyPrefix]
    public static bool OnTriggerEnter2DPrefix(Collider2D other,
        ref bool ___mainMenuMode,
        ref ColorCode.Option ___colorCode,
        ref ShapeCode.Option ___shapeCode,
        ref Aircraft __instance,
        ref bool ___reachExit,
        ref AircraftVoiceAndSubtitlesController ___aircraftVoiceAndSubtitles
        )
    {
        bool CONTINUE_ORIGINAL_FUNCTION = true;
        bool IGNORE_ORIGINAL_FUNCTION = false;

        if (___mainMenuMode || !other.CompareTag("CollideCheck"))
        {
            return IGNORE_ORIGINAL_FUNCTION;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Waypoint"))
        {
            Waypoint waypoint = other.GetComponent<WaypointRef>().waypoint;
            if (___colorCode == waypoint.colorCode && ___shapeCode == waypoint.shapeCode)
            {
                // get height
                AircraftHeight ah1 = __instance.GetComponent<AircraftHeight>();

                bool specialCondition = ah1.height > 3000f;
                if (specialCondition)
                {
                    // Debug.Log(1111111);

                    WaypointManager.Instance.Handoff(waypoint);
                    ___aircraftVoiceAndSubtitles.PlayHandOff();
                    AircraftManager.Instance.AircraftHandOffEvent.Invoke(__instance.gameObject.transform.position);
                    ___reachExit = true;
                    __instance.Invoke("ConditionalDestroy", 2f);
/*
                    foreach (Aircraft componentsInChild in AircraftManager.Instance.OutboundRoot.GetComponentsInChildren<Aircraft>())
                    {
                        if (componentsInChild.direction == Aircraft.Direction.Outbound)
                        {
                            Debug.Log(componentsInChild.colorCode + " " + componentsInChild.shapeCode);
                        }
                    }*/

                }
                else
                {
                    // do nothing, or maybe call a new voice line?
                }

                return IGNORE_ORIGINAL_FUNCTION;
            }
        }

        return CONTINUE_ORIGINAL_FUNCTION;
    }
}
