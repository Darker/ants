using UnityEngine;
using System.Collections;
using RTS;
using Units;
namespace Actions
{
    public class Move : Action
    {
        public Move(string def, WorldObject act) : base(def, act) {
            destination = ResourceManager.InvalidPosition;
        
        }


        public override void Execute()
        {
            if (targetObj != null)
            {
                //Try to find non-overlapping position
                destination = targetObj.transform.position;

            }
            else if (destination == ResourceManager.InvalidPosition)
            {
                destination = targetLoc;
            }

            StartMove();
            if (rotating)
                TurnToTarget();
            else if (moving)
                MakeMove();
        }
        public override bool Start(Vector3? targ_location = null, WorldObject target = null) {
            if(!targ_location.HasValue) {
                Debug.LogError(this.actor.name+" did invalid move without any target location.");
                return false;
            }
            //Debug.Log(this.actor.name + " moves at " + targ_location.Value);
            return base.Start(targ_location.Value, target);
        }

        protected bool moving = false;
        protected bool rotating = false;
        protected Vector3 destination;
        protected Quaternion targetRotation;

        public float moveSpeed = 100f,
                     rotateSpeed = 1f;

        public override void Reset()
        {
            moving = rotating = false;
            destination = ResourceManager.InvalidPosition;
            base.Reset();
        }

        protected void StartMove()
        {
            
            targetRotation = Quaternion.LookRotation(destination - actor.transform.position);

            //sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
            Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
            rotating = actor.transform.rotation != targetRotation && actor.transform.rotation != inverseTargetRotation;
            //Move if turned to the target
            moving = !rotating;//Mathf.Abs(targetRotation.eulerAngles.y)<80;
        }
        private void TurnToTarget()
        {
            actor.transform.rotation = Quaternion.RotateTowards(actor.transform.rotation, targetRotation, rotateSpeed);
            
            //Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
            actor.updatePosition();

            /*if (Mathf.Abs(Quaternion.LookRotation(destination - actor.transform.position).y) < 80)
            {
                moving = true;
            }
            if (actor.transform.rotation == targetRotation || actor.transform.rotation == inverseTargetRotation)
            {
                rotating = false;
            }*/
        }
        private void MakeMove()
        {
            //Move towards destination
            actor.transform.position = Vector3.MoveTowards(actor.transform.position, destination, Time.deltaTime * moveSpeed);

            
            //Update HUD info
            actor.updatePosition();
            //Terminate running status if the target was reached
            if (actor.transform.position == destination && targetObj==null)
            {
                Stop();
            }
        }


        protected override string getDefName()
        {
            return "action/move";
        }
        public override bool CanStart()
        {
            return base.CanStart();
        }
        /*protected override void InitDef() {
            def.warmup = 5;
            def.cooldown = 10;
            def.description = "Spawn ant egg";
            def.cancelable = false;
        }*/

    }

}