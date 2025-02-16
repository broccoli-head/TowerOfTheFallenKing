using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    public static void Save()
    {
        List<Saveable> saveable = ComponentHelper.FindObjectsOfInterface<Saveable>();
        foreach (Saveable obj in saveable) 
        {
            obj.Save();
        }
    }

    public static void SaveVar<T>(string name, T value)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{name}.json");
        try
        {
            VarWrapper<T> Var = new VarWrapper<T>() { Value = value};
            string json = JsonUtility.ToJson(Var);
            File.WriteAllText(filePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save Var: {name} to file: {filePath}. Error: {e.Message}");
        }
    }


    public static T LoadVar<T>(string name)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{name}.json");
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<VarWrapper<T>>(json).Value;
            }
            else
            {
                throw new Exception();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load {name} from file: {filePath}. Error: {e.Message}");
        }

        return default(T);
    }
}

public interface Saveable
{
    void Save();
}

[System.Serializable]
public class ListWrapper<T>
{
    public List<T> List;
}

public class VarWrapper<T>
{
    public T Value;
}
