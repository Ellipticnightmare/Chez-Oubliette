using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

[RequireComponent(typeof(EventTrigger))]
public class Ingredient : Food
{
    [Range(1, 25)]
    public int cost = 1;
    public List<statTypes> increaseStats = new List<statTypes>();
    public statTypes decreaseStats;
    public bool isClicking, isFeeding, isDrag;
    public Transform initialTransform;
    public Image inIm;
    public Adventurer adven;
    public BigFoodType preferenceType = BigFoodType.Premium;
    public string path;
    public Vector2 startPos;
    private void Awake()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { Drag((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }
    public void GenerateVariables(TestFood inFood)
    {
        base.foodName = inFood.itemName;
        path = inFood.path;
        base.foodCount = inFood.itemCount;
        switch (foodName)
        {
            default:
                preferenceType = BigFoodType.Premium;
                break;
            #region Proteins Section
            case "Bacon":
                increaseStats.Add(statTypes.Strength);
                increaseStats.Add(statTypes.Strength);
                increaseStats.Add(statTypes.Strength);
                decreaseStats = statTypes.Health;
                cost = 4;
                preferenceType = BigFoodType.ᚠ;
                path = "proteins/";
                break;
            case "Steak":
                increaseStats.Add(statTypes.Strength);
                increaseStats.Add(statTypes.Durability);
                increaseStats.Add(statTypes.Durability);
                cost = 5;
                preferenceType = BigFoodType.ᚠ;
                path = "proteins/";
                break;
            case "Chicken":
                increaseStats.Add(statTypes.Strength);
                increaseStats.Add(statTypes.Agility);
                increaseStats.Add(statTypes.Health);
                cost = 5;
                preferenceType = BigFoodType.ᚣ;
                path = "proteins/";
                break;
            case "Salmon":
                increaseStats.Add(statTypes.Intelligence);
                increaseStats.Add(statTypes.Intelligence);
                increaseStats.Add(statTypes.Intelligence);
                increaseStats.Add(statTypes.Intelligence);
                cost = 7;
                preferenceType = BigFoodType.ᚣ;
                path = "proteins/";
                break;
            case "Grubs":
                increaseStats.Add(statTypes.Agility);
                increaseStats.Add(statTypes.Intelligence);
                decreaseStats = statTypes.Durability;
                cost = 3;
                preferenceType = BigFoodType.ᛄ;
                path = "proteins/";
                break;
            #endregion
            #region Rices Section
            case "Bread":
                increaseStats.Add(statTypes.Intelligence);
                increaseStats.Add(statTypes.Durability);
                increaseStats.Add(statTypes.Durability);
                cost = 6;
                preferenceType = BigFoodType.ᛄ;
                path = "rices/";
                break;
            case "Cheese":
                increaseStats.Add(statTypes.Health);
                increaseStats.Add(statTypes.Health);
                decreaseStats = statTypes.Agility;
                cost = 5;
                preferenceType = BigFoodType.ᛄ;
                path = "rices/";
                break;
            case "Potato":
                increaseStats.Add(statTypes.Durability);
                increaseStats.Add(statTypes.Durability);
                decreaseStats = statTypes.Moxie;
                cost = 2;
                preferenceType = BigFoodType.ᛄ;
                path = "rices/";
                break;
            case "Eggs":
                increaseStats.Add(statTypes.Strength);
                increaseStats.Add(statTypes.Strength);
                increaseStats.Add(statTypes.Moxie);
                decreaseStats = statTypes.Health;
                cost = 5;
                preferenceType = BigFoodType.ᚠ;
                path = "rices/";
                break;
            case "Sashimi":
                increaseStats.Add(statTypes.Intelligence);
                increaseStats.Add(statTypes.Intelligence);
                increaseStats.Add(statTypes.Moxie);
                increaseStats.Add(statTypes.Moxie);
                cost = 7;
                preferenceType = BigFoodType.ᚣ;
                path = "rices/";
                break;
            #endregion
            #region Fillers Section
            case "Apple":
                increaseStats.Add(statTypes.Health);
                increaseStats.Add(statTypes.Health);
                cost = 3;
                preferenceType = BigFoodType.ᛄ;
                path = "fillers/";
                break;
            case "DragonFruit":
                increaseStats.Add(statTypes.Moxie);
                increaseStats.Add(statTypes.Intelligence);
                increaseStats.Add(statTypes.Intelligence);
                preferenceType = BigFoodType.ᛄ;
                cost = 5;
                path = "fillers/";
                break;
            case "Onion":
                increaseStats.Add(statTypes.Durability);
                increaseStats.Add(statTypes.Durability);
                increaseStats.Add(statTypes.Durability);
                decreaseStats = statTypes.Moxie;
                cost = 3;
                preferenceType = BigFoodType.ᚣ;
                path = "fillers/";
                break;
            case "PepperRed":
                increaseStats.Add(statTypes.Durability);
                increaseStats.Add(statTypes.Strength);
                preferenceType = BigFoodType.ᚣ;
                cost = 2;
                path = "fillers/";
                break;
            case "Watermelon":
                increaseStats.Add(statTypes.Strength);
                increaseStats.Add(statTypes.Strength);
                cost = 2;
                preferenceType = BigFoodType.ᛄ;
                path = "fillers/";
                break;
            #endregion
            #region Drinks Sections
            case "Cherry":
                increaseStats.Add(statTypes.Health);
                increaseStats.Add(statTypes.Moxie);
                cost = 2;
                preferenceType = BigFoodType.ᚠ;
                path = "drinks/";
                break;
            case "Cookie":
                increaseStats.Add(statTypes.Moxie);
                increaseStats.Add(statTypes.Moxie);
                increaseStats.Add(statTypes.Moxie);
                decreaseStats = statTypes.Health;
                cost = 5;
                preferenceType = BigFoodType.ᚠ;
                path = "drinks/";
                break;
            case "Fish":
                increaseStats.Add(statTypes.Intelligence);
                increaseStats.Add(statTypes.Intelligence);
                increaseStats.Add(statTypes.Moxie);
                cost = 4;
                preferenceType = BigFoodType.ᚣ;
                path = "drinks/";
                break;
            case "Honey":
                increaseStats.Add(statTypes.Durability);
                increaseStats.Add(statTypes.Durability);
                increaseStats.Add(statTypes.Health);
                cost = 5;
                preferenceType = BigFoodType.ᛄ;
                path = "drinks/";
                break;
            case "Jam":
                increaseStats.Add(statTypes.Moxie);
                increaseStats.Add(statTypes.Moxie);
                cost = 2;
                preferenceType = BigFoodType.ᚣ;
                path = "drinks/";
                break;
            case "Waffles":
                increaseStats.Add(statTypes.Strength);
                increaseStats.Add(statTypes.Intelligence);
                decreaseStats = statTypes.Agility;
                cost = 4;
                preferenceType = BigFoodType.ᛄ;
                path = "drinks/";
                break;
            #endregion
            #region Premium Ingredients
            case "Avocado":
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Agility);
                cost = 6;
                preferenceType = BigFoodType.Premium;
                path = "premium/";
                break;
            case "Boar":
                increaseStats.Add(statTypes.Strength);
                increaseStats.Add(statTypes.Strength);
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Luck);
                cost = 8;
                preferenceType = BigFoodType.Premium;
                path = "premium/";
                break;
            case "Brownie":
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Moxie);
                cost = 6;
                preferenceType = BigFoodType.Premium;
                path = "premium/";
                break;
            case "Honeycomb":
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Health);
                increaseStats.Add(statTypes.Health);
                cost = 8;
                preferenceType = BigFoodType.Premium;
                path = "premium/";
                break;
            case "Cantaloupe":
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Intelligence);
                cost = 6;
                preferenceType = BigFoodType.Premium;
                path = "premium/";
                break;
            case "Pumpkin Pie":
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Durability);
                cost = 6;
                preferenceType = BigFoodType.Premium;
                path = "premium/";
                break;
            case "Pineapple":
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Durability);
                increaseStats.Add(statTypes.Agility);
                cost = 8;
                preferenceType = BigFoodType.Premium;
                path = "premium/";
                break;
            case "Roll":
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Luck);
                increaseStats.Add(statTypes.Luck);
                cost = 6;
                preferenceType = BigFoodType.Premium;
                path = "premium/";
                break;
                #endregion
        }
        foodImg = Resources.Load<Sprite>("ingredientImages/" + path + foodName);
        BuildFoodData();
    }
    public void BuildFoodData()
    {
        if (GetComponentInChildren<Text>() != null)
        {
            Text numCount = GetComponentInChildren<Text>();
            numCount.text = base.foodCount.ToString();
            inIm.sprite = Resources.Load<Sprite>("ingredientImages/" + path + foodName);
        }
    }
    public void FedAdventurer()
    {
        FindObjectOfType<GameManager>().ClearAdData();
        if (adven.AdventurerGold >= cost)
        {
            adven.AdventurerGold -= cost;
            int tip = 0; ;
            if (adven.canEat.Contains((Adventurer.foodType)preferenceType))
            {
                if (adven.Personality == Adventurer.personality.Generous)
                    tip = Random.Range(adven.timesVisitedShop + 11, (cost + Random.Range(1, 31)) * (adven.timesVisitedShop + 1));
                else
                    tip = Random.Range(adven.timesVisitedShop + 1, (cost + Random.Range(0, 21)) * (adven.timesVisitedShop + 1));
                FindObjectOfType<GameManager>().playerMoney += tip;
            }
            else if (adven.Personality == Adventurer.personality.Generous)
            {
                tip = Random.Range(adven.timesVisitedShop + 1, (cost + Random.Range(0, 21)) * (adven.timesVisitedShop + 1));
                FindObjectOfType<GameManager>().playerMoney += tip;
            }
            if (preferenceType == BigFoodType.Premium)
                FindObjectOfType<GameManager>().playerMoney += cost * 2;
            else
                FindObjectOfType<GameManager>().playerMoney += cost;
            foreach (var item in increaseStats)
            {
                switch (item)
                {
                    case statTypes.Strength:
                        adven.Strength++;
                        break;
                    case statTypes.Agility:
                        adven.Agility++;
                        break;
                    case statTypes.Intelligence:
                        adven.Intelligence++;
                        break;
                    case statTypes.Durability:
                        adven.Durability++;
                        break;
                    case statTypes.Moxie:
                        adven.Moxie++;
                        break;
                    case statTypes.Health:
                        adven.Health++;
                        break;
                    case statTypes.Luck:
                        adven.Luck++;
                        break;
                }
            }
            switch (decreaseStats)
            {
                case statTypes.Strength:
                    adven.Strength--;
                    break;
                case statTypes.Agility:
                    adven.Agility--;
                    break;
                case statTypes.Intelligence:
                    adven.Intelligence--;
                    break;
                case statTypes.Durability:
                    adven.Durability--;
                    break;
                case statTypes.Moxie:
                    adven.Moxie--;
                    break;
                case statTypes.Health:
                    adven.Health--;
                    break;
                case statTypes.Luck:
                    adven.Luck--;
                    break;
            }
            foodCount--;
            WriteOutData();
        }
        FindObjectOfType<FoodManager>().GenerateIngredients();
    }
    public void WriteOutData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Data/Foods/" + foodName + ".fmid");

        TestFood newFood = new TestFood();
        newFood.itemCount = base.foodCount;
        newFood.itemName = base.foodName;
        newFood.Foodie = (TestFood.foodType)System.Enum.Parse(typeof(TestFood.foodType), base.FoodType.ToString());
        bf.Serialize(file, newFood);
        file.Close();
    }
    void Drag(PointerEventData data)
    {
        transform.position = data.position;
        transform.SetParent(FindObjectOfType<GameManager>().foodDragHandler);
        Vector2 freshPos = new Vector2(transform.position.x, transform.position.y);
        if (Vector2.Distance(startPos, freshPos) > 1.5f)
            isDrag = true;
    }
    public void ClickDown(BaseEventData data)
    {
        startPos = this.gameObject.transform.position;
        initialTransform = this.transform.parent;
        FindObjectOfType<GameManager>().BuildFoodData(this);
    }
    public void ClickUp(BaseEventData data)
    {
        if (isDrag)
        {
            if (!isFeeding)
                FindObjectOfType<FoodManager>().GenerateIngredients();
            else
                FedAdventurer();
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isFeeding = true;
            adven = other.GetComponent<Adventurer>();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        isFeeding = false;
        adven = null;
    }
    public int typeOf(statTypes inType)
    {
        int x = 0;
        foreach (var item in increaseStats)
        {
            if (item == inType)
                x++;
        }
        if (inType == statTypes.Health)
            x = x * 5;
        return x;
    }

    public enum statTypes
    {
        Strength,
        Agility,
        Intelligence,
        Durability,
        Moxie,
        Health,
        Luck
    };
    public enum BigFoodType
    {
        ᚠ,
        ᚣ,
        ᛄ,
        Premium
    };
}