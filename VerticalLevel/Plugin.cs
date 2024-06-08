﻿using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace VerticalLevel;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource MyLogger { get; private set; }

    private void Awake()
    {
        MyLogger = Logger;

        // Plugin startup logic
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

        SceneManager.sceneLoaded += OnSceneLoaded;
        Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Logger.LogInfo($"Scene loaded: {scene.name}");

        if ((scene.name == "MapPlayer" || scene.name == "London") &&
            AircraftManager.Instance != null)
        {
            Logger.LogInfo("Hooking AircraftManager");
            AircraftManager.Instance.AircraftCreateEvent.AddListener(HookAircraft);
        }
    }

    private void HookAircraft(Vector2 pos, Aircraft aircraft)
    {
        Logger.LogInfo("Aircraft created: " + aircraft.name);

        AircraftStatHelper cp = aircraft.gameObject.AddComponent<AircraftStatHelper>();
        cp.m_Aircraft = aircraft;

        AircraftHeight ah = aircraft.gameObject.AddComponent<AircraftHeight>();
        ah.m_Aircraft = aircraft;
    }
}
