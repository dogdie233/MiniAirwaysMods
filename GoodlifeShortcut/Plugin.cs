using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace GoodlifeShortcut;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static List<(KeyCode key, int apronIndex)> ApronShortcuts { get; private set; } = new();
    public static ManualLogSource MyLogger { get; private set; }

    private void Awake()
    {
        MyLogger = Logger;

        LoadConfig();

        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        gameObject.AddComponent<EasyTakeoffBehaviour>();
        harmony.PatchAll();

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void LoadConfig()
    {
        var keysWithIndex = Config.Bind(
            "General",
            "ApronShortcutKey",
            "Z,X",
            "每个的停机坪的快捷键\n使用[空]则为跳过此位置的停机坪\n例如：Z,,X表示Z选择第一位停机坪，X表示选择第三位停机坪"
        ).Value.Split(',').Index().Where(u => !string.IsNullOrEmpty(u.element));

        foreach (var u in keysWithIndex)
        {
            if (!Enum.TryParse<KeyCode>(u.element, true, out var key))
            {
                MyLogger.LogError($"Couldn't parse key `{u.element}`, skip it");
                continue;
            }

            ApronShortcuts.Add((key, u.key));
        }
    }
}
