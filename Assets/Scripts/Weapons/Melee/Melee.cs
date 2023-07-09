using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    //Editor tools
    private enum MeleeTypeDropdown {SingleSword, DualWieldedChainSwords, Kick};
    [SerializeField]
    [Tooltip ("What kind of melee to perform")]
    MeleeTypeDropdown meleeType = new MeleeTypeDropdown();

    [SerializeField]
    private LayerMask layersToIgnore;

    [SerializeField] private bool isPlayer = false;

    //Script variables
    private bool canAttack = true;
    private float meleeCooldown;

    //Variables inherited from scriptable object
    private int damage;
    private float attacksPerSecond;
    private float criticalMultiplier;
    private float attackDelay;
    private float range;
    private float enemyKnockback;
    private bool hasBackwardsKnockback;
    private float backwardsKnockback;
    private AudioClip swingSFX;
    private AudioClip hitSFX;

    //Required components
    private MeleeAttacks meleeScriptable;
    private Transform virtualCamera;
    private AudioSource meleeAudioSource;
    private ForceReceiver forceReceiverScript;
    private Ray meleeRaycast;

    private void Start() 
    {
        UpdateMelee(this);
        forceReceiverScript = GetComponent<ForceReceiver>();
    }

    public void UseMelee()
    {
        if(!canAttack) return;

        canAttack = false;
        meleeAudioSource.PlayOneShot(swingSFX);
        StartCoroutine("Attack");
        StartCoroutine("MeleeCooldown");
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackDelay);

        meleeRaycast = new Ray(virtualCamera.position, virtualCamera.forward);
        Debug.DrawRay(virtualCamera.position, virtualCamera.forward, Color.green, 10);
        if(Physics.Raycast(meleeRaycast, out RaycastHit hitInfo, range, layersToIgnore))
        {
            meleeAudioSource.PlayOneShot(hitSFX);

            //Deal damage to hurtboxes
            if(hitInfo.transform.CompareTag("Hurtbox"))
            {
                hitInfo.transform.parent.gameObject.GetComponent<Health>().TakeDamage(damage);
            }

            //Apply force to rigidbodies
            Rigidbody rb = hitInfo.transform.gameObject.GetComponent<Rigidbody>();
            if(rb)
            {
                Vector3 forceDirection = (rb.transform.position - transform.position).normalized;
                rb.AddForce(forceDirection * enemyKnockback, ForceMode.Impulse);
            }

            if(hasBackwardsKnockback)
            {
                forceReceiverScript.ReceiveExplosion((transform.position - meleeRaycast.GetPoint(hitInfo.distance)), backwardsKnockback, false);
            }
        }
    }

    private IEnumerator MeleeCooldown()
    {
        yield return new WaitForSeconds(meleeCooldown);

        canAttack = true;
    }

    public void UpdateMelee(Melee meleeScriptToPullFrom)
    {
        meleeType = meleeScriptToPullFrom.meleeType;
        meleeScriptable = (MeleeAttacks) Resources.Load(meleeType.ToString());
        damage = meleeScriptable.damage;
        attacksPerSecond = meleeScriptable.attacksPerSecond;
        meleeCooldown = 1/attacksPerSecond;
        criticalMultiplier = meleeScriptable.criticalMultiplier;
        attackDelay = meleeScriptable.attackDelay;
        range = meleeScriptable.range;
        enemyKnockback = meleeScriptable.enemyKnockback;
        hasBackwardsKnockback = meleeScriptable.hasBackwardsKnockback;
        backwardsKnockback = meleeScriptable.backwardsKnockback;
        swingSFX = meleeScriptable.weaponSwingSFX;
        hitSFX = meleeScriptable.hitSFX;

        if(!isPlayer) return;

        virtualCamera = GetComponentInChildren<CameraMove>().transform;
        AudioSource[] audioSources = GetComponents<AudioSource>();
        meleeAudioSource = audioSources[1];
    }   
}
