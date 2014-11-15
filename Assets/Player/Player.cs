using UnityEngine;
using System.Collections;


public class Player : MonoBehaviour
{
    public Color color;
    public string pname = "Unnamed";
    public bool human = true;
    

    protected virtual void Start()
    {
        if (name.Length > 0)
            pname = name;

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }
    protected virtual void OnGUI()
    {

    }
    protected virtual void Awake()
    {

    }
}
