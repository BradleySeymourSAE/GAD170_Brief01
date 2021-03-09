using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;



/// <summary>
/// This class handles all the data related to a characters stats.
/// 
/// TODO:
///  Generate some Physical stats for our character.
///  Calculate our Dancing stats based on our physical stats.
///  SetPercentageValue based on the decimal value coming in turn this into a %.
///  ReturnDancePowerLevel return a power level based on our dancing stats.
///  AddXP based on the xp coming in, add some xp.
///  LevelUp increase our level as well as increase our threshold for levelling up, finally increase our physical stats.
///  DistributePhysicalStatsOnLevelUp increase each of our physical stats by a value, and recalculate our dancing stats.
/// 
/// </summary>
public class Stats : MonoBehaviour
{
    private const string styled = "---";

   
    [Header(styled + " Level Stats " + styled)]
    public int level;
    public int currentXp;
    public int xpThreshold = 10;
    public int maxLevel = 99;
    public int experienceBase = 50;
    public bool hasReachedMilestone = false;
    public int previousThreshold;
    float levelProgress;
    float experienceStep;

    // Scaling Factors & Base Points
    [Header(styled + " Scaling Factors " + styled)]
    public int skillPointScaling = 1;
    public int luckScalingFactor = 1;
    public float levelScalingFactor = 1.2f;
    public int baseSkillPoints = 5;

   

    // Dance Stats (Calculated from Character Attrs)
    [Header(styled + " Dancing Stats " + styled)]
    public int style;
    public int luck; 
    public int rhythm;

    // Character Attributes 
    [Header(styled + " Basic Stats " + styled)]
    public int agility = 2;
    public int intelligence = 1;
    public int strength = 2;

    // Modifiers 
    [Header(styled + " Modifiers " + styled)]
    public float agilityMultiplier = 0.5f;
    public float strengthMultiplier = 1f;
    public float intelligenceMultiplier = 2f;

    //  Number of attrs and totalPowerLevel would be private scoped
    public int numberOfAttributes;
    public float percentageChanceToWin;
    public int totalPowerLevelPoints;

    #region character references, no mods required
    [HideInInspector]
    public AnimationController animController; // reference to our animation controller on our character
    [HideInInspector]
    public SFXHandler sfxHandler; // reference to our sfx Handler in our scene
    [HideInInspector]
    public ParticleHandler particleHandler; // a refernce to our particle system that is played when we level up.  
    public UIManager uIManager; // a reference to the UI Manager in our scene.
    public StatsUI statsUI; // a referecence to our stats ui for this character.
    #endregion

    /// <summary>
    /// Called on the very first frame of the game
    /// </summary>
    private void Start()
    {
        // If the characters level is less than or equal to zero, we want the characters level to start at 1.
        if (level <= 0)
        { 
            level = 1;
        }

        // Removed CalculateDanceStats from the start function as this will run both functions once started which 
        // will not return the desired result.

        //sets up the references to other scripts we need for functionality.
        SetUpReferences();

        // Generate some physical stats for our character 
        GeneratePhysicalStatsStats(); 
    }

	/// <summary>
	/// This function should set our starting stats of Agility, Strength and Intelligence
	/// to some default random values and 
	/// </summary>
	public void GeneratePhysicalStatsStats()
    {
       agility = Random.Range(1, 4); // random agility value between 1 minimum inclusive and 4 max exlusive.
       strength = Random.Range(1, 4);  // random strength value between 1 minimum inclusive and 4 max exlusive.
       intelligence = Random.Range(1, 3); // random intelligence value between 1 minimum inclusive and 3 max exlusive.
       Debug.Log("[GeneratePhysicalStats]: " + " Agility: " + agility + " Strength: " + strength + " Intelligence: " + intelligence);

        // Calculate the dancing stats for our character based on the random range calculated above 
        CalculateDancingStats(agility, strength, intelligence);

        // update our current UI for our character
        UpdateStatsUI(); 
    }

    /// <summary>
    ///     This function should set our Style, Rhythm & Luck to values based on our currrent characters Agility, Strength and Intelligence Values.
    /// <param name="_agility">The characters agility level</param>
    /// <param name="_strength">The characters strength level</param>
    /// <param name="_intelligence">The characters intelligence level</param>
    /// <returns></returns>
    /// </summary>
    public void CalculateDancingStats(int _agility, int _strength, int _intelligence)
    {
        Debug.Log("[CalculateDancingStats]: " + "Character Stats for Agility: " + _agility + " Strength: " + _strength + " Intelligence: " + _intelligence);

        // Style is calculated based on the characters 'Agility' * agilityMultiplier (0.5f)
        // Rhythm is calculated based on the characters 'Strength' * strengthMultiplier (1f)
        // Luck is calculated based on the characters 'Intelligence' Stat * intelligenceMultiplier (2f)


        // We want to multiply agility value by its multiplier and cast the value from a float to an integer. 
        style = (int)(_agility * agilityMultiplier);
        rhythm = (int)(_strength * strengthMultiplier);
        luck = (int)(_intelligence * intelligenceMultiplier);

        // Extra Debugging Methods
        // Debug.Log("[CalculateDancingStats]: " + " Style Type: " + style.GetType() + " Rhythm Type: " + rhythm.GetType() + " Luck Type: " + luck.GetType());

        Debug.Log("[CalculateDancingStats]: " + "Style: " + style + " Rhythm: " + rhythm + " Luck: " + luck);

        // Store global var for total power level points.
        totalPowerLevelPoints = agility + strength + intelligence + style + rhythm;

        // Update the Character Stats UI to reflect the changes made
        UpdateStatsUI(); 
    }


    /// <summary>
    /// This is takes in a normalised value i.e. 0.0f - 1.0f, and is used to display our percentage % chance to win.
    /// </summary>
    /// <param name="normalisedValue"></param>
    public void SetPercentageValue(float normalisedValue)
    {
        // Set percentage to be a normalised value.
        // See BattleHandler.cs Line 49
        Debug.Log("[SetPercentageValue]: " + " Setting percentage value for " + normalisedValue);

        // Round float value to the nearest whole decimal number
        normalisedValue = Mathf.Round(normalisedValue);

        // Sets the percentage chance of winning for a characters Stats
        percentageChanceToWin = normalisedValue;

        Debug.Log("[SetPercentageValue]: " + " Percentage: " + percentageChanceToWin + "%");
        
        // Update Stats UI To reflect the changes made
        UpdateStatsUI();
    }

    /// <summary>
    ///     Returns a Characters Dance Power Level 
    /// </summary>
    /// <returns></returns>
    public int ReturnDancePowerLevel()
    {
        // Create a characters dance power level based off the characters overall stats & level.
      
        // Luckiness is the modifier in this case that balances the game
        // Calculated using a range of 1 and luck + a scaling factor 
        int luckiness = Random.Range(1, luck + luckScalingFactor);
      
        numberOfAttributes = 5; // Total number of character attributes (Which is 5 if you dont count luck)
        totalPowerLevelPoints = agility + strength + intelligence + rhythm + style;    // Example Base Points: 4 + 5 + 8 + 5 + 2 + 16


        // Debugging - Debug.Log("Power Level: " + powerLevel);
          // int powerLevel = level + (totalPowerLevelPoints + numberOfAttributes) * luckiness;
        
        // Debug log power level to the console.
        Debug.Log("[ReturnDancePowerLevel]: " + " Power Level: " + level + (totalPowerLevelPoints + numberOfAttributes) * luckiness);

        // Removed power level int variable. I am unsure why I added so much useless code.
        // However I guess it can be excused while debugging and testing the conditions.

       // Return the current characters level + (16 + 5) * luckiness;
        return level + (totalPowerLevelPoints + numberOfAttributes) * luckiness;
    }

    /// <summary>
    /// A function called when the battle is completed and some xp is to be awarded.
    /// <param name="exp">The amount of experience to be awarded</param>
    /// </summary>
    public void AddXP(int exp)
    {
        // Simple Error Handling check to see whether the function was setup correctly.
        if (exp == 0)
        { 
            Debug.LogWarning("[AddXP]: " + "This character needs some experience added. Experience added is currently " + exp); 
        }


        // Add experience to the current players XP.
        currentXp += exp;

        // Update UIManager to show the player experience gained UI based on our new experience value.
        uIManager.ShowPlayerXPUI(currentXp);

        // Check if the characters current experience has reached the threshold required to level up.
        if (currentXp  >= xpThreshold)
		{
            previousThreshold = xpThreshold; // Store the previous experience threshold for the character.
            LevelUp(currentXp, previousThreshold); // Level up the player using the experience gained.
		}
    }

    /// <summary>
    ///     Handle the leveling up logic for a character
    /// </summary>
    /// <param name="p_currentXP">Current experience of the character</param>
    /// <param name="p_previousThreshold">Previous level up threshold</param>
    private void LevelUp(int p_currentXP, int p_previousThreshold)
    {
        Debug.LogWarning("[LevelUp]" + "Level up character function has been called with Experience Points: " + p_currentXP + " and previous threshold of " + p_previousThreshold);
        int currentLevel = level; // Current characters level 
        int levelCap = maxLevel; // Max Character Level 
       
        // Do a check to see whether the players current experience is greater than or equal to the old threshold. If the current level of the character 
        // is not greater than or equal to the max level
        if (p_currentXP >= xpThreshold && !(currentLevel >= levelCap))
        {
            // Increase the characters level
            level += 1;
        }


       // Simple Experience Scaling - Increase the threshold for when the character should level up based on: 
       // the previous threshold & power function. 
      
        xpThreshold = (int)(p_previousThreshold + Mathf.Pow(experienceBase * level, levelScalingFactor));
            
        Debug.Log("[LevelUp]: " + "New threshold for character " + xpThreshold);

       
        // We could do an enum based approach and only assign a total amont of points each milestone? 
        // Dont assign points to the luck value as this would just make the game incredibly unbalanced.
        // EDIT: Moved basePoints to a public variable at the top of the script as this should be a custom var.

        baseSkillPoints = 5; // base amount of points to award 
        int intelligencePoint = 0;

        // Check to see whether the player has reached a milestone. 
        if (
            level % 10 == 0 ||
            level % 25 == 0 ||
            level % 50 == 0 || 
            level % 75 == 0 ||
            level % 99 == 0
           )
		{
           // Has reached a milestone 
            hasReachedMilestone = true; 
		}
        else
		{
            // Otherwise has not reached a milestone.
            hasReachedMilestone = false;
		}

        // Debuging - Check for milestone 
        Debug.Log("[LevelUp]: " + "Has character reached milestone: " + hasReachedMilestone);

        for (int i = 1; i <= level; i++)
		{
            // If level is percentage of 1. (Testing)  
            if (level % i == 10 || level % i == 25 || level % i == 50 || level % i == 75 || level % i == 99)
			{
                intelligencePoint += 1;
			}                
		}

 
        Debug.Log("[LevelUp]: " + "Checking for milestone... " + (intelligencePoint == 0 ? "Milestone Achieved: " + intelligencePoint : "Milestone not achieved." + intelligencePoint));

        // If character has reached a milestone 
        if (hasReachedMilestone == true)
        { 
            // Add scaling factor to intelligence point
            intelligencePoint += skillPointScaling;
        }

        // Update the characters physical stats. Add intelligence points & reachedMilestone value to be handled by stats on level up.
        DistributePhysicalStatsOnLevelUp(baseSkillPoints, intelligencePoint);

        // Display Fancy Particle Effects
        ShowLevelUpEffects(); 
    }

    /// <summary>
    /// A function used to assign a random amount of points ot each of our skills.
    /// <param name="p_PointsPool">Default amount of points to distribute to the characters skills</param>
    /// <param name="p_intelligencePoints">Increase intelligence value by this value. </param>
    /// </summary>
    public void DistributePhysicalStatsOnLevelUp(int p_PointsPool, int p_intelligencePoints)
    {
        Debug.LogWarning("[DistributePhysicalStatsOnLevelUp]: " + " Points: " + p_PointsPool + " Increase Intelligence: " + p_intelligencePoints);
        // We need the current player's level
        int newStrength, // local var for storing new strength points 
            newAgility; // local var for storing new agility points


     
        if (p_intelligencePoints >= 1 && hasReachedMilestone == true)
        { 
            // Add the intelligence point(s) to the current character
            intelligence += p_intelligencePoints;

           // We want to reset has reached milestone value after we have done this otherwise this will
           // be called every level up.
        }


        // Check to see whether the characters agility is greater than strength 
        if (agility > strength)
		{
            Debug.Log("[DistributePhysicalStatsOnLevelUp]: " + " Agility " + agility + " is greater than Strength " + strength);
            // Random integer value calculated for strength local var 
            newStrength = Random.Range(1, p_PointsPool);

            // Remove points from the skill points pool.
            p_PointsPool -= strength;

            // Add points to the characters strength and agility. 
            strength += newStrength;
            agility += p_PointsPool;
        }
        // Otherwise if the characters strength skill is greater than its agility skill
        else if (strength > agility)
		{
            Debug.Log("[DistributePhysicalStatsOnLevelUp]: " + "Strength " + strength + " is greater than agility " + agility);
            // Random integer value calculated for agility local var 
            newAgility = Random.Range(1, p_PointsPool);
           
            // Remove points from the skill points pool.
            p_PointsPool -= newAgility;

            // Add points to the characters agility, then assign the rest to the characters strength.
            agility += newAgility;
            strength += p_PointsPool;
		}
        else 
		{
            // Otherwise - If the characters strength & agility skills are the same value. 
            if (strength == agility)
            { 
                Debug.Log("[DistributePhysicalStatsOnLevelUp]: " + "Strength" + strength + " and Agility " + agility + " are equal!");

                // Random integer value calculated for agility local var 
                newStrength = Random.Range(1, p_PointsPool + 1);

                // Remove points from the skill points pool.
                p_PointsPool -= newStrength;

                // Assign points randomly between both.
                strength += newStrength;
                agility += p_PointsPool;
            }
            // It would have to be one of those values, or an error would occur. Which would be handled here.
        }



        // Check to see if intelligence point value is greater than or equal to 1 as it can be scaled 
        // If milestone has been reached, increase the characters intelligence, agility and strength values randomly.
        // Check whether a character has reached a milestone
        if (p_intelligencePoints >= 1 && hasReachedMilestone == true)
        { 
            // Add Agility & Strength random value between 1 inclusive, 3 exclusive. 
            agility += Random.Range(1, 3);
            strength += Random.Range(1, 3);
            
            // Increase intelligence by point.
            intelligence += 1;

            Debug.Log("[DistributePhysicalStatsOnLevelUp]: " + "Reached milestone: " + hasReachedMilestone + " Adding Agility: " + agility + " Strength: " + strength +  " Intelligence: " + intelligence);
       
            // Reset the milestone reached boolean to default value or it will be called every level up.
            hasReachedMilestone = false;
        }

        // After calculating players physical stats, recalculate dancing stats again
        CalculateDancingStats(agility, strength, intelligence);

        // Update Stats UI to reflect the changes back to the character
        UpdateStatsUI();
    }

    #region No Modifications Required
    /// <summary>
    /// Get's all the script references required for this charactert
    /// </summary>
    private void SetUpReferences()
    {
        animController = GetComponent<AnimationController>(); // just getting a reference to our animation component on our dancer...this is behind the scenes for the dancing to occur.
        sfxHandler = FindObjectOfType<SFXHandler>(); // Finds a reference to our sfxHandler script that is in our scene.
        particleHandler = GetComponentInChildren<ParticleHandler>(); // searching through the child objects of this object to find the particle system.
    }

    /// <summary>
    /// If our statsUI field is not null, then we pass in a reference to ourself and update the stats.
    /// </summary>
    public void UpdateStatsUI()
    {
        // this just updates our UI for our character to show new stats.
        if (statsUI != null)
        {
            statsUI.UpdateStatsUI(this); // pass in a reference to our own stat script.
        }
    }

    /// <summary>
    /// Shows the level up effects whenever the character has levelled up
    /// </summary>
    private void ShowLevelUpEffects()
    {
        // plays the level up sound effect.
        sfxHandler.LevelUp();
        // emits a particle effect to show we have levelled up
        particleHandler.Emit();
        // Displays a UI Message to the player we have levelled up
        uIManager.ShowLevelUI();
    }
    #endregion 
}
