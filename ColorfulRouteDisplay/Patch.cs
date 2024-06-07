using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UIElements;

namespace ColorfulRouteDisplay;

[HarmonyPatch]
public class Patch
{
    public static List<Color> Colors { get; private set; }
    public static Color? EndColor { get; private set; }
    public static float ColorfulLength { get; set; } = 10f;

    static Patch()
    {
        Colors = new()
        {
            ColorCode.GetColor(ColorCode.Option.Red),
            ColorCode.GetColor(ColorCode.Option.Yellow),
            ColorCode.GetColor(ColorCode.Option.Green),
            ColorCode.GetColor(ColorCode.Option.LightBlue),
            ColorCode.GetColor(ColorCode.Option.Pink)
        };
    }

    [HarmonyPatch(typeof(Aircraft), "Start")]
    [HarmonyPrefix]
    public static void AircraftStartPrefix(Aircraft __instance)
    {
        EndColor ??= __instance.LandingGuideLine.startColor;
    }

    [HarmonyPatch(typeof(Aircraft), "ShowPath")]
    [HarmonyPostfix]
    public static void AircraftShowPathPostfix(Aircraft __instance, List<Vector3> path, bool success = true)
    {
        Func<Vector3, float> lengthCalculator = path.Count > 100 ? ((Vector3 vec) => Mathf.Abs(vec.x) + Mathf.Abs(vec.y)) : ((Vector3 vec) => vec.magnitude);
        float length = GenerateDelta(path).Sum(lengthCalculator);

        var colorKeys = GenerateGradientColorKeys(length);
        if (!success)
            colorKeys = [colorKeys[^1] with { time = 0f }, colorKeys[^1] with { time = 1f }];
        __instance.LandingGuideLine.SetColorGradient(new Gradient()
        {
            colorKeys = GenerateGradientColorKeys(length),
            alphaKeys = __instance.LandingGuideLine.colorGradient.alphaKeys
        });
    }

    public static GradientColorKey[] GenerateGradientColorKeys(float length)
    {
        var colors = new GradientColorKey[Colors.Count + 1];
        var step = (ColorfulLength / Colors.Count / length);
        for (int i = 0; i < Colors.Count; i++)
            colors[i] = new GradientColorKey(Colors[i], i * step);
        colors[colors.Length - 1] = new GradientColorKey(EndColor ?? Color.white, Colors.Count * step);

        return colors;
    }

    public static IEnumerable<Vector3> GenerateDelta(IEnumerable<Vector3> vectors)
    {
        var enumerator = vectors.GetEnumerator();
        if (!enumerator.MoveNext())
            yield break;
        var prev = enumerator.Current;
        while (enumerator.MoveNext())
        {
            var curr = enumerator.Current;
            yield return curr - prev;
            prev = curr;
        }
    }
}
