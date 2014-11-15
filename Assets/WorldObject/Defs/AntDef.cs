using UnityEngine;
using System.Collections;
namespace Defs
{
    public class AntDef : Def
    {
        //Time to spend as egg before hatching - warmth does affect this value
        public float hatchTime = 1;
        //How much does warmth affect hatch time - time increments n times faster
        //Values smaller than 1 would cause ants to hatch slower
        public float warmthRate = 2;
        //
    }
}