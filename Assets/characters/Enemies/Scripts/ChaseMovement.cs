using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Enemy))]
public class ChaseMovement : MonoBehaviour, Controller
{
    private Enemy enemy;
    private Vector2 playerPos;
    private Rigidbody2D rb;
    private GameObject player;
    private Vector2 moveDirection;


    private float chaseSpeed;
    private float AttackDistance = 0;
    private bool XBlocked = false;
    private bool YBlocked = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GetComponent<Enemy>();
        chaseSpeed = enemy.chaseSpeed;
        AttackDistance = enemy.AttackDistance;

        if (player != null)
        {
            playerPos = player.transform.position;
        }
        else
        {
            Debug.LogError("Nie znaleziono obiektu gracza!");
        }

        StartCoroutine(MoveToPlayer());
    }

    void FixedUpdate()
    {
        // Je¿eli przeciwnik jest "zamro¿ony" zmniejsza czas pozosta³y do odmro¿enia i przerywa wykonywanie funkcji
        if(enemy.FreezeTime > 0)
        {
            enemy.FreezeTime -= Time.fixedDeltaTime;
            if (enemy.FreezeTime < 0)
                enemy.FreezeTime = 0;
            return;
        }

        // Porusza przeciwnika w kierunku ustalonym w MoveToPlayer
        rb.velocity = moveDirection * chaseSpeed;
        
    }


    // Aktualizuje kierunek w którym porusza siê przeciwnik, ruch dodawany w FixedUpdate
    // Wykonywane co 0.5 sekundy, ¿eby zapobiec dr¿eniu przeciwnika w przypadku, gdy odleg³oœæ
    //  w osi X i Y, by³a bardzo podobna i kierunek w którym porusza³ siê przeciwnik zmienia³ siê co klatkê
    IEnumerator MoveToPlayer()
    {
        playerPos = player.transform.position;
        Vector2 mechPos = transform.position;

        float distanceX = Mathf.Abs(playerPos.x - mechPos.x);
        float distanceY = Mathf.Abs(playerPos.y - mechPos.y);
        float distance = Vector2.Distance(playerPos, mechPos);

        // Gracz jest  dalej na osi X ni¿ na osi Y
        if (distanceX >= distanceY || YBlocked)
        {
            // Je¿eli gracz jest poza zasiêgiem przeciwnika, przeciwnik goni gracza
            if (distance > (AttackDistance + gameObject.GetComponent<Collider2D>().bounds.size.x) )
            {
                enemy.CanAttack = false;
                moveDirection = (playerPos.x - mechPos.x > 0) ? transform.right : -transform.right;

                Vector2 pos = (Vector2)transform.position + GetComponent<Collider2D>().bounds.size.x * moveDirection;
                Vector2 Size = new Vector2( GetComponent<Collider2D>().bounds.size.x / 2.1f, GetComponent<Collider2D>().bounds.size.y * 0.99f);
                Collider2D col = Physics2D.OverlapCapsule(pos, Size, CapsuleDirection2D.Vertical,0f);
                if(col != null && !col.isTrigger)
                {
                    XBlocked = true;
                }
                else
                {
                    XBlocked= false;
                }
            }
            // Gdy gracz jest w zasiêgu ataku przeciwnika:
            //  przeciwnik zatrzymuje siê a, skrypt Enemy jest powiadamiany o mo¿liwoœci ataku 
            else
            {
                moveDirection = Vector2.zero;
                enemy.CanAttack = true;
            }
        }
        // Gracz jest dalej na osi Y ni¿ na osi X
        if (distanceY >= distanceX || XBlocked)
        {
            // Je¿eli gracz jest poza zasiêgiem przeciwnika, przeciwnik goni gracza
            if (distance > (AttackDistance + gameObject.GetComponent<Collider2D>().bounds.size.y))
            {
                enemy.CanAttack = false;

                // kierunek najpierw przypisywany do temp Direction, jeœli kierunek nie jest zablokowany przypisuje go do moveDirection
                Vector2 tempDirection  = (playerPos.y - mechPos.y > 0) ? transform.up : -transform.up;

                Vector2 pos = (Vector2)transform.position + GetComponent<Collider2D>().bounds.size.y * tempDirection;
                Vector2 Size = new Vector2(GetComponent<Collider2D>().bounds.size.x * 0.99f, GetComponent<Collider2D>().bounds.size.y / 2.1f);
                Collider2D col = Physics2D.OverlapCapsule(pos, Size, CapsuleDirection2D.Vertical, 0f);
                if (col != null && !col.isTrigger)
                {
                    YBlocked = true;
                }
                else
                {
                    moveDirection = tempDirection;
                    YBlocked = false;
                }
            }
            // Gdy gracz jest w zasiêgu ataku przeciwnika:
            //  przeciwnik zatrzymuje siê a, skrypt Enemy jest powiadamiany o mo¿liwoœci ataku 
            else
            {
                moveDirection = Vector2.zero;
                enemy.CanAttack = true;
            }
        }


        //kierunek w którym patrzy przeciwnik, u¿ywane w Detection
        enemy.FacingDirection = moveDirection == Vector2.zero ? Vector2.down : moveDirection;

        

        yield return new WaitForSeconds((UnityEngine.Random.value / 2) + 0.3f);
        StartCoroutine(MoveToPlayer());

    }

    public void Disable(float time)
    {
        enemy.FreezeTime += time;
    }

    private void Update()
    {
        chaseSpeed = enemy.chaseSpeed;
    }


}
