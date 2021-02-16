using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiedInDungeonQuest : Quest
{
    int curDeathCount;
    void Update()
    {
        description = "Celebrate " + base.questsInSequence[questCheck].questThreshold + " Funerals by watching Adventurers die in Dungeons" + " to gain " + Reward();
        curDeathCount = FindObjectOfType<GameManager>().HeroesDied;
        if (curDeathCount >= questsInSequence[questCheck].questThreshold)
            UpdateToNextQuestStage(false);
        if (mainProgressBar != null)
            mainProgressBar.fillAmount = (float)questCheck / questsInSequence.Length;
        if (currentProgressBar != null)
            currentProgressBar.fillAmount = (float)curDeathCount / questsInSequence[questCheck].questThreshold;
    }
}