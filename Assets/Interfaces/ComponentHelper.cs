using System.Collections.Generic;
using UnityEngine;

public class ComponentHelper : MonoBehaviour
{
    public static T GetInterfaceComponent<T>(GameObject obj) where T : class
    {
        T component = null;
        Component[] components = obj.GetComponents<Component>();

        foreach (var comp in components)
        {
            if (comp is T)
            {
                component = comp as T;
                break;
            }
        }

        return component;
    }



    public static List<T> FindObjectsOfInterface<T>() where T : class
    {
        List<T> results = new List<T>();

        // Znajdz wszystkie komponenty MonoBehaviour w scenie
        MonoBehaviour[] allObjects = GameObject.FindObjectsOfType<MonoBehaviour>();

        // Sprawdz, ktore implementuja interfejs T
        foreach (var obj in allObjects)
        {
            if (obj is T t)
            {
                results.Add(t);
            }
        }

        return results;
    }


    public static Texture2D TextureFromSprite(Sprite sprite)
    {
        if(sprite == null)
            return null;

        // Get the texture and the rect corresponding to the sprite
        Texture2D texture = sprite.texture;
        Rect rect = sprite.textureRect;

        // Check if the texture is readable
        if (!texture.isReadable)
        {
            Debug.LogError("Tekstura sprite'a nie jest ustawiona jako Read/Write Enabled w ustawieniach importu.");
            return null;
        }

        try
        {
            // Extract pixels from the specified rect area
            Color[] pixels = texture.GetPixels(
                Mathf.FloorToInt(rect.x),
                Mathf.FloorToInt(rect.y),
                Mathf.FloorToInt(rect.width),
                Mathf.FloorToInt(rect.height)
            );

            // Create a new texture with extracted pixels
            Texture2D newTexture = new Texture2D(
                Mathf.FloorToInt(rect.width),
                Mathf.FloorToInt(rect.height),
                TextureFormat.RGBA32, // Ensure RGBA32 format
                false // No mipmaps
            );

            // Set pixels and apply changes
            newTexture.SetPixels(pixels);
            newTexture.Apply();

            // Set texture settings
            newTexture.wrapMode = TextureWrapMode.Clamp;
            newTexture.filterMode = FilterMode.Bilinear;

            return newTexture;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Blad podczas ekstrakcji tekstury ze sprite'a: " + e.ToString());
            return null;
        }
    }

    public static Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;

        Graphics.Blit(source, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D result = new Texture2D(newWidth, newHeight);
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        result.filterMode = FilterMode.Point;
        result.wrapMode = TextureWrapMode.Clamp;

        return result;
    }



}