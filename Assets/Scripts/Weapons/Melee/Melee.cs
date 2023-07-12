using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    //Editor tools
    private enum MeleeTypeDropdown { SingleSword, DualWieldedChainSwords, Kick };
    [SerializeField]
    [Tooltip("What kind of melee to perform")]
    MeleeTypeDropdown meleeType = new MeleeTypeDropdown();

    [SerializeField]
    private LayerMask layersToIgnore;

    //Script variables
    private bool canAttack = true;
    private float meleeCooldown;
    private bool isParrying = false;
    private bool isPlayer = false;
    private bool hasParried = false;
    private bool hasModel = false;
    private int animatorIndex = 0;
    private List<Animator> animators = new List<Animator>();

    //Variables inherited from scriptable object
    private int damage;
    private float attacksPerSecond;
    private float criticalMultiplier;
    private float attackDelay;
    private float range;

    private float enemyKnockback;
    private bool hasBackwardsKnockback;
    private float backwardsKnockback;

    private bool canParry;
    private float parryRange;
    private float parryWindow;
    private float parryActiveTime;
    private float parryMultiplier;

    [HideInInspector] public GameObject meleeModel;

    private AudioClip swingSFX;
    private AudioClip hitSFX;
    private AudioClip parrySFX;

    //Required components
    private MeleeAttacks meleeScriptable;
    private Transform virtualCamera;
    private AudioSource meleeAudioSource;
    private ForceReceiver forceReceiverScript;
    private Ray meleeRaycast;
    private Ray parryRaycast;

    private void Start()
    {
        forceReceiverScript = GetComponent<ForceReceiver>();
        if (transform.CompareTag("Player")) isPlayer = true;
        else isPlayer = false;

        UpdateMelee(this);
    }

    private void Update()
    {
        if (!isParrying) return;
        Parry();
    }

    public void UseMelee()
    {
        if (!canAttack) return;

        canAttack = false;
        meleeAudioSource.PlayOneShot(swingSFX);
        if (canParry) StartCoroutine(ParryTimer(parryWindow));
        StartCoroutine("Attack");
        StartCoroutine("MeleeCooldown");
        if(hasModel) PlayAnimation("sword_swing");
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackDelay);

        meleeRaycast = new Ray(virtualCamera.position, virtualCamera.forward);
        Debug.DrawRay(virtualCamera.position, virtualCamera.forward, Color.green, 10);
        if (Physics.Raycast(meleeRaycast, out RaycastHit hitInfo, range, layersToIgnore))
        {
            meleeAudioSource.PlayOneShot(hitSFX);

            //Deal damage to hurtboxes
            if (hitInfo.transform.CompareTag("Hurtbox"))
            {
                hitInfo.transform.parent.gameObject.GetComponent<Health>().TakeDamage(damage);
            }

            //Apply force to rigidbodies
            Rigidbody rb = hitInfo.transform.gameObject.GetComponent<Rigidbody>();
            if (rb)
            {
                if(!rb.CompareTag("Bullet"))
                {
                    Vector3 forceDirection = (rb.transform.position - transform.position).normalized;
                    rb.AddForce(forceDirection * enemyKnockback, ForceMode.Impulse);
                }
            }

            if (hasBackwardsKnockback)
            {
                forceReceiverScript.ReceiveExplosion((transform.position - meleeRaycast.GetPoint(hitInfo.distance)), backwardsKnockback, false);
            }
        }
    }

    private void Parry()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(virtualCamera.position + (virtualCamera.forward * (range/2)), range, layersToIgnore);
        foreach (Collider collider in nearbyObjects)
        {
            if (collider.CompareTag("Bullet"))
            {
                //Hit enemy bullets if script is in player
                if (collider.gameObject.layer == 9)
                {
                    if (isPlayer)
                    {
                        //Turn bullet into a friendly one
                        collider.gameObject.layer = 3;
                        ReflectBullet(collider);
                    }
                }

                //Hit player bullets if script is in enemy
                if(collider.gameObject.layer == 3)
                {
                    if(!isPlayer)
                    {
                        //Turn bullet into an enemy one
                        collider.gameObject.layer = 9;
                        ReflectBullet(collider);
                    }
                }
            }
        }
    }

    private void ReflectBullet(Collider bulletToReflect)
    {
        bulletToReflect.transform.position = virtualCamera.transform.position + virtualCamera.forward;
        bulletToReflect.GetComponent<Rigidbody>().velocity = Vector3.zero;

        //Multiply bullet damage 
        Bullets bulletScript = bulletToReflect.GetComponent<Bullets>();
        if (bulletScript) bulletScript.damage *= parryMultiplier;

        //Multiply explosion damage
        Explosion explosionScript = bulletToReflect.GetComponent<Explosion>();
        if (explosionScript) explosionScript.damage *= parryMultiplier;

        Rigidbody rb = bulletToReflect.GetComponent<Rigidbody>();
        Vector3 forceDirection = (virtualCamera.forward);
        rb.AddForce(forceDirection * enemyKnockback, ForceMode.Impulse);
        meleeAudioSource.PlayOneShot(parrySFX);
        StartCoroutine(ParryTimer(parryActiveTime));
    }

    private IEnumerator ParryTimer(float parryTimer)
    {
        isParrying = true;

        yield return new WaitForSeconds(parryTimer);

        isParrying = false;
    }

    private IEnumerator MeleeCooldown()
    {
        yield return new WaitForSeconds(meleeCooldown);

        canAttack = true;
    }

    private void PlayAnimation(string animationClipName)
    {
        if(animators.Count == 1)
        {
            animators[0].Play(animationClipName);
        }
        else
        {
            animators[animatorIndex].Play(animationClipName);
            if(animatorIndex == 0) animatorIndex = 1;
            else animatorIndex = 0;
        }
    }

    public void UpdateMelee(Melee meleeScriptToPullFrom)
    {
        meleeType = meleeScriptToPullFrom.meleeType;
        meleeScriptable = (MeleeAttacks)Resources.Load(meleeType.ToString());
        damage = meleeScriptable.damage;
        attacksPerSecond = meleeScriptable.attacksPerSecond;
        meleeCooldown = 1 / attacksPerSecond;
        criticalMultiplier = meleeScriptable.criticalMultiplier;
        attackDelay = meleeScriptable.attackDelay;
        range = meleeScriptable.range;

        //Knockback stats
        enemyKnockback = meleeScriptable.enemyKnockback;
        hasBackwardsKnockback = meleeScriptable.hasBackwardsKnockback;
        backwardsKnockback = meleeScriptable.backwardsKnockback;

        //Parry stats
        canParry = meleeScriptable.canParry;
        parryRange = meleeScriptable.parryRange;
        parryWindow = meleeScriptable.parryWindow;
        parryActiveTime = meleeScriptable.parryActiveTime;
        parryMultiplier = meleeScriptable.parryMultiplier;

        //Visuals
        if(meleeScriptable.weaponModel) 
        {
            meleeModel = meleeScriptable.weaponModel;
            hasModel = true;
        }
        else hasModel = false;

        //Audio clips
        swingSFX = meleeScriptable.weaponSwingSFX;
        hitSFX = meleeScriptable.hitSFX;
        parrySFX = meleeScriptable.parrySFX;

        if (!isPlayer) return;

        virtualCamera = GetComponentInChildren<CameraMove>().transform;
        AudioSource[] audioSources = GetComponents<AudioSource>();
        meleeAudioSource = audioSources[1];

        //Create new weapon
        GameObject weaponHolder = transform.GetComponentInChildren<CameraMove>().gameObject.transform.Find("Weapon Holder").gameObject;
        GameObject newWeapon = Instantiate(meleeModel, weaponHolder.transform.position, weaponHolder.transform.rotation);
        newWeapon.transform.parent = weaponHolder.transform;
        animators.Clear();
        animators.AddRange(newWeapon.GetComponentsInChildren<Animator>());
        foreach(Animator animator in animators) Debug.Log(animator.gameObject.name);
    }
}
