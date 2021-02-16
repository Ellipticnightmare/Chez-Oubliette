using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxOutShop : Quest
{
    int curShopTier;
    void Update()
    {
        description = "Upgrade your shop to the next tier to gain " + Reward();
        if (FindObjectOfType<GameManager>().runTutorial)
        {
            curShopTier = FindObjectOfType<GameManager>().curShop.shopTier;
            if (curShopTier >= questsInSequence[questCheck].questThreshold)
                UpdateToNextQuestStage(false);
            if (mainProgressBar != null)
                mainProgressBar.fillAmount = (float)questCheck / questsInSequence.Length;
            if (currentProgressBar != null)
                currentProgressBar.fillAmount = .5f;
        }
    }
}