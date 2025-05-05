using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
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

    public float delay;
    public bool repeat;
    public float RepeatFrequency;
    [Min(1)] public int MaxRepeatCount = 1;

    private int RepeatCounter = 0;

    void Awake()
    {
        //jesli nasz pokoj nie znajduje siê na liscie "wyczyszczonych", spawnuje przeciwnika
        if (!LevelLoader.CleanedRooms.Contains(Level.CurrentlyOnRoom))
        {
            StartCoroutine(Spawn());
        }
    }


    public IEnumerator Spawn()
    {
        // zabezpieczenie przed pustym spawnerem
        if(Enemies.Count == 0)
        {
            Destroy(gameObject);
            yield break;
        }
            

        //delay tylko za pierwszym razem
        yield return new WaitForSeconds(delay);
            delay = 0;

        // wybiera losowego przeciwnika i spawnuje go na scenie
        int randomIndex = Random.Range(0, Enemies.Count);
        Enemy randomEnemy = Instantiate(Enemies[randomIndex], transform.position, Quaternion.identity);

        // upewnia sie ze przeciwnik jest aktywny i ustawia mu odpowiednie statystyki
        randomEnemy.gameObject.SetActive(true);

        if (Random.Range(0, 100) < AddDamageChance)
            randomEnemy.AdditionalDamage += AddDamage;

        if (Random.Range(0, 100) < AddHPChance)
            randomEnemy.HP += AddHP;

        if (Random.Range(0, 100) < AddSpeedChance)
            randomEnemy.speed += AddSpeed;

        randomEnemy.attackMode = AttackMode;


        if (repeat && RepeatCounter < MaxRepeatCount)
        {
            RepeatCounter++;
            yield return new WaitForSeconds(RepeatFrequency);
            StartCoroutine(Spawn());
        }   
        else
            Destroy(gameObject);

        yield break;
    }
}