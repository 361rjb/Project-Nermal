using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class BulletCreatorEditorScript : EditorWindow
{

    //Static Colors
    Color white = Color.white;
    Color black = Color.black;
    Color blue = Color.blue;
    Color green = Color.green;
    Color grey = new Color(0.9f, 0.9f, 0.9f, 1.0f);
    Color greyLines = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    //Default Rect Sizes
    static Rect bulletArea = new Rect(200, 0, 500, 500);
    static Rect optionsArea = new Rect(0, 0, bulletArea.x, bulletArea.height);
    static Rect buttonArea = new Rect(0, bulletArea.height, bulletArea.width + optionsArea.width, 100);
    static Rect saveButtonPos = new Rect(0, buttonArea.y + (buttonArea.height / 4), 100, 50);
    static Rect loadButtonPos = new Rect(saveButtonPos.width, buttonArea.y + (buttonArea.height / 4), 100, 50);
    static Rect newBulletButtonPos = new Rect(saveButtonPos.width + loadButtonPos.width, buttonArea.y + (buttonArea.height / 4), 100, 50);
    static Rect playAnimationButtonPos = new Rect(saveButtonPos.width + loadButtonPos.width + newBulletButtonPos.width, buttonArea.y + (buttonArea.height / 4), 100, 50);


    //Saving/Loading vars
    bool didLoad;

    

    //Text Styles
    GUIStyle centerStyle = new GUIStyle();
    GUIStyle leftStyle = new GUIStyle();

    // bullet variables
    Object selectedBullet;
    BulletObject currentBullet;
    string bulletAssetPath;
    string bulletName = "new Bullet";

    //Drawing
    Rect bulletRect = new Rect();
    Texture bulletSpriteAsTexture;
    Vector2 bulletSpriteSizeMiddlePos = new Vector2();

    Sprite slowRadiusTexture;
    Rect slowRadiusRect = new Rect();
    Sprite targetRadiusTexture;
    Rect targetRadiusRect = new Rect();

    //Static Vector positions
    static Vector2 zeroCoord = new Vector2(optionsArea.width + (bulletArea.width/9), bulletArea.height/2);

    //Grid Lines
    static Vector2 gridLineXStart = new Vector2(zeroCoord.x, 0);
    static Vector2 gridLineXEnd = new Vector2(zeroCoord.x, bulletArea.height);
    static Vector2 gridLineYStart= new Vector2(optionsArea.width, zeroCoord.y);
    static Vector2 gridLineYEnd = new Vector2(optionsArea.width+ bulletArea.width, zeroCoord.y);

    //Path Array
    List<BulletWaypoints> waypointList = new List<BulletWaypoints>();
    List<Vector2> waypointDrawingPoints = new List<Vector2>();

    Rect waypointVisual = new Rect(0, 0, 10, 10);
    Vector2 currentDrawingPoint; //Used for removing on click

    //Waypoint Values
    int currentSelectedWaypoint = -1;


    //Mouse And Click Position
    Vector2 mousePos;

    //Animation Variables
    bool isPlayingAnimation = false;
    Vector2 currentBulletDrawPosition = zeroCoord;
    int currentAnimIndex = 0;
    Vector2 moveDir = new Vector2();


    [MenuItem("Window/Bullet Editor/Bullet Creator")]
    public static void ShowWindow()
    {
        GetWindow<BulletCreatorEditorScript>("Bullet Creator");

    }

    private void Awake()
    {
        //Set Text Styles
        centerStyle.richText = true;
        centerStyle.alignment = TextAnchor.MiddleCenter;

        leftStyle.richText = true;
        leftStyle.alignment = TextAnchor.MiddleLeft;

        //Start list
        waypointDrawingPoints.Add(zeroCoord);
        waypointList.Add(new BulletWaypoints());

        //Find Textures
        slowRadiusTexture = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/SlowRadius.png", typeof(Sprite));
        targetRadiusTexture = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/TargetRadius.png", typeof(Sprite));
    }

    private void OnGUI()
    {
        //Draw White Background Space
        EditorGUI.DrawRect(bulletArea, white);
        //Right Click on bullet Area
        BulletAreaSelection();
        //Draw Line
        Handles.BeginGUI();
        Handles.color = black;
        Handles.DrawAAPolyLine(3, buttonArea.position, bulletArea.position + bulletArea.size);
        Handles.color = greyLines;
        Handles.DrawAAPolyLine(2, gridLineXStart, gridLineXEnd);
        Handles.DrawAAPolyLine(2, gridLineYStart, gridLineYEnd);
        Handles.EndGUI();

        //Draw Bullet Options

        EditorGUI.DrawRect(optionsArea, grey);
        DrawOptions();

        //Draw Button Area
        EditorGUI.DrawRect(buttonArea, white);
        DrawButtons();



        //Draw Bullet Components on bulletArea
        DrawBullet();
        DrawBulletPath();



        



        Repaint();
    }

    //Draw options on side
    void DrawOptions()
    {
        
        //Start Options
        EditorGUILayout.BeginScrollView(optionsArea.position, false, true, GUILayout.Width(optionsArea.width), GUILayout.Height(optionsArea.height));
        //EditorGUILayout.BeginFoldoutHeaderGroup(false, );
        GUILayout.Label("<color=#000000>\n<b>Bullet Options</b> \n</color>", centerStyle);
        //EditorGUILayout.EndFoldoutHeaderGroup();

        //Draw Image Option
        GUILayout.Label("<color=#000000>\n This Bullet</color>", leftStyle);
        selectedBullet = EditorGUILayout.ObjectField(selectedBullet, typeof(BulletObject), false);
        if (selectedBullet != null)
        {


            if (currentBullet != (BulletObject)selectedBullet)
            {
                didLoad = false;
            }

            if (selectedBullet != null && didLoad)
            {

                //Name of Bullet
                GUILayout.Label("<color=#000000>\n Name</color>", leftStyle);
                currentBullet.bulletName = EditorGUILayout.TextField(currentBullet.bulletName);

                //Draw Image Option
                GUILayout.Label("<color=#000000>\n Bullet Image</color>", leftStyle);
                currentBullet.bulletImage = (Sprite)EditorGUILayout.ObjectField(currentBullet.bulletImage, typeof(Sprite), false);


                //Slider for slow Speed
                GUILayout.Label("<color=#000000>\n Acceleration</color>", leftStyle);
                currentBullet.maxSpeed = EditorGUILayout.Slider(currentBullet.maxSpeed, -5f, 5f);

                //Slider for Speed
                GUILayout.Label("<color=#000000>\n Minimum Speed</color>", leftStyle);
                currentBullet.minSpeed = EditorGUILayout.Slider(currentBullet.minSpeed, 0.01f, 10f);

                //Slider for slow Speed
                GUILayout.Label("<color=#000000>\n Maximum Speed</color>", leftStyle);
                currentBullet.maxSpeed = EditorGUILayout.Slider(currentBullet.maxSpeed, 0.01f, 10f);


                GUILayout.Label("<color=#000000>\n<b>Waypoint Options</b> </color>", centerStyle);
                if (currentSelectedWaypoint >= 0 && currentSelectedWaypoint < waypointList.Count)
                {
                    GUILayout.Label("<color=#000000> Waypoint Index : " + currentSelectedWaypoint + "\n</color>", leftStyle);

                    //Slider for Target Radius
                    GUILayout.Label("<color=#000000>\n Target Radius</color>", leftStyle);
                    waypointList[currentSelectedWaypoint].targetRadius = EditorGUILayout.Slider(waypointList[currentSelectedWaypoint].targetRadius, 0.1f, 1f);

                    //Slider for Slow Radius
                    GUILayout.Label("<color=#000000>\n Slow Radius</color>", leftStyle);
                    waypointList[currentSelectedWaypoint].slowRadius = EditorGUILayout.Slider(waypointList[currentSelectedWaypoint].slowRadius, 0.1f, 3f);

                    //Slider for Speed
                    GUILayout.Label("<color=#000000>\n Speed</color>", leftStyle);
                    waypointList[currentSelectedWaypoint].speed = EditorGUILayout.Slider(waypointList[currentSelectedWaypoint].speed, 0.01f, 10f);

                    //Slider for Slow Speed
                    GUILayout.Label("<color=#000000>\n Slow Speed</color>", leftStyle);
                    waypointList[currentSelectedWaypoint].slowSpeed = EditorGUILayout.Slider(waypointList[currentSelectedWaypoint].slowSpeed, 0.01f, 10f);

                }
                else
                {
                    GUILayout.Label("<color=#000000> No Waypoint Selected\n</color>", leftStyle);
                }
            }
        }
        //End Options
        EditorGUILayout.EndScrollView();
    }

    //Draw Buttons At Bottom
    void DrawButtons()
    {
        //Save button
        if (GUI.Button(saveButtonPos, "Save"))
        {
            SaveBullet();
        }

        //Load Button
        if (GUI.Button(loadButtonPos, "Load"))
        {
            LoadBullet();
        }

        //New Bullet Button
        if (GUI.Button(newBulletButtonPos, "New Bullet"))
        {
            NewBullet();
        }

        //Should Play Animation Button
        if(GUI.Button(playAnimationButtonPos, "Play Animation"))
        {
            isPlayingAnimation = !isPlayingAnimation;
          //  PlayAnimation();
        }
    }

    //Save bullet selected
    void SaveBullet()
    {
        //Save Current Bullet
        if (currentBullet != null)
        {
            //Set Label of Bullet
            AssetDatabase.SetLabels(currentBullet, new[] { "BulletObj" });
            //Load Bullet
            currentBullet = (BulletObject)AssetDatabase.LoadAssetAtPath(bulletAssetPath, typeof(BulletObject));
            //Rename Assset to new String
            AssetDatabase.RenameAsset(bulletAssetPath, currentBullet.bulletName);
            
            //Save Waypoints
            for(int i = 0; i < waypointDrawingPoints.Count; i++)
            {
                //Relative to zero Coord
                waypointList[i].position = (waypointDrawingPoints[i] - zeroCoord) / 10f;

            }
            //Set List
            currentBullet.bulletPath = null;
            currentBullet.bulletPath = waypointList;

        }
    }

    //Create New Bullet
    void NewBullet()
    {
        SaveBullet();
        BulletObject newBullet = (BulletObject)ScriptableObject.CreateInstance(typeof(BulletObject));
        newBullet.name = bulletName;

        AssetDatabase.CreateAsset(newBullet, "Assets/Objects/Bullets/" + bulletName + ".asset");
        //Label it so it can be found when saving again
        AssetDatabase.SetLabels(newBullet, new[] { "BulletObj" });
        currentBullet = newBullet;
        selectedBullet = currentBullet;
        bulletAssetPath = AssetDatabase.GetAssetPath(currentBullet);
        //Create prefab and reset data feilds

    }

    //Load bullet selected
    void LoadBullet()
    {
        if (selectedBullet != null)
        {
            //Create New Bullet At path
            BulletObject newBullet = (BulletObject)AssetDatabase.LoadAssetAtPath("Assets/Objects/Bullets/BulletObjects/" + selectedBullet.name + ".asset", typeof(BulletObject));

            Debug.Log(newBullet);
            if (newBullet == selectedBullet)
            {
                //Set Selected
                currentBullet = (BulletObject)selectedBullet;

                //If there is a image
                if (currentBullet.bulletImage != null)
                {
                    //Set Default Sprite Values
                    bulletSpriteSizeMiddlePos.x = -(currentBullet.bulletImage.rect.width / 2);
                    bulletSpriteSizeMiddlePos.y = -(currentBullet.bulletImage.rect.height / 2);
                    bulletRect.position = zeroCoord + bulletSpriteSizeMiddlePos;
                    bulletRect.height = currentBullet.bulletImage.rect.height;
                    bulletRect.width = currentBullet.bulletImage.rect.width;
                }

                //Reset List  
                Debug.Log("Reseting List");
                
                waypointList.Clear();
                waypointList = currentBullet.bulletPath;

                //Reset Drawing Positions
                waypointDrawingPoints.Clear();
                //waypointDrawingPoints = new List<Vector2>();
                for (int i = 0; i < waypointList.Count; i++)
                {
                    waypointDrawingPoints.Add((waypointList[i].position* 10f) + zeroCoord);

                }

                //No Selected Waypoint
                currentSelectedWaypoint = -1;

                bulletAssetPath = AssetDatabase.GetAssetPath(newBullet);
                didLoad = true;
            }
        }
    }


    //Draw Bullet
    void DrawBullet()
    {
        //If there is nothing to draw return
        if (currentBullet == null || currentBullet.bulletImage == null)
            return;

        //If the animation is playing and the list exists
        if (isPlayingAnimation && waypointDrawingPoints.Count > 1)
        {
            //If the animation index is higher than the count return and set to one
            if (currentAnimIndex + 1 >= waypointDrawingPoints.Count || currentAnimIndex + 1 >= waypointList.Count)
            {
                //The origin
                currentBulletDrawPosition = zeroCoord;
                
                //For the plus one
                currentAnimIndex = -1;
                //direction to move in
                moveDir = waypointDrawingPoints[currentAnimIndex + 1] - currentBulletDrawPosition ;
                moveDir.Normalize();

                return;
            }


            //Slow Radius Check
            if( Vector2.Distance(currentBulletDrawPosition, waypointDrawingPoints[currentAnimIndex + 1]) < waypointList[currentAnimIndex+1].slowRadius*100f)
            {
                //Lerp with slow speed
                currentBulletDrawPosition = Vector2.LerpUnclamped(currentBulletDrawPosition, currentBulletDrawPosition + moveDir, Time.deltaTime * waypointList[currentAnimIndex+1].slowSpeed /*replace with slow speed and fast speed*/);
            }
            //regular speed
            else
            {
                //Lerp with target speed
                currentBulletDrawPosition = Vector2.LerpUnclamped(currentBulletDrawPosition, currentBulletDrawPosition + moveDir, Time.deltaTime * waypointList[currentAnimIndex+1].speed /*replace with slow speed and fast speed*/);
            }
               
            //If the size distance is withan the target next anim point
            if (currentAnimIndex + 1 < waypointDrawingPoints.Count && Vector2.Distance(currentBulletDrawPosition, waypointDrawingPoints[currentAnimIndex+1]) < waypointList[currentAnimIndex+1].targetRadius*100f /*CHANGE TO SLOW RADIUS AND ADD REGULAR RADIUS*/)
            {
                //Increment
                currentAnimIndex++;

                //if the size is out of bounds dont check new direction
                if (currentAnimIndex + 1 < waypointDrawingPoints.Count || currentAnimIndex + 1 < waypointList.Count)
                {
                    moveDir = waypointDrawingPoints[currentAnimIndex + 1] - currentBulletDrawPosition;
                }
                
                moveDir.Normalize();
            }

        }
        else
        {
            //Else dont draw updated position and keep at zero coord
            currentBulletDrawPosition = zeroCoord;
            currentAnimIndex = 0;
            if (currentAnimIndex + 1 < waypointDrawingPoints.Count)
            {
                moveDir = waypointDrawingPoints[currentAnimIndex + 1] - currentBulletDrawPosition;
            }
            moveDir.Normalize();
        }
        
        //Set Rect position with currently Lerped or unlerped position
        bulletRect.position = currentBulletDrawPosition - (bulletRect.size/2);

        GUI.color = Color.clear;
        EditorGUI.DrawTextureTransparent(bulletRect, currentBullet.bulletImage.texture, ScaleMode.ScaleToFit);
        GUI.color = Color.white;
    }

    //Bullet Area Selection
    void BulletAreaSelection()
    {
        Event e = Event.current;
        
        //Check to see if background area contains mouse
        if(bulletArea.Contains(e.mousePosition) && e.type == EventType.ContextClick)
        {
            mousePos = e.mousePosition;
            GenericMenu menu = new GenericMenu();

            //What did you Click on
            menu.AddDisabledItem(new GUIContent("Bullet Waypoint Field"));
            //Add Waypoint option
            menu.AddItem(new GUIContent("Add Waypoint"), false, AddBulletWaypoint);

            menu.ShowAsContext();
        }
    }

    //Add Bullet path Vector
    void AddBulletWaypoint()
    {
        //Add drawing point at add position
        waypointDrawingPoints.Add(mousePos);
        //Add new Waypoint
        waypointList.Add(new BulletWaypoints());
    }

    //Remove Bullet Waypoint from list
    void RemoveBulletWaypoint()
    {
        //Remove At current draw position
        waypointDrawingPoints.Remove(currentDrawingPoint);
        //Remove current selected Waypoint
        waypointList.RemoveAt(currentSelectedWaypoint);
        //No waypoint selected
        currentSelectedWaypoint = -1;
    }

    //Draw bullet path
    void DrawBulletPath()
    {
        if (currentBullet == null)
            return;

        //Draw lines 
        Handles.BeginGUI();
        Handles.color = black;
        for (int i = 0; i < waypointDrawingPoints.Count; i++)
        {
            //Position of waypoint
            waypointVisual.position = waypointDrawingPoints[i] - (waypointVisual.size/2);
            EditorGUI.DrawRect(waypointVisual, black);

            //If this is the selected waypoint draw radi
            if(i == currentSelectedWaypoint)
            {
                //Draw Slow/Target Radi

                slowRadiusRect.size = slowRadiusTexture.rect.size*waypointList[currentSelectedWaypoint].slowRadius*4f;
                slowRadiusRect.position = waypointDrawingPoints[currentSelectedWaypoint] - (slowRadiusRect.size/2f);

                targetRadiusRect.size = targetRadiusTexture.rect.size * waypointList[currentSelectedWaypoint].targetRadius*4f;
                targetRadiusRect.position = waypointDrawingPoints[currentSelectedWaypoint] - (targetRadiusRect.size / 2f);

                //Set to clear for alpha
                GUI.color = Color.clear;
                EditorGUI.DrawTextureTransparent(slowRadiusRect, slowRadiusTexture.texture, ScaleMode.ScaleToFit, 1f);
                EditorGUI.DrawTextureTransparent(targetRadiusRect, targetRadiusTexture.texture, ScaleMode.ScaleToFit, 1f);
                //Set Back
                GUI.color = Color.white;
            }

            Event e = Event.current;
            //Check to see if clicked on
            if (waypointVisual.Contains(e.mousePosition) && e.type == EventType.ContextClick)
            {
                currentDrawingPoint = waypointDrawingPoints[i];
                mousePos = e.mousePosition;
                GenericMenu menu = new GenericMenu();

                //What did you Click on
                menu.AddDisabledItem(new GUIContent("Waypoint"));
                //Remove waypoint options
                menu.AddItem(new GUIContent("Remove Waypoint"), false, RemoveBulletWaypoint);
                
                currentSelectedWaypoint = i;
                menu.ShowAsContext();
            }

            //Mouse click to select
            if(waypointVisual.Contains(e.mousePosition) && e.button == 0 && e.isMouse)
            {
                currentSelectedWaypoint = i;
                Debug.Log(currentSelectedWaypoint);
            }

            //Draw Line
            if (i + 1 < waypointDrawingPoints.Count)
            {
                
                Handles.DrawAAPolyLine(3, waypointDrawingPoints[i], waypointDrawingPoints[i + 1]);
                
            }
        }
        Handles.EndGUI();
    }
}
