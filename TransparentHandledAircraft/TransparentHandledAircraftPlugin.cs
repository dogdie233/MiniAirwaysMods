using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

namespace TransparentHandledAircraft
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class TransparentHandledAircraftPlugin : BaseUnityPlugin
    {
        public static ManualLogSource MyLogger { get; private set; }

        private void Awake()
        {
            MyLogger = Logger;

            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
