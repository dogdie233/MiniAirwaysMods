using System.Collections.Generic;

using UnityEngine;

namespace GoodlifeShortcut;

public class EasyTakeoffBehaviour : MonoBehaviour
{
    public static List<KeyCode> shortcuts = new()
    {
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R
    };
    private static bool occupied = false;

    public static bool QwQ
    {
        set => occupied = value;
    }

    private void Update()
    {
        if (TakeoffTaskManager.Instance == null)
            return;

        for (var i = 0; i < shortcuts.Count && i < TakeoffTaskManager.Instance.Aprons.Count; i++)
        {
            if (!Input.GetKeyUp(shortcuts[i]))
                continue;

            var apron = TakeoffTaskManager.Instance.Aprons[i];
            if (!apron.Interactable || !apron.isOccupied || apron.takeoffTask == null || !apron.takeoffTask.inCommand)
                continue;

            Plugin.MyLogger.LogInfo($"Use shortcut key {shortcuts[i]} to select apron{i}");
            apron.takeoffTask.OnPointUp();
            break;
        }

        for (var i = 0; i < shortcuts.Count && i < TakeoffTaskManager.Instance.Aprons.Count; i++)
        {
            if (!Input.GetKeyDown(shortcuts[i]))
                continue;

            var apron = TakeoffTaskManager.Instance.Aprons[i];
            if (!apron.Interactable || apron.takeoffTask == null || apron.takeoffTask.inCommand)
                continue;

            Plugin.MyLogger.LogInfo($"Use shortcut key {shortcuts[i]} to takeoff apron{i}");
            apron.takeoffTask.OnPointDown();
            break;
        }
    }
}
