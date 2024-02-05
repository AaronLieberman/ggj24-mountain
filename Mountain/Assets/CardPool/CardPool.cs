using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CardPool", menuName = "GGJ24/Card Pool", order = 1)]
public class CardPool : ScriptableObject
{
	public string poolName = "Unnamed Card Pool";
	public List<GameObject> tilePlacementObjects;
}