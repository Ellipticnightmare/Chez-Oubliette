using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public Quest[] quests;
    private void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/Data/Quests/" + quests[Random.Range(0, quests.Length)].title + ".qmdb"))
            ReadData();
        else
            WriteData();
    }
    public void ReadData()
    {
        foreach (var item in quests)
        {
            item.ReadData();
        }
    }
    public void WriteData()
    {
        foreach (var item in quests)
        {
            item.WriteData();
        }
    }
    public void SurvivedInDungeon(Dungeon inDungeon)
    {
        foreach (var item in quests)
        {
            DungeonThroughQuest inQuest = item.gameObject.GetComponent<DungeonThroughQuest>();
            if (inQuest != null)
            {
                if (inQuest.checkDungeon.dungeonName == inDungeon.dungeonName)
                {
                    item.backupNum++;
                    item.WriteData();
                }
            }
        }
    }
}
[System.Serializable]
public class QuestCheckData
{
    public string questName;
    public int questStage;
    public int backupData;
}