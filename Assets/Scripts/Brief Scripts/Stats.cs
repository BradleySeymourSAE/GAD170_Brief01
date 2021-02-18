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
   
    public int level;
    public int currentXp;
    public int xpThreshold = 10;
    int skillPointScaling = 1;

    int previousThreshold;
    public int experienceBase = 50;
    float levelScaling = 1.2f;
    float levelProgress;
    float experienceStep;


    // Dance Stats (Calculated from Character Attrs)
    public int style;
    public int luck; 
    public int rhythm;

    // Character Attributes 
    public int agility = 2;
    public int intelligence = 1;
    public int strength = 2;

    // Modifiers 
    public float agilityMultiplier = 0.5f;
    public float strengthMultiplier = 1f;
    public float intelligenceMultiplier = 2f;

   
    public float percentageChanceToWin;

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
        agility = 2;
        intelligence = 1;
        strength = 3;

        if (level <= 0)
            level = 1;

        // Removed CalculateDanceStats from the start function as this will run BOTH FUNCTIONS at the same 
        // time and the dancing stats will not return the correct values, if any at all.

        SetUpReferences();//sets up the references to other scripts we need for functionality.
        GeneratePhysicalStatsStats(); // we want to generate some physical stats.
    }

    /// <summary>
    /// This function should set our starting stats of Agility, Strength and Intelligence
    /// to some default RANDOM values.
    /// </summary>
    public void GeneratePhysicalStatsStats()
    {
       agility = Random.Range(1, 2);
       strength = Random.Range(1, 4);
       intelligence = Random.Range(1, 3);


        CalculateDancingStats(agility, strength, intelligence);
        UpdateStatsUI(); // update our current UI for our character
    }

    /// <summary>
    /// This function should set our style, luck and ryhtmn to values
    /// based on our currrent agility,intelligence and strength.
    /// </summary>
    public void CalculateDancingStats(int _agility, int _strength, int _intelligence)
    {
        Debug.LogWarning("Generate Calculate Dancing Stats has been called");
        // take our physical stats and translate them into dancing stats,
        // Style = Agility * .5f
        // Rhythm = Strength * 1.0f 
        // Luck = intelligence * 2.0f
        // int -> float -> int

        // Check that any of the values are not equal to 0
        // Ensure the multipliers are not less than 0.0f (Minimum Threshold)
        if (agilityMultiplier <= 0.0f)
            agilityMultiplier = 0.5f;
        if (strengthMultiplier <= 0.0f)
            strengthMultiplier = 1f;
        if (intelligenceMultiplier <= 0.0f)
            intelligenceMultiplier = 2f;


        // As we are multiplying by a float (Modifiers) 
        // We need to parse the characters dance attributes (style, rhythm, luck) as floats.
        float _style = _agility * agilityMultiplier;
        float _rhythm = _strength * strengthMultiplier;
        float _luck = _intelligence * intelligenceMultiplier;


        style = (int)_style;
        rhythm = (int)_rhythm;
        luck = (int)_luck;

        // DEBUGGING: Return the type (int, string etc)
        // Debug.Log("Style is type: " + style.GetType());
        // Debug.Log("Rhythm is type: " + rhythm.GetType());
        // Debug.Log("Luck is type: " + luck.GetType());
        // Debug.Log("Style: " + style + " Rhythm: " + rhythm + " Luck: " + luck);

        UpdateStatsUI(); // update our current UI for our character
    }


    /// <summary>
    /// This is takes in a normalised value i.e. 0.0f - 1.0f, and is used to display our % chance to win.
    /// </summary>
    /// <param name="normalisedValue"></param>
    public void SetPercentageValue(float normalisedValue)
    {
        // Set percentage to be a normalised value.
        // See BattleHandler.cs Line 50

        // convert value into whole number
        normalisedValue = Mathf.Round(normalisedValue);

        // DEBUGGING 
        // Debug.Log("Set Percentage Value: " + normalisedValue);

        percentageChanceToWin = normalisedValue;
        UpdateStatsUI();
    }

    // FEEDBACK: - Implement current level into the power level calculation to BALANCE the game, as apparently after 
    //             you reach level 2 - It becomes quite easy to defeat the opponent. 
    //           - Could also add the levelling system for the NPC's aswell 
    public int ReturnDancePowerLevel()
    {
        // generate a number of points based off of our luck,style and rhythm, add randomness in calculation
        // to ensure that there is not always a draw, by default it just returns 0. 
        int luckiness = Random.Range(2, luck);
        int number_of_attributes = 6;
        int s_currentLevel = level;

        // Example Base Points: 4 + 5 + 8 + 5 + 2 + 16
        int basePoints = agility + strength + intelligence + rhythm + style;

        // To get the power level we divide the base points by the amount of amount of attributes 
        // Then multiply by luck

        // int powerLevel = (basePoints / number_of_attributes) * luckiness;
        // However another method we could test is adding the base points an attrs and multiplying by luck

        int powerLevel = s_currentLevel + (basePoints + number_of_attributes) * luckiness;

        if (powerLevel != 0)
            return powerLevel;
        else
		{
            Debug.LogWarning("ReturnBattlePoints has been called we probably want to create some battle points based on our stats");
            powerLevel = 0;
            return powerLevel;
        }
    }

    /// <summary>
    /// A function called when the battle is completed and some xp is to be awarded.
    /// The amount of xp gained is coming into this function
    /// </summary>
    public void AddXP(int exp)
    {
        if (exp == 0)
            Debug.LogWarning("This character needs some xp to be given, the xpGained from the fight was: " + exp);

        // Check to see if the player has leveled up
        // Use the experience step in the update level ui slider to determine the sliders
        // movement speed 

        experienceStep = (float)exp / ((float)xpThreshold - (float)previousThreshold);
        currentXp += exp;

        // Display XP Gained to the player 
        uIManager.ShowPlayerXPUI(currentXp);

        if (currentXp  >= xpThreshold)
		{
            // Level up!
            // Store last xp threshold in a local variable 
            previousThreshold = xpThreshold;
            LevelUp(currentXp, previousThreshold);
		}


        // Calculates the percentage needed to level up again, temporarily convert all int to floats 
       
        levelProgress = ((float)currentXp - (float)previousThreshold) / ((float)xpThreshold - (float)previousThreshold);
        // UpdateExperienceSliderUI();
    }

    private enum PointsMilestones
	{
      Base = 1,
      Pro = 2,
      Master = 3
	}

    private void LevelUp(int p_CurrentXP, int p_previousThreshold)
    {
        Debug.LogWarning("Level up has been called");
        int s_CurrentLevel = level;
        int maxLevel = 99;

        // Increase the players level
        Debug.Log("Current XP: " + p_CurrentXP + " Threshold: " + xpThreshold + " Not Level greater than or equal to maxLevel: " + !(level >= maxLevel));
        if (p_CurrentXP >= xpThreshold && !(s_CurrentLevel >= maxLevel))
            level += 1;

        // Simple experience scaling 
       // Increase the threshold for when the player should level up...based on our new level
       float s_newThreshold = p_previousThreshold + Mathf.Pow(experienceBase * level, levelScaling);

        // Convert back to integer
        xpThreshold = (int)s_newThreshold;

        // How many points should we assign to the players stats? 
        // We could do an enum based approach and only assign a total amont of points each milestone? 
        // We dont want to add any points to the luck value as this would just make the game incredibly unbalanced.

        int basePoints = 5;
        int intelligencePoint = 0;
        bool s_reachedMilestone = false;

        switch (level)
		{
            case 10:
            case 25:
            case 50:
            case 75:
            case 99:
                s_reachedMilestone = true;
                break;
            default:
            break;
		}


        if (s_reachedMilestone)
		{
            if (skillPointScaling == 0)
                intelligencePoint = 1;
            else
                intelligencePoint += skillPointScaling;
		}

        // Increase core characters stats
        // Focus on strength and the agility, and only add intelligence for each milestone 

        DistributePhysicalStatsOnLevelUp(basePoints, intelligencePoint);
        ShowLevelUpEffects(); // displays some fancy particle effects.
    }
    
    /// <summary>
    /// A function used to assign a random amount of points ot each of our skills.
    /// </summary>
    public void DistributePhysicalStatsOnLevelUp(int p_PointsPool, int p_Intelligence)
    {
        Debug.LogWarning("DistributePhysicalStatsOnLevelUp has been called " + p_PointsPool);
        // We need the current player's level
        int s_currentLevel = level;
        int remainder;

        if (p_Intelligence == 1)
            intelligence += p_Intelligence;

        if (agility > strength)
        { 
            int _strength = Random.Range(1, p_PointsPool);
            remainder = p_PointsPool - _strength;
            strength += _strength;
            agility += remainder;
        }
        else if (strength > agility)
		{
            int _agility = Random.Range(1, p_PointsPool);
            remainder = p_PointsPool - _agility;
            agility += _agility;
            strength += remainder;
		}
        else
        { 
            if (strength == agility)
			{
                // If they are both the same value, then we will just randomise the points pool
                int s_randomStrength = Random.Range(1, p_PointsPool + 1);
                int s_randomAgility = p_PointsPool - s_randomStrength;
                agility += s_randomAgility;
                strength += s_randomStrength;
			}
		}

        // Run a switch statement to check whether the player has met milestones 
        // If the player has met them (Level 10, 25, 50, 75, 99) add an extra point or two
        // to the player's intelligence (Which in turn gives them more luck 

        switch (s_currentLevel)
        { 
            case 10:
            case 25:
            case 50:
            case 75:
            case 99:
            if (agility < strength)
                agility += 2;
            if (strength < agility)
                strength += 2;


            // Regardless, we want to add an extra point to intelligence
            intelligence += 1;
            break;
            default:
            break;
        }


        // After we have calculated the current players physical stats we then need to recalculate 
        // the dancing stats again - process and update the new vaues (Including the UI)
            
        Debug.Log("Intelligence: " + intelligence + " Agility: " + agility + " Strength: " + strength);

        CalculateDancingStats(agility, strength, intelligence);
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
