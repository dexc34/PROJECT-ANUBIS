using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Melee Attack", menuName = "Melee")]
public class MeleeAttacks : ScriptableObject
{
    [Tooltip ("How much damage an indiviual hit deals")]
    public int damage;

    [Tooltip("How many times you may attack per second")]
    public float attacksPerSecond;
    
    [Tooltip ("How much damage will be multiplied when hitting critical areas (eg. headshots)")]
    public float criticalMultiplier;

    [Tooltip ("How long it takes for the attack hitbox to come out after receiving input in seconds")]
    public float attackDelay;

    [Tooltip ("How far an attack reaches")]
    public float range;

    [Tooltip ("How much knockback the attack deals")]
    public float knockback;

    [Tooltip ("Model that will be used for the attack (eg. knife)")]
    public GameObject weaponModel;

    [Tooltip ("Sound that will play when weapon is used")]
    public AudioClip weaponSwingSFX;

    [Tooltip ("Sound that will play when attack connects")]
    public AudioClip hitSFX;
}
