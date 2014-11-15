using UnityEngine;
using System.Collections;
using RTS;
namespace Units
{
    public class Egg : Ant
    {
        public string hatchName = "Ant";
        //Cached prefab


        public float hatchTime = 10;
        public float layTime = 0;
        //Progress until hatch
        public float progress {
            get { return Mathf.Min(1, (Time.time - layTime) / hatchTime); }
        }


        public override void WalkAt(Vector3 pos, GameObject mesh)
        {
            //Eggs can't move
            return;
        }

        protected override void Start()
        {
            base.Start();
            if (layTime == 0)
            {
                layTime = Time.time;
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (progress >= 1)
                Hatch();



            base.Update();
        }

        public void Hatch()
        {
            Vector3 spawnPoint = transform.position;
            Quaternion rotation = transform.rotation;

            GameObject prefab = ResourceManager.GetObject(hatchName);
            if (prefab)
            {
                GameObject newUnit = (GameObject)Instantiate(prefab, spawnPoint, rotation);
                WorldObject obj = newUnit.GetComponent<WorldObject>();
                if (obj)
                {
                    //Inherit properties
                    obj.SpawnFrom(this);
                    //Inherit object selection
                    obj.StealSelectionFrom(this);
                }
                else
                {
                    Debug.LogError("Egg spawned non-world object! Check your prefabs!");
                }
            }
            else
            {
                Debug.LogError("The prefab for the egg was null!");
            }
            //newUnit.transform.parent = units.transform;

            Destroy(gameObject);
        }
    }
}