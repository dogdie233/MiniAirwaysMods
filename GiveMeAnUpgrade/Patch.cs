using DG.Tweening;

using HarmonyLib;

using Mono.Cecil;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using static UnityEngine.GraphicsBuffer;

namespace GiveMeAnUpgrade;

[HarmonyPatch]
public class Patch
{
    [HarmonyPatch(typeof(ClockController), "Start")]
    [HarmonyPostfix]
    public static void Start(ClockController __instance, Button ___fastSpeedButton)
    {
        var button = GameObject.Instantiate(___fastSpeedButton.gameObject, ___fastSpeedButton.transform.parent).GetComponent<Button>();
        if (button.targetGraphic is Image image)
            image.sprite = Plugin.UpgradeButtonSprite;
        else if (button.targetGraphic is RawImage rawImage)
            rawImage.texture = Plugin.UpgradeButtonSprite.texture;
        button.gameObject.name = "UpgradeButton";
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnUpgradeButtonClick);
    }

    private static void FindTweenAndAppendCallback(object target, TweenCallback callback)
    {
        var tweens = DOTween.TweensByTarget(target);
        Debug.Log($"find {tweens.Count}");
        foreach (var tween in tweens)
        {
            if (tween.onComplete == null)
                return;

            tween.onComplete += callback;
        }
    }

    private static TweenCallback Holyshit(object target, TweenCallback callback)
        => () => FindTweenAndAppendCallback(target, callback);

    [HarmonyPatch(typeof(ClockController), "ClockSubButtonsShowCoroutine")]
    [HarmonyPostfix]
    public static IEnumerator ClockSubButtonsShowCoroutine(IEnumerator result,
        CanvasGroup ___pauseButtonCanvasGroup,
        CanvasGroup ___normalSpeedButtonCanvasGroup,
        CanvasGroup ___fastSpeedButtonCanvasGroup,
        ClockController __instance)
    {
        bool next = result.MoveNext();
        yield return result.Current;  // WaitForSecondsRealtime(delay);


        next = result.MoveNext();
        Holyshit(___pauseButtonCanvasGroup, Holyshit(___normalSpeedButtonCanvasGroup, () =>
        {
            var tweens = DOTween.TweensByTarget(___fastSpeedButtonCanvasGroup);
            tweens[0].onComplete += () =>
            {
                var upgradeButton = ___fastSpeedButtonCanvasGroup.transform.parent.Find("UpgradeButton");
                Plugin.MyLogger.LogInfo($"{upgradeButton == null}");
                if (upgradeButton == null)
                    return;

                upgradeButton.gameObject.SetActive(true);
                ((RectTransform)upgradeButton.transform).DOAnchorPosY(-360f, 0.3f, false).SetUpdate(true).SetEase(Ease.OutBack);
                upgradeButton.GetComponent<CanvasGroup>().DOFade(1f, 0.3f).SetUpdate(true);
            };
        }))();
        yield return result.Current;


        if (result.Current != null)
        {
            next = result.MoveNext();
            var upgradeButton = ___fastSpeedButtonCanvasGroup.transform.parent.Find("UpgradeButton");
            if (upgradeButton != null)
            {
                upgradeButton.GetComponent<CanvasGroup>().DOFade(1f, 0.3f).SetUpdate(true);
                ((RectTransform)upgradeButton.transform).DOAnchorPosX(-100f, 0.3f, false).SetUpdate(true);
            }
            if (next)
                yield return result.Current;
        }
        while (next)
        {
            yield return result.Current;
            next = result.MoveNext();
        }
    }

    [HarmonyPatch(typeof(ClockController), "ClockSubButtonsHideCoroutine")]
    [HarmonyPostfix]
    public static IEnumerator ClockSubButtonsHideCoroutine(IEnumerator result,
        CanvasGroup ___pauseButtonCanvasGroup,
        CanvasGroup ___normalSpeedButtonCanvasGroup,
        CanvasGroup ___fastSpeedButtonCanvasGroup,
        ClockController __instance)
    {
        bool next = result.MoveNext();

        Holyshit(___pauseButtonCanvasGroup, Holyshit(___normalSpeedButtonCanvasGroup, Holyshit(___fastSpeedButtonCanvasGroup, () =>
        {
            var upgradeButton = ___fastSpeedButtonCanvasGroup.transform.parent.Find("UpgradeButton");
            if (upgradeButton == null)
                return;

            ((RectTransform)upgradeButton.transform).DOAnchorPosY(0f, 0.3f, false).SetUpdate(true).SetEase(Ease.InBack);
            upgradeButton.GetComponent<CanvasGroup>().DOFade(0f, 0.3f).SetUpdate(true).OnComplete(delegate
            {
                if (upgradeButton == null)
                    return;

                upgradeButton.gameObject.SetActive(false);
            });
        })))();

        while (next)
        {
            yield return result.Current;
            next = result.MoveNext();
        }
    }

    private static void OnUpgradeButtonClick()
    {
        if (UpgradeManager.Instance == null)
            return;

        UpgradeManager.Instance.EnableUpgrade();
    }
}
