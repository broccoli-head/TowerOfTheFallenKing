using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerLive : MonoBehaviour, ReciveDamage, Saveable
{
    // Zmienna okreslajaca, czy stan postaci ma byc zaladowany przy starcie.
    public bool load = true; 
    // Aktualne punkty zycia gracza.
    public float HitPoints;
    // czas po kt�oym zaczyna si� regeneracja hp
    public float HealthRegenerationDelay = 10;
    // predkosc regeneracji hp
    public float HealthRegenerationSpeed = 5;
    [ReadOnly] public float NoDamageTakenTimer;

    // Poczatkowe punkty zycia gracza.
    [ReadOnly] public float StartHP;


    // Zmienna przechowujaca obrazenia zadawane przez okreslony czas.
    private float damage = 0;
    // Lista efektow wizualnych zwiazanych z obrazeniami.
    List<GameObject> effects = new List<GameObject>();


    // Flaga mowiaca, czy gracz jest "wyeksponowany" (zwiekszone obrazenia).
    private bool isExposed = false;
    // Mnoznik obrazen spowodowany efektem "exposed".
    private float Exposition;
    // Przechowuje instancje klasy odpowiedzialnej za zarzadzanie efektem "exposed".
    ExposedEffect exposed;
    


    // Flaga mowiaca, czy gracz jest pod wplywem efektu "cleanse".
    private bool cleanse = false;

    // Flaga mówiąca, czy gracz jest czerwony
    private bool isRed = false;

    private SpriteRenderer spriteRenderer;
    private Color spriteColor;

    public AudioSource audioSource;
    public static PlayerLive Instance;

    private Aim aimCircle;


    void Start()
    {
        Instance = this;

        if (load)
            // laduje zapisany stan gracza, jesli load jest true.
            Load(); 
        else
            // Inicjalizuje StartHP, jesli nie ladowane sa dane.
            StartHP = HitPoints; 

        // Tworzy nowa instancje do obslugi ExposedEffect.
        exposed = new ExposedEffect(this);

        aimCircle = GetComponentInChildren<Aim>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color; //pobiera obecny kolor, najczesciej jest to #ffffff 
    }



    void Update()
    {
        if(HitPoints < StartHP)
            NoDamageTakenTimer += Time.deltaTime;

        // Zapobiega, by obrazenia nie byly ujemne.
        if (damage < 0)
            damage = 0;

        // Stosuje obrazenia w czasie (DPS).
        if(damage > 0)
        {
            Damage(damage * Time.deltaTime);
        }
            

        if (HitPoints <= 0)
        {
            if (FindObjectOfType<MusicPlayer>())
            {
                GameObject musicPlayer = FindObjectOfType<MusicPlayer>().gameObject;

                if (musicPlayer != null)
                    Destroy(musicPlayer);
            }

            if (audioSource != null)
                Destroy(audioSource);

            aimCircle.gameObject.SetActive(false);

            GameObject deathScreen = ObjectsFinder.FindInactiveObjects("DeathScreen");
            deathScreen.SetActive(true);

            LevelLoader.CleanedRooms.Clear();
        }

        if(NoDamageTakenTimer >= HealthRegenerationDelay && HitPoints < StartHP)
        {
            HitPoints += HealthRegenerationSpeed * Time.deltaTime;
            if(HitPoints > StartHP)
                HitPoints = StartHP;
        }
    }



    // Przeladowana metoda Damage z dodatkowymi parametrami
    public void Damage(float damage, Potion.DamageType DamageType, Potion.DamagePlace DamagePlace, bool EnemyOnly)
    {
        // Sprawdza czy powinnismy otrzymac obrazenia 
        if (DamageType != Potion.DamageType.None && !EnemyOnly && this.isActiveAndEnabled)
        {
            if (!cleanse)
            {
                // Jesli gracz nie uzyl "cleanse", zadaje obrazenia.
                Damage(damage);
            }
            else
            {
                // Jesli uzyto "cleanse", resetuje efekt.
                cleanse = false;
            }
        }
    }



    // Zadaje obrazenia w oparciu o DPS, czas trwania i efekt wizualny
    public void Damage(float DPS, float Time, Potion.DamageType DamageType, Potion.DamagePlace DamagePlace, GameObject EffectObject, bool EnemyOnly)
    {
        if (DamageType != Potion.DamageType.None && !EnemyOnly && this.isActiveAndEnabled)
        {
            if (!cleanse)
            {
                // Dodaje DPS do otrzymywanych obrazen.
                damage += DPS;

                // Spawnuje efekt wizualny.
                var effect = Instantiate(EffectObject, transform);
                
                // Dodaje efekt do listy.
                effects.Add(effect);

                try
                {
                    // Uruchamia Coroutine do usuniecia efektu po czasie.
                    if(this.gameObject.activeInHierarchy)
                        StartCoroutine(endDamage(DPS, Time, effect));
                }
                catch
                {
                    // Obsluguje ewentualne bledy (np. jesli coroutine nie moze zostac uruchomione).
                }
            }
            else
                // Resetuje "cleanse" jesli powinnismy otrzymac damage, ale byl on aktywny.
                cleanse = false; 
        }

    }

    

    // Zadaje obrazenia graczowi z uwzglednieniem ewentualnego efektu "exposed".
    public void Damage(float dmg)
    {
        if (isExposed)
        {
            // Zwieksza obrazenia, jesli gracz jest "wyeksponowany".
            dmg *= 1 + (Exposition / 100f);

        }
        // Puszcza dzwiek obrazen
        if(audioSource != null)
            if (!audioSource.isPlaying && CommlinkOpener.checkVisibility())
                audioSource.PlayOneShot(audioSource.clip);

        NoDamageTakenTimer = 0;

        // Odejmowanie obrazen od punktow zycia.
        HitPoints -= dmg;
        StartCoroutine(Flash());
    }


    
    // Wywoluje efekt "Expose", jesli nie jest aktywne "cleanse".
    public void Expose(List<ExpositionData> ExpositionOverTime)
    {
        if (!cleanse)
            // Aktywuje efekt Expose.
            exposed.Expose(ExpositionOverTime); 
    }


    
    // Resetuje wszystkie efekty i obrazenia.
    public void AddCleanse()
    {
        foreach (var item in effects)
        {
            // Usuwa efekty wizualne zwiazane z obrazeniami.
            Destroy(item); 
        }

        // Resetuje otrzymywane obrazenia.
        damage = 0;

        // Resetuje efekt "Expose".
        exposed.ResetEffect();

        // Ustawia flage "cleanse" na true.
        cleanse = true;
    }

    public IEnumerator Flash()
    {
        if(this.isActiveAndEnabled)
        {
            if (isRed) yield break; //jesli juz jest czerwony to nie zmieniaj koloru

            //zmienia kolor sprita na czerwony
            spriteRenderer.color = new Color(1f, 0.6f, 0.6f);
            isRed = true;

            //po 0.3s powraca poprzedni kolor
            yield return new WaitForSeconds(0.3f);
            spriteRenderer.color = spriteColor;

            //czeka 0.3s przed kolejnym zmienieniem koloru
            yield return new WaitForSeconds(0.3f);
            isRed = false;
        }
    }


    // Coroutine usuwajaca efekt LeaveDamage.
    public IEnumerator endDamage(float damage, float time, GameObject effect)
    {
        if (this.isActiveAndEnabled)
        {
            StartCoroutine(Flash());

            // Czeka przez czas trwania efektu.
            yield return new WaitForSeconds(time);
            // Zmniejsza obrazenia leave damageu o obrazenia zakonczonego efektu.
            this.damage -= damage;

            // Usuwa efekt wizualny.
            Destroy(effect);

            yield break;
        } 
    }

    public void Heal(float HP)
    {
        HitPoints += HP;
        if (HitPoints > StartHP)
            HitPoints = StartHP;
    }


    
    // Zapisuje stan postaci (punkty zycia).
    public void Save()
    {
        SaveManager.SaveVar<float>("HitPoints", HitPoints);
        SaveManager.SaveVar<float>("StartHP", StartHP);
    }

    

    // laduje stan postaci (punkty zycia).
    public void Load()
    {
        HitPoints = SaveManager.LoadVar<float>("HitPoints");
        StartHP = SaveManager.LoadVar<float>("StartHP");
    }



    
    // Klasa odpowiedzialna za zarzadzanie efektem "exposed".
    private class ExposedEffect
    {
        // Lista przechowujaca zmiany mnoznika obrazen.
        private List<ExpositionData> ExpositionOverTime = new List<ExpositionData>(); 

        // Referencja do glownej klasy gracza.
        private PlayerLive super;

        // Referencja do uruchomionej coroutine.
        private Coroutine coroutine;

        

        public ExposedEffect(PlayerLive player)
        {
            super = player;
        }


        
        // Naklada efekt "Expose" na gracza.
        public void Expose(List<ExpositionData> ExpositionOverTime)
        {
            // Jesli nie mamy juz aktywnego efektu "exposed", naklada go.
            if (!super.isExposed)
            {
                this.ExpositionOverTime = ExpositionOverTime;
                super.isExposed = true;

                // Przerwanie poprzedniej coroutine, jesli juz zostala uruchomiona.
                if (coroutine != null)
                    super.StopCoroutine(coroutine);

                // Uruchomienie nowej coroutine dla efekty "exposed".
                coroutine = super.StartCoroutine(ChangeExposedForce());
            }
            // Jesli efekt juz jest aktywny, dodaje nowe elementy do listy.
            else
            {     
                this.ExpositionOverTime.AddRange(ExpositionOverTime);
            }
        }

        

        // Coroutine zmieniajaca mnoznik obrazen z listy.
        private IEnumerator ChangeExposedForce()
        {
            // Jesli lista jest pusta, efekt "exposed" jest usuwany.
            if (ExpositionOverTime.Count <= 0)
            {
                // Resetuje mnoznik.
                super.Exposition = 0f;

                // Ustawia flage exposed na false.
                super.isExposed = false; 

                yield break;
            }
            else
            {
                // Ustawia nowy mnoznik obrazen.
                super.Exposition = ExpositionOverTime[0].exposition;

                // Czeka przez okreslony czas, az mnoznik wygasnie.
                yield return new WaitForSeconds(ExpositionOverTime[0].time);

                // Usuwa aktualny mnoznik.
                ExpositionOverTime.RemoveAt(0);

                // Uruchamia coroutine dla nastepnego mnoznika.
                super.StartCoroutine(ChangeExposedForce());
            }
        }

        

        // Resetuje efekty "exposed" (czysci liste i zatrzymuje coroutine).
        public void ResetEffect()
        {
            // Czysci liste efektow.
            ExpositionOverTime.Clear();

            // Resetuje mnoznik obrazen.
            super.Exposition = 0f; 

            // Ustawia flage exposed na false.
            super.isExposed = false; 

            if (coroutine != null)
                // Zatrzymuje coroutine, jesli jest aktywna.
                super.StopCoroutine(coroutine); 
        }

    }

}
