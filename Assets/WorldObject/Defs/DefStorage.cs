using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;


namespace Defs
{
    public static class DefStorage
    {
        private static Dictionary<string, Def> map = new Dictionary<string,Def>();
        //private static Def[] defs;
        //public static Dictionary<string, Def> storage { get { return stor; } }
        /*public static Def this[string i]
        {
            get { return getDef(i); }
            set { 
                value.name = i;
                registerDef(value);
            }
        }*/
        public static Def getDef(string name) {
            if (!map.ContainsKey(name))
            {
                Debug.LogError("Def not found! (" + name + ")");
                return null;
            }
            return map[name];
        }
        public static Def getDef(string name, Type defType)
        {
            if (!map.ContainsKey(name))
            {
                Debug.LogWarning("Creating empty def '"+name+"' to avoid errors!");
                //Create empty def

                //Get the info about constructor (using array literal)
                ConstructorInfo constructor = defType.GetConstructor(new Type[] { typeof(string)});
                //Initialise the Type instance
                System.Object action = constructor.Invoke(new System.Object[] {name});
                //If it's child of the main class
                if (action is Def)
                {
                    map.Add(name, (Def)action);
                    return (Def)action;
                }
                //Error otherwise
                else
                {
                    Debug.LogError("'" + defType.FullName + "' is not child of Def!");
                    //Return just Def, errors will be thrown quite likely
                    Def ret = new Def(name);
                    map.Add(name, ret);
                    return ret;
                }
            }
            else
              return map[name];
        }
        
        public static void registerDef(Def def) {
            if (map.ContainsKey(def.name))
            {
                Debug.LogError("Def name conflict! (" + def.name + ")");
            }
            else
            {
                map.Add(def.name, def);
            }
        }
        //Register all defs from this container
        public static void init(GameObject container)
        {
            Def[] defs = container.GetComponents<Def>();
            int l = defs.Length;
            for (int i = 0; i < l; i++)
            {
                try
                {
                    map.Add(defs[i].name, defs[i]);
                }
                catch (ArgumentException e)
                {
                    Debug.LogWarning("Def name conflict for '" + defs[i].name + "'!");
                }
            }
        }
    }
}