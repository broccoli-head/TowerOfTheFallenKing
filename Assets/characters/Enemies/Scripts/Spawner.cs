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

    void Awake()
    {
        //jesli nasz pokoj nie znajduje siê na liscie "wyczyszczonych", spawnuje przeciwnika
        if (!LevelLoader.CleanedRooms.Contains(Level.CurrentlyOnRoom))
        {
            int randomIndex = Random.Range(0, Enemies.Count);
            Enemy randomEnemy = Instantiate(Enemies[randomIndex], transform.position, Quaternion.identity);

            randomEnemy.gameObject.SetActive(true);
            if (Random.Range(0, 100) < AddDamageChance)
                randomEnemy.MeleeStats.AttackDamage += AddDamage;

            if (Random.Range(0, 100) < AddHPChance)
                randomEnemy.HP += AddHP;

            if (Random.Range(0, 100) < AddSpeedChance)
                randomEnemy.speed += AddSpeed;

            randomEnemy.attackMode = AttackMode;
        }
        Destroy(gameObject);
    }
}