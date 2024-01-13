using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapContent", menuName = "ScriptableObjects/MapContentScriptObject", order = 1)]
public class MapScriptObj : ScriptableObject
{
    public List<string> mapVal;
    //public int[,] mapVal = new int[10, 10];
    //public List<int[]> mapVal;
}
