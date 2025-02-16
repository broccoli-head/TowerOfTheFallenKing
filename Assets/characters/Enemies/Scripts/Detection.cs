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
        // na pocz�tku klatki przyjm�jemy �e przeciwnik nie widzi gracza
        isDetected = false;

        //Sluch - na podstawie odleg�o�ci
        foreach(Collider2D item in Physics2D.OverlapCircleAll(transform.position, HDistance) )
        {
            if (item.CompareTag("Player"))
            {
                // je�li gracz jest wystarczaj�co blisko: skrypt Enemy jest powiadamiany o zauwa�eniu gracza
                // zmienne odpowiedzialne za to czy gracz jest widziany teraz i czy by� zauwa�ony wcze�niej s� ustawiane na true
                // czas w kt�rym przeciwnik pami�ta o graczu, ale go nie widzi ustawiany na 0
                enemy.PlayerDetected = true;
                isDetected = true;
                WasDetected = true;
                MemoryTimer = 0;
                break;
            } 
        }
        //Wzrok na podstawie odleg�o�ci i k�ta mi�dzy graczem a kierunkiem w kt�rym patrzy przeciwnik
        Vector2 Direction = enemy.FacingDirection;
        Distance = Vector2.Distance(enemy.Player.transform.position, transform.position);
        Vector2 PlayerDirection = enemy.Player.transform.position - transform.position;
        Angle = Vector2.Angle(PlayerDirection, Direction);

        //sprawdza czy gracz jest w polu widzenia
        if (Distance <= VDistance && Angle <= (FoV / 2))
        {
            // przyjm�jemy �e poziom oczu przeciwnika jest lekko pod g�rn� kraw�dzi� obiektu
            float Eye = gameObject.GetComponent<Collider2D>().bounds.size.y * 0.45f;
            Vector3 EyePosition = transform.position + new Vector3(0,Eye,0);

            //lekko zmniejszamy wzrost gracza �eby mie� pewno�� �e promie� nie przeleci obok
            float PlayerSizeY = enemy.Player.GetComponent<Collider2D>().bounds.size.y * 0.99f; 

            Vector2[] Directions =
            {
                //kierunek od oczu przeciwnika do g�rnej kraw�dzi gracza
                (enemy.Player.transform.position + new Vector3(0,PlayerSizeY / 2,0)) - EyePosition,
                //kierunek od oczu przeciwnika do �rodka gracza
                enemy.Player.transform.position - EyePosition,
                //kierunek od oczu przeciwnika do dolnej kraw�dzi gracza
                (enemy.Player.transform.position - new Vector3(0,PlayerSizeY / 2,0)) - EyePosition,
            };

            //wystrzeliwuje promienie w kierunku gracza �eby sprawdzi� czy nic go nie zas�ania
            foreach (var direction in Directions)
            {
                RaycastHit2D[] rayAll = Physics2D.RaycastAll(EyePosition, direction.normalized, VDistance);
                Debug.DrawRay(EyePosition, direction);
                foreach (var ray in rayAll)
                {
                    // przechodzi do nast�pnego trafionego obiektu je�li promie� trafi� w obiekt z kt�rego jest wystrzeliwany
                    if (ray.collider.gameObject == this.gameObject)
                    {
                        continue;
                    }
                    //promie� trafia w gracza - oznacza to, �e gracza nic nie zas�ania i przeciwnik go widzi
                    if (ray.collider.CompareTag("Player"))
                    {
                        // je�li gracz zosta� zauwa�ony: skrypt Enemy jest powiadamiany o zauwa�eniu gracza
                        // zmienne odpowiedzialne za to czy gracz jest widziany teraz i czy by� zauwa�ony wcze�niej s� ustawiane na true
                        // czas w kt�rym przeciwnik pami�ta o graczu, ale go nie widzi ustawiany na 0
                        enemy.PlayerDetected = true;
                        isDetected = true;
                        WasDetected = true;
                        MemoryTimer = 0;
                    }
                    else
                    {
                        //przerywa gdy promie� trafi� w co� innego ni� gracz (gracz jest zas�oni�ty wi�c przeciwnik go nie widzi)
                        break;
                    }
                        
                }
            }


        }
        

        // gdy przeciwnik nie widzi gracza ale widzia� go wcze�niej
        if(!isDetected && WasDetected)
        {
            // zwi�kszamy czas w kt�rym przeciwnik pami�ta o graczu
            MemoryTimer += Time.deltaTime;

            // je�li czas pami�ci przekracza maksymalny czas pami�ci przeciwnika
            if(MemoryTimer > MemoryTime) 
            {
                // zmienna odpowiedzialna za to czy gracz by� widoczny ustawiana na false
                // skrypt Enemy powiadamiany o tym �e przeciwnik nie widzi gracza
                // czas w kt�rym przeciwnik pami�ta o graczu przywracany do 0
                WasDetected = false;
                enemy.PlayerDetected = false;
                MemoryTimer = 0;
            }
        }

    }
}
