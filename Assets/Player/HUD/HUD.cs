using UnityEngine;
using System.Collections;
using RTS;
using Actions;
using Units;

public class HUD : MonoBehaviour {
    public GUISkin resourceSkin, ordersSkin, selectBoxSkin;

    public Texture2D activeCursor;
    //Currently, windows cursor is used by default
    public static Texture2D defaultCursor;
    public Texture2D helpCursor;
    // [ally, neutral, enemy]
    public Texture2D[] selectCursors;

    // i = |x|*((x+1)/2)+|y|*((y+1)/2+2)
    public Texture2D[] panCursors;
    public Texture2D[] moveCursors, attackCursors, harvestCursors;

    public GUISkin mouseCursorSkin;
    //ORDERS BAR
    //List of actions and scrolling for this list
    private WorldObject lastSelection;
    private float sliderValue;
    public Texture2D buttonClick;
    public Texture2D buttonHover;
    private int buildAreaHeight = 0;  //Calculated on START
    private const int BUILD_IMAGE_WIDTH = 64,
                      SCROLL_BAR_WIDTH = 3,
                      BUTTON_SPACING = 7,
                      BUILD_IMAGE_HEIGHT = 64;
    //Size
    private const int ORDERS_BAR_WIDTH = 50,
                      RESOURCE_BAR_HEIGHT = 20,
                      SELECTION_NAME_HEIGHT = 15;
    //Player (parent class) refference
    private User user;

	// Use this for initialization
	void Start () {
	    //Initialise player
        user = transform.root.GetComponent<User>();
        //Initialise select box skin
        ResourceManager.StoreSelectBoxItems(selectBoxSkin);
        //Select default cursor
        //ResourceManager.loadTexture(ref defaultCursor, "textures
        SetCursorState(CursorState.Default);

        //Calculate size of elements
        buildAreaHeight = Screen.height - RESOURCE_BAR_HEIGHT - SELECTION_NAME_HEIGHT - 2 * BUTTON_SPACING;
	}

    public Rect GetPlayingArea() {
      return new Rect(0, RESOURCE_BAR_HEIGHT, Screen.width - ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT);
    }
	
	// Update is called once per frame
    void OnGUI()
    {
        if (user)
        {
            DrawOrdersBar();
            DrawResourcesBar();
            DrawMouseCursor();
        }
	}
    /** ORDERS/ACTIONS **/
    void DrawOrdersBar()
    {
        GUI.skin = ordersSkin;
        GUI.BeginGroup(new Rect(Screen.width - ORDERS_BAR_WIDTH, RESOURCE_BAR_HEIGHT, ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT));
        GUI.Box(new Rect(0, 0, ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT), "");
        //Draw the Box
        WorldObject sel = user.SelectedObject;
        if (sel)
        {
            string selectionName = sel.objectName;

            if (sel.owner == user.player)
            {
                //reset slider value if the selected object has changed
                if (lastSelection && lastSelection != sel)
                    sliderValue = 0.0f;
                DrawActions(sel.GetActions());
                //store the current selection
                lastSelection = sel;
            }
            GUI.Label(new Rect(0, 10, ORDERS_BAR_WIDTH, SELECTION_NAME_HEIGHT), selectionName);
        }
        //Close box
        GUI.EndGroup();
    }
    private void DrawActions(Action[] actions) {
        GUIStyle buttons = new GUIStyle();
        buttons.hover.background = buttonHover;
        buttons.active.background = buttonClick;
        GUI.skin.button = buttons;
        int numActions = actions.Length;
        //define the area to draw the actions inside
        GUI.BeginGroup(new Rect(0, 0, ORDERS_BAR_WIDTH, buildAreaHeight));
        //draw scroll bar for the list of actions if need be
        if(numActions >= MaxNumRows(buildAreaHeight))
            DrawSlider(buildAreaHeight, numActions / 2.0f);
        //display possible actions as buttons and handle the button click for each
        for(int i = 0; i < numActions; i++) {
            //int column = i % 2;
            //int row = i / 2;
            int column = 0;
            int row = i;

            Rect pos = GetButtonPos(row, column);
            //if (actions[i] == null)
            //    continue;
            Texture2D action = actions[i].GetMenuIcon();
            if (action)
            {
                //create the button and handle the click of that button
                if (GUI.Button(pos, action))
                {
                    Debug.Log("Click!");
                    if (user.SelectedObject)
                        user.SelectedObject.ClickAction(actions[i]);
                }
            }
        }
        GUI.EndGroup();
    }
    private int MaxNumRows(int areaHeight)
    {
        return areaHeight / BUILD_IMAGE_HEIGHT;
    }

    private Rect GetButtonPos(int row, int column)
    {
        int left = SCROLL_BAR_WIDTH + column * BUILD_IMAGE_WIDTH;
        float top = row * BUILD_IMAGE_HEIGHT - sliderValue * BUILD_IMAGE_HEIGHT;
        return new Rect(left, top, BUILD_IMAGE_WIDTH, BUILD_IMAGE_HEIGHT);
    }

    private void DrawSlider(int groupHeight, float numRows)
    {
        //slider goes from 0 to the number of rows that do not fit on screen
        sliderValue = GUI.VerticalSlider(GetScrollPos(groupHeight), sliderValue, 0.0f, numRows - MaxNumRows(groupHeight));
    }
    private Rect GetScrollPos(int groupHeight)
    {
        return new Rect(BUTTON_SPACING, BUTTON_SPACING, SCROLL_BAR_WIDTH, groupHeight - 2 * BUTTON_SPACING);
    }
    /** RESOURCES **/
    private void DrawResourcesBar()
    {
        GUI.skin = resourceSkin;
        GUI.BeginGroup(new Rect(0, 0, Screen.width, RESOURCE_BAR_HEIGHT));
        GUI.Box(new Rect(0, 0, Screen.width, RESOURCE_BAR_HEIGHT), "");
        GUI.EndGroup();
    }

    /** CURSORS **/
    private void DrawMouseCursor()
    {
        if (!MouseInBounds()||activeCursorState==CursorState.Default||activeCursor==null)
        {
            Screen.showCursor = true;
        }
        else
        {
            Screen.showCursor = false;
            GUI.skin = mouseCursorSkin;
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
            UpdateCursorAnimation();
            Rect cursorPosition = GetCursorDrawPosition();
            GUI.Label(cursorPosition, activeCursor);
            GUI.EndGroup();
        }
    }

    private CursorState activeCursorState;
    private int currentFrame = 0;
    private int cursorParam = 0;
    private void UpdateCursorAnimation()
    {
        //sequence animation for cursor (based on more than one image for the cursor)
        //change once per second, loops through array of images
        if (activeCursorState == CursorState.Move)
        {
            currentFrame = (int)Time.time % moveCursors.Length;
            activeCursor = moveCursors[currentFrame];
        }
        else if (activeCursorState == CursorState.Attack)
        {
            currentFrame = (int)Time.time % attackCursors.Length;
            activeCursor = attackCursors[currentFrame];
        }
        else if (activeCursorState == CursorState.Harvest)
        {
            currentFrame = (int)Time.time % harvestCursors.Length;
            activeCursor = harvestCursors[currentFrame];
        }
    }
    private Rect GetCursorDrawPosition()
    {
        //Return null if there's no active cursor
        if (!activeCursor)
            return new Rect(0,0,0,0);
        //set base position for custom cursor image
        float leftPos = Input.mousePosition.x;
        float topPos = Screen.height - Input.mousePosition.y; //screen draw coordinates are inverted
        //adjust position base on the type of cursor being shown
        if (activeCursorState == CursorState.Pan) {
            
            
        }
        //leftPos = Screen.width - activeCursor.width;
        //else if (activeCursorState == CursorState.PanDown) topPos = Screen.height - activeCursor.height;
        else if (activeCursorState == CursorState.Move || activeCursorState == CursorState.Select || activeCursorState == CursorState.Harvest)
        {
            topPos -= activeCursor.height / 2;
            leftPos -= activeCursor.width / 2;
        }
        return new Rect(leftPos, topPos, activeCursor.width, activeCursor.height);
    }
    public void SetCustomCursor(Texture2D cursor)
    {
        activeCursorState = CursorState.Custom;
        activeCursor = cursor;
    }
    public void SetCursorState(CursorState newState, int param=0)
    {
        if (cursorParam != param)
        {
            //Debug.Log("Changing param from " + cursorParam + " to " + param);
            cursorParam = param;
        }
        //If both state and param are equal just quit
        else if (newState == activeCursorState)
            return;
        activeCursorState = newState;
        switch (newState)
        {
            //Custom cursor must be set diferently
            case CursorState.Custom :
                break;
            case CursorState.Select:
                activeCursor = selectCursors[param];
                break;
            case CursorState.Attack:
                currentFrame = (int)Time.time % attackCursors.Length;
                activeCursor = attackCursors[currentFrame];
                break;
            case CursorState.Harvest:
                currentFrame = (int)Time.time % harvestCursors.Length;
                activeCursor = harvestCursors[currentFrame];
                break;
            case CursorState.Move:
                currentFrame = (int)Time.time % moveCursors.Length;
                activeCursor = moveCursors[currentFrame];
                break;
            case CursorState.Pan:
                activeCursor = panCursors[cursorParam];
                break;
            case CursorState.Help:
                activeCursor = helpCursor;
                break;
            case CursorState.Default:
                activeCursor = null;
                break;
            /*case CursorState.PanLeft:
                activeCursor = leftCursor;
                break;
            case CursorState.PanRight:
                activeCursor = rightCursor;
                break;
            case CursorState.PanUp:
                activeCursor = upCursor;
                break;
            case CursorState.PanDown:
                activeCursor = downCursor;
                break;*/
            default: 
                //Use default cursor
                activeCursorState = CursorState.Default;
                
                break;
        }
    }
    public bool SetCursorStateIf(CursorState set, CursorState ifstate, int param = 0)
    {
        if (activeCursorState == ifstate)
        {
            SetCursorState(set, param);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool MouseInBounds()
    {
        //Screen coordinates start in the lower-left corner of the screen
        //not the top-left of the screen like the drawing coordinates do
        Vector3 mousePos = Input.mousePosition;
        bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width - ORDERS_BAR_WIDTH;
        bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height - RESOURCE_BAR_HEIGHT;
        return insideWidth && insideHeight;
    }
}
