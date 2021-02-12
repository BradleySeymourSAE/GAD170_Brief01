using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Agent to dance against.
/// 
/// Provided with framework, no modification required
/// </summary>
public class NPC : MonoBehaviour
{
    [HideInInspector]
    public Stats myStats;
    public GameObject uiCanvas;

    private void Awake()
    {
        myStats = GetComponent<Stats>();

        if (uiCanvas == null)
        {
            uiCanvas = transform.GetChild(0).gameObject;
          
        }
        uiCanvas.SetActive(false);
    }

    void LevelUp(int level)
    {
        Debug.Log("NPC Level Up!");
        myStats.level = level;
        myStats.DistributePhysicalStatsOnLevelUp(5);
    }
}