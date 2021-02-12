using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles core of player dance interactions with NPCs
/// 
/// Provided with framework, no modification required
/// </summary>
public class Player : MonoBehaviour
{
    private NPC currentOpponent;
    private Stats myStats;
    private Stats currentNPCStats;
    private Rigidbody body;
    private PlayerController controller;
    private Animator anim;
    private BattleHandler battleHandler;
    private UIManager uiManager;

    // Start is called before the first frame update
    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        body = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        battleHandler = FindObjectOfType<BattleHandler>();
        myStats = GetComponent<Stats>();
        uiManager.EnableNPCStatsUI(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentOpponent != null)
        {
            //currently colliding with an NPC
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(DanceOff()); 
            }
        }
    }

    IEnumerator DanceOff()
    {
        battleHandler.BeginBattlePhase(myStats, currentOpponent.myStats);
        controller.enabled = false;
        currentOpponent.transform.LookAt(transform.position);
        body.velocity = Vector3.zero;
        currentOpponent.uiCanvas.SetActive(false);     
        yield return new WaitForSeconds(3f);
        battleHandler.Battle(myStats, currentOpponent.myStats);
        currentOpponent.uiCanvas.SetActive(true);   
        controller.enabled = true;
        currentOpponent.transform.LookAt(transform.position + Vector3.forward);
        // recalculate players % winnage
        myStats.SetPercentageValue(battleHandler.SimulateBattle(myStats, currentNPCStats));
        currentNPCStats.SetPercentageValue(battleHandler.SimulateBattle(currentNPCStats, myStats));
    }

    //Check for colliding with NPC, can then interact, 
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<NPC>())
        {
            currentOpponent = other.GetComponent<NPC>();
            currentNPCStats = other.GetComponent<Stats>();
            currentNPCStats.UpdateStatsUI();
            currentOpponent.uiCanvas.SetActive(true);
            uiManager.EnableNPCStatsUI(true);
            // calculate players % winnage
            myStats.SetPercentageValue(battleHandler.SimulateBattle(myStats, currentNPCStats));
            currentNPCStats.SetPercentageValue(battleHandler.SimulateBattle(currentNPCStats, myStats));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<NPC>())
        {
            if(other.GetComponent<NPC>() == currentOpponent)
            {          
                currentOpponent.uiCanvas.SetActive(false);
                //calculate Reset % winnage
                myStats.SetPercentageValue(0);
                currentNPCStats.SetPercentageValue(0);
                currentNPCStats.UpdateStatsUI();
                uiManager.EnableNPCStatsUI(false);
                currentNPCStats = null;
                currentOpponent = null;
            }
        }
    }
}
