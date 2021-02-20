using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO:
///     - SimulateBattle needs to calculate a normalised (decimal) value to display the % chance of winning a fight.
///     - Battle needs to handle the logic for who wins a fight and telling the winner they get some XP for their trouble.
/// </summary>
public class BattleHandler:MonoBehaviour
{
    public SFXHandler sfxHandler; // reference to our sfx Handler to play sound effects.
    public enum DanceStates { 
        Draw = 0, 
        NPC = -1, 
        Player = 1 
    }

    public int lossRatio = 35;

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
            Debug.LogWarning(" Simulate battle called; but Player or NPC battle points is 0, most likely the logic has not be setup for this yet");


        float _playerPoints = myPoints;
        float _npcPoints = opponentPoints;
        double winningPercentage;


        if (_playerPoints > _npcPoints)
            winningPercentage = (_npcPoints / _playerPoints) * 100f;
        else
            winningPercentage = (_playerPoints / _npcPoints) * 100f;


        // Debug.Log("Chance Of Winning: " + (float)Math.Round(_percentage, 2));
        return (float)Math.Round(winningPercentage, 2);
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
        int winner = 0;

        if (playerPowerLevel <= 0 || npcPowerLevel <= 0)
            Debug.LogWarning("Player or NPC battle points is 0 - Has the logic been setup yet?");


        if (playerPowerLevel == npcPowerLevel)
            winner = (int)DanceStates.Draw;
        else if (playerPowerLevel > npcPowerLevel)
            winner = (int)DanceStates.Player;
        else
            if (playerPowerLevel < npcPowerLevel)
            winner = (int)DanceStates.NPC;

        // Check to see who wins, if they win we want to give them a base xp value of baseExperience

        int baseXP = player.experienceBase;
        int loserXP = baseXP - lossRatio;

        int s_currentPlayerLevel = player.level;
        int s_currentNPCLevel = npc.level;

        // TODO: We could add the NPC's Current Level, and set the xp gained based off of the 
        // level. Vice Versa.

        // This could prevent OP Players and balance the game a bit 
        switch (winner)
        {
            case 0:
                // If there was a draw
                // We can assign the base xp to both, and add the other players level 
                
                int s_playerXP = baseXP + (s_currentNPCLevel * 2);
                int s_npcXP = baseXP + (s_currentPlayerLevel * 2);


                player.AddXP(s_playerXP);
                npc.AddXP(s_npcXP);
                break;
            case 1:

                // If the player wins 
                baseXP = baseXP + 15 + (s_currentNPCLevel * 2);

                player.AddXP(baseXP);
                npc.AddXP(loserXP);
                break;
            case -1:
                // If the NPC Wins
                baseXP = baseXP + 15 + (s_currentPlayerLevel * 2);

                player.AddXP(loserXP);
                npc.AddXP(baseXP);
                break;
            default:
                Debug.LogWarning("Error in BattleHandler.." + winner);
                break;
        }

        // We need to set the winner to a float type before parsing it to 
        // SetWinningEffects() as it takes the param as a float - NOT Integer.

        float _winnerResult = winner;

        SetWinningEffects(player, npc, _winnerResult);
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
