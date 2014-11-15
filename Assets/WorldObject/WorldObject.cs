using UnityEngine;
using System.Collections;
using RTS;
using Defs;
using Actions;
using System.Collections.Generic;

namespace Units
{
    public class WorldObject : MonoBehaviour
    {
        private static HUD renderingContext;

        public string objectName;
        public string hudImagePath = "Textures/GUI/Ant.png";
        private LazyResource<Texture2D> hudImage;

        public int hitPoints, maxHitPoints;

        //Refference to the user on this PC, the one who's watching the game, regardless of alegiance
        public User user;
        //Do not change delibiretally
        public Player owner;
        //Definition of this object
        public Def def;

        protected Action[] actions = { };
        protected List<int> queue;
        protected int selectedAction = -1;
        //Initialised in unity
        public string[] actions_str;

        public int act_hostile = -1;
        public int act_friendly = -1;
        public int act_terrain = -1;

        public Vector3 pos
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }
        public Quaternion rot
        {
            get
            {
                return transform.rotation;
            }
            set
            {
                transform.rotation = value;
            }
        }

        protected bool _selected = false;
        public bool selected
        {
            get { 
                return _selected; 
            }
            set {
                _selected = value;
                if (!value)
                    selectedAction = -1;
            }
        }
        /** GAME SECTION **/
        public virtual void ActionOnObject(Vector3 hitPoint, WorldObject hitObject, User controller)
        {
            //No object given - target is ground...
            if (hitObject == null)
            {
                //If special action is selected
                if (selectedAction >= 0)
                {
                    actions[selectedAction].Start(hitPoint);
                }
                //Do default action
                else if (act_terrain >= 0 && act_terrain < actions.Length)
                {
                    actions[act_terrain].Start(hitPoint);
                }
            }
            else
            {
                //If special action is selected
                if (selectedAction >= 0)
                {
                    actions[selectedAction].Start(hitPoint, hitObject);
                }
                //Do default action
                else 
                {
                    Debug.LogWarning("Default actions for WorldOBjects are not implemented in default ActionOnObject!");
                }
            }
            //Anyway - reset selected action
            selectedAction = -1;
        }

        //Spawn unit from other world object
        public virtual void SpawnFrom(WorldObject parent)
        {
            this.user = parent.user;
        }
        //Conditionally switch selection
        public virtual void StealSelectionFrom(WorldObject selected)
        {
            if (!user)
                return;

            if (user.SelectedObject == selected)
            {
                user.SelectedObject = this;
            }
        }

        /* ACTIONS */
        public Action[] GetActions()
        {
            return actions;
        }
        public bool ClickAction(string actionToPerform)
        {
            int l = actions.Length;
            for (int i = 0; i < l; i++)
            {
                if (actions[i].def.name == actionToPerform)
                    return ClickAction(actions[i]);
            }

            Debug.LogWarning("Tried to perform unexistent action: '" + actionToPerform + "'!");
            return false;
        }
        //Action icon clicked
        public virtual bool ClickAction(Action action)
        {
            if (this.Can(action))
            {
                selectedAction = findActionIndex(action);
                if (action.def.instant)
                {
                    ActionOnObject(ResourceManager.InvalidPosition, null, user);
                    selectedAction = -1;
                    Debug.Log("Instant action " + action.GetName());
                }
            }
            return true;
        }
        public Action findAction(string name)
        {
            int l = actions.Length;
            for (int i = 0; i < l; i++)
            {
                if (actions[i].def.name == name)
                    return actions[i];
            }
            return null;
        }
        protected int findActionIndex(string name)
        {
            int l = actions.Length;
            for (int i = 0; i < l; i++)
            {
                if (actions[i].def.name == name)
                    return i;
            }
            return -1;
        }
        protected int findActionIndex(Action a)
        {
            return findActionIndex(a.def.name);
        }
        //WARNING: it might be needed to check if the action is in actions list
        //         in that case, compare action NAMES, CLASS NAMES but not object instances if the action is given externally
        //         - This is not supposed to check whether the action can be started (eg. it's cooldown, resources...) 
        //           but whether the object is capable of doing action (isn't stunned, dead, doing something else...)
        public virtual bool Can(Action action)
        {
            //Test if action is within allowed actions
            int l = actions.Length;
            for (int i = 0; i < l; i++)
            {
                if (actions[i] == action)
                {
                    return true;
                }
            }
            return false;
        }
        public virtual bool Can(string action)
        {
            return Can(findAction(action));
        }

        protected bool TryAction(int offset, Vector3 pos, WorldObject obj, ref bool result)
        {
            if (offset >= 0 && offset < actions.Length)
            {
                result = actions[offset].Start(pos, obj);
                return true;
            }
            return false;
        }
        protected bool TryAction(int offset, Vector3 pos, WorldObject obj)
        {
            bool dummy = false;
            return TryAction(offset, pos, obj, ref dummy);
        }
        public void ListActions()
        {
            string ret = "";
            int l = actions.Length;
            for (int i = 0; i < l; i++)
            {
                ret += "actions[" + i + "] = \"" + actions[i].GetName() + "\"\n";
            }
            Debug.Log("Action list for '"+this.name+"':\n" + ret);
        }


        public bool SelectLocation(Vector3 position, WorldObject worldObject = null, User controller = null)
        {
            if (user == null)
                user = this.user;
            //If no action is selected, let the game change selection
            if (selectedAction < 0)
            {
                /*selected = false;
                if (controller.SelectedObject)
                    controller.SelectedObject.selected = false;
                controller.SelectedObject = worldObject;
                worldObject.selected = true;*/

                return true;
            }
            else
            {
                ActionOnObject(position, worldObject, controller);
                return false;
            }
        }
        public void updatePosition()
        {
            CalculateBounds();
        }

        public virtual Relationship relationTo(WorldObject other)
        {
            if (this.owner == null || other.owner == null)
                return Relationship.Neutral;
            else if (this.owner == other.owner)
                return Relationship.Ally;
            else
                return Relationship.Enemy;
        }
        /** RENDERING SECTON **/



        //Draw box over selected item
        private void DrawSelection()
        {
            if (renderingContext)
            {
                GUI.skin = ResourceManager.SelectBoxSkin;
                Rect playingArea = renderingContext.GetPlayingArea();

                Rect selectBox = WorkManager.CalculateSelectionBox(selectionBounds, playingArea);
                //Draw the selection box around the currently selected object, within the bounds of the playing area
                GUI.BeginGroup(playingArea);
                DrawSelectionBox(selectBox);
                GUI.EndGroup();
            }
        }
        protected virtual void DrawSelectionBox(Rect selectBox)
        {
            GUI.Box(selectBox, "");
        }


        protected Bounds selectionBounds;
        public void CalculateBounds()
        {
            selectionBounds = new Bounds(transform.position, Vector3.zero);
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                selectionBounds.Encapsulate(r.bounds);
            }
        }

        //Show proper hover icon for defined target
        public virtual void SetHoverState(GameObject hoverObject)
        {
            //only handle input if owned by interacting player and currently selected
            if (_selected && owner != null && owner == user.player)
            {
                WorldObject obj = hoverObject.GetComponentInChildren<WorldObject>();
                //ground
                if (hoverObject.name == "Ground")
                {
                    renderingContext.SetCursorState(CursorState.Move);
                }
                //World Objects
                else if (obj)
                {
                    //Enemy
                    if (obj.owner != owner)
                    {
                        renderingContext.SetCursorState(CursorState.Attack);
                    }
                    else if (obj.owner == owner && obj != this)
                    {
                        renderingContext.SetCursorState(CursorState.Help);
                    }
                }
            }
        }

        public virtual Texture2D getHoverCursor(int action)
        {
            if (action >= 0 && action < actions.Length)
            {
                return actions[action].def.cursor;
            }
            return null;
        }
        /** Constructor **/
        public WorldObject()
        {
            hudImage = new LazyResource<Texture2D>(hudImagePath);
        }


        /** UNITY CALLBACKS**/

        // Use this for initialization
        public static GameObject GameUser = null;

        protected virtual void Start()
        {
            //Get game user
            if (user == null)
            {
                GameUser = GameObject.Find("User");

                //OPTIMIZE: all can be static
                user = GameUser.GetComponentInChildren<User>();//transform.root.GetComponentInChildren<User>();
                if (user == null)
                    Debug.LogWarning("User is null!");
                renderingContext = GameUser.GetComponentInChildren<HUD>();
                if (renderingContext == null)
                    Debug.LogWarning("HUD is null!");
            }
            else if (renderingContext == null)
            {
                renderingContext = user.GetComponentInParent<HUD>();
                if (!renderingContext)
                    Debug.Log("Can't load HUD!");
            }
            //Get unit name
            if (objectName.Length == 0)
            {
                objectName = name;
            }
            //Load actions
            int l = actions_str.Length;
            actions = new Action[l];
            for (int i = 0; i < l; i++)
            {
                actions[i] = Action.fromString(actions_str[i], "", this);
                if (actions[i] != null)
                    Debug.Log("Created action...");
                else
                    Debug.LogWarning("Failed to create action!");
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            //Unfortunatelly, all actions need to be updated
            int l = actions.Length;
            for (int i = 0; i < l; i++)
            {
                actions[i].Update();
            }

        }
        protected virtual void OnGUI()
        {

            if (_selected)
                DrawSelection();
        }
        protected virtual void Awake()
        {
            selectionBounds = ResourceManager.InvalidBounds;
            CalculateBounds();
        }
    }
}