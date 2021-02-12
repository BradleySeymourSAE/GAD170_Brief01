using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Routes physics and game information to to the unity animator on this GO/children
/// 
/// Provided with framework, no modification required
/// </summary>
public class AnimationController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody body;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        body = GetComponent<Rigidbody>();
    }

    public void BattleResult(float outcome)
    {
        if (outcome > 0)
        {
            anim.SetTrigger("Win");
        }
        else
        {
            anim.SetTrigger("Lose");
        }
    }

    void FixedUpdate()
    {
        anim.SetFloat("Velocity", body.velocity.sqrMagnitude);
    }

    public void Dance()
    {
        //only dance off if we are the target    
        anim.SetTrigger("Dance");
    }
}
