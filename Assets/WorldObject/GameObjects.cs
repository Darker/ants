using UnityEngine;
using System.Collections;
using RTS;
using Units;

public class GameObjects : MonoBehaviour {
    public static bool created = false;

    public GameObject[] units;

    void Start()
    {

    }
    void Update()
    {

    }

    public GameObject GetObject(string name)
    {
        WorldObject unit;
        int l = units.Length;
        for (int i = 0; i < l; i++)
        {
            unit = units[i].GetComponent<WorldObject>();
            if (unit && unit.name == name)
             return units[i];
        }
        return null;
    }

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(transform.gameObject);
            ResourceManager.SetGameObjectList(this);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
