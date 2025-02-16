using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class AttackHandler : MonoBehaviour
{

    Enemy enemy;
    float AttackDamage;
    Potion.DamageType AttackType;
    float AttackSpeed;
    float AttackDistance;
    float AttackDelay;
    bool AttackReady = true;


    void Start()
    {
        enemy = GetComponent<Enemy>();
        AttackDamage = enemy.AttackDamage;
        AttackType = enemy.AttackType;
        AttackSpeed = enemy.AttackSpeed;
        AttackDelay = enemy.AttackDelay;
        AttackDistance = enemy.AttackDistance;
    }

    void Update()
    {
        if (enemy.CanAttack && AttackReady)
            StartCoroutine(MeleeAttack());
    }

    public IEnumerator MeleeAttack()
    {
        AttackReady = false;
        //op�nienie ataku
        yield return new WaitForSeconds(AttackDelay);

        //zadawanie obra�e�
        Collider2D[] Attacked = Physics2D.OverlapCircleAll(transform.position, AttackDistance + (GetComponent<Collider2D>().bounds.size.y / 2));
        foreach (Collider2D obj in Attacked)
        {
            //przeciwnik nie atakuje samego siebie
            if (obj.gameObject == gameObject) continue;

            // je�li frendly fire jest wy��czony pomija innych przeciwnik�w
            if (!enemy.FriendlyFire)
                if (obj.TryGetComponent<Enemy>(out Enemy en))
                    continue;

            //proboje pobra� component implem�tuj�cy ReviceDamage je�li go znajdzie zadaje obra�enia
            var target = ComponentHelper.GetInterfaceComponent<ReviceDamage>(obj.gameObject);
            if (target != null)
            {
                target.Damage(AttackDamage, AttackType, Potion.DamagePlace.Zone, false);
            }
        }

        // nast�pny atak mo�e by� wykonany dopiero po pewnym czasie
        yield return new WaitForSeconds(AttackSpeed);
        AttackReady = true;
        yield break;
    }
}
