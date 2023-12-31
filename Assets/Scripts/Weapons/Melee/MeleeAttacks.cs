using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Melee Attack", menuName = "Melee")]
public class MeleeAttacks : ScriptableObject
{
    
    public int damage;

    [Tooltip("How many times you may attack per second")]
    public float attacksPerSecond;
    
    [Tooltip ("How much damage will be multiplied when hitting critical areas (eg. headshots)")]
    public float criticalMultiplier;

    [Tooltip ("How long it takes for the attack hitbox to come out after receiving input in seconds")]
    public float attackDelay;

    [Tooltip ("How far an attack reaches")]
    public float range;


    [Header ("Knockback")]

    [Tooltip ("How much knockback the attack deals")]
    public float enemyKnockback;

    [Tooltip ("Does the player receive backwards knockback when melee connects")]
    public bool hasBackwardsKnockback = false;

    [Tooltip ("How much backwards force the player receives (may ignore if previous bool was left as false)")]
    public float backwardsKnockback;

    [HideInInspector] public bool canParry;
    [HideInInspector] public float parryRange;
    [HideInInspector] public float parryWindow;
    [HideInInspector] public float parryActiveTime;
    [HideInInspector] public float parryMultiplier;


    [Header ("Visuals")]

    [Tooltip ("Model that will be used for the attack (eg. knife)")]
    public GameObject weaponModel;

    [Tooltip ("Animation clip that will play when using melee")]
    public AnimationClip swingAnimation;

    [Header ("Audio")]

    [Tooltip ("Sound that will play when weapon is used")]
    public AudioClip weaponSwingSFX;

    [Tooltip ("Sound that will play when attack connects")]
    public AudioClip hitSFX;

    [Tooltip ("Sound that will play when reflecting bullets")]
    public AudioClip parrySFX;
}
