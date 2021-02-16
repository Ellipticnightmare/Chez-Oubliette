using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "floor", menuName = "scriptables/dungeon/floorBuilder")]
public class Floor : ScriptableObject
{
    [SerializeField] public Enemy[] floorData;
}