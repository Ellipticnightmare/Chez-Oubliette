using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expedition : MonoBehaviour
{
    public Text displayCost;
    int cost;
    float timeRemaining = 300;
    float minutes, seconds;
    GameManager manage;
    bool isOnExpedition;
    // Update is called once per frame
    void Update()
    {
        if (manage == null)
            manage = FindObjectOfType<GameManager>();
        if(!manage.isDayTime)
            cost = (manage.curDay + manage.unlockedDungeons.Count + (manage.curShop.shopTier * 3) + manage.expeditionsSent);
        else
            cost = (manage.curDay + manage.unlockedDungeons.Count + (manage.curShop.shopTier * 6) + manage.expeditionsSent);
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);

        if (!isOnExpedition)
        {
            displayCost.text = cost + "";

            if (manage.playerMoney >= cost)
                displayCost.color = Color.cyan;
            else
                displayCost.color = Color.red;
        }
        else
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
                ReturnExpedition();
            displayCost.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            displayCost.color = Color.red;
        }
    }
    public void SendExpedition()
    {
        if (isOnExpedition)
        {
            if(manage.premiumMoney >= 1)
            {
                manage.premiumMoney--;
                timeRemaining = 0;
            }
        }
        if (manage.playerMoney >= cost)
        {
            isOnExpedition = true;
            manage.playerMoney -= cost;
            manage.expeditionsSent++;
        }
    }
    public void ReturnExpedition()
    {
        isOnExpedition = false;
        timeRemaining = 300;
        FindObjectOfType<GameManager>().CheckExpedition();
    }
}