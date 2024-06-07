using HarmonyLib;

using System.Collections.Generic;

using UnityEngine;

namespace TimeScaler;

[HarmonyPatch]
public class Patch
{
    public static HashSet<KeyCode> BlackListKey { get; set; } = new()
    {
        KeyCode.LeftControl,
        KeyCode.RightControl,
        KeyCode.LeftShift,
        KeyCode.RightShift,
        KeyCode.LeftAlt,
        KeyCode.RightAlt,
        KeyCode.LeftCommand,
        KeyCode.RightCommand,
        KeyCode.LeftWindows,
        KeyCode.RightWindows
    };

    [HarmonyPatch(typeof(LevelManager), "Update")]
    [HarmonyPostfix]
    public static void Update(LevelManager __instance)
    {
        void ApplyInput(KeyCode from, KeyCode to, int levelBegin)
        {
            for (var key = from; key <= to; key++)
            {
                if (!Input.GetKeyDown(key))
                    continue;

                var level = key - from + levelBegin;
                if (!Plugin.EnabledLevel.Contains(level))
                    continue;

                var speed = Helper.CalculateTimeScale(level);
                TimeManager.Instance.SetSpeed(speed);
                Plugin.MyLogger.LogInfo($"Set Speed to level {level}({speed}f)");
            }
        }

        if (TimeManager.Instance == null)
            return;

        if (Plugin.ForceCleanInput)
        {
            foreach (var key in BlackListKey)
            {
                if (Input.GetKey(key))
                    return;
            }
        }
        
        ApplyInput(KeyCode.Alpha3, KeyCode.Alpha9, 3);
        ApplyInput(KeyCode.Keypad3, KeyCode.Keypad9, 3);
    }
}
