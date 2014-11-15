using UnityEngine;
using System.Collections;
using RTS;
using Units;

public class UserInput:MonoBehaviour
{
    private User player;

    // Use this for initialization
    void Start()
    {
        player = transform.root.GetComponent<User>();
    }
    void Update()
    {
        MoveCamera();
        RotateCamera();
        MouseActivity();
    }
    /** CAMERA **/
    //4 stands for [0, 0] vector and therefore no arrow cursor
    public int panCursorOffset = 4;
    //private float speedIntegrate = 0;
    //private int oldOffset = 4;
    private bool lastMoved = false;
    private void MoveCamera()
    {
        //Get mouse position
        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;
        //Create vector for movement
        Vector3 movement = new Vector3(0, 0, 0);

        float speedup = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 1 : 10;

        //horizontal camera movement
        if (xpos >= 0 && xpos < ResourceManager.ScrollWidth)
        {
            movement.x -= ResourceManager.ScrollSpeed * speedup;
        }
        else if (xpos <= Screen.width && xpos > Screen.width - ResourceManager.ScrollWidth)
        {
            movement.x += ResourceManager.ScrollSpeed * speedup;
        }
        //vertical camera movement
        if (ypos >= 0 && ypos < ResourceManager.ScrollWidth)
        {
            movement.z -= ResourceManager.ScrollSpeed * speedup;
        }
        else if (ypos <= Screen.height && ypos > (Screen.height - ResourceManager.ScrollWidth))
        {
            movement.z += ResourceManager.ScrollSpeed * speedup;
        }
        
        float zoom = ResourceManager.ZoomSpeed * (2*Input.GetAxis("Mouse ScrollWheel") + Input.GetAxis("Zoom"));
        //if (Input.GetAxis("Zoom") != 0)
        //    Debug.Log(Input.GetAxis("Zoom")+" Zoom:"+zoom);
        //If no movement. just exit
        if (movement.sqrMagnitude == 0 && zoom==0)
        {
            //speedIntegrate = 0;
            //player.hud.SetCursorState(CursorState.Default);
            if(lastMoved) {
              lastMoved=false;
              player.hud.SetCursorStateIf(CursorState.Default, CursorState.Pan);
            }

            return;
        }
        lastMoved = true;

        // CHOSE CURSOR
        int x, y;
        //Converting signed floats to <-1,0,1>
        x = movement.x != 0 ? (movement.x > 0 ?  1 : -1) : 0;
        y = movement.z != 0 ? (movement.z > 0 ? -1 :  1) : 0;

        panCursorOffset = (x + 1) * 3 + (y + 1);
        /*if (oldOffset != offset)
        {
            Debug.Log("Changing offset from " + oldOffset + " to " + offset + " for vector [" + x + "; " + y + "].");
            oldOffset = offset;
        }*/

        
        /*if (offset != 4)  //Offset 4 stands for [0,0]
        {
            player.hud.SetCursorState(CursorState.Pan, offset);
        }
        else
        {
            player.hud.SetCursorState(CursorState.Default);
        }*/



        //make sure movement is in the direction the camera is pointing
        //but ignore the vertical tilt of the camera to get sensible scrolling
        //movement = Camera.main.transform.TransformDirection(movement);
        //Up/down movement will be calculated diferently
        //movement.z = -movement.y;
        //movement.y = 0;
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        //Vector3 down = Camera.main.transform.up;

        forward.y = 0f;
        right.y = 0f;  // not needed unless your camera is also rotated around z
        movement = forward.normalized * movement.z + right.normalized * movement.x;
        movement.y = zoom*ResourceManager.ZoomSpeed;
        //movement.y += zoom;
        //Debug.Log("After:"+movement);

        //away from ground movement

        //calculate desired camera position based on received input
        Vector3 origin = Camera.main.transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;
        //limit away from ground movement to be between a minimum and maximum distance
        if (destination.y > ResourceManager.MaxCameraHeight)
        {
            destination.y = ResourceManager.MaxCameraHeight;
        }
        else if (destination.y < ResourceManager.MinCameraHeight)
        {
            destination.y = ResourceManager.MinCameraHeight;
        }
        //TODO: add different limits depending on terrain height

        //if a change in position is detected perform the necessary update
        if (destination != origin)
        {
            //speedIntegrate += ResourceManager.ScrollSpeed / 5;
            //Debug.Log(destination);
            Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
        }

    }

    private void RotateCamera()
    {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;

        //detect rotation amount if middle button is pressed
        if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.RightAlt))
        {
            //destination.x -= Input.GetAxis("Mouse Y")/5;
            //destination.z -= Input.GetAxis("Mouse X") * ResourceManager.RotateAmount;
            destination.x -= Input.GetAxis("Mouse Y") * ResourceManager.RotateAmount;
            destination.y += Input.GetAxis("Mouse X") * ResourceManager.RotateAmount;
        }
        //Limit camera angle
        if (destination.x < 5)
            destination.x = 5;
        else if (destination.x > 85)
            destination.x = 85;

        //if a change in position is detected perform the necessary update
        if (destination != origin)
        {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.RotateSpeed);
        }
    }

    private void MouseActivity()
    {
        if (Input.GetMouseButtonDown(0)) 
            LeftMouseClick();
        else if (Input.GetMouseButtonDown(1))
            RightMouseClick();
        else
        {
            MouseHover();
        }
    }

    private void LeftMouseClick()
    {
        if (player.hud.MouseInBounds())
        {
            GameObject hitObject = FindHitObject();
            Vector3 hitPoint = FindHitPoint();
            if (hitObject && hitPoint != ResourceManager.InvalidPosition)
            {
                //Two button mode - right=action, left=select
                /*if (player.SelectedObject) 
                    player.SelectedObject.MouseClick(hitObject, hitPoint, player);
                else */
                //First let the selected object decide if he wants to give up the selection (= no action is performed)
                if (player.SelectedObject)
                {


                }
                WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject>();
                if ((!player.SelectedObject || player.SelectedObject.SelectLocation(hitPoint, worldObject, player)))
                {
                    
                    if (worldObject)
                    {
                        //we already know the player has no selected object
                        player.SelectedObject = worldObject;
                        //Reset cursor
                        player.hud.SetCursorState(CursorState.Default);

                        //worldObject.selected = true;  // - happens in SelecedObject setter
                        //Debug.Log("Selected: '" + hitObject.name + "'");
                    }
                    //Clicked on a non-world object
                    else
                    {
                        player.SelectedObject = null;
                        //Debug.Log("Selected '" + hitObject.name + "' is not world object!");
                    }
                }

            }
            else
            {
                //Debug.Log("No object.");
            }
        }
        else
        {
            //Debug.Log("Mouse in HUD");
        }
    }
    private void RightMouseClick()
    {
        WorldObject sel = player.SelectedObject;

        if (player.hud.MouseInBounds() && sel)
        {
            GameObject hitObject = FindHitObject();
            Vector3 hitPoint = FindHitPoint();
            Debug.Log(hitPoint + " on " + hitObject);
            if (hitObject && hitPoint != ResourceManager.InvalidPosition)
            {
                //if (player.SelectedObject is Unit)
                //    Debug.Log("Unit!");

                sel.ActionOnObject(hitPoint, hitObject.GetComponentInChildren<WorldObject>(), player);


            }
        }
    }

    private GameObject lastHover = null;
    private void MouseHover()
    {
        if (player.hud.MouseInBounds())
        {
            GameObject hoverObject = FindHitObject();
            //If change
            if (hoverObject != lastHover)
            {

                //Debug.Log("Mouse out: " + (lastHover?lastHover.name:"*null*"));
                lastHover = hoverObject;
                
                //If there's something hovered over
                if (hoverObject)
                {
                    //Debug.Log("Mouseon: " + hoverObject.name);
                    //Debug.Log("Name: " + hoverObject.name);
                    WorldObject obj = hoverObject.GetComponentInChildren<WorldObject>();

                    if (player.SelectedObject && player.SelectedObject.owner == player.player)
                        player.SelectedObject.SetHoverState(hoverObject);
                    else if (obj)
                    {
                        //Isn't neutral
                        /*if (obj.owner)
                        {
                            player.hud.SetCursorState(CursorState.Select, obj.owner == player.player ? 0 : 2);
                        }
                        else
                        {
                            //Select neutral object
                            player.hud.SetCursorState(CursorState.Select, 1);
                        }*/
                    }
                    //Not a game object
                    else
                    {
                        player.hud.SetCursorStateIf(CursorState.Default, CursorState.Select);
                    }
                }
            }
        }
    }

    private GameObject FindHitObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) 
            return hit.collider.gameObject;
        return null;
    }
    private Vector3 FindHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) return hit.point;
          return ResourceManager.InvalidPosition;
    }
}