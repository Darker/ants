using UnityEngine;
using System.Collections;
using RTS;


public class LazyResource<T> where T : UnityEngine.Object
{

    //Path is read-only
    public string path { get { return _path; } }
    private string _path = "";
    //Whether NOT FOUND warning was thrown
    //in that case, further load attemts are ommited and the resource returns always null...
    public bool failed = false;
    //Constructor uses the path as first parameter
    public LazyResource(string path) {
        _path = path;
    }
    //Cached resource
    private T cache = null;

    public T res
    {
        get
        {
            //Does not re-try if it failed before
            if (cache == null && !failed)
            {
                //Load the proper type of resource
                cache = (T)Resources.Load(_path, typeof(T));
                //Throw warning (once)
                if (cache == null)
                {
                    Debug.LogWarning("Icon not found at '" + _path + "'!");
                    failed = true;
                }
            }
            //Can return null
            return cache;
        }
    }
}
