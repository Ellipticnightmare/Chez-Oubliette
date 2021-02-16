using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using TMPro;

public class AdventurerManager : GameManager
{
    [HideInInspector]
    public List<string> recurringAdventurers, regularAdventurers = new List<string>();
    [HideInInspector]
    public int adventurerCount;
    [HideInInspector]
    public GameObject AdventurerPrefab;
    public GameObject displayInPauseUI;
    public void StartDataLoad()
    {
        recurringAdventurers.Clear();
        regularAdventurers.Clear();
        if (!Directory.Exists(Application.persistentDataPath + "/Data"))
        {
            Debug.Log("ERROR");
            Directory.CreateDirectory(Application.persistentDataPath + "/Data/Raw");
            Directory.CreateDirectory(Application.persistentDataPath + "/Data/Characters");
            Directory.CreateDirectory(Application.persistentDataPath + "/Data/Foods");
            FindObjectOfType<FoodManager>().GenerateIngredients();
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else
        {
            Read("/Data/Raw/recurringAdventurers.sdmd", recurringAdventurers);
            Read("/Data/Raw/regularAdventurers.sdmd", regularAdventurers);
        }
    }

    public void SpawnNewAdventurer()
    {
        GameObject newAdventurer;
        if (dayTimer > 0)
        {
            List<Adventurer> checkCount = new List<Adventurer>();
            checkCount.AddRange(FindObjectsOfType<Adventurer>());
            List<string> checkNames = new List<string>();
            foreach (var item in checkCount)
            {
                checkNames.Add(item.charName);
            }
            adventurerCount = checkCount.Count;
            int RNGSpawn;

            if (inShops.Count < base.curShop.customerLimit)
            {
                Adventurer newAdven = null;
                int spawnRecurring = Random.Range(0, 21);
                if (spawnRecurring >= 15 && (recurringAdventurers.Count > 0 || regularAdventurers.Count > 0))
                {
                    int recheckSpawnRecurring = Random.Range(0, 21);
                    if (recheckSpawnRecurring > 15 && regularAdventurers.Count > 0)
                    {
                            RNGSpawn = Random.Range(0, regularAdventurers.Count);
                        if (checkNames.Contains(regularAdventurers[RNGSpawn]))
                            SpawnNewAdventurer();
                        else
                        {
                            StopCoroutine(FindObjectOfType<GameManager>().spawnNewCustomer());
                            StartCoroutine(FindObjectOfType<GameManager>().spawnNewCustomer());
                            return;
                        }
                    }
                    else
                    {
                        RNGSpawn = Random.Range(0, recurringAdventurers.Count);
                        if (checkNames.Contains(recurringAdventurers[RNGSpawn]))
                            SpawnNewAdventurer();
                        else
                        {
                            StopCoroutine(FindObjectOfType<GameManager>().spawnNewCustomer());
                            StartCoroutine(FindObjectOfType<GameManager>().spawnNewCustomer());
                            return;
                        }
                    }
                }
                else
                {
                    newAdventurer = Instantiate(AdventurerPrefab, base.spawnArea.transform);
                    newAdven = newAdventurer.GetComponent<Adventurer>();
                    newAdven.GetComponent<Adventurer>().SetupAdventurer(0);
                }
                base.inShops.Add(newAdven);
            }
        }
    }
    public void beatDungeon(string updateText)
    {
        base.StartCoroutine(UpdateAdventurerText(updateText));
        base.HeroesSurvived++;
        FindObjectOfType<QuestManager>().SurvivedInDungeon(curDungeon);
    }
    public void FellInDungeon(string updateText)
    {
        StartCoroutine(UpdateAdventurerText(updateText));
        HeroesDied++;
    }
    public void UpdateGameData()
    {
        if (recurringAdventurers.Count > 0)
            Write("/Data/Raw/recurringAdventurers.sdmd", recurringAdventurers);
        if (regularAdventurers.Count > 0)
            Write("/Data/Raw/regularAdventurers.sdmd", regularAdventurers);
    }
    public void Read(string fileName, List<string> checkList)
    {
        checkList.Clear();
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
            TestData check = (TestData)bf.Deserialize(file);
            file.Close();
            List<string> roughList = new List<string>();
            roughList.AddRange(check.text.Split('|'));
            foreach (var item in roughList)
            {
                if (item != "" || item != null)
                    checkList.Add(item);
            }
        }
        else
        {
            if (checkList.Count > 0)
                Write(fileName, checkList);
        }
    }
    public void Write(string fileName, List<string> checkList)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + fileName);

        TestData check = new TestData();
        string toJoin = string.Join("|", checkList);
        check.text = toJoin;

        bf.Serialize(file, check);
        file.Close();
    }


    public void BuildAdventurerDetails()
    {
        string filePath = "/Data/Characters/";
        foreach (var item in recurringAdventurers)
        {
            if(File.Exists(Application.persistentDataPath + filePath + item + ".sdcd"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + filePath + item + ".sdcd", FileMode.Open);
                AdventurerTestData checkov = (AdventurerTestData)bf.Deserialize(file);
                file.Close();
                GameObject newDisplayUI = Instantiate(displayInPauseUI, RecurringDisplayHolder);
                newDisplayUI.GetComponentInChildren<TMP_Text>().text = "<color=green>" + item + "\n\n" + "<color=blue>Trait: " + checkov.personaliT;
            }
        }
        foreach (var item in regularAdventurers)
        {
            if(File.Exists(Application.persistentDataPath + filePath + item + ".sdcd"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + filePath + item + ".sdcd", FileMode.Open);
                AdventurerTestData checkov = (AdventurerTestData)bf.Deserialize(file);
                file.Close();
                GameObject newDisplayUI = Instantiate(displayInPauseUI, RecurringDisplayHolder);
                newDisplayUI.GetComponentInChildren<TMP_Text>().text = "<color=green>" + item + "\n\n" + "<color=blue>Trait: " + checkov.personaliT;
            }
        }
    }
}
#region SerializableObjects
[System.Serializable]
class TestData
{
    public string text;
}
#endregion