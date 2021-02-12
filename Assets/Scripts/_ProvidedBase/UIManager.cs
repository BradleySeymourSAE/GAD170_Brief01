using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Splashes simple ui elements when certain game events are triggered
/// 
/// Provided with framework, no modification required
/// </summary>
public class UIManager : MonoBehaviour
{
    public GameObject npcLevelUI;
    public GameObject playerXPUI;

    public StatsUI npcStatsUI; // only show/update when player is in range.
    
    public void EnableNPCStatsUI(bool enabled)
    {
        npcStatsUI.gameObject.SetActive(enabled);
    }


    public void ShowLevelUI()
    {
        StartCoroutine(NPCLevelUI());
    }

    public void ShowPlayerXPUI(int xp)
    {
        StartCoroutine(PlayerXPUI(xp));
    }

    IEnumerator NPCLevelUI()
    {
        npcLevelUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        npcLevelUI.SetActive(false);
    }

    IEnumerator PlayerXPUI(int xp)
    {
        int xpDisplay = 0;
        playerXPUI.SetActive(true);
        while (xpDisplay < xp)
        {
            xpDisplay++;
            playerXPUI.GetComponentInChildren<UnityEngine.UI.Text>().text = "+" + xpDisplay.ToString() + "XP";
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        playerXPUI.SetActive(false);
    }
}
