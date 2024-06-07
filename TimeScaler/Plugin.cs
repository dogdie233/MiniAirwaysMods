using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using System.Collections.Generic;
using System.Linq;

namespace TimeScaler;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource MyLogger { get; private set; }
    public static HashSet<int> EnabledLevel { get; set; }
    public static bool ForceCleanInput { get; set; }

    private void Awake()
    {
        MyLogger = Logger;

        LoadConfig();

        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void LoadConfig()
    {
        EnabledLevel = Config.Bind(
            "General",
            "EnabledSpeedRate",
            "3,4,5,6,7,8,9",
            "Speed rate for responding to keyboard input"
        ).Value.Split(',')
            .Select(v => (int.TryParse(v, out var num), num))
            .Where(t => t.Item1)
            .Select(t => t.num)
            .ToHashSet();

        ForceCleanInput = Config.Bind(
            "General",
            "ForceCleanInput",
            false,
            "Triggering the toggle speed only when the Ctrl, Shift, and other buttons are not held down"
        ).Value;
    }
}
