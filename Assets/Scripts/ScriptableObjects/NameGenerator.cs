using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "nameGenerator", menuName = "scriptables/shop/nameComponentDatabase")]
public class NameGenerator : ScriptableObject
{
    [SerializeField] public string[] firstNameComponents, lastNameComponents;
}