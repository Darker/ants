using UnityEngine;
using System.Collections.Generic;

namespace Units
{
    public class Building : WorldObject
    {
        public Texture2D buildImage;

        public float maxBuildProgress = 1f;
        protected Queue<Unit> buildQueue;
        private Vector3 spawnPoint;

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
        }

        protected override void OnGUI()
        {
            base.OnGUI();
        }
        /*public override void ActionOnObject(GameObject hitObject, Vector3 hitPoint, User controller)
        {
            base.ActionOnObject(hitObject, hitPoint, controller);
            Debug.Log("Buildings can't walk! Not even" + this.name);
        }*/
    }
}