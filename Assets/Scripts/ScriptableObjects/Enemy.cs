using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "enemy", menuName = "scriptables/dungeon/Enemies")]
public class Enemy : ScriptableObject
{
    [Range(1, 36)]
    [SerializeField] public int CR = 1;
}