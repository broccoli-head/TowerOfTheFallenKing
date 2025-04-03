using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        // sprawdzamy czy gracz jest w zasiêgu ataku
        float distance = Vector2.Distance(enemy.Player.transform.position, transform.position);
        if (distance <= enemy.MeleeStats.AttackDistance)
        {
            enemy.MeleeStats.InRange = true;
        }
        else
        {
            enemy.MeleeStats.InRange = false;
        }



        // na poczatku klatki przyjmojemy ze przeciwnik nie widzi gracza
        isDetected = false;

        //Sluch - na podstawie odleglosci
        foreach(Collider2D item in Physics2D.OverlapCircleAll(transform.position, HDistance) )
        {
            if (item.CompareTag("Player"))
            {
                // jesli gracz jest wystarczajaco blisko: skrypt Enemy jest powiadamiany o zauwazeniu gracza
                // zmienne odpowiedzialne za to czy gracz jest widziany teraz i czy byl zauwazony wczesniej sa ustawiane na true
                // czas w ktorym przeciwnik pamieta o graczu, ale go nie widzi ustawiany na 0
                enemy.PlayerDetected = true;
                isDetected = true;
                WasDetected = true;
                MemoryTimer = 0;
                break;
            } 
        }
        //Wzrok na podstawie odleglosci i kata miedzy graczem a kierunkiem w ktorym patrzy przeciwnik
        Vector2 Direction = enemy.FacingDirection;
        Distance = Vector2.Distance(enemy.Player.transform.position, transform.position);
        Vector2 PlayerDirection = enemy.Player.transform.position - transform.position;
        Angle = Vector2.Angle(PlayerDirection, Direction);

        //sprawdza czy gracz jest w polu widzenia
        if (Distance <= VDistance && Angle <= (FoV / 2))
        {
            // przyjmojemy ze poziom oczu przeciwnika jest lekko pod gorna krawedzia obiektu
            float Eye = gameObject.GetComponent<Collider2D>().bounds.size.y * 0.45f;
            Vector3 EyePosition = transform.position + new Vector3(0,Eye,0);

            //lekko zmniejszamy wzrost gracza zeby miec pewnosc ze promien nie przeleci obok
            float PlayerSizeY = enemy.Player.GetComponent<Collider2D>().bounds.size.y * 0.99f; 

            Vector2[] Directions =
            {
                //kierunek od oczu przeciwnika do gornej krawedzi gracza
                (enemy.Player.transform.position + new Vector3(0,PlayerSizeY / 2,0)) - EyePosition,
                //kierunek od oczu przeciwnika do srodka gracza
                enemy.Player.transform.position - EyePosition,
                //kierunek od oczu przeciwnika do dolnej krawedzi gracza
                (enemy.Player.transform.position - new Vector3(0,PlayerSizeY / 2,0)) - EyePosition,
            };

            //wystrzeliwuje promienie w kierunku gracza zeby sprawdzic czy nic go nie zaslania
            foreach (var direction in Directions)
            {
                RaycastHit2D[] rayAll = Physics2D.RaycastAll(EyePosition, direction.normalized, VDistance);
                Debug.DrawRay(EyePosition, direction);
                foreach (var ray in rayAll)
                {
                    // przechodzi do nastepnego trafionego obiektu jesli promien trafil w obiekt z ktorego jest wystrzeliwany
                    if (ray.collider.gameObject == this.gameObject)
                    {
                        continue;
                    }
                    //promien trafia w gracza - oznacza to, ze gracza nic nie zaslania i przeciwnik go widzi
                    if (ray.collider.CompareTag("Player"))
                    {
                        // jesli gracz zostal zauwazony: skrypt Enemy jest powiadamiany o zauwazeniu gracza
                        // zmienne odpowiedzialne za to czy gracz jest widziany teraz i czy byl zauwazony wczesniej sa ustawiane na true
                        // czas w ktorym przeciwnik pamieta o graczu, ale go nie widzi ustawiany na 0
                        enemy.PlayerDetected = true;
                        isDetected = true;
                        WasDetected = true;
                        MemoryTimer = 0;
                    }
                    else
                    {
                        //przerywa gdy promien trafil w cos innego niz gracz (gracz jest zasloniety wiec przeciwnik go nie widzi)
                        break;
                    }
                        
                }
            }


        }
        

        // gdy przeciwnik nie widzi gracza ale widzial go wczesniej
        if(!isDetected && WasDetected)
        {
            // zwiekszamy czas w ktorym przeciwnik pamieta o graczu
            MemoryTimer += Time.deltaTime;

            // jesli czas pamieci przekracza maksymalny czas pamieci przeciwnika
            if(MemoryTimer > MemoryTime) 
            {
                // zmienna odpowiedzialna za to czy gracz byl widoczny ustawiana na false
                // skrypt Enemy powiadamiany o tym ze przeciwnik nie widzi gracza
                // czas w ktorym przeciwnik pamieta o graczu przywracany do 0
                WasDetected = false;
                enemy.PlayerDetected = false;
                MemoryTimer = 0;
            }
        }

    }
}
