using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonThroughQuest : Quest
{
    public int adventurersSurvived;
    public Dungeon checkDungeon;
    private void Update()
    {
        description = "Have " + questsInSequence[questCheck].questThreshold + " Adventurers survive in the " + checkDungeon.dungeonName + " to gain " + Reward();
        adventurersSurvived = base.backupNum;
        if (adventurersSurvived >= questsInSequence[questCheck].questThreshold)
            UpdateToNextQuestStage(false);
        if (mainProgressBar != null)
            mainProgressBar.fillAmount = (float)questCheck / questsInSequence.Length;
        if (currentProgressBar != null)
            currentProgressBar.fillAmount = (float)adventurersSurvived / questsInSequence[questCheck].questThreshold;
    }
}