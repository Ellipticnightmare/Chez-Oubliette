using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Adventurer : MonoBehaviour
{
    public int Strength, Agility, Intelligence, Moxie, Durability, Health, Luck, timesVisitedShop, statModifier, AdventurerGold;
    int originalStrength, originalAgility, originalIntelligence, originalMoxie, originalDurability, originalHealth, originalLuck;
    public string charName;
    public bool hasEnteredDungeon, secondChance;
    public GameObject adventurerUpdate;
    public Image dungeonProgress, floorProgress;
    Text dungeonUpdateText;
    public int enemiesDefeated, enemyCount, floorCount;
    Vector3 startPos;
    public personality Personality = personality.blank;
    public enum AdventurerRace
    {
        human,
        elf,
        dwarf,
        dragonborn,
        tiefling,
        triton,
        gnome
    };
    public enum AdventurerClass
    {
        fighter,
        wizard,
        bard,
        rogue,
        cleric,
        paladin
    };
    public enum personality
    {
        Generous,
        Nervous,
        Cocky,
        Determined,
        Explorer,
        Trapmaster,
        Sturdy,
        Skillful,
        blank
    };
    public string generateName(AdventurerRace inRace)
    {
        string outName = "";
        string firstName = "";
        string lastName = "";
        NameGenerator generator = (NameGenerator)Resources.Load("nameGenerators/" + inRace);
        int X = Random.Range(1, 4);
        int Y = Random.Range(1, 4);
        while (X > 0)
        {
            firstName = firstName + generator.firstNameComponents[Random.Range(0, generator.firstNameComponents.Length)];
            X--;
        }
        while (Y > 0)
        {
            lastName = lastName + generator.lastNameComponents[Random.Range(0, generator.lastNameComponents.Length)];
            Y--;
        }
        firstName = (char.ToUpper(firstName[0]) + ((firstName.Length > 1) ? firstName.Substring(1).ToLower() : string.Empty));
        lastName = (char.ToUpper(lastName[0]) + ((lastName.Length > 1) ? lastName.Substring(1).ToLower() : string.Empty));
        outName = firstName + " " + lastName;
        return outName;
    }
    public AdventurerClass curClass;
    public AdventurerRace curRace;
    public List<foodType> canEat = new List<foodType>();
    public int CR, curDungeonFloor, goldEarned, levelsGained;
    private bool isEnteringDungeon;
    private void Update()
    {
        CR = Mathf.RoundToInt((abilityModifier(Strength) +
              abilityModifier(Agility) +
              abilityModifier(Intelligence) +
              abilityModifier(Durability) +
              abilityModifier(Moxie) +
              statModifier + Luck) / 2);

        Strength = CapStat(Strength);
        Agility = CapStat(Agility);
        Intelligence = CapStat(Intelligence);
        Durability = CapStat(Durability);
        Moxie = CapStat(Moxie);
        Luck = CapLuck();
    }
    public int CapStat(int statCap)
    {
        int output = 0;
        if (statCap > (18 + statModifier))
            output = 18 + statModifier;
        else
            output = statCap;
        return output;
    }
    public int CapLuck()
    {
        int output = 0;
        if (Luck > 10)
            output = 10;
        else
            output = Luck;
        return output;
    }
    public void SetupAdventurer(int timesVisitedShop)
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        trigger.triggers.Add(entry);
        if (statModifier > 20)
            statModifier = 20;
        string pathFind = "";
        if (FindObjectOfType<GameManager>().DebugTest)
            pathFind = "characters/testAnim";
        else
            pathFind = "characters/" + curRace + curClass;
        StartCoroutine(AnimateSprite(pathFind));
        goldEarned = 0;
        if (timesVisitedShop <= 0)
        {
            Personality = personality.blank;
            Luck = Mathf.FloorToInt(Random.Range(-16, 7) / 2);
            if (Luck <= 0)
                Luck = 0;
            AdventurerGold = Random.Range(0, 21) + Random.Range(0, 21) + Random.Range(0, 21);
            statModifier = FindObjectOfType<GameManager>().curShop.shopTier;
            int x = UnityEngine.Random.Range(0, 6);
            int y = UnityEngine.Random.Range(0, 6);
            curRace = (AdventurerRace)x;
            curClass = (AdventurerClass)y;

            charName = generateName(curRace);

            switch (curRace)
            {
                case AdventurerRace.human:
                    canEat.Add(foodType.ᚠ);
                    canEat.Add(foodType.ᚣ);
                    canEat.Add(foodType.ᛄ);
                    break;
                case AdventurerRace.elf:
                    canEat.Add(foodType.ᛄ);
                    break;
                case AdventurerRace.dragonborn:
                    canEat.Add(foodType.ᚠ);
                    break;
                case AdventurerRace.triton:
                    canEat.Add(foodType.ᚣ);
                    break;
                case AdventurerRace.dwarf:
                    canEat.Add(foodType.ᛄ);
                    canEat.Add(foodType.ᚠ);
                    break;
                case AdventurerRace.tiefling:
                    canEat.Add(foodType.ᚣ);
                    canEat.Add(foodType.ᚠ);
                    break;
                case AdventurerRace.gnome:
                    canEat.Add(foodType.ᚣ);
                    canEat.Add(foodType.ᛄ);
                    break;
            }
            switch (curClass)
            {
                case AdventurerClass.fighter:
                    Strength = 14;
                    Agility = 13;
                    Intelligence = 13;
                    Moxie = 3;
                    Durability = 13;
                    originalHealth = 23 + ((statModifier + 1) * Random.Range(2, 5));
                    break;
                case AdventurerClass.wizard:
                    Strength = 8;
                    Agility = 14;
                    Intelligence = 15;
                    Durability = 14;
                    Moxie = 8;
                    originalHealth = 17 + ((statModifier + 1) * Random.Range(2, 5));
                    break;
                case AdventurerClass.bard:
                    Strength = 8;
                    Agility = 15;
                    Intelligence = 12;
                    Durability = 13;
                    Moxie = 15;
                    originalHealth = 20 + ((statModifier + 1) * Random.Range(2, 5));
                    break;
                case AdventurerClass.rogue:
                    Strength = 8;
                    Agility = 15;
                    Intelligence = 11;
                    Durability = 14;
                    Moxie = 14;
                    originalHealth = 17 + ((statModifier + 1) * Random.Range(2, 5));
                    break;
                case AdventurerClass.cleric:
                    Strength = 8;
                    Agility = 14;
                    Intelligence = 8;
                    Durability = 14;
                    Moxie = 12;
                    originalHealth = 20 + ((statModifier + 1) * Random.Range(2, 5));
                    break;
                case AdventurerClass.paladin:
                    Strength = 15;
                    Agility = 8;
                    Intelligence = 8;
                    Durability = 15;
                    Moxie = 15;
                    originalHealth = 17 + ((statModifier + 1) * Random.Range(2, 5));
                    break;
            }
        }
        else
        {
            originalHealth += Random.Range(abilityModifier(Durability), originalHealth);
            if (Personality == personality.Sturdy)
                originalHealth = Mathf.RoundToInt(originalHealth * 1.25f);
            if (timesVisitedShop == 1)
            {
                if (Personality == personality.Skillful)
                {
                    int RandCheck = Random.Range(0, 5);
                    int RandCheck2 = Random.Range(0, 5);
                    switch (RandCheck)
                    {
                        case 0:
                            Strength++;
                            break;
                        case 1:
                            Agility++;
                            break;
                        case 2:
                            Intelligence++;
                            break;
                        case 3:
                            Durability++;
                            break;
                        case 4:
                            Moxie++;
                            break;
                    }
                    switch (RandCheck2)
                    {
                        case 0:
                            Strength++;
                            break;
                        case 1:
                            Agility++;
                            break;
                        case 2:
                            Intelligence++;
                            break;
                        case 3:
                            Durability++;
                            break;
                        case 4:
                            Moxie++;
                            break;
                    }
                }
                timesVisitedShop--;
                WriteAdventurerFromData();
            }
            if (!FindObjectOfType<AdventurerManager>().regularAdventurers.Contains(this.charName))
                switch (curRace)
                {
                    case AdventurerRace.human:
                        canEat.Add(foodType.ᚠ);
                        canEat.Add(foodType.ᚣ);
                        canEat.Add(foodType.ᛄ);
                        break;
                    case AdventurerRace.elf:
                        canEat.Add(foodType.ᛄ);
                        break;
                    case AdventurerRace.dragonborn:
                        canEat.Add(foodType.ᚠ);
                        break;
                    case AdventurerRace.triton:
                        canEat.Add(foodType.ᚣ);
                        break;
                    case AdventurerRace.dwarf:
                        canEat.Add(foodType.ᛄ);
                        canEat.Add(foodType.ᚠ);
                        break;
                    case AdventurerRace.tiefling:
                        canEat.Add(foodType.ᚣ);
                        canEat.Add(foodType.ᚠ);
                        break;
                    case AdventurerRace.gnome:
                        canEat.Add(foodType.ᚣ);
                        canEat.Add(foodType.ᛄ);
                        break;
                }
            else
            {
                canEat.Add(foodType.ᚠ);
                canEat.Add(foodType.ᚣ);
                canEat.Add(foodType.ᛄ);
            }
        }
        originalStrength = Strength;
        originalAgility = Agility;
        originalDurability = Durability;
        originalIntelligence = Intelligence;
        originalMoxie = Moxie;
        originalLuck = Luck;
        Health = originalHealth;
    }
    public int PullFloorData(Dungeon inDungeon)
    {
        int curFloorCR = 0;
        foreach (var item in inDungeon.floors[curDungeonFloor].floorData)
        {
            curFloorCR += item.CR;
        }
        curFloorCR = Mathf.CeilToInt(curFloorCR * .5f) + inDungeon.starCount;
        return curFloorCR;
    }
    public int abilityModifier(int inStat)
    {
        int output = 0;
        if (inStat > 10)
            output = Mathf.RoundToInt((inStat - 10) / 2);
        else
            output = 0;
        return output;
    }
    public void RunDungeon(Dungeon inDungeon)
    {
        if (!hasEnteredDungeon)
        {
            adventurerUpdate = GameObject.Instantiate(adventurerUpdate, FindObjectOfType<GameManager>().adventurerUpdateHandler);
            dungeonProgress = adventurerUpdate.GetComponent<AdventurerDisplayProgress>().DungeonProgress;
            floorProgress = adventurerUpdate.GetComponent<AdventurerDisplayProgress>().FloorProgress;
            dungeonUpdateText = adventurerUpdate.GetComponent<AdventurerDisplayProgress>().Display;
            hasEnteredDungeon = true;
            floorCount = inDungeon.floors.Length;
            enemyCount = inDungeon.floors[0].floorData.Length;
            if (Personality != personality.Trapmaster)
            {
                foreach (var item in inDungeon.statModifiers)
                {
                    switch (item)
                    {
                        case Dungeon.statType.Strength:
                            if (Strength > inDungeon.penaltyCheck)
                                Strength -= inDungeon.statPenalty;
                            break;
                        case Dungeon.statType.Agility:
                            if (Agility > inDungeon.penaltyCheck)
                                Agility -= inDungeon.statPenalty;
                            break;
                        case Dungeon.statType.Intelligence:
                            if (Intelligence > inDungeon.penaltyCheck)
                                Intelligence -= inDungeon.statPenalty;
                            break;
                        case Dungeon.statType.Durability:
                            if (Durability > inDungeon.penaltyCheck)
                                Durability -= inDungeon.statPenalty;
                            break;
                        case Dungeon.statType.Moxie:
                            if (Moxie > inDungeon.penaltyCheck)
                                Moxie -= inDungeon.statPenalty;
                            break;
                    }
                }
            }
        }
        int dungeonCR = PullFloorData(inDungeon);

        int dungeonCheck = Random.Range(0, 101) + Luck + abilityModifier(Durability);
        int damageCheck = 0;
        if (CR >= dungeonCR)
        {
            damageCheck = Random.Range(0, dungeonCR);
            StartCoroutine(passThroughDungeonFloor(inDungeon, damageCheck));
        }
        else
        {
            int challengeCheck;
            if (Personality == personality.Cocky)
                challengeCheck = (dungeonCR) - (CR + 2);
            else
                challengeCheck = (dungeonCR) - CR;
            if (challengeCheck > 20)
            {
                int healthLoss = Random.Range(dungeonCR, dungeonCR + 41) + ((inDungeon.starCount + (curDungeonFloor + 1)) * Random.Range(1, 9) + curDungeonFloor);
                if (dungeonCheck >= 50)
                    StartCoroutine(passThroughDungeonFloor(inDungeon, healthLoss));
                else
                    StartCoroutine(diedInDungeonFloor(inDungeon, Random.Range(0, inDungeon.floors[curDungeonFloor].floorData.Length)));
            }
            else if (challengeCheck >= 10)
            {
                damageCheck = Random.Range(dungeonCR, dungeonCR + 21) + ((inDungeon.starCount + (curDungeonFloor + 1)) * Random.Range(1, 9) + curDungeonFloor);
                StartCoroutine(passThroughDungeonFloor(inDungeon, damageCheck));
            }
            else
            {
                damageCheck = (Random.Range(1, 9) * dungeonCR);
                StartCoroutine(passThroughDungeonFloor(inDungeon, damageCheck));
            }
        }
    }
    public IEnumerator passThroughDungeonFloor(Dungeon inDungeon, int damageCheck)
    {
        if (Health > damageCheck)
        {
            foreach (var item in inDungeon.floors[curDungeonFloor].floorData)
            {
                if (Health > damageCheck)
                {
                    floorProgress.fillAmount = (float)enemiesDefeated / enemyCount;
                    dungeonUpdateText.text = charName + " is currently fighting a " + item.name;
                    yield return new WaitForSeconds(3.5f);
                    dungeonUpdateText.text = item.name + " has been Slain";
                    enemiesDefeated++;
                    floorProgress.fillAmount = (float)enemiesDefeated / enemyCount;
                    Health -= damageCheck + item.CR;
                    Debug.Log(Health);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    StartCoroutine(diedInDungeonFloor(inDungeon, Random.Range(0, inDungeon.floors[curDungeonFloor].floorData.Length)));
                    StopCoroutine(passThroughDungeonFloor(inDungeon, damageCheck));
                }
            }
            StartCoroutine(MoveToNextDungeonFloor(inDungeon, damageCheck));
        }
        else
            StartCoroutine(diedInDungeonFloor(inDungeon, Random.Range(0, inDungeon.floors[curDungeonFloor].floorData.Length)));
    }
    public IEnumerator diedInDungeonFloor(Dungeon inDungeon, int deathCheck)
    {
        int secondWind = Random.Range(0, 11);
        if (secondWind <= Luck && !secondChance)
        {
            if (Personality != personality.Determined)
                secondChance = true;
        }
        Debug.Log("Haha I dieded!");
        Debug.Log(deathCheck);
        Debug.Log("I will die by the " + inDungeon.floors[curDungeonFloor].floorData[deathCheck].name);
        foreach (var item in inDungeon.floors[curDungeonFloor].floorData)
        {
            floorProgress.fillAmount = (float)enemiesDefeated / enemyCount;
            if (inDungeon.floors[curDungeonFloor].floorData[deathCheck] == item)
            {
                if (!secondChance || Personality != personality.Determined)
                {
                    dungeonUpdateText.text = charName + " is currently fighting a " + item.name;
                    yield return new WaitForSeconds(3.5f);
                    Debug.Log("I died here!");
                    DiedInDungeon(inDungeon);
                    StopCoroutine(diedInDungeonFloor(inDungeon, deathCheck));
                }
                else
                {
                    int finalChanceCheck = Random.Range(1, 21);
                    if (finalChanceCheck > Luck + abilityModifier(Durability))
                    {
                        dungeonUpdateText.text = item.name + " has been Slain";
                        enemiesDefeated++;
                        floorProgress.fillAmount = (float)enemiesDefeated / enemyCount;
                        yield return new WaitForSeconds(1f);
                    }
                }
            }
            else
            {
                dungeonUpdateText.text = item.name + " has been Slain";
                enemiesDefeated++;
                floorProgress.fillAmount = (float)enemiesDefeated / enemyCount;
                yield return new WaitForSeconds(1f);
            }
        }
    }
    public IEnumerator MoveToNextDungeonFloor(Dungeon inDungeon, int damageCheck)
    {
        dungeonProgress.fillAmount = (float)curDungeonFloor / FindObjectOfType<GameManager>().curDungeon.floors.Length;
        enemiesDefeated = 0;
        curDungeonFloor++;
        dungeonProgress.fillAmount = (float)curDungeonFloor / FindObjectOfType<GameManager>().curDungeon.floors.Length;
        if (Health <= (Health / 2) || Personality == personality.Nervous)
        {
            dungeonUpdateText.text = charName + " is resting before moving forwards";
            Health += Random.Range(1, Health / 4);
            if (Health > originalHealth)
                Health = originalHealth;
            yield return new WaitForSeconds(4);
        }
        dungeonUpdateText.text = charName + " is progressing to floor " + (curDungeonFloor + 1);
        if (inDungeon.floors.Length - 1 >= curDungeonFloor)
        {
            enemyCount = inDungeon.floors[curDungeonFloor].floorData.Length;
            Health -= damageCheck - Luck;
            if (Health > 0)
            {
                yield return new WaitForSeconds(1 + (curDungeonFloor * 5));
                goldEarned += PullFloorData(inDungeon) * Random.Range(1, 21) + Random.Range(1, 21);
                if (damageCheck >= 30)
                    levelsGained += 1;
                else
                {
                    if (Random.Range(1, 21) > 16)
                        levelsGained += 1;
                }
                RunDungeon(inDungeon);
            }
            else
                DiedInDungeon(inDungeon);
        }
        else
        {
            SurvivedDungeon(inDungeon, goldEarned);
        }
    }
    public void SurvivedDungeon(Dungeon inDungeon, int totalGoldEarned)
    {
        if (Personality == personality.Explorer)
            goldEarned += Random.Range(20, 41);
        Destroy(adventurerUpdate);
        if (levelsGained <= 0)
            levelsGained = 1;
        string updateText = charName + " has defeated " + inDungeon.dungeonName + " and won " + goldEarned + " Gold Pieces, gaining " + levelsGained + " Total Levels";
        Health = originalHealth;
        statModifier += levelsGained;
        Health += (statModifier * Random.Range(1, 5));
        AdventurerGold += goldEarned;
        FindObjectOfType<AdventurerManager>().beatDungeon(updateText);
        ResetStats();
        if (Personality == personality.blank)
        {
            int randCheck = Random.Range(0, 13);
            switch (randCheck)
            {
                case 0:
                    Personality = personality.Cocky;
                    break;
                case 1:
                    Personality = personality.Determined;
                    break;
                case 2:
                    Personality = personality.Explorer;
                    break;
                case 3:
                    Personality = personality.Generous;
                    break;
                case 4:
                    Personality = personality.Nervous;
                    break;
                case 5:
                    Personality = personality.Sturdy;
                    break;
                case 6:
                    Personality = personality.Trapmaster;
                    break;
                default:
                    Personality = personality.Skillful;
                    break;
            }
        }
    }
    public void DiedInDungeon(Dungeon inDungeon)
    {
        string updateText = charName + " was slain in " + inDungeon.dungeonName + " on floor " + (curDungeonFloor + 1);
        FindObjectOfType<AdventurerManager>().FellInDungeon(updateText);
        if (File.Exists(Application.persistentDataPath + "/Data/Characters/" + charName + ".sdcd"))
        {
            File.Delete(Application.persistentDataPath + "/Data/Characters/" + charName + ".sdcd");
        }
        Destroy(adventurerUpdate);
        Destroy(this.gameObject);
    }
    void WriteAdventurerFromData()
    {
        timesVisitedShop++;
        bool isReturning = false;
        if (FindObjectOfType<AdventurerManager>().regularAdventurers.Contains(charName))
        {
            isReturning = true;
        }
        else if (FindObjectOfType<AdventurerManager>().recurringAdventurers.Contains(charName))
        {
            if (Random.Range(0, 21) >= (10 - timesVisitedShop))
            {
                isReturning = true;
                if (timesVisitedShop >= 5)
                {
                    FindObjectOfType<AdventurerManager>().recurringAdventurers.Remove(charName);
                    FindObjectOfType<AdventurerManager>().regularAdventurers.Add(charName);
                }
            }
            else
            {
                if (Random.Range(0, 21) >= 10)
                {
                    isReturning = false;
                    FindObjectOfType<AdventurerManager>().recurringAdventurers.Remove(charName);
                    //Destroy Player Data
                }
            }
        }
        else
        {
            if (Random.Range(0, 21) >= 13)
                isReturning = true;
        }
        if (isReturning)
        {
            if (!FindObjectOfType<AdventurerManager>().recurringAdventurers.Contains(charName))
                FindObjectOfType<AdventurerManager>().recurringAdventurers.Add(charName);
            string AdventurerData = "";
            List<string> AdventurerRawCollectedData = new List<string>();
            AdventurerRawCollectedData.Add(Strength + "#" + Agility + "#" + Intelligence + "#" + Moxie + "#" + Durability + "#" + statModifier + "#" + timesVisitedShop + "#" + originalHealth);
            AdventurerRawCollectedData.Add(charName + "#" + AdventurerGold + "#" + curRace.ToString() + "#" + curClass.ToString());
            AdventurerData = string.Join("|", AdventurerRawCollectedData);

            AdventurerTestData check = new AdventurerTestData();
            check.text = AdventurerData;
            check.personaliT = Personality.ToString();

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/Data/Characters/" + charName + ".sdcd");

            bf.Serialize(file, check);
            file.Close();

            FindObjectOfType<AdventurerManager>().UpdateGameData();

            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public enum foodType
    {
        ᚠ,
        ᚣ,
        ᛄ
    };
    public void StartHighlight()
    {
        FindObjectOfType<GameManager>().BuildAdData(this);
        startPos = transform.position;
    }
    public void EndHightlight()
    {
        FindObjectOfType<GameManager>().EndDragAdventurer();
        transform.SetParent(FindObjectOfType<GameManager>().spawnArea.transform);
        if (isEnteringDungeon)
            EnterDungeon();
        if (transform.position != startPos)
            transform.position = Vector2.zero;
    }
    public void BeginDragData()
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("Respawn").transform);
        FindObjectOfType<GameManager>().StartDragAdventurer();
    }
    public void OnDrag(PointerEventData data)
    {
        transform.position = data.position;
    }
    public void EnterDungeon()
    {
        FindObjectOfType<GameManager>().ClearAdData();
        Canvas.ForceUpdateCanvases();
        transform.SetParent(null);
        GetComponent<Image>().enabled = false;
        GetComponent<EventTrigger>().enabled = false;
        RunDungeon(FindObjectOfType<GameManager>().curDungeon);
        FindObjectOfType<GameManager>().inShops.Remove(this);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("door"))
            isEnteringDungeon = true;
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("door"))
            isEnteringDungeon = false;
    }
    IEnumerator AnimateSprite(string checkPath)
    {
        Image im = GetComponent<Image>();
        if (im != null)
        {
            im.sprite = Resources.Load<Sprite>(checkPath + "01");
            yield return new WaitForSeconds(.2f);
            im.sprite = Resources.Load<Sprite>(checkPath + "02");
            yield return new WaitForSeconds(.2f);
            im.sprite = Resources.Load<Sprite>(checkPath + "03");
            yield return new WaitForSeconds(.2f);
            im.sprite = Resources.Load<Sprite>(checkPath + "04");
            yield return new WaitForSeconds(.2f);
            im.sprite = Resources.Load<Sprite>(checkPath + "05");
            yield return new WaitForSeconds(.2f);
            StartCoroutine(AnimateSprite(checkPath));
        }
    }

    public void BuildAdventurerFromData(string inCharName)
    {
        string AdventurerData = "";
        string personal = "";
        if (File.Exists(Application.persistentDataPath + "/Data/Characters/" + inCharName + ".sdcd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Data/Characters/" + inCharName + ".sdcd", FileMode.Open);
            AdventurerTestData checkov = (AdventurerTestData)bf.Deserialize(file);
            file.Close();
            AdventurerData = checkov.text;
            personal = checkov.personaliT;
        }
        else
            Debug.Log("Didn't Find File: " + inCharName);
        string[] AdventurerRawCollectedData = AdventurerData.Split('|');
        string[] Stats = AdventurerRawCollectedData[0].Split('#');
        #region split stats
        Strength = int.Parse(Stats[0]);
        Agility = int.Parse(Stats[1]);
        Intelligence = int.Parse(Stats[2]);
        Moxie = int.Parse(Stats[3]);
        Durability = int.Parse(Stats[4]);
        statModifier = int.Parse(Stats[5]);
        timesVisitedShop = int.Parse(Stats[6]);
        originalHealth = int.Parse(Stats[7]);
        #endregion
        string[] metaData = AdventurerRawCollectedData[1].Split('#');
        #region split metaData
        charName = metaData[0];
        AdventurerGold = int.Parse(metaData[1]);
        curRace = (AdventurerRace)System.Enum.Parse(typeof(AdventurerRace), metaData[2]);
        curClass = (AdventurerClass)System.Enum.Parse(typeof(AdventurerClass), metaData[3]);
        Personality = (personality)System.Enum.Parse(typeof(personality), personal);
        #endregion
        SetupAdventurer(timesVisitedShop);
    }
    public void ResetStats()
    {
        Strength = originalStrength + Random.Range(1, levelsGained + 2);
        Intelligence = originalIntelligence + Random.Range(1, levelsGained + 2);
        Agility = originalAgility + Random.Range(1, levelsGained + 2);
        Durability = originalDurability + Random.Range(1, levelsGained + 2);
        Moxie = originalMoxie + Random.Range(1, levelsGained + 2);
        Health = originalHealth;
        Luck = originalLuck;
        WriteAdventurerFromData();
    }
}
#region SerializableObjects
[System.Serializable]
class AdventurerTestData
{
    public string text;
    public string personaliT;
}
#endregion