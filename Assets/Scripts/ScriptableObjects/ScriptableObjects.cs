using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "database", menuName = "databaseScriptables")]
public class ScriptableObjects : ScriptableObject
{
    [SerializeField] public string doIHoldAnything = "Nope!";
    [SerializeField] public NameGenerator[] nameObjs;
    [SerializeField] public Shop[] shopObjs;
    [SerializeField] public Dungeon[] dungeonObjs;
    [SerializeField] public Floor[] floorObjs;
    [SerializeField] public Enemy[] enemyObjs;
}