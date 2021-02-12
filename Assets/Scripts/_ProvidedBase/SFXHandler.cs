using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple SFX feedback for basic game events
/// 
/// Provided with framework, no modification required
/// </summary>
public class SFXHandler : MonoBehaviour
{
    public AudioClip levelUpClip;
    public AudioClip playerWins;
    public AudioClip playerLoses;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void LevelUp()
    {
        source.PlayOneShot(levelUpClip);
    }

    public void BattleResult(float outcome)
    {
        if(outcome > 0)
        {
            source.PlayOneShot(playerWins);
        }
        else if(outcome < 0)
        {
            source.PlayOneShot(playerLoses);
        }
    }
}
