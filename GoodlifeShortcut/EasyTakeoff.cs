using System.Collections.Generic;

using UnityEngine;

namespace GoodlifeShortcut;

public class EasyTakeoffBehaviour : MonoBehaviour
{
    private void Update()
    {
        if (TakeoffTaskManager.Instance is not { } takeoffTaskManager)
            return;
        
        var shortcuts = Plugin.ApronShortcuts;
        foreach (var u in shortcuts)
        {
            if (u.apronIndex >= takeoffTaskManager.Aprons.Count || !Input.GetKeyUp(u.key))
                continue;

            var apron = takeoffTaskManager.Aprons[u.apronIndex];
            if (!apron.Interactable || !apron.isOccupied || apron.takeoffTask == null || !apron.takeoffTask.inCommand)
                continue;
            
            Plugin.MyLogger.LogInfo($"Use shortcut key {u.key} to select apron {u.apronIndex}");
            apron.takeoffTask.OnPointUp();
            break;
        }

        foreach (var u in shortcuts)
        {
            if (u.apronIndex >= takeoffTaskManager.Aprons.Count || !Input.GetKeyDown(u.key))
                continue;

            var apron = takeoffTaskManager.Aprons[u.apronIndex];
            if (!apron.Interactable || apron.takeoffTask == null || apron.takeoffTask.inCommand)
                continue;

            Plugin.MyLogger.LogInfo($"Use shortcut key {u.key} to takeoff apron {u.apronIndex}");
            apron.takeoffTask.OnPointDown();
            break;
        }
    }
}
