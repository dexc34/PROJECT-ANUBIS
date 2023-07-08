using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Melee : MonoBehaviour
{
    //Editor tools
    private enum MeleeTypeDropdown {SingleSword, DualWieldedChainSwords};
    [SerializeField]
    [Tooltip ("What kind of melee to perform")]
    MeleeTypeDropdown meleeType = new MeleeTypeDropdown();

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
    private float knockback;

    //Required components
    private MeleeAttacks meleeScriptable;
    private Transform virtualCamera;

    private void Start() 
    {
        UpdateMelee(this);
    }

    public void UseMelee()
    {
        if(!canAttack) return;

        Debug.Log("Used melee");
        canAttack = false;
        StartCoroutine("Attack");
        StartCoroutine("MeleeCooldown");
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackDelay);
        Debug.DrawRay(virtualCamera.position, virtualCamera.forward, Color.cyan, 1);
        if(Physics.Raycast(virtualCamera.position, virtualCamera.forward, out RaycastHit hitInfo, range, 15))
        {
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
                rb.AddForce(forceDirection * knockback, ForceMode.Impulse);
            }
        }
    }

    private IEnumerator MeleeCooldown()
    {
        yield return new WaitForSeconds(meleeCooldown);

        canAttack = true;
        Debug.Log("Melee cooldown over");
    }

    public void UpdateMelee(Melee meleeScriptToPullFrom)
    {
        meleeType = meleeScriptToPullFrom.meleeType;
        meleeScriptable = (MeleeAttacks) AssetDatabase.LoadAssetAtPath("Assets/Scripts/Weapons/Melee/" + meleeType.ToString() + ".asset", typeof (MeleeAttacks));
        damage = meleeScriptable.damage;
        attacksPerSecond = meleeScriptable.attacksPerSecond;
        meleeCooldown = 1/attacksPerSecond;
        criticalMultiplier = meleeScriptable.criticalMultiplier;
        attackDelay = meleeScriptable.attackDelay;
        range = meleeScriptable.range;
        knockback = meleeScriptable.knockback;

        if(!isPlayer) return;

        virtualCamera = GetComponentInChildren<CameraMove>().transform;
        Debug.Log("Updated melee");
    }   
}
