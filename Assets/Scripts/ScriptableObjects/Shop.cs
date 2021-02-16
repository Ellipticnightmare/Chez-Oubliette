using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "shop", menuName = "scriptables/shop/ShopBuilder")]
public class Shop : ScriptableObject
{
    [Range(1, 6)]
    [SerializeField] public int customerLimit = 3;
    [Range(1, 5)]
    [SerializeField] public int shopTier = 1;
    [SerializeField] public Sprite shopBackground, shopCounter;
    [SerializeField] public int shopCost, Rent;
}