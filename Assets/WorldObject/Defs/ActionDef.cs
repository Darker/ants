using UnityEngine;
using System.Collections;
namespace Defs
{
    public class ActionDef : Def
    {
        public string HoverCursor = "Textures/cursors/select";
        public string MenuIcon = "Textures/GUI/build/ant";

        public string description = "Blank action. Doesn't do anything";
        public string hotkey = "";
        public string title = "Nothing";
        public float  cooldown = 0;
        public float  warmup = 0;
        //The action can be canceled during progress (replaced by different action)
        public bool cancelable = true;
        //The action happens instantly, without given position or target
        public bool instant = false;

        public Texture2D cursor;


    }
}