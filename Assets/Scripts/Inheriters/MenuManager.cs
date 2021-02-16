using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : FoodManager
{
    public Image fauxFillBar;
    float fillDelay = 0;
    private void Awake()
    {
        fillDelay = Random.Range(4, 11);
        StartCoroutine(LoadingGame(Random.Range(4, 11)));
        if (!Directory.Exists(Application.persistentDataPath + "/Data/Raw") ||
            !Directory.Exists(Application.persistentDataPath + "/Data/Characters") ||
            !Directory.Exists(Application.persistentDataPath + "/Data/Foods") ||
            !Directory.Exists(Application.persistentDataPath + "/Data/Quests"))
        {
            Debug.Log("ERROR");
            Directory.CreateDirectory(Application.persistentDataPath + "/Data/Raw");
            Directory.CreateDirectory(Application.persistentDataPath + "/Data/Characters");
            Directory.CreateDirectory(Application.persistentDataPath + "/Data/Foods");
            Directory.CreateDirectory(Application.persistentDataPath + "/Data/Quests");
            GenerateIngredients();
        }
    }
    private void Update()
    {
        fauxFillBar.fillAmount += (float)1 / fillDelay * Time.deltaTime;
    }
    IEnumerator LoadingGame(float inRange)
    {
        yield return new WaitForSeconds(inRange += 1);
        SceneManager.LoadScene("Shopfront");
    }
}