using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This handles all the battle logic of who is determined to be the winner of a fight between
/// two characters.
/// 
/// TODO:
///     - SimulateBattle needs to calculate a normalised (decimal) value to display the % chance of winning a fight.
///     - Battle needs to handle the logic for who wins a fight and telling the winner they get some XP for their trouble.
/// </summary>
public class BattleHandler:MonoBehaviour
{
    public SFXHandler sfxHandler; // reference to our sfx Handler to play sound effects.
    
    /// <summary>
    /// Returns a float of the percentage chance to win the fight based on your characters current stats.
    /// </summary>
    /// <param name="MyStats"></param>
    /// <param name="Opponent"></param>
    /// <returns></returns>
    public float SimulateBattle(Stats MyStats, Stats Opponent)
    {
        int myPoints = MyStats.ReturnDancePowerLevel(); // our current powerlevel
        int opponentPoints = Opponent.ReturnDancePowerLevel(); // our opponents current power level

        if (myPoints <= 0 || opponentPoints <= 0)
        {
            Debug.LogWarning(" Simulate battle called; but Player or NPC battle points is 0, most likely the logic has not be setup for this yet");
        }

        // we probably want to compare our powerlevels...hope they aren't over 9000.
        // we need to return a normalised (decimal) value....how much do you remember about percentages?
        // don't forget that we are returning a float...but diving 2 ints...what happens?

        Debug.LogWarning("Simulate battle called, but the logic hasn't been set up yet, so defaulting to 0");
        return 0;
    }


    /// <summary>
    /// Is called when the player presses space bar.
    /// This function should take a player and npc 
    /// then determine who has won and give some xp and show some sweet winning effects.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="npc"></param>
    public void Battle(Stats player, Stats npc)
    {     
        int playerPowerLevel = player.ReturnDancePowerLevel(); // player powerlevel
        int npcPowerLevel = npc.ReturnDancePowerLevel(); // npc powerlevel
        
        if(playerPowerLevel <= 0 || npcPowerLevel <=0)
        {
            Debug.LogWarning("Player or NPC battle points is 0, most likely the logic has not be setup for this yet");
        }

        // we probably want to compare our powerlevels...hope they aren't over 9000.
        // if someone wins...we probs wanna give some xp...but how much?
        // also if we want to show some winning effects 0 = draw, -1 = npc, 1 = player

        Debug.Log("Draw yo, need to git gud");
        SetWinningEffects(player, npc, 0); // setting the winning effects to a draw.
        player.AddXP(0); // player gets no xp
        npc.AddXP(0); // npc gets no xp either
    }

    #region No Modifications Required Section
    /// <summary>
    /// Is called at the begining of a fight, and sets the two characters to their dancing states.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="npc"></param>
    public void BeginBattlePhase(Stats player, Stats npc)
    {
        player.animController.Dance();
        npc.animController.Dance();
    }

    /// <summary>
    /// Takes in the player and npc stat scripts called at the end of the fight and sets the dancers states to either win, or lose state.
    /// 1 = player wins
    /// 0 = draw
    /// -1 = npc has won
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="NPC"></param>
    /// <param name="outcome"></param>
    public void SetWinningEffects(Stats Player, Stats NPC, float BattleResult)
    {
        Player.animController.BattleResult(BattleResult);
        // give the npc the opposite of what ever the result is.
        NPC.animController.BattleResult(BattleResult * -1);
        // Play the appropriate sfx depending who won.
        sfxHandler.BattleResult(BattleResult);
    }
    #endregion
}
