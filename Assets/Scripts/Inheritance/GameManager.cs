using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour, IStoreListener
{
    #region Variables
    #region Public
    #region Numbers
    [HideInInspector]
    public int Rent, curDay, HeroesSurvived, HeroesDied, playerMoney, previousHeroesSurvived, expeditionsSent, expeditionsToSpawn;
    [HideInInspector]
    public int premiumMoney = 10;
    [HideInInspector]
    public float dayTimer;
    [HideInInspector]
    public Text buyShopUIText, buyNewExpeditionText;
    [HideInInspector]
    public Expedition[] expeditionSpawns;
    [HideInInspector]
    public GameObject newExpeditionSpawn, ExpeditionHolder;
    [HideInInspector]
    public Button buynewExped;
    #endregion
    #region Objects
    [HideInInspector]
    public Shop curShop;
    [HideInInspector]
    public Dungeon curDungeon, dungeonToDiscover;
    public List<Dungeon> unlockedDungeons = new List<Dungeon>();
    [HideInInspector]
    public GameObject pauseMenu, shopFront, villageFront; //Parent Objects
    [HideInInspector]
    public GameObject ShopUI, shopPurchaseItem, PremiumShopUIHolder, PremiumCoinShop, PremiumFoodShop, adventurerUpdateObj, adventurerDetailArea, door; //Shop Objects
    public GameObject HelpUI;
    [HideInInspector]
    public Transform RecurringDisplayHolder, RegularDisplayHolder;
    [HideInInspector]
    public List<Adventurer> inShops = new List<Adventurer>();
    [HideInInspector]
    public List<Dungeon> undiscoveredDungeons = new List<Dungeon>();
    #endregion
    #region UI
    [HideInInspector]
    public Transform foodDragHandler, adventurerUpdateHandler, spawnArea;
    [HideInInspector]
    public Dropdown dungeonSelectionDropdown;
    [HideInInspector]
    public Text goldText, premiumText, dayDisplayText, adventurerUpdateText;
    [HideInInspector]
    public Image clockImage, storeCounter, storeBackground;
    [HideInInspector]
    public Color[] tierColors;
    [HideInInspector]
    public TMP_Text adventurerDetailsText;
    #endregion
    #region Bools
    public bool DebugTest = false;
    public bool isDayTime, runTutorial = true;
    #endregion
    #endregion
    #region Private
    #region stringData
    private string fileName = "/Data/Raw/gameData.sdmd";
    #endregion
    #endregion
    #endregion

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (Directory.Exists(Application.persistentDataPath + "/Data"))
            Read();
        else
            WriteAsNew();
    }
    public virtual void Start()
    {
        if (PlayerPrefs.GetString("curShop") == null || PlayerPrefs.GetString("curShop") == "")
        {
            curShop = Resources.Load<Shop>("shops/basicShop");
            PlayerPrefs.SetString("curShop", curShop.name);
        }
        else
            curShop = Resources.Load<Shop>("shops/" + PlayerPrefs.GetString("curShop"));
        if ((PlayerPrefs.GetString("curDungeon") == null || PlayerPrefs.GetString("curDungeon") == ""))
        {
            curDungeon = Resources.Load<Dungeon>("dungeons/spicyRatDungeon");
            PlayerPrefs.SetString("curDungeon", curDungeon.name);
        }
        else
            curDungeon = Resources.Load<Dungeon>("dungeons/" + PlayerPrefs.GetString("curDungeon"));
        if (adventurerUpdateObj == null)
            adventurerUpdateObj = GameObject.FindGameObjectWithTag("updateObj");
        if (spawnArea == null)
            spawnArea = GameObject.FindGameObjectWithTag("SpawnArea").transform;
        if (storeCounter == null)
            storeCounter = GameObject.FindGameObjectWithTag("storeCounter").GetComponent<Image>();
        if (storeBackground == null)
            storeBackground = GameObject.FindGameObjectWithTag("storeBackground").GetComponent<Image>();
        if (adventurerUpdateText == null)
            adventurerUpdateText = GameObject.FindGameObjectWithTag("updateText").GetComponent<Text>();
        if (adventurerDetailsText == null)
            adventurerDetailsText = GameObject.FindGameObjectWithTag("detailsText").GetComponent<TMP_Text>();
        if (adventurerDetailArea == null)
            adventurerDetailArea = GameObject.FindGameObjectWithTag("detailsObj");
        if (door == null)
            door = GameObject.FindGameObjectWithTag("door");
        door.SetActive(false);
        storeBackground.sprite = curShop.shopBackground;
        storeCounter.sprite = curShop.shopCounter;
        adventurerUpdateText.text = "";
        adventurerUpdateObj.SetActive(false);
        if (runTutorial)
        {
            dayTimer = 600f;
            isDayTime = true;
            PopulateDropdown();
            if (m_StoreController == null)
                InitializePurchasing();
            Advertisement.Initialize(gameId, false);
            StartCoroutine(SpawnInExpeditions());
            SetupStoreFront();
        }
    }
    public void Update()
    {
        if (runTutorial)
        {
            expeditionSpawns = FindObjectsOfType<Expedition>();
            buyNewExpeditionText.text = (expeditionSpawns.Length + expeditionSpawns.Length) + "";
            goldText.text = "x " + AbbreviationUtility.AbbreviateNumber(playerMoney);
            premiumText.text = "x " + AbbreviationUtility.AbbreviateNumber(premiumMoney);
            if (dayTimer >= 0)
                dayTimer -= Time.deltaTime;
            else if (isDayTime && FindObjectsOfType<Adventurer>().Length <= 0)
                ShiftDayPhase();
            clockImage.fillAmount = (float)dayTimer / 600;
            dayDisplayText.text = AbbreviationUtility.AbbreviateNumber(curDay);
            if (!isDayTime)
            {
                foreach (var item in GameObject.FindGameObjectsWithTag("itemBackground"))
                {
                    foreach (var item2 in unlockedDungeons)
                    {
                        if (item.transform.parent.GetComponentInChildren<Text>().text == item2.dungeonName)
                            item.GetComponent<Image>().color = tierColors[item2.starCount];
                    }
                }
            }
            if (premiumMoney < (expeditionSpawns.Length + expeditionSpawns.Length))
                buynewExped.enabled = false;
            else
                buynewExped.enabled = true;
        }
    }

    #region GamePlay Management
    #region MicroTransactions
    public void BuyNormalCurrency(string inCheck)
    {
        string[] checkData = inCheck.Split('|');
        if (premiumMoney >= int.Parse(checkData[1]))
        {
            playerMoney += int.Parse(checkData[0]);
            premiumMoney -= int.Parse(checkData[1]);
        }
        ToggleIAPShop();
    }
    #endregion
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string curSceneName = SceneManager.GetActiveScene().name;
        if (curSceneName == "Shopfront")
        {
            Screen.orientation = ScreenOrientation.Landscape;
            FindObjectOfType<AdventurerManager>().StartDataLoad();
        }
        else
            Screen.orientation = ScreenOrientation.Portrait;
    }
    public void UpdatePlayerVariables()
    {
        PlayerPrefs.SetString("curShop", curShop.name);
        PlayerPrefs.SetString("curDungeon", curDungeon.name);
    }
    public void SetupStoreFront()
    {
        Rent = curShop.Rent;
        ClearAdData();
        previousHeroesSurvived = HeroesSurvived;
        RunNewCustomer();
        BuyNewShopUI();
    }
    public void PopulateDropdown()
    {
        unlockedDungeons.Sort(SortByStars);
        List<string> options = new List<string>();
        foreach (var item in unlockedDungeons)
        {
            options.Add(item.dungeonName);
        }
        dungeonSelectionDropdown.ClearOptions();
        dungeonSelectionDropdown.AddOptions(options);
        dungeonSelectionDropdown.value = dungeonSelectionDropdown.options.FindIndex(option => option.text == curDungeon.dungeonName);
        dungeonSelectionDropdown.RefreshShownValue();
    }
    static int SortByStars(Dungeon p1, Dungeon p2)
    {
        return p1.starCount.CompareTo(p2.starCount);
    }
    public void SetFromDropdown()
    {
        string newDungeon = "";
        foreach (var item in unlockedDungeons)
        {
            if (item.dungeonName == dungeonSelectionDropdown.options[dungeonSelectionDropdown.value].text)
                newDungeon = item.name;
        }
        PlayerPrefs.SetString("curDungeon", newDungeon);
    }
    public void BuyNewShop()
    {
        BuyNewShopUI();
        Shop[] allShops = Resources.LoadAll<Shop>("shops/");
        foreach (var item in allShops)
        {
            if (item.shopTier == curShop.shopTier + 1)
            {
                if (playerMoney >= item.shopCost)
                {
                    playerMoney -= item.shopCost;
                    PlayerPrefs.SetString("curShop", item.name);
                }
            }
        }
    }
    public void ShiftDayPhase()
    {
        if (HelpUI.activeInHierarchy)
            HelpUI.GetComponent<HelpManager>().CloseHelp();
        else
        {
            inShops.Clear();
            undiscoveredDungeons.Clear();
            foreach (var item in Resources.LoadAll<Dungeon>("dungeons/"))
            {
                if (!unlockedDungeons.Contains(item))
                    undiscoveredDungeons.Add(item);
            }
            foreach (var item in FindObjectsOfType<Adventurer>())
            {
                Destroy(item.gameObject);
            }
            isDayTime = !isDayTime;
            if (isDayTime)
            {
                FindObjectOfType<NavigationManager>().SwapToDay();
                FindObjectOfType<FoodManager>().GenerateIngredients();
                dayTimer = 600;
                SetupStoreFront();
            }
            else
            {
                FindObjectOfType<NavigationManager>().SwapToNight();
                int AdvertisementCheck = Random.Range(1, 101);
                if (AdvertisementCheck > 75)
                    PlayAd();
                playerMoney += HeroesSurvived - previousHeroesSurvived;
                if (dungeonToDiscover != null)
                {
                    unlockedDungeons.Add(dungeonToDiscover);
                    undiscoveredDungeons.Remove(dungeonToDiscover);
                    dungeonToDiscover = null;
                }
                dayTimer = 0;
                foreach (var item in FindObjectsOfType<Adventurer>())
                {
                    Destroy(item);
                }
                PopulateDropdown();
                curDay++;
                if (Rent > 0)
                    playerMoney -= Rent;
                if (playerMoney <= -1000)
                {
                    StartCoroutine(failedGame());
                    return;
                }
                StopAllCoroutines();
                StartCoroutine(spawnNewCustomer());
            }
            if (curDay > 0)
            {
                Write();
            }
        }
    }
    public void CheckExpedition()
    {
        int expeditionCheck = Random.Range(0, 401);
        if (expeditionCheck < 50)
        {
            //Discovered Treasure
            int playerGain = Random.Range(1, 101 + (curDay));
            playerMoney += playerGain;
            int premiumGain = Random.Range(1, 21);
            premiumMoney += premiumGain;
            StartCoroutine(UpdateAdventurerText("The expedition Found a chest containing " + playerGain + " Gold Pieces and " + premiumGain + " Star Coins!"));
        }
        else if (expeditionCheck >= 50 && expeditionCheck < 150)
        {
            dungeonToDiscover = undiscoveredDungeons[Random.Range(0, undiscoveredDungeons.Count)];
            unlockedDungeons.Add(dungeonToDiscover);
            undiscoveredDungeons.Remove(dungeonToDiscover);
            StartCoroutine(UpdateAdventurerText("The Expedition discovered an ancient ruined site, and uncovered the " + dungeonToDiscover.dungeonName));
            dungeonToDiscover = null;
        }
        else if (expeditionCheck >= 150 && expeditionCheck < 375)
        {
            int playerGain = Random.Range(1, 101 + (curDay));
            playerMoney += playerGain;
            int premiumGain = Random.Range(1, 21);
            premiumMoney += premiumGain;
            int randFoodItem = Random.Range(0, FindObjectOfType<FoodManager>().availableIngredients.Count);
            int randAmountGained = Random.Range(1, 11);
            FindObjectOfType<FoodManager>().availableIngredients[randFoodItem].foodCount += randAmountGained;
            FindObjectOfType<FoodManager>().GenerateIngredients();
            StartCoroutine(UpdateAdventurerText("Wow! The expedition discovered an ancient ruined temple, and found a preserve of magically fresh food, foraging " + randAmountGained + " piece(s) of " +
                                                FindObjectOfType<FoodManager>().availableIngredients[randFoodItem].foodName + "! We also uncovered a chest containing " + playerGain + " Gold and "
                                                + premiumGain + " Star Coins!"));
        }
        else
            StartCoroutine(UpdateAdventurerText("The Expedition travelled far and wide, and returned empty-handed"));
    }
    public void OpenShop(int i)
    {
        int bonusCost = 0;
        if (isDayTime)
            bonusCost = 2 * curShop.shopTier;
        if (i != 3)
            ShopUI.transform.parent.transform.parent.gameObject.SetActive(true);
        #region BuildDataToBuy
        List<Ingredient> allIngredients = new List<Ingredient>();
        Sprite[] proteinList = Resources.LoadAll<Sprite>("ingredientImages/proteins");
        Sprite[] fillersList = Resources.LoadAll<Sprite>("ingredientImages/fillers");
        Sprite[] ricesList = Resources.LoadAll<Sprite>("ingredientImages/rices");
        Sprite[] drinksList = Resources.LoadAll<Sprite>("ingredientImages/drinks");
        Sprite[] premiumList = Resources.LoadAll<Sprite>("ingredientImages/premium");
        Ingredient[] toDelete = FindObjectsOfType<Ingredient>();
        foreach (var item in toDelete)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in proteinList)
        {
            string newAddress = Application.persistentDataPath + "/Data/Foods/" + item.name + ".fmid";
            if (File.Exists(newAddress))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(newAddress, FileMode.Open);
                TestFood check = (TestFood)bf.Deserialize(file);
                file.Close();
                GameObject newIngredient = Instantiate(new GameObject("Holder"), null);
                newIngredient.AddComponent<Ingredient>().GenerateVariables(check);
                allIngredients.Add(newIngredient.GetComponent<Ingredient>());
            }
        }
        foreach (var item in fillersList)
        {
            string newAddress = Application.persistentDataPath + "/Data/Foods/" + item.name + ".fmid";
            if (File.Exists(newAddress))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(newAddress, FileMode.Open);
                TestFood check = (TestFood)bf.Deserialize(file);
                file.Close();
                GameObject newIngredient = Instantiate(new GameObject("Holder"), null);
                newIngredient.AddComponent<Ingredient>().GenerateVariables(check);
                allIngredients.Add(newIngredient.GetComponent<Ingredient>());
            }
        }
        foreach (var item in drinksList)
        {
            string newAddress = Application.persistentDataPath + "/Data/Foods/" + item.name + ".fmid";
            if (File.Exists(newAddress))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(newAddress, FileMode.Open);
                TestFood check = (TestFood)bf.Deserialize(file);
                file.Close();
                GameObject newIngredient = Instantiate(new GameObject("Holder"), null);
                newIngredient.AddComponent<Ingredient>().GenerateVariables(check);
                allIngredients.Add(newIngredient.GetComponent<Ingredient>());
            }
        }
        foreach (var item in ricesList)
        {
            string newAddress = Application.persistentDataPath + "/Data/Foods/" + item.name + ".fmid";
            if (File.Exists(newAddress))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(newAddress, FileMode.Open);
                TestFood check = (TestFood)bf.Deserialize(file);
                file.Close();
                GameObject newIngredient = Instantiate(new GameObject("Holder"), null);
                newIngredient.AddComponent<Ingredient>().GenerateVariables(check);
                allIngredients.Add(newIngredient.GetComponent<Ingredient>());
            }
        }
        foreach (var item in premiumList)
        {
            string newAddress = Application.persistentDataPath + "/Data/Foods/" + item.name + ".fmid";
            if (File.Exists(newAddress))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(newAddress, FileMode.Open);
                TestFood check = (TestFood)bf.Deserialize(file);
                file.Close();
                GameObject newIngredient = Instantiate(new GameObject("Holder"), null);
                newIngredient.AddComponent<Ingredient>().GenerateVariables(check);
                allIngredients.Add(newIngredient.GetComponent<Ingredient>());
            }
        }
        #endregion
        if (i == 0)
        {
            foreach (var item in allIngredients)
            {
                if (item.preferenceType == Ingredient.BigFoodType.ᚠ)
                {
                    GameObject ShopPurchaseItem = Instantiate(shopPurchaseItem, ShopUI.transform);
                    ShopPurchaseItem.GetComponent<Image>().sprite = item.foodImg;
                    ShopPurchaseItem.GetComponent<Button>().onClick.AddListener(delegate { BuyProduct(item); });
                    Debug.Log(item.foodName);
                    ShopPurchaseItem.GetComponentInChildren<Text>().text = Mathf.FloorToInt(item.cost - (item.cost / 2) + bonusCost).ToString();
                    if (Mathf.FloorToInt(item.cost - (item.cost / 2)) > playerMoney)
                    {
                        ShopPurchaseItem.GetComponent<Button>().interactable = false;
                        ShopPurchaseItem.GetComponent<Image>().color = Color.black;
                        ShopPurchaseItem.GetComponentInChildren<Text>().color = Color.red;
                    }
                    else
                        ShopPurchaseItem.GetComponentInChildren<Text>().color = Color.green;
                }
                else
                    Destroy(item);
            }
        }
        else if (i == 1)
        {
            foreach (var item in allIngredients)
            {
                if (item.preferenceType == Ingredient.BigFoodType.ᚣ)
                {
                    GameObject ShopPurchaseItem = Instantiate(shopPurchaseItem, ShopUI.transform);
                    ShopPurchaseItem.GetComponent<Image>().sprite = item.foodImg;
                    ShopPurchaseItem.GetComponent<Button>().onClick.AddListener(delegate { BuyProduct(item); });
                    Debug.Log(item.foodName);
                    ShopPurchaseItem.GetComponentInChildren<Text>().text = Mathf.FloorToInt(item.cost - (item.cost / 2) + bonusCost).ToString();
                    if (Mathf.FloorToInt(item.cost - (item.cost / 2)) > playerMoney)
                    {
                        ShopPurchaseItem.GetComponent<Button>().interactable = false;
                        ShopPurchaseItem.GetComponent<Image>().color = Color.black;
                        ShopPurchaseItem.GetComponentInChildren<Text>().color = Color.red;
                    }
                    else
                        ShopPurchaseItem.GetComponentInChildren<Text>().color = Color.green;
                }
                else
                    Destroy(item);
            }
        }
        else if (i == 2)
        {
            foreach (var item in allIngredients)
            {
                if (item.preferenceType == Ingredient.BigFoodType.ᛄ)
                {
                    GameObject ShopPurchaseItem = Instantiate(shopPurchaseItem, ShopUI.transform);
                    ShopPurchaseItem.GetComponent<Image>().sprite = item.foodImg;
                    ShopPurchaseItem.GetComponent<Button>().onClick.AddListener(delegate { BuyProduct(item); });
                    Debug.Log(item.foodName);
                    ShopPurchaseItem.GetComponentInChildren<Text>().text = Mathf.FloorToInt(item.cost - (item.cost / 2) + bonusCost).ToString();
                    if (Mathf.FloorToInt(item.cost - (item.cost / 2)) > playerMoney)
                    {
                        ShopPurchaseItem.GetComponent<Button>().interactable = false;
                        ShopPurchaseItem.GetComponent<Image>().color = Color.black;
                        ShopPurchaseItem.GetComponentInChildren<Text>().color = Color.red;
                    }
                    else
                        ShopPurchaseItem.GetComponentInChildren<Text>().color = Color.green;
                }
                else
                    Destroy(item);
            }
        }
        else if (i == 3)
        {
            foreach (var item in allIngredients)
            {
                if (item.preferenceType == Ingredient.BigFoodType.Premium)
                {
                    GameObject ShopPurchaseItem = Instantiate(shopPurchaseItem, PremiumShopUIHolder.transform);
                    ShopPurchaseItem.GetComponent<Image>().sprite = item.foodImg;
                    ShopPurchaseItem.GetComponent<Button>().onClick.AddListener(delegate { BuyProduct(item); });
                    Debug.Log(item.foodName);
                    ShopPurchaseItem.GetComponentInChildren<Text>().text = Mathf.FloorToInt(item.cost - (item.cost / 2) + bonusCost).ToString();
                    if (Mathf.FloorToInt(item.cost - (item.cost / 2)) > premiumMoney)
                    {
                        ShopPurchaseItem.GetComponent<Button>().interactable = false;
                        ShopPurchaseItem.GetComponent<Image>().color = Color.black;
                        ShopPurchaseItem.GetComponentInChildren<Text>().color = Color.red;
                    }
                    else
                        ShopPurchaseItem.GetComponentInChildren<Text>().color = Color.green;
                }
                else
                    Destroy(item);
            }
        }
    }
    public void ToggleIAPoptions()
    {
        foreach (var item in PremiumShopUIHolder.GetComponentsInChildren<Button>())
        {
            Destroy(item.gameObject);
        }
        PremiumCoinShop.SetActive(!PremiumCoinShop.activeInHierarchy);
        PremiumFoodShop.SetActive(!PremiumFoodShop.activeInHierarchy);
        if (PremiumFoodShop.activeInHierarchy)
            OpenShop(3);
    }
    public void BuyProduct(Ingredient newProduct)
    {
        int bonusCost = 0;
        if (isDayTime)
            bonusCost = 2 * curShop.shopTier;
        if (newProduct.preferenceType != Ingredient.BigFoodType.Premium && FindObjectOfType<GameManager>().playerMoney >= Mathf.FloorToInt(newProduct.cost - (newProduct.cost / 2)))
            FindObjectOfType<GameManager>().playerMoney -= Mathf.FloorToInt(newProduct.cost - (newProduct.cost / 2) + bonusCost);
        else if (newProduct.preferenceType == Ingredient.BigFoodType.Premium && FindObjectOfType<GameManager>().premiumMoney >= Mathf.FloorToInt(newProduct.cost - (newProduct.cost / 2)))
            FindObjectOfType<GameManager>().premiumMoney -= Mathf.FloorToInt(newProduct.cost - (newProduct.cost / 2) + bonusCost);
        FindObjectOfType<FoodManager>().BuyIngredient(newProduct);
    }
    public void CloseShopUI()
    {
        ShopUI.transform.parent.transform.parent.gameObject.SetActive(false);
        foreach (var item in ShopUI.GetComponentsInChildren<Button>())
        {
            Destroy(item.gameObject);
        }
        Write();
    }
    public void GenerateNewExpeditonTeam()
    {
        if (premiumMoney >= (expeditionSpawns.Length + expeditionSpawns.Length))
        {
            if (expeditionSpawns.Length < curShop.shopTier)
            {
                GameObject newExpedition = Instantiate(newExpeditionSpawn, ExpeditionHolder.transform);
                expeditionsToSpawn++;
            }
        }
    }
    public IEnumerator SpawnInExpeditions()
    {
        if (PlayerPrefs.HasKey("expeditionsMax"))
            expeditionsToSpawn = PlayerPrefs.GetInt("expeditionsMax");
        else
        {
            expeditionsToSpawn = 1;
            PlayerPrefs.SetInt("expeditionsMax", 1);
        }
        expeditionSpawns = FindObjectsOfType<Expedition>();
        if (expeditionSpawns.Length == 0)
        {
            while (expeditionsToSpawn > 0)
            {
                GameObject newExpedition = Instantiate(newExpeditionSpawn, ExpeditionHolder.transform);
                expeditionsToSpawn--;
            }
        }
        yield return new WaitForSeconds(.01f);
    }
    #endregion

    #region Drag Management
    public void StartDragAdventurer()
    {
        adventurerDetailArea.SetActive(false);
        door.SetActive(true);
    }
    public void EndDragAdventurer()
    {
        adventurerDetailArea.SetActive(true);
        door.SetActive(false);
    }
    #endregion

    #region Display Management
    public void BuildAdData(Adventurer inAd)
    {
        string raceCapital = (char.ToUpper(inAd.curRace.ToString()[0]) + ((inAd.curRace.ToString().Length > 1) ? inAd.curRace.ToString().Substring(1).ToLower() : string.Empty));
        string classCapital = (char.ToUpper(inAd.curClass.ToString()[0]) + ((inAd.curClass.ToString().Length > 1) ? inAd.curClass.ToString().Substring(1).ToLower() : string.Empty));
        string foodToConsume = "";
        for (int i = 0; i < inAd.canEat.Count; i++)
        {
            string ToAdd = "";
            switch (inAd.canEat[i])
            {
                case Adventurer.foodType.ᚠ:
                    ToAdd = "<sprite index= 0>";
                    break;
                case Adventurer.foodType.ᚣ:
                    ToAdd = "<sprite index= 1>";
                    break;
                case Adventurer.foodType.ᛄ:
                    ToAdd = "<sprite index= 2>";
                    break;
            }
            if (i < inAd.canEat.Count)
                foodToConsume += ToAdd + ", ";
            else if (i >= inAd.canEat.Count)
                foodToConsume += ToAdd;
        }
        adventurerDetailsText.text = inAd.charName + "    Level: " + inAd.statModifier + "\n" + "Race: " + raceCapital + "    Class: " + classCapital + "\n" + "Prefers: " + foodToConsume +
            "\n" + "Skill Score: " + inAd.CR + "\n\n" + "Strength: " + inAd.Strength + "\n" + "Agility: " + inAd.Agility + "\n" + "Intelligence: " + inAd.Agility + "\n" +
            "Durability: " + inAd.Durability + "\n" + "Moxie: " + inAd.Moxie + "\n" + "Health: " + inAd.Health + "\n\n" + "Gold: " + inAd.AdventurerGold;
    }
    public void BuildFoodData(Ingredient inFood)
    {
        string checkType = "";
        string checkStat = "";
        foreach (var item in inFood.increaseStats)
        {
            if (!checkStat.Contains(item.ToString()))
                checkStat += (item + " by " + inFood.typeOf(item).ToString()) + "\n";
        }
        int decreaseCheck = 1;
        if (inFood.decreaseStats == Ingredient.statTypes.Health)
            decreaseCheck = 5;
        switch (inFood.preferenceType)
        {
            case Ingredient.BigFoodType.ᚠ:
                checkType = "<sprite index= 0>";
                break;
            case Ingredient.BigFoodType.ᚣ:
                checkType = "<sprite index= 1>";
                break;
            case Ingredient.BigFoodType.ᛄ:
                checkType = "<sprite index= 2>";
                break;
        }
        string dataPush = inFood.foodName + "   " + checkType + "\n" + "Costs: " + inFood.cost + "\n" + "\n Increase " + checkStat + "\n Decrease " + inFood.decreaseStats.ToString() + " by " + decreaseCheck;

        adventurerDetailsText.text = dataPush;
    }
    public void ClearAdData()
    {
        string statsToDecrease = "";
        foreach (var item in curDungeon.statModifiers)
        {
            if (!statsToDecrease.Contains(item.ToString()))
                statsToDecrease += ", " + item.ToString();
        }
        if (statsToDecrease.Length > 0)
        {
            statsToDecrease = statsToDecrease.Substring(2);
            adventurerDetailsText.text = curDungeon.dungeonName + "\n\n" + "Difficulty: " + (Mathf.FloorToInt((float)(curDungeon.statPenalty + curDungeon.starCount) / curDungeon.statModifiers.Count) + Mathf.Abs(firstFloorCr(curDungeon.floors[0]) - dungeonCR(curDungeon.floors[curDungeon.floors.Length - 1]))) + "\n\n" + "Reduces: " + statsToDecrease + "\n" + "by: " + curDungeon.statPenalty + " at Level: " + curDungeon.penaltyCheck;
        }
        else
        {
            adventurerDetailsText.text = curDungeon.dungeonName + "\n\n" + "Difficulty: " + Mathf.Abs(firstFloorCr(curDungeon.floors[0]) - dungeonCR(curDungeon.floors[curDungeon.floors.Length - 1])) + "\n\n" + "Reduces no stats upon entry";
        }
    }
    public int firstFloorCr(Floor inFloor)
    {
        int CROut = 100;
        foreach (var item in inFloor.floorData)
        {
            if (item.CR < CROut)
                CROut = item.CR;
        }
        return CROut;
    }
    public int dungeonCR(Floor inFloor)
    {
        int curFloorCR = 0;
        foreach (var item in inFloor.floorData)
        {
            curFloorCR += item.CR;
        }
        curFloorCR = Mathf.CeilToInt(curFloorCR * .5f);
        return curFloorCR;
    }
    public IEnumerator UpdateAdventurerText(string updateData)
    {
        adventurerUpdateObj.SetActive(true);
        adventurerUpdateText.text = updateData;
        yield return new WaitForSeconds(4.5f);
        float fadeDelay = 4;
        while (fadeDelay > 0)
        {
            fadeDelay -= Time.deltaTime;
        }
        adventurerUpdateObj.SetActive(false);
        adventurerUpdateText.text = "";
    }
    public IEnumerator failedGame()
    {
        Debug.Log("Failed To Make a Profit");
        string shopName = "PLACEHOLDER SHOP NAME";
        adventurerUpdateObj.SetActive(true);
        adventurerUpdateText.text = "After " + curDay + " days, " + shopName + " closed its doors permanently, the owner passing on their treasures to their child, to hopefully follow in their footsteps";
        yield return new WaitForSeconds(10f);
        ClearData(true);
    }
    public void BuyNewShopUI()
    {
        Debug.Log("Searching For Shops");
        Shop[] allShops = Resources.LoadAll<Shop>("shops/");
        foreach (var item in allShops)
        {
            Debug.Log(item.name);
            if (item.shopTier == (curShop.shopTier + 1))
                buyShopUIText.text = "Buy New Shop for: " + item.shopCost;
        }
    }
    #endregion

    #region Customer Management
    public void RunNewCustomer()
    {
        StartCoroutine(spawnNewCustomer());
        FindObjectOfType<AdventurerManager>().SpawnNewAdventurer();
    }
    public IEnumerator spawnNewCustomer()
    {
        yield return new WaitForSeconds((Random.Range(7, 11)));
        FindObjectOfType<AdventurerManager>().SpawnNewAdventurer();
        StartCoroutine(spawnNewCustomer());
    }
    #endregion

    #region Data Management
    public void ClearData(bool didFail)
    {
        Debug.Log("Deleting Data");
        if (didFail)
            PlayerPrefs.SetInt("premiumMoney", premiumMoney);
        else
            PlayerPrefs.DeleteKey("premiumMoney");
        PlayerPrefs.DeleteKey("curDungeon");
        PlayerPrefs.DeleteKey("curShop");
        Directory.Delete(Application.persistentDataPath + "/Data", true);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void Read()
    {
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
            managerData check = (managerData)bf.Deserialize(file);
            file.Close();
            List<string> rawData = new List<string>();
            rawData.AddRange(check.data.Split('|'));
            curDay = int.Parse(rawData[0]);
            playerMoney = int.Parse(rawData[1]);
            Rent = int.Parse(rawData[2]);
            curShop = Resources.Load<Shop>("/shops/" + rawData[3]);
            premiumMoney = int.Parse(rawData[4]);
            HeroesSurvived = int.Parse(rawData[5]);
            HeroesDied = int.Parse(rawData[6]);
            foreach (var item in Resources.LoadAll<Dungeon>("dungeons/"))
            {
                if (check.unlockedDungeons.Contains(item.name) && !unlockedDungeons.Contains(item))
                    unlockedDungeons.Add(item);
                else if (!check.unlockedDungeons.Contains(item.name))
                    undiscoveredDungeons.Add(item);
            }
        }
        else
            WriteAsNew();
    }
    public void Write()
    {
        PlayerPrefs.SetInt("expeditionsMax", expeditionsToSpawn);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + fileName);

        managerData check = new managerData();
        List<string> toJoinList = new List<string>();
        foreach (var item in unlockedDungeons)
        {
            check.unlockedDungeons.Add(item.name);
        }
        toJoinList.Add(curDay.ToString());
        toJoinList.Add(playerMoney.ToString());
        toJoinList.Add(Rent.ToString());
        toJoinList.Add(curShop.name);
        toJoinList.Add(premiumMoney.ToString());
        toJoinList.Add(HeroesSurvived.ToString());
        toJoinList.Add(HeroesDied.ToString());
        string toJoin = string.Join("|", toJoinList);

        check.data = toJoin;
        bf.Serialize(file, check);
        file.Close();
    }
    public void WriteAsNew()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + fileName);

        managerData check = new managerData();
        List<string> toJoinList = new List<string>();
        foreach (var item in unlockedDungeons)
        {
            check.unlockedDungeons.Add(item.name);
        }
        toJoinList.Add(0.ToString());
        toJoinList.Add(500.ToString());
        toJoinList.Add(100.ToString());
        toJoinList.Add(curShop.name);
        if (!PlayerPrefs.HasKey("premiumMoney"))
            toJoinList.Add(10.ToString());
        else
            toJoinList.Add(PlayerPrefs.GetInt("premiumMoney").ToString());
        toJoinList.Add(0.ToString());
        toJoinList.Add(0.ToString());
        string toJoin = string.Join("|", toJoinList);

        check.data = toJoin;
        bf.Serialize(file, check);
        file.Close();

        Read();
    }
    #endregion

    #region Monetization
    #region Purchases
    public GameObject IAPShop, adPopUp;
    static IStoreController m_StoreController;
    static IExtensionProvider m_StoreExtensionProvider;
    public products ProductChoice;
    public static string productToBuy = "consumable";
    string[] TypeNames = System.Enum.GetNames(typeof(products));
    public void ToggleIAPShop()
    {
        IAPShop.SetActive(!IAPShop.activeInHierarchy);
    }
    public void InitializePurchasing()
    {
        if (isInitialized())
            return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var item in TypeNames)
        {
            builder.AddProduct(item, ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }
    bool isInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }
    public void BuyConsumable(int productBuy)
    {
        ProductChoice = (products)productBuy;
        productToBuy = ProductChoice.ToString();
        BuyProductID(ProductChoice.ToString());
    }
    void BuyProductID(string productID)
    {
        if (isInitialized())
        {
            Product product = m_StoreController.products.WithID(productID);
            if (product != null && product.availableToPurchase)
                m_StoreController.InitiatePurchase(product);
            else
                Debug.Log("Zilch");
        }
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializeFailureReason:" + error);
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (string.Equals(args.purchasedProduct.definition.id, productToBuy, System.StringComparison.Ordinal))
        {
            Debug.Log("Success!");
            switch (ProductChoice)
            {
                case products.Product_10_Premium:
                    premiumMoney += 10;
                    break;
                case products.Product_50_Premium:
                    premiumMoney += 50;
                    break;
                case products.Product_100_Premium:
                    premiumMoney += 100;
                    break;
                case products.Product_250_Premium:
                    premiumMoney += 250;
                    break;
                case products.Product_500_Premium:
                    premiumMoney += 500;
                    break;
                case products.Product_1000_Premium:
                    premiumMoney += 1000;
                    break;
            }
        }
        ToggleIAPShop();
        return PurchaseProcessingResult.Complete;
    }
    void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        ToggleIAPShop();
        throw new System.NotImplementedException();
    }

    public enum products
    {
        Product_10_Premium,
        Product_50_Premium,
        Product_100_Premium,
        Product_250_Premium,
        Product_500_Premium,
        Product_1000_Premium
    };
    #endregion
    #region Advertising
    string gameId = "4001240";

    public void PlayAd()
    {
        if (Advertisement.IsReady())
            Advertisement.Show();
    }
    #endregion
    #endregion
}
[System.Serializable]
class managerData
{
    public string data;
    public List<string> unlockedDungeons = new List<string>();
}
public static class AbbreviationUtility
{
    private static readonly SortedDictionary<long, string> abbreviations = new SortedDictionary<long, string>
    {
        {1000, "K" },
        {1000000, "M" },
        {1000000000, "B" },
        {1000000000000, "T" },
        {1000000000000000, "q" },
        {1000000000000000000, "Q" }
    };
    public static string AbbreviateNumber(long number)
    {
        for (int i = abbreviations.Count - 1; i >= 0; i--)
        {
            KeyValuePair<long, string> pair = abbreviations.ElementAt(i);
            if (Mathf.Abs(number) >= pair.Key)
            {
                int roundedNumber = Mathf.FloorToInt(number / pair.Key);
                return roundedNumber.ToString() + pair.Value;
            }
        }
        return number.ToString();
    }
}