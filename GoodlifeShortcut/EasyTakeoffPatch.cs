using HarmonyLib;

using System.Linq;

namespace GoodlifeShortcut;

[HarmonyPatch]
public class EasyTakeoffPatch
{
    [HarmonyPatch(typeof(TakeoffTask), nameof(TakeoffTask.OnPointDown))]
    [HarmonyPrefix]
    public static void TakeoffTaskOnPointDownCheck(ref bool __runOriginal)
    {
        __runOriginal = __runOriginal && !TakeoffTaskManager.Instance.Aprons.Any(apron => apron.takeoffTask != null && apron.takeoffTask.inCommand);
    }
}
