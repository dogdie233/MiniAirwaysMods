using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using UnityEngine;
using UnityEngine.Networking;

namespace GiveMeAnUpgrade;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource MyLogger { get; private set; }
    public static Sprite UpgradeButtonSprite { get; set; }

    private void Awake()
    {
        MyLogger = Logger;

        Logger.LogInfo("Drawing upgrade button image");

        var image = Helper.GenerateUpgradeImage();
        UpgradeButtonSprite ??= Sprite.Create(image, new Rect(0f, 0f, image.width, image.height), image.texelSize / 2);

        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnDestroy()
    {
        Destroy(UpgradeButtonSprite.texture);
        Destroy(UpgradeButtonSprite);
    }
}
