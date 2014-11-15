using UnityEngine;
using System.Collections;
using RTS;
using Units;

public class User : MonoBehaviour
{

    public string username;
    public bool human;
    public HUD hud;
    //The player who's using this PC
    public Player player;

    private WorldObject _SelectedObject;
    public WorldObject SelectedObject {
        get {return _SelectedObject;}
        set {
            if(_SelectedObject!=null)
              _SelectedObject.selected = false;
            _SelectedObject = value;
            if (value != null)
            {
                value.selected = true;

            }
        }
   }

    // Use this for initialization
    void Start()
    {
        hud = GetComponentInChildren<HUD>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}