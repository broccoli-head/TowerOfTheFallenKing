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
    [ReadOnly] public bool YBlocked = false;

    [ReadOnly] public byte XCounter = 0;
    private byte MaxMovesInOneAxis = 3;

    AudioSource audioSource;
    AudioClip footstepsSound;
    GameObject pauseMenu;
    GameObject inventory;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GetComponent<Enemy>();
        chaseSpeed = enemy.chaseSpeed;
        AttackDistance = enemy.MeleeStats.AttackDistance;

        audioSource = GetComponent<AudioSource>();
        footstepsSound = enemy.footstepsSound;
        pauseMenu = ObjectsFinder.FindInactiveObjects("Menu");
        inventory = ObjectsFinder.FindInactiveObjects("Inventory");


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


    void Update()
    {
        chaseSpeed = enemy.chaseSpeed;

        if (audioSource != null)
        {
            //pu�� d�wi�k chodzenia, je�eli przeciwnik si� porusza
            if (rb.velocity.magnitude > 0.1f && !pauseMenu.activeSelf && !inventory.activeSelf)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = footstepsSound;
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }
        }
    }


    void FixedUpdate()
    {
        // Jezeli przeciwnik jest "zamrozony" zmniejsza czas pozostaly do odmrozenia i przerywa wykonywanie funkcji
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


    // Aktualizuje kierunek w ktorym porusza sie przeciwnik, ruch dodawany w FixedUpdate
    // Wykonywane co 0.5 sekundy, zeby zapobiec drzeniu przeciwnika w przypadku, gdy odleglosc
    //  w osi X i Y, byla bardzo podobna i kierunek w ktorym poruszal sie przeciwnik zmienial sie co klatke
    IEnumerator MoveToPlayer()
    {
        playerPos = player.transform.position;
        Vector2 mechPos = transform.position;

        float distanceX = Mathf.Abs(playerPos.x - mechPos.x);
        float distanceY = Mathf.Abs(playerPos.y - mechPos.y);
        float distance = Vector2.Distance(playerPos, mechPos);

        // Gracz jest  dalej na osi X niz na osi Y
        if ((distanceX >= distanceY && XCounter <= MaxMovesInOneAxis) || YBlocked)
        {
            // Jezeli gracz jest poza zasiegiem przeciwnika, przeciwnik goni gracza
            if (distance > AttackDistance)
            {
                XCounter++;

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
            // Gdy gracz jest w zasiegu ataku, przeciwnik zatrzymuje sie 
            else
            {
                moveDirection = Vector2.zero;
            }
        }
        // Gracz jest dalej na osi Y niz na osi X
        if (distanceY >= distanceX || XBlocked || XCounter > MaxMovesInOneAxis)
        {
            // Jezeli gracz jest poza zasiegiem przeciwnika, przeciwnik goni gracza
            if (distance > (AttackDistance + gameObject.GetComponent<Collider2D>().bounds.size.y))
            {
                XCounter = 0;

                // kierunek najpierw przypisywany do temp Direction, jesli kierunek nie jest zablokowany przypisuje go do moveDirection
                Vector2 tempDirection  = (playerPos.y - mechPos.y > 0) ? transform.up : -transform.up;

                Vector2 pos = (Vector2)transform.position + GetComponent<Collider2D>().bounds.size.y * tempDirection;
                Vector2 Size = new Vector2(GetComponent<Collider2D>().bounds.size.x * 0.99f, GetComponent<Collider2D>().bounds.size.y / 2.1f);
                Collider2D col = Physics2D.OverlapCapsule(pos, Size, CapsuleDirection2D.Vertical, 0f);
                if (col != null && !col.isTrigger && col.gameObject != this.gameObject)
                {
                    YBlocked = true;
                }
                else
                {
                    moveDirection = tempDirection;
                    YBlocked = false;
                }
            }
            // Gdy gracz jest w zasiegu ataku, przeciwnik zatrzymuje sie 
            else
            {
                moveDirection = Vector2.zero;
            }
        }


        //kierunek w ktorym patrzy przeciwnik, uzywane w Detection
        enemy.FacingDirection = moveDirection == Vector2.zero ? Vector2.down : moveDirection;

        

        yield return new WaitForSeconds((UnityEngine.Random.value / 2) + 0.3f);
        StartCoroutine(MoveToPlayer());

    }

    public void Disable(float time)
    {
        enemy.FreezeTime += time;
    }

}
