using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class FoodManager : MonoBehaviour
{
    public List<Ingredient> availableIngredients = new List<Ingredient>();
    public Transform ingredientHolder;
    public GameObject freshIngredient;
    private void OnEnable()
    {
        GenerateIngredients();
    }
    public void GenerateIngredients()
    {
        foreach (var item in FindObjectsOfType<Ingredient>())
        {
            Destroy(item.gameObject);
        }
        Texture2D[] proteinList = Resources.LoadAll<Texture2D>("ingredientImages/proteins");
        Sprite[] fillersList = Resources.LoadAll<Sprite>("ingredientImages/fillers");
        Sprite[] ricesList = Resources.LoadAll<Sprite>("ingredientImages/rices");
        Sprite[] drinksList = Resources.LoadAll<Sprite>("ingredientImages/drinks");
        Texture2D[] premiumList = Resources.LoadAll<Texture2D>("ingredientImages/premium");
        foreach (var item in proteinList)
        {
            string newAddress = Application.persistentDataPath + "/Data/Foods/" + item.name + ".fmid";
            if (File.Exists(newAddress))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(newAddress, FileMode.Open);
                TestFood check = (TestFood)bf.Deserialize(file);
                file.Close();
                if (check.itemCount > 0)
                {
                    GameObject newIngredient = Instantiate(freshIngredient, ingredientHolder);
                    newIngredient.GetComponent<Ingredient>().GenerateVariables(check);
                    availableIngredients.Add(newIngredient.GetComponent<Ingredient>());
                }
                else
                {
                    Debug.Log("No more food of type: " + item.name);
                }
            }
            else
                InitialWrite(newAddress, item.name, "proteins/", 5);
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
                if (check.itemCount > 0)
                {
                    GameObject newIngredient = Instantiate(freshIngredient, ingredientHolder);
                    newIngredient.GetComponent<Ingredient>().GenerateVariables(check);
                    availableIngredients.Add(newIngredient.GetComponent<Ingredient>());
                }
            }
            else
                InitialWrite(newAddress, item.name, "fillers/", 5);
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
                if (check.itemCount > 0)
                {
                    GameObject newIngredient = Instantiate(freshIngredient, ingredientHolder);
                    newIngredient.GetComponent<Ingredient>().GenerateVariables(check);
                    availableIngredients.Add(newIngredient.GetComponent<Ingredient>());
                }
            }
            else
                InitialWrite(newAddress, item.name, "rices/", 5);
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
                if (check.itemCount > 0)
                {
                    GameObject newIngredient = Instantiate(freshIngredient, ingredientHolder);
                    newIngredient.GetComponent<Ingredient>().GenerateVariables(check);
                    availableIngredients.Add(newIngredient.GetComponent<Ingredient>());
                }
            }
            else
                InitialWrite(newAddress, item.name, "drinks/", 5);
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
                if (check.itemCount > 0)
                {
                    GameObject newIngredient = Instantiate(freshIngredient, ingredientHolder);
                    newIngredient.GetComponent<Ingredient>().GenerateVariables(check);
                    availableIngredients.Add(newIngredient.GetComponent<Ingredient>());
                }
            }
            else
                InitialWrite(newAddress, item.name, "premium/", 1);
        }
    }
    public void InitialWrite(string Address, string newItemName, string foodType, int FoodCount)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Address);

        TestFood newFood = new TestFood();
        newFood.itemName = newItemName;
        newFood.path = foodType;
        if (foodType == "premium/")
            newFood.Foodie = TestFood.foodType.premium;
        else
            newFood.Foodie = TestFood.foodType.ᚠ;
        newFood.itemCount = FoodCount;
        bf.Serialize(file, newFood);
        file.Close();
    }
    public void BuyIngredient(Ingredient inName)
    {
        bool doIHaveIt = true;
        bool didIHaveItAfterAll = false;
        foreach (var item in availableIngredients)
        {
            if (item == inName)
            {
                didIHaveItAfterAll = true;
                item.foodCount += 5;
                item.WriteOutData();
            }
            else
                doIHaveIt = false;
        }
        if (!doIHaveIt && !didIHaveItAfterAll)
        {
            string newAddress = Application.persistentDataPath + "/Data/Foods/" + inName.foodName + ".fmid";
            Debug.Log("Buying new Ingredient at: " + newAddress);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(newAddress, FileMode.Open);
            TestFood check = (TestFood)bf.Deserialize(file);
            file.Close();
            check.itemCount += 5;

            file = File.Create(newAddress);
            bf.Serialize(file, check);
            file.Close();
        }
    }
}
[System.Serializable]
public class TestFood
{
    public string itemName;
    public string path;
    public int itemCount;
    public foodType Foodie;
    public enum foodType
    {
        ᚠ,
        ᚣ,
        ᛄ,
        premium
    };
}