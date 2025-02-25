using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Spawner : MonoBehaviour
{
    public List<Enemy> Enemies = new List<Enemy>();
    [Header ("Chances are in percentage from 0% to 100%")]
    [Space(10)]

    public float AddDamage = 0;
    [Range(0, 100)]
    public float AddDamageChance = 0f;

    public float AddHP = 0;
    [Range(0, 100)]
    public float AddHPChance = 0f;

    public float AddSpeed = 0;
    [Range(0, 100)]
    public float AddSpeedChance = 0f;

    [Space(10)]
    public Enemy.AttackMode AttackMode;
    LevelLoader levelLoader;

    void Awake()
    {
        levelLoader = LevelLoader.Instance;

        if (!levelLoader.ActualLevel.Cleaned)
        {
            int randomIndex = Random.Range(0, Enemies.Count);
            Enemy randomEnemy = Instantiate(Enemies[randomIndex], transform.position, Quaternion.identity);

            randomEnemy.gameObject.SetActive(true);
            if (Random.Range(0, 100) < AddDamageChance)
                randomEnemy.AttackDamage += AddDamage;

            if (Random.Range(0, 100) < AddHPChance)
                randomEnemy.HP += AddHP;

            if (Random.Range(0, 100) < AddSpeedChance)
                randomEnemy.speed += AddSpeed;

            randomEnemy.attackMode = AttackMode;
        }
        else
            Debug.Log("Already cleaned");
        
    }
}