using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "dungeon", menuName = "scriptables/dungeon/DungeonBuilder")]
public class Dungeon : ScriptableObject
{
    public string dungeonName;
    [Range(0, 6)]
    public int starCount;
    [SerializeField] public Floor[] floors;
    public List<statType> statModifiers = new List<statType>();
    public int penaltyCheck, statPenalty;

    public enum statType
    {
        Strength,
        Agility,
        Intelligence,
        Durability,
        Moxie
    };
}