using UnityEngine;
using System.Collections.Generic;

namespace Defs
{
    //Generally a storage of parameters for various objects
    public class Def : MonoBehaviour
    {
        //protected Dictionary<string, Texture2D> text2D;
        public new string name = "unnamed_def";

        public Def(string name)
        {
            //text2D = new Dictionary<string,Texture2D>();

            DefStorage.registerDef(this);
        }
        public Def()
        {
            //ERROR
            //Debug.LogWarning("Unnamed def created!");
        }


        private static bool storInitiated = false;

        protected virtual void Start()
        {
            if (!storInitiated)
            {
                DefStorage.init(gameObject);
                storInitiated = true;
            }
        }
        /*public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }*/
    }
}