using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    /// <summary>
    /// Our variables used to determine our fighting power.
    /// </summary>
    public int style;
    public int luck; 
    public int rhythm;

    /// <summary>
    /// Our physical stats that determine our dancing stats.
    /// </summary>
    public int agility;
    public int intelligence;
    public int strength;

    /// <summary>
    /// Used to determine the conversion of 1 physical stat, to 1 dancing stat.
    /// </summary>
    public float agilityMultiplier = 0.5f;
    public float strengthMultiplier = 1f;
    public float inteligenceMultiplier = 2f;

    /// <summary>
    /// A float used to display what the chance of winning the current fight is.
    /// </summary>
    public float perecentageChanceToWin;

    #region character references, no mods required
    [HideInInspector]
    public AnimationController animController; // reference to our animation controller on our character
    [HideInInspector]
    public SFXHandler sfxHandler; // reference to our sfx Handler in our scene
    [HideInInspector]
    public ParticleHandler particleHandler; // a reference to our particle system that is played when we level up.  
    public UIManager uIManager; // a reference to the UI Manager in our scene.
    public StatsUI statsUI; // a referecence to our stats ui for this character.
    #endregion

    /// <summary>
    /// Called on the very first frame of the game
    /// </summary>
    private void Start()
    {
        SetUpReferences();//sets up the references to other scripts we need for functionality.
        GeneratePhysicalStatsStats(); // we want to generate some physical stats.
        CalculateDancingStats();// using those physical stats we want to generate some dancing stats.
    }

    /// <summary>
    /// This function should set our starting stats of Agility, Strength and Intelligence
    /// to some default RANDOM values.
    /// </summary>
    public void GeneratePhysicalStatsStats()
    {
        Debug.LogWarning("Generate Physical Stats has been called");

        // Let's set up agility, intelligence and strength to some default Random values.

        UpdateStatsUI(); // update our current UI for our character
    }

    /// <summary>
    /// This function should set our style, luck and ryhtmn to values
    /// based on our currrent agility,intelligence and strength.
    /// </summary>
    public void CalculateDancingStats()
    {
        Debug.LogWarning("Generate Calculate Dancing Stats has been called");
        // what we want is for you to take our physical stats and translate them into our dancing stats,
        // based on the multiplier of that stat as follows:
        // our Style should be based on our Agility.
        // our Rhythm should be based on our Strength.
        // our Luck should be based on our intelligence.
        // hint...your going to need to convert our ints into floats, then back to ints.

        UpdateStatsUI(); // update our current UI for our character
    }

    /// <summary>
    /// This takes in a normalised value i.e. 0.0f - 1.0f, and is used to display our % chance to win.
    /// </summary>
    /// <param name="normalisedValue"></param>
    public void SetPercentageValue(float normalisedValue)
    {
        // Essentially we want to set our percentage to win, to be a percentage using our normalised value (decimal value of a fraction)
        // How can we convert out normalised value into a whole number?
        
        Debug.LogWarning("SetPercentageValue has been called we probably want to convert our normalised value to a percentage");
        UpdateStatsUI(); // update our current UI for our character
    }

    /// <summary>
    /// Used to generate a number of battle points that is used in combat.
    /// </summary>
    /// <returns></returns>
    public int ReturnDancePowerLevel()
    {
        // TODO: 
        //   generate a number of points based off of our luck,style and rythm,
        //   add some randomness in the calculation
      

        Debug.LogWarning("ReturnBattlePoints has been called we probably want to create some battle points based on our stats");
        return 0;
    }

    /// <summary>
    /// A function called when the battle is completed and some xp is to be awarded.
    /// The amount of xp gained is coming into this function
    /// </summary>
    public void AddXP(int xpGained)
    {
        Debug.LogWarning("This character needs some xp to be given, the xpGained from the fight was: " + xpGained);
        // we probably want to do something with the xpGained.

        //We probably want to display the xp we just gained, by default it is 0
        uIManager.ShowPlayerXPUI(0);

        // We probably also want to check to see if the player can level up and if so do something....what should we be checking?
    }


    // purpose of the function would be - Things to think about 



    /// <summary>
    /// A function used to handle actions associated with levelling up.
    /// </summary>
    private void LevelUp()
    {
        Debug.LogWarning("Level up has been called");
        // we probs want to increase our level....
        // As well as probably want to increase our threshold for when we should level up...based on our current new level
        // TODO: increase our physical stats (if only we had a function to do that for us)
            // Add a fucntion to increase physical stats     
      
        ShowLevelUpEffects(); // displays some fancy particle effects.
    }
    
    /// <summary>
    /// A function used to assign a random amount of points ot each of our skills.
    /// </summary>
    public void DistributePhysicalStatsOnLevelUp(int PointsPool)
    {
        Debug.LogWarning("DistributePhysicalStatsOnLevelUp has been called " + PointsPool);
        // let's share these points somewhat evenly or based on some forumal to increase our current physical stats
        // then let's recalculate our dancing stats again to process and update the new values.
    }

    #region No Mods Required
    private void SetUpReferences()
    {
        animController = GetComponent<AnimationController>(); // just getting a reference to our animation component on our dancer...this is behind the scenes for the dancing to occur.
        sfxHandler = FindObjectOfType<SFXHandler>(); // Finds a reference to our sfxHandler script that is in our scene.
        particleHandler = GetComponentInChildren<ParticleHandler>(); // searching through the child objects of this object to find the particle system.
    }


    public void UpdateStatsUI()
    {
        // this just updates our UI for our character to show new stats.
        if (statsUI != null)
        {
            statsUI.UpdateStatsUI(this); // pass in a reference to our own stat script.
        }
    }

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
