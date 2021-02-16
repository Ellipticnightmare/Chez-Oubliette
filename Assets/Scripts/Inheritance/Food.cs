using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [HideInInspector]
    public foodType FoodType;
    [HideInInspector]
    public string foodName;
    [HideInInspector]
    public Sprite foodImg;
    [HideInInspector]
    public int foodCount;
    public enum foodType
    {
        ᚠ,
        ᚣ,
        ᛄ,
        premium
    };
}