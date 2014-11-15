using UnityEngine;
using System.Collections;
using RTS;
using Units;
namespace Actions
{
    public class SpawnEgg : Action
    {
        private Queen q;
        public string hatchName = "Ant";
        public SpawnEgg(string def, Queen queen) : base(def, queen)
        {
            q = queen;
        }

        public override void Execute()
        {
            running = false;

            GameObject prefab = ResourceManager.GetObject("Ant_egg");
            if (prefab)
            {
                Bounds bounds = prefab.renderer.bounds;
                //Get egg dimensions
                Vector3 dimensions = bounds.size;
                //Get location behind the queen
                Vector3 spawnPos = q.transform.position - q.transform.forward * dimensions.z;//q.transform.TransformPoint(-1*Vector3.forward * dimensions.x); //q.transform.position;
                //Spawn the egg there
                GameObject newUnit = (GameObject)UnityEngine.Object.Instantiate(prefab, spawnPos, q.transform.rotation);
                Egg obj = newUnit.GetComponent<WorldObject>() as Egg;
                obj.hatchName = hatchName;
                obj.SpawnFrom(this.actor);
            }

        }

        protected override string getDefName()
        {
            return "action/egg";
        }
        public override bool CanStart()
        {
            return q!=null && base.CanStart();
        }
        /*protected override void InitDef() {
            def.warmup = 5;
            def.cooldown = 10;
            def.description = "Spawn ant egg";
            def.cancelable = false;
        }*/

    }

}