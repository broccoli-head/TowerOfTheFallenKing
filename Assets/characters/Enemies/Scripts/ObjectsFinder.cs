using UnityEngine;
using System.Linq;

class ObjectsFinder
{
    public static GameObject FindInactiveObjects(string tag)
    {
        //szuka obiektów z podanym tagiem
        GameObject obj = Resources.FindObjectsOfTypeAll<Transform>().FirstOrDefault(
            transform => transform.CompareTag(tag)
        )?.gameObject;

        //?.gameObject zwraca null jesli gameobject nie istnieje
        return obj;
    }
};