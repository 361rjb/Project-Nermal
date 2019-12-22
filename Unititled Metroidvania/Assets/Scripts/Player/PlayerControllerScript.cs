using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    Transform thisTransform;

    GameObject thisGameObject;

    Rigidbody2D thisRigidBody2D;

    Animator thisAnimator;

    public GameObject yInteract;

    //Input Variables
    float xInput = 0.0f;
    float yInput = 0.0f;
    float jumpInput = 0.0f;
    float lastJumpInput = 0.0f;
    float slowWalkInput = 0.0f;

    public bool pausedGame = false;


    public float interactInput = 0.0f;
    float lastInteractInput = 0.0f;

    //Game Checks
    bool isGrounded = false;
    bool falling = false;
    bool fastFall = false;
    bool touchedCeil = false;

    //Movement Vars
    float jumpedY;

    //Editor Variables
    [Header ("Movement")]
    [SerializeField]
    float playerWalkSpeed = 0;

    [SerializeField]
    float playerAirSpeed = 0;

    [SerializeField]
    float playerSlowWalkSpeed = 0;

    [SerializeField]
    float jumpHeightLimit = 5f;

    [SerializeField]
    float jumpInitialVelocity;

    [SerializeField]
    float jumpHoldVelocity;

    [SerializeField]
    int shortHopFrameLength;
    int shortHopCounter;

    [SerializeField]
    float fastFallSpeed;

    [SerializeField]
    int fastFallFrameLength;

    [SerializeField]
    GameObject groundCheck;
    Collider2D groundColliderCheck;
    [SerializeField]
    GameObject ceilingCheck;
    Collider2D ceilingColliderCheck;

    [SerializeField]
    LayerMask groundLayer;

    [HideInInspector]
   public LevelManagerScript currentLevelScript;

    LevelTransition levelTransScript;

    [HideInInspector]
    public bool canControl = true;

    // Start is called before the first frame update
    void Start()
    {
        thisTransform = transform;
        thisGameObject = gameObject;
        thisRigidBody2D = thisGameObject.GetComponent<Rigidbody2D>();
        groundColliderCheck = groundCheck.GetComponent<Collider2D>();
        ceilingColliderCheck = ceilingCheck.GetComponent<Collider2D>();
        thisAnimator = GetComponent<Animator>();
        levelTransScript = GameObject.Find("LevelTransition").GetComponent<LevelTransition>();
        yInteract.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (!canControl)
        {
            Transition();
            thisAnimator.SetBool("Transitioning", true);

            thisAnimator.SetBool("Jump", false);
            thisAnimator.SetBool("HoldJump", false);
            thisAnimator.SetBool("Falling", true);
            thisAnimator.SetBool("Attacking", false);
            thisAnimator.SetBool("Dash", false);
        }
        else
        {
            thisAnimator.SetBool("Transitioning", false);
            GetInput();
            GroundCheck();
            CeilingCheck();
        }
        if(thisRigidBody2D.velocity.magnitude > 40)
        {
            thisRigidBody2D.velocity = new Vector2(0, 0);
        }
    }

    void Transition()
    {
        thisRigidBody2D.velocity = new Vector2(0.0f, thisRigidBody2D.velocity.y);
    }

    void GroundCheck()
    {
        if(groundColliderCheck.IsTouchingLayers(groundLayer))
        {
            isGrounded = true;
            falling = false;
            fastFall = false;

        }
        else
        {
            isGrounded = false;
        }


    }

    void CeilingCheck()
    {
        if(ceilingColliderCheck.IsTouchingLayers(groundLayer))
        {
            touchedCeil = true;
        }
        else
        {
            touchedCeil = false;    
        }
    }

    
    void GetInput()
    {
        if(pausedGame)
        { return; }
       // xInput = Input.GetAxis("Horizontal");
        //yInput = Input.GetAxis("Vertical");
       // slowWalkInput = Input.GetAxisRaw("SlowWalk");
        lastJumpInput = jumpInput;
        // jumpInput = Input.GetAxisRaw("Jump");


        lastInteractInput = interactInput;
        interactInput = Input.GetAxisRaw("Interact");
    }


   

   public void SetLevel(LevelManagerScript lv)
    {
        currentLevelScript = lv;
    }

    public void LevelTransition()
    {
        levelTransScript.Transition();

    }

    public void SetPlayerPositionWithDoors(string currentDoor, string conenctedDoor)
    {
        StartCoroutine(WaitForLevelLoad(currentDoor,  conenctedDoor));
    }

    IEnumerator WaitForLevelLoad(string currentDoor, string conenctedDoor)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        GameObject[] doorways = GameObject.FindGameObjectsWithTag("Doorway");

        foreach (GameObject door in doorways)
        {

            if (door.name == conenctedDoor + "_to_" + currentDoor)
            {
                Door.side testSide = door.GetComponent<DoorwayScript>().thisDoorSide;

                switch(testSide)
                {
                    case Door.side.UP:
                        transform.position = door.transform.position - (Vector3.up*2);
                        break;

                    case Door.side.DOWN:
                        transform.position = door.transform.position + (Vector3.up*2);
                        break;

                    case Door.side.RIGHT:
                        transform.position = door.transform.position - (Vector3.right*2);
                        break;

                    case Door.side.LEFT:
                        transform.position = door.transform.position + (Vector3.right*2);
                        break;

                    default:
                        break;
                }

                
                Physics2D.IgnoreCollision(door.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                yield return new WaitForSecondsRealtime(1f);
                Physics2D.IgnoreCollision(door.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
                

                break;
            }
        }
    }

}
