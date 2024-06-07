using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

namespace GoodlifeShortcut;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource MyLogger { get; private set; }

    private void Awake()
    {
        MyLogger = Logger;

        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        gameObject.AddComponent<EasyTakeoffBehaviour>();
        harmony.PatchAll();

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }
}
