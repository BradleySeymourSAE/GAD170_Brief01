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
    public int lossRatio = 35;
    public int loserExperience;
    public int baseExperienceGained;
    public int playerLevel;
    public int npcLevel;

    /// <summary>
    /// Returns a float of the percentage chance to win the fight based on your characters current stats.
    /// </summary>
    /// <param name="CharacterStats"></param>
    /// <param name="OpponentStats"></param>
    /// <returns></returns>
    public float SimulateBattle(Stats CharacterStats, Stats OpponentStats)
    {
        int myPoints = CharacterStats.ReturnDancePowerLevel(); // our current powerlevel
        int opponentPoints = OpponentStats.ReturnDancePowerLevel(); // our opponents current power level
        double winningPercentage;

        // If my points or the opponents points are less than or equal to 0
        if (myPoints <= 0 || opponentPoints <= 0)
        { 
            // Log a warning to the console.
            Debug.LogWarning("Simulate battle called but player or the NPC's current battle points is 0");
        }


        // If my current stat points are greater than the opponents stat points 
        if (myPoints > opponentPoints)
		{
            // We calculate the winning percentage eg (100 / 200) * 100f; Return -> 50.00
            winningPercentage = (float)opponentPoints / myPoints * 100f;
		}
        else
		{
            winningPercentage = (float)myPoints / opponentPoints * 100f;
		}


        // Debugging 
        Debug.Log("[SimulateBattle]: " + "Calculate winning percentage: " + winningPercentage);
        Debug.Log("[SimulateBattle]: " + "Chance of winning: " + (float)Math.Round(winningPercentage, 2));

        // Return the float to 2 decimal places.
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
        baseExperienceGained = player.experienceBase; // base experience value to be gained for defeating opponent.
        loserExperience = baseExperienceGained - lossRatio; // loser experience 
        playerLevel = player.level; // the current players level 
        npcLevel = npc.level; // the current npc level 

        // Local Variables 
        int playerPowerLevel = player.ReturnDancePowerLevel(); // player powerlevel
        int npcPowerLevel = npc.ReturnDancePowerLevel(); // npc powerlevel
        int winner = 0;

        if (playerPowerLevel <= 0 || npcPowerLevel <= 0)
        { 
            Debug.LogWarning("Something went wrong in BattleHandler.Battle - Player or NPC battle points is equal to 0");
        }

        // Check if the players power level is greater than the npc's power level 
        if (playerPowerLevel > npcPowerLevel)
		{
            // PLAYER WINS 
            Debug.Log("[Battle]: " + "PLAYER HAS WON THE BATTLE!");
            winner = 1;
		}
        // If players power level is less than the npc's power level 
        else if (playerPowerLevel < npcPowerLevel)
		{
            // NPC WINS 
            Debug.Log("[Battle]: " + "NPC HAS WON THE BATTLE!");
            winner = -1;
		}
        else
		{
            // If the players power level is the same as the npc's power level 
            if (playerPowerLevel == npcPowerLevel)
			{
                // Its a draw!
                Debug.Log("[Battle]: " + "PLAYER & NPC DRAWED!");
                winner = 0;
			}
		}


        // <Bradley>: We could add the NPC's Current Level, and set the xp gained based off of the 
        // level. Vice Versa.

        // This could prevent OP Players and balance the game a bit 

        if (winner == 0)
		{
           baseExperienceGained = baseExperienceGained + (npcLevel + 2);

            Debug.Log("Adding Experience " + baseExperienceGained + " to PLAYER AND NPC for drawing in the fight!");
            player.AddXP(baseExperienceGained);
           npc.AddXP(baseExperienceGained);
		}
        else if (winner == 1)
		{
            // Player won the battle.

            // Calculate base experience + (NPC Current Level * 2);
            baseExperienceGained = baseExperienceGained + 15 + (npcLevel * 2);

            Debug.Log("Adding Experience " + baseExperienceGained + " to PLAYER for winning the battle! Adding Experience " + loserExperience + " to the NPC for losing the battle!");
            // Add experience to player for winning 
            player.AddXP(baseExperienceGained);
            // Add loser experience to npc 
            npc.AddXP(loserExperience);
		}
        else
		{
            if (winner == -1)
			{
                // NPC Won the battle 
                baseExperienceGained = baseExperienceGained + (playerLevel);

                Debug.Log("Adding Experience " + baseExperienceGained + " to the NPC for winning the battle! Adding Experience " + loserExperience + " to the PLAYER for losing the battle!");
                player.AddXP(loserExperience);
                npc.AddXP(baseExperienceGained);
            }
            // If there should be an else here, something clearly went wrong lol.
		}

      
        Debug.Log("[Battle]: " + "Battle has completed and we have a winner! Calling SetWinningEffects...");
        // Update Winning Effects UI to display changes accordingly
		// This function takes the variable 'battleResult' as a float
        // Casted the winner result from a integer.
        SetWinningEffects(player, npc, (float)winner);
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
