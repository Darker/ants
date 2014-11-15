using UnityEngine;
using System.Collections;

public interface Mouse {
    public void MouseOver();
    public void MouseOut();
    public Texture2D GetCursor();
    
    public bool isMouseOver = false;
}
