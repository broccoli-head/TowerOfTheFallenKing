using UnityEngine;

public interface ReviceDamage
{

    public void Damage(float Damage, Potion.DamageType DamageType, Potion.DamagePlace DamagePlace, bool EnemyOnly);

    public void Damage(float DPS, float Time, Potion.DamageType DamageType, Potion.DamagePlace DamagePlace, GameObject EffectObject, bool EnemyOnly);

    public void AddCleanse();

}