using UnityEngine;
using System.Collections;
using RTS;
using Defs;
using System;
using System.Reflection;
using Units;
using System.Collections.Generic;
/**
 * Class for unit/building action
 * Contains cursor information, button image and other GUI details as well as the actual function,
 * cooldown, warmup etc.
 * Every world object needs own action instance
 */


namespace Actions
{
    public abstract class Action
    {
        private static Dictionary<string, ConstructorInfo> constructors = new Dictionary<string, ConstructorInfo>();
        public static Action fromString(string className, string defName, WorldObject actor)
        {
            ConstructorInfo constructor;

            //Try to load cache
            if (constructors.ContainsKey(className))
            {
                constructor = constructors[className];
            }
            else
            {
                //Get the Assembly (namespace)
                //Assembly assembly = Assembly.Load("Actions");
                //Get the exact class Type
                Type t = Type.GetType("Actions." + className);
                if (t == null)
                {
                    Debug.LogError("Type 'Actions." + className + "' not found!");
                    constructors.Add(className, null);
                    return null;
                }
                //Get the info about constructor (using array literal)
                constructor = t.GetConstructor(new Type[] { typeof(string), actor.GetType() });
                //Save in cache
                constructors.Add(className, constructor);
            }
            //Constructor can be (theoretically) null
            if (constructor!=null)
            {
                //Initialise the Type instance
                System.Object action = constructor.Invoke(new System.Object[] { defName, actor });
                //If it's child of the main class
                if (action is Action)
                    return (Action)action;
                //Error otherwise
                else
                {
                    Debug.LogError("'" + className + "' is not child of Action!");
                    //Nullify the constructor as it obviously sucks
                    constructors[className] = null;
                    return null;
                }
            }
            else
            {
                Debug.LogError("No suitable constructor found for 'Actions." + className + "'.");
                return null;
            }
        }

        // TIMING
        //Used to calculate remaining cooldown
        protected float lastUsed = -9999999;
        //Used to calculate warmup delay
        protected float lastStarted = -9999999;
        //Tells that the action is running
        protected bool running = false;

        // ACTION TARGETS
        protected WorldObject targetObj;
        protected Vector3 targetLoc;

        // RESOURCES
        public ActionDef def;
        protected LazyResource<Texture2D> menuImage;

        // REFERENCES
        protected WorldObject actor;  //Object that uses (is capable of) this action

        //private Texture2D cursorTex = null;
        //public Texture2D cursor { get { return ResourceManager.loadTexture(ref cursorTex, HoverCursor); } }

        public Action(string def, WorldObject act)
        {
            if (act)
                actor = act;
            else
            {
                Debug.LogError("Action with no parent object!");
            }

            if (def.Length > 0)
                this.def = DefStorage.getDef(def) as ActionDef;
            else
                this.def = DefStorage.getDef(getDefName()) as ActionDef;
            if (this.def == null)
                this.def = new ActionDef();
            //this.def = ResourceManager.
            menuImage = new LazyResource<Texture2D>(this.def.MenuIcon);
            //Init def
            //InitDef();
        }
        //Allows classes to alter the definition
        /*protected virtual void InitDef()
        {
            
        }*/
        //Allows classes to override def name
        protected virtual string getDefName()
        {
            return "";
        }

        //Start the action
        //Returns on success
        public virtual bool Start(Vector3? targ_location = null, WorldObject target = null)
        {
            if (!CanStart())
            {
                Debug.Log("Action '" + this.GetName() + "' can't start.");
                return false;
            }
            else
            {
                Debug.Log("Action '" + this.GetName() + "' started.");
            }
            //If it's running already it must be reset
            if(running)
              Reset();

            if (targ_location.HasValue)
                targetLoc = targ_location.Value;
            else
                targetLoc = ResourceManager.InvalidPosition;
            targetObj = target;
            //Start normally
            running = true;
            lastStarted = Time.time;
            //May cause problems, let's see
            //if (def.warmup == 0)
            //    Execute();
            return true;
        }
        //Stop the action if possible (returns false if not stopped)
        public virtual bool Stop(bool force = false)
        {
            running = false;
            Reset();
            return true;
        }
        //Pauses actions that support it
        public virtual bool Pause()
        {
            return false;
        }
        //Execute the action - do whatever the action actually does
        //RESPONSIBLE FOR TURNING OFF THE RUNNING SETTING!!!
        public abstract void Execute();

        //Updates the action (on cooldowns etc)
        public virtual void Update()
        {
            if (running && Time.time - lastStarted >= def.warmup)
            {
                lastUsed = Time.time;
                Execute();
            }
        }
        //Reset various variables that are used during progress
        // - calling base.() is IMPERATIVE!
        public virtual void Reset()
        {
            targetLoc = ResourceManager.InvalidPosition;
            targetObj = null;
        }


        //Function to get menu image - allows to alter icons as needed
        public virtual string GetMenuIconPath()
        {
            return def.MenuIcon;
        }
        //Getter for the menu icon texture
        public virtual Texture2D GetMenuIcon()
        {
            return menuImage.res;
        }
        //Get title
        public virtual string GetName()
        {
            return def.name;
        }
        public override string ToString()
        {
            return GetName();
        }
        //constructor
        /*public Action()
        {
            //Preloads static menu icon
            
        }*/

        public virtual bool CanStart()
        {
            float time = Time.time;
            return (def.cooldown==0 || time - lastUsed >= def.cooldown) && (def.cancelable || !running);
        }
        //Initally, all actions are allowed
        public virtual bool AllowsAction(Action a)
        {
            return true;
        }
        /** EVENTS **/
        /*private Dictionary<string, ArrayList<Action>> events = new Dictionary<string,ArrayList<Action>>;
        public void addEvent(string name, Action action) {
            if(!events.ContainsKey(name)) {
                
            }

        }*/
    }
}
