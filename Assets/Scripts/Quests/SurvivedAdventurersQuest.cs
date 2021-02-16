using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivedAdventurersQuest : Quest
{
    int curSurvivalCount;

    void Update()
    {
        description = "Have " + base.questsInSequence[questCheck].questThreshold + " Adventurers survive in Dungeons" + " to gain " + Reward();
        curSurvivalCount = FindObjectOfType<GameManager>().HeroesSurvived;
        if (curSurvivalCount >= base.questsInSequence[questCheck].questThreshold)
            base.UpdateToNextQuestStage(false);
        if (mainProgressBar != null)
            mainProgressBar.fillAmount = (float)base.questCheck / base.questsInSequence.Length;
        if (currentProgressBar != null)
            currentProgressBar.fillAmount = (float)curSurvivalCount / base.questsInSequence[questCheck].questThreshold;
    }
}