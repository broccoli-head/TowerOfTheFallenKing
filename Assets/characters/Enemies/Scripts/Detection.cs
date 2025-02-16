using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Enemy))]
public class Detection : MonoBehaviour
{
    Enemy enemy;
    float FoV;
    float VDistance;
    float HDistance;
    float MemoryTime;
    float MemoryTimer = 0;
    bool WasDetected = false;
    
    [ReadOnly] public  bool isDetected;
    [ReadOnly] public float Distance;
    [ReadOnly] public float Angle;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        FoV = enemy.FieldOfViev;
        VDistance = enemy.VievDistance;
        HDistance = enemy.HearingDistance;
        MemoryTime = enemy.MemoryTime;
    }

    void Update()
    {
        // na pocz¹tku klatki przyjmójemy ¿e przeciwnik nie widzi gracza
        isDetected = false;

        //Sluch - na podstawie odleg³oœci
        foreach(Collider2D item in Physics2D.OverlapCircleAll(transform.position, HDistance) )
        {
            if (item.CompareTag("Player"))
            {
                // jeœli gracz jest wystarczaj¹co blisko: skrypt Enemy jest powiadamiany o zauwa¿eniu gracza
                // zmienne odpowiedzialne za to czy gracz jest widziany teraz i czy by³ zauwa¿ony wczeœniej s¹ ustawiane na true
                // czas w którym przeciwnik pamiêta o graczu, ale go nie widzi ustawiany na 0
                enemy.PlayerDetected = true;
                isDetected = true;
                WasDetected = true;
                MemoryTimer = 0;
                break;
            } 
        }
        //Wzrok na podstawie odleg³oœci i k¹ta miêdzy graczem a kierunkiem w którym patrzy przeciwnik
        Vector2 Direction = enemy.FacingDirection;
        Distance = Vector2.Distance(enemy.Player.transform.position, transform.position);
        Vector2 PlayerDirection = enemy.Player.transform.position - transform.position;
        Angle = Vector2.Angle(PlayerDirection, Direction);

        //sprawdza czy gracz jest w polu widzenia
        if (Distance <= VDistance && Angle <= (FoV / 2))
        {
            // przyjmójemy ¿e poziom oczu przeciwnika jest lekko pod górn¹ krawêdzi¹ obiektu
            float Eye = gameObject.GetComponent<Collider2D>().bounds.size.y * 0.45f;
            Vector3 EyePosition = transform.position + new Vector3(0,Eye,0);

            //lekko zmniejszamy wzrost gracza ¿eby mieæ pewnoœæ ¿e promieñ nie przeleci obok
            float PlayerSizeY = enemy.Player.GetComponent<Collider2D>().bounds.size.y * 0.99f; 

            Vector2[] Directions =
            {
                //kierunek od oczu przeciwnika do górnej krawêdzi gracza
                (enemy.Player.transform.position + new Vector3(0,PlayerSizeY / 2,0)) - EyePosition,
                //kierunek od oczu przeciwnika do œrodka gracza
                enemy.Player.transform.position - EyePosition,
                //kierunek od oczu przeciwnika do dolnej krawêdzi gracza
                (enemy.Player.transform.position - new Vector3(0,PlayerSizeY / 2,0)) - EyePosition,
            };

            //wystrzeliwuje promienie w kierunku gracza ¿eby sprawdziæ czy nic go nie zas³ania
            foreach (var direction in Directions)
            {
                RaycastHit2D[] rayAll = Physics2D.RaycastAll(EyePosition, direction.normalized, VDistance);
                Debug.DrawRay(EyePosition, direction);
                foreach (var ray in rayAll)
                {
                    // przechodzi do nastêpnego trafionego obiektu jeœli promieñ trafi³ w obiekt z którego jest wystrzeliwany
                    if (ray.collider.gameObject == this.gameObject)
                    {
                        continue;
                    }
                    //promieñ trafia w gracza - oznacza to, ¿e gracza nic nie zas³ania i przeciwnik go widzi
                    if (ray.collider.CompareTag("Player"))
                    {
                        // jeœli gracz zosta³ zauwa¿ony: skrypt Enemy jest powiadamiany o zauwa¿eniu gracza
                        // zmienne odpowiedzialne za to czy gracz jest widziany teraz i czy by³ zauwa¿ony wczeœniej s¹ ustawiane na true
                        // czas w którym przeciwnik pamiêta o graczu, ale go nie widzi ustawiany na 0
                        enemy.PlayerDetected = true;
                        isDetected = true;
                        WasDetected = true;
                        MemoryTimer = 0;
                    }
                    else
                    {
                        //przerywa gdy promieñ trafi³ w coœ innego ni¿ gracz (gracz jest zas³oniêty wiêc przeciwnik go nie widzi)
                        break;
                    }
                        
                }
            }


        }
        

        // gdy przeciwnik nie widzi gracza ale widzia³ go wczeœniej
        if(!isDetected && WasDetected)
        {
            // zwiêkszamy czas w którym przeciwnik pamiêta o graczu
            MemoryTimer += Time.deltaTime;

            // jeœli czas pamiêci przekracza maksymalny czas pamiêci przeciwnika
            if(MemoryTimer > MemoryTime) 
            {
                // zmienna odpowiedzialna za to czy gracz by³ widoczny ustawiana na false
                // skrypt Enemy powiadamiany o tym ¿e przeciwnik nie widzi gracza
                // czas w którym przeciwnik pamiêta o graczu przywracany do 0
                WasDetected = false;
                enemy.PlayerDetected = false;
                MemoryTimer = 0;
            }
        }

    }
}
