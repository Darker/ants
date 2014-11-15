using UnityEngine;
using System.Collections;

namespace Units
{
    public class Unit : WorldObject, Mouse
    {
        public int attack = 0,
                   defense = 0,
                   vision = 5;
        private Vector3 destination;
        private Quaternion targetRotation;
        protected bool moving = false;
        protected bool rotating = false;

        public float moveSpeed = 100f,
                     rotateSpeed = 1f;

        public void StartMove(Vector3 destination)
        {

            this.destination = destination;

            targetRotation = Quaternion.LookRotation(destination - transform.position);
            //targetRotation.y += 90;
            rotating = true;
            moving = false;
        }


        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            if (rotating)
                TurnToTarget();
            else if (moving)
                MakeMove();
        }

        private void TurnToTarget()
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
            //sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
            Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
            updatePosition();

            if (transform.rotation == targetRotation || transform.rotation == inverseTargetRotation)
            {
                rotating = false;
                moving = true;
            }
        }
        private void MakeMove()
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);
            updatePosition();

            if (transform.position == destination)
                moving = false;
        }

        protected override void OnGUI()
        {
            base.OnGUI();
        }
        public override void ActionOnObject(Vector3 hitPoint, WorldObject hitObject, User controller)
        {
            //No object given - target is ground...
            if (hitObject == null)
            {
                //If special action is selected
                if (selectedAction >= 0)
                {
                    if (!TryAction(selectedAction, hitPoint, null))
                    {
                        Debug.LogWarning("Selected action (#"+selectedAction+") was invalid in '" + this.name + "'.");
                        ListActions();
                    }
                }
                //Do default action
                else if (act_terrain >= 0 && act_terrain < actions.Length)
                {
                    actions[act_terrain].Start(hitPoint);
                }
                //Use basic walk function
                else
                {
                    WalkAt(hitPoint, null);
                }
            }
            else
            {
                Relationship relation = hitObject.relationTo(this);
                //If special action is selected
                if (selectedAction >= 0)
                {
                    actions[selectedAction].Start(hitPoint, hitObject);
                }
                //Do default action
                else if(relation==Relationship.Enemy)
                {
                    if(!TryAction(act_hostile, hitPoint, hitObject))
                      Debug.LogWarning("No action for enemy '"+hitObject.name+"'.");
                }
                else if (relation == Relationship.Ally)
                {
                    if (!TryAction(act_friendly, hitPoint, hitObject))
                        Debug.LogWarning("No action for allied '" + hitObject.name + "'.");
                }
                else if (relation == Relationship.Item)
                {
                    Debug.LogWarning("No action for items, such as '" + hitObject.name + "'.");
                }
            }
            //Anyway - reset selected action
            selectedAction = -1;
        }

        public virtual void WalkAt(Vector3 pos, GameObject mesh)
        {
            //Debug.Log(this.name + " walk to " + pos);
            //Debug.Log(this.name + " walk to " + pos);
            StartMove(pos);
        }
        //[Obsolete("InteractAt is deprecated. Instead, default action is called on interaction.")]
        public virtual void InteractAt(Vector3 pos, WorldObject obj)
        {
            //Debug.Log(this.name + " walk to " + pos);
            //Debug.Log(this.name + " interact with  " + obj.name + " at " + pos);
        }

        public override void SpawnFrom(WorldObject parent)
        {
            base.SpawnFrom(parent);
            this.owner = parent.owner;
        }
        /** Mouse intreface **/
        public virtual Texture2D GetCursor()
        {
            return null;
        }
    }
}