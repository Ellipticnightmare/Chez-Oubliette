using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    public string title = "";
    [HideInInspector]
    public string description;
    [HideInInspector]
    public int questCheck = 0;
    public QuestData[] questsInSequence;
    [HideInInspector]
    public int premiumToGain, goldToGain, backupNum;

    public Text descriptionText;
    public Image mainProgressBar, currentProgressBar;
    public void LateUpdate()
    {
        descriptionText.text = description;
        premiumToGain = questsInSequence[questCheck].premiumReward;
        goldToGain = questsInSequence[questCheck].goldReward;
    }
    public void UpdateToNextQuestStage(bool builtFromData)
    {
        questCheck++;
        if (questCheck >= questsInSequence.Length && !builtFromData)
            CompletedQuest();
        if (!builtFromData)
        {
            FindObjectOfType<GameManager>().playerMoney += goldToGain;
            FindObjectOfType<GameManager>().premiumMoney += premiumToGain;
            WriteData();
        }
    }
    public void CompletedQuest()
    {
        Destroy(this.gameObject);
    }
    public void CheckInQuest(QuestCheckData inData)
    {
        if (inData.questName == title)
        {
            questCheck = inData.questStage - 1;
            UpdateToNextQuestStage(true);
            backupNum = inData.backupData;
        }
    }
    public string Reward()
    {
        string output = "";
        if (premiumToGain != 0 && goldToGain != 0)
            output += premiumToGain + " Star Coins and " + goldToGain + " Gold Pieces";
        else if (premiumToGain != 0)
            output += premiumToGain + " Star Coins";
        else if (goldToGain != 0)
            output += goldToGain + " Gold Pieces";
        return output;
    }
    public void ReadData()
    {
        if (File.Exists(Application.persistentDataPath + "/Data/Quests/" + title + ".qmdb"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Data/Quests/" + title + ".qmdb", FileMode.Open);
            QuestCheckData check = (QuestCheckData)bf.Deserialize(file);

            CheckInQuest(check);
        }
    }
    public void WriteData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Data/Quests/" + title + ".qmdb");
        QuestCheckData check = new QuestCheckData();

        check.questName = title;
        check.questStage = questCheck;
        check.backupData = backupNum;

        bf.Serialize(file, check);
        file.Close();

        ReadData();
    }
}
[System.Serializable]
public class QuestData
{
    public int premiumReward, goldReward, questThreshold;
    public int backupNumber;
}