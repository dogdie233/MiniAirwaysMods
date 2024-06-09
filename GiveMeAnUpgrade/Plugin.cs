using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using UnityEngine;

namespace GiveMeAnUpgrade;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource MyLogger { get; private set; } = null!;
    public static Sprite UpgradeButtonSprite { get; set; } = null!;

    private void Awake()
    {
        MyLogger = Logger;

        Logger.LogInfo("Drawing upgrade button image");

        var image = Helper.GenerateUpgradeImage();
        UpgradeButtonSprite ??= Sprite.Create(image, new Rect(0f, 0f, image.width, image.height), image.texelSize / 2);

        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        InputTools.AddBinding(BindingDescriptionBuilder
            .Create(KeyCode.C, TryGetAnUpgrade)
            .NotAllowModifierKeys()
            .Build());

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnDestroy()
    {
        Destroy(UpgradeButtonSprite.texture);
        Destroy(UpgradeButtonSprite);
    }

    private void Update()
    {
        InputTools.PoolEvent();
    }

    internal static void TryGetAnUpgrade()
    {
        if (UpgradeManager.Instance == null)
            return;

        UpgradeManager.Instance.EnableUpgrade();
        MyLogger.LogInfo("Get a upgrade");
    }
}
