using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.FullSerializer;

public class LevelLoader : MonoBehaviour, Saveable
{
    public GameObject PlayerPrefab;

    public Level LevelZero;
    public string FinalLevel;

    public Level ActualLevel { get; private set; }

    private List<int> path = new List<int>();
    private string filePath;
    public bool LevelChangeCoolDown = false;

    public static LevelLoader Instance { get; private set; }
    public static List<string> CleanedRooms = new List<string>();


    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "path.json");

        // jesli istnieje juz jakis levelLoader usuwamu nasz game object
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // jesli nie ma innego obiektu ustawiamy swoj obiekt jako glowny
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        // laduje sciezke leveli
        Load();

        // Ustawia poprzedni level we wszystkich levelach
        LevelZero.SetPreviousLevel(null);
    }

    // Do wywolania w mainMenu, laduje pierwszy poziom
    public void StartGame()
    {
        if (LevelChangeCoolDown)
            return;

        LevelChangeCoolDown = true;
        StartCoroutine(LevelCoolDown());
        ActualLevel = LevelZero;
        Level.Load(ActualLevel.LevelName);
    }

    public void ContinueGame()
    {
        if(path == null || path.Count == 0)
        {
            StartGame();
            return;
        }

        Level levelToLoad = LevelZero;
        // przechodzi po sciezce leveli i laduje ostatni level
        foreach (int i in path) {
            // jesli index w sciezce jest prawidlowy ustawia level do zaladowania na nastepny level
            if (levelToLoad.NextLevels != null && i < levelToLoad.NextLevels.Length && i >= 0)
                levelToLoad = levelToLoad.NextLevels[i];

            // jesli zapisany index jest nieprawidlowy
            else
            {
                Debug.Log($"The save file at path: {filePath} contains invalid or corrupted data");
                StartGame();
                //przerywa petle i zwraca funkcje
                return;
            }
                
        }
        ActualLevel = levelToLoad;
        Level.Load(levelToLoad.LevelName);

    }

    public void NextLevel(int i = 0)
    {
        if (LevelChangeCoolDown)
            return;

        // jesli nie ma nastepnych leveli
        if (ActualLevel.NextLevels == null || ActualLevel.NextLevels.Length == 0)
        {
            // tworzenie obiektu klasy level dla koncowego levelu
            Level lvl = new Level();
            lvl.LevelName = FinalLevel;
            lvl.SetPreviousLevel(ActualLevel);

            // zmiana aktualnego levelu na koncowy level i jego ladowanie
            ActualLevel = lvl;
            Level.Load(ActualLevel.LevelName);
            return;
        }

        // jesli index jest nieprawidlowy ustawia go na 0
        if (i < 0 || i >= ActualLevel.NextLevels.Length)
            i = 0;

        // dodawanie levelu do path
        path.Add(i);
        
        // Ustawianie aktualnego levelu na nastepny level
        ActualLevel = ActualLevel.NextLevels[i];
        // ladowanie nastepnego levelu
        Level.Load(ActualLevel.LevelName);

        //Zapobiega zbyt szybkiej zmianie leveli
        LevelChangeCoolDown = true;
        StartCoroutine(LevelCoolDown());

    }

    public void PrevLevel()
    {
        if (LevelChangeCoolDown)
            return;

        // jesli poprzedni level istnieje
        if (ActualLevel.PreviousLevel != null)
        {
            //usuwa ostatni level z path
            if (path != null && path.Count != 0)
                path.RemoveAt(path.Count - 1);

            // ustawia aktualny level na poprzedni i laduje poprzedni level
            ActualLevel = ActualLevel.PreviousLevel;
            Level.Load(ActualLevel.LevelName);

            //Zapobiega zbyt szybkiej zmianie leveli
            LevelChangeCoolDown = true;
            StartCoroutine(LevelCoolDown());
        }
        else return;
    }

    public void SideLevel(int i = 0)
    {
        if (LevelChangeCoolDown)
            return;

        // jesli nie ma pobocznych leveli
        if (ActualLevel.SideLevels == null || ActualLevel.SideLevels.Length == 0)
        {
            Debug.LogWarning("No side levels available.");
            return;
        }

        // gdy index jest nieprawidlowy ustawia go na 0
        if (i < 0 || ActualLevel.SideLevels.Length <= i)
            i = 0;
        
        // Laduje poboczny level
        Level.Load(ActualLevel.SideLevels[i]);

        //Zapobiega zbyt szybkiej zmianie leveli
        LevelChangeCoolDown = true;
        StartCoroutine(LevelCoolDown());
    }

    public void MainLevel()
    {
        if (LevelChangeCoolDown)
            return;

        // Laduje glowny level
        Level.Load(ActualLevel.LevelName);

        //Zapobiega zbyt szybkiej zmianie leveli
        LevelChangeCoolDown = true;
        StartCoroutine(LevelCoolDown());
    }


    public void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(new ListWrapper<int> { List = path });
            File.WriteAllText(filePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save path to file: {filePath}. Error: {e.Message}");
        }
    }

    public void Load()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                path = JsonUtility.FromJson<ListWrapper<int>>(json).List;
            }
            else
            {
                path = new List<int>();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load path from file: {filePath}. Error: {e.Message}");
            path = new List<int>();
        }
    }


    private void OnValidate()
    {
        if (LevelZero == null)
            Debug.LogWarning("LevelZero is not assigned.");
        if (string.IsNullOrEmpty(FinalLevel))
            Debug.LogWarning("FinalLevel is not assigned.");
        if (PlayerPrefab == null)
        {
            Debug.LogError("PlayerPrefab is not assigned!");
        }
    }

    public IEnumerator LevelCoolDown()
    {
        yield return new WaitForSeconds(1.2f);
        LevelChangeCoolDown = false;
        yield break;
    }


}


[System.Serializable]

public class Level
{
    public string LevelName;

    [HideInInspector]
    public Level PreviousLevel;

    public string[] SideLevels = { };

    public Level[] NextLevels = { };

    public static string ComingFromRoom = "";
    public static string CurrentlyOnRoom = "Default";


    // Ustawia Poprzedni level na podany level i poprzedni level w nastepnych levelach na ten level
    public void SetPreviousLevel(Level level)
    {
        PreviousLevel = level;

        foreach (var item in NextLevels)
        {
            item.SetPreviousLevel(this);
        }
    }

    public static void Load(string name)
    {
        Debug.Log("£aduje " + name);
        // Zapisuje przed zaladowaniem levelu
        SaveManager.Save();
        try
        {
            ComingFromRoom = CurrentlyOnRoom;
            CurrentlyOnRoom = name;

            SceneTransition sceneTransition = GameObject.FindObjectOfType<SceneTransition>();
            if (sceneTransition != null)
                sceneTransition.StartCoroutine(sceneTransition.SwitchScenes(name));
            else
                SceneManager.LoadScene(name);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load scene: {name}. Error: {e.Message}");
        }

    }

}