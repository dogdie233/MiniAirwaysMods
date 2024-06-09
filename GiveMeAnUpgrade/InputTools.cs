using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace GiveMeAnUpgrade;

public class BindingDescription
{
    public readonly KeyCode mainKey;
    public readonly KeyCode[] additionalKey;
    public readonly KeyCode[] blacklist;
    public readonly Action action;

    internal BindingDescription(KeyCode mainKey, KeyCode[] additionalKey, KeyCode[] blacklist, Action action)
    {
        this.mainKey = mainKey;
        this.additionalKey = additionalKey;
        this.blacklist = blacklist;
        this.action = action;
    }
}

public class BindingDescriptionBuilder
{
    public KeyCode mainKey;
    public HashSet<KeyCode> additionalKey = new();
    public HashSet<KeyCode> blacklistKey = new();
    public Action action;

    public BindingDescriptionBuilder(KeyCode mainKey, Action action)
    {
        this.mainKey = mainKey;
        this.action = action;
    }

    public BindingDescription Build()
    {
        RuleCheck();

        return new BindingDescription(mainKey, additionalKey.ToArray(), blacklistKey.ToArray(), action);
    }

    private void RuleCheck()
    {
        if (action == null)
            throw new ArgumentException($"{nameof(action)} could not be null");

        foreach (var key in blacklistKey)
        {
            if (additionalKey.Contains(key))
                throw new ArgumentException("Blacklist key is also in additional key");
        }
    }

    public static BindingDescriptionBuilder Create(KeyCode mainKey, Action action)
        => new BindingDescriptionBuilder(mainKey, action);
}

public static class BindingDescriptionBuilderExtension
{
    /// <summary>
    /// Add modifier keys into blacklist
    /// </summary>
    public static BindingDescriptionBuilder NotAllowModifierKeys(this BindingDescriptionBuilder builder)
    {
        builder.blacklistKey.Add(KeyCode.LeftControl);
        builder.blacklistKey.Add(KeyCode.RightControl);
        builder.blacklistKey.Add(KeyCode.LeftShift);
        builder.blacklistKey.Add(KeyCode.RightShift);
        builder.blacklistKey.Add(KeyCode.LeftAlt);
        builder.blacklistKey.Add(KeyCode.RightAlt);
        builder.blacklistKey.Add(KeyCode.LeftCommand);
        builder.blacklistKey.Add(KeyCode.RightCommand);
        builder.blacklistKey.Add(KeyCode.LeftWindows);
        builder.blacklistKey.Add(KeyCode.RightWindows);
        return builder;
    }
}

public static class InputTools
{
    private static int updateFrame = 0;
    private static List<BindingDescription> bindings = new();

    public static void AddBinding(BindingDescription description)
    {
        bindings.Add(description);
    }

    public static void PoolEvent()
    {
        if (Time.frameCount <= updateFrame)
            return;

        updateFrame = Time.frameCount;

        foreach (var binding in bindings)
        {
            if (binding.blacklist.Any(Input.GetKey))
                continue;

            if (Input.GetKeyDown(binding.mainKey) || binding.additionalKey.Any(Input.GetKey))
                binding.action();
        }
    }
}
