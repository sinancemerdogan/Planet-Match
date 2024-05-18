using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemsSO : ScriptableObject
{
    public string itemName;
    public Item prefab;
    public GameObject explosionEffect;
}
