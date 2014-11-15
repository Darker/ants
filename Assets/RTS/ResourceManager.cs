using UnityEngine;
using System.Collections;




namespace RTS
{
    public static class ResourceManager 
    {
        /*Camera settings**/
        public static int ZoomSpeed { get { return 30; } }
        public static int ScrollSpeed { get { return 300; } }

        public static float RotateSpeed { get { return 500; } }
        public static float RotateAmount { get { return 50; } }
        //Padding where scrolling happens
        public static int ScrollWidth { get { return 10; } }
        //Limits
        public static float MinCameraHeight { get { return 20; } }
        public static float MaxCameraHeight { get { return 3000; } }

        //Invalid position
        private static Vector3 invalidPosition = new Vector3(-99999, -99999, -99999);
        public static Vector3 InvalidPosition { get { return invalidPosition; } }
        //Invalid bounds
        private static Bounds invalidBounds = new Bounds(new Vector3(-99999, -99999, -99999), new Vector3(0, 0, 0));
        public static Bounds InvalidBounds { get { return invalidBounds; } }
        //Skins
        private static GUISkin selectBoxSkin;
        public static GUISkin SelectBoxSkin { get { return selectBoxSkin; } }

        public static void StoreSelectBoxItems(GUISkin skin)
        {
            selectBoxSkin = skin;
        }

        public static Texture2D loadTexture(ref Texture2D target, string name)
        {
            if (target == null)
            {
                target = (Texture2D)Resources.Load(name, typeof(Texture2D));
            }
            return target;
        }
        public static Object loadResource(ref Object target, string name)
        {
            if (target == null)
            {
                target = Resources.Load(name, target.GetType());
            }
            return target;
        }

        private static GameObjects gameObjectList;

        public static void SetGameObjectList(GameObjects objectList)
        {
            gameObjectList = objectList;
        }
        public static GameObject GetObject(string name)
        {
            if(gameObjectList)
               return gameObjectList.GetObject(name);
            return null;
        }
        //protected Dictionary<string, Object> resources;
    }
}
