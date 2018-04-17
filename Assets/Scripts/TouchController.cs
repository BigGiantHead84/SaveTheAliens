using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchController : MonoBehaviour
{
    GameObject player;
    Transform playerTrans;
    public float maxRotationZ;
    public float rotationSpeed;

    GameObject leftThrust;
    GameObject rightThrust;
    Rigidbody2D rbPlayer;
    CircleCollider2D GroundCheck;
    public float forceX;
    public float forceY;
    private Vector2 fp; // first finger position
    private Vector2 lp; // last finger position
    private float angle;
    private float swipeDistanceX;
    private float swipeDistanceY;
    public PhysicControls currentControl;
    GameObject touchText;
    bool isTouchingGround = false;


    public enum PhysicControls
    {
        direct, thrusters
    }

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        playerTrans = player.GetComponent<Transform>();
        GroundCheck = GetComponent<CircleCollider2D>();
        touchText = GameObject.Find("TouchCText");
        print("Quoaternion identity rotation: " + player.transform.localEulerAngles);

        leftThrust = GameObject.FindWithTag("Thruster_left");
        rightThrust = GameObject.FindWithTag("Thruster_right");

        rbPlayer = player.GetComponent<Rigidbody2D>();

        if (currentControl == PhysicControls.direct)
        {
            Debug.Log("Direct, disabling thrusters...");
            leftThrust.SetActive(false);
            rightThrust.SetActive(false);
            touchText.GetComponent<Text>().text = "Direct";
        }
        else if (currentControl == PhysicControls.thrusters)
        {
            touchText.GetComponent<Text>().text = "Thrusters";
        }
#if UNITY_EDITOR
        Debug.Log("Unity Editor");
#elif UNITY_ANDROID 
        Debug.Log("Unity Android");
#endif
    }

    // Update is called once per frame
    void Update()
    {
        //print("Euler z rotation of Player: " + player.transform.localEulerAngles.z);
#if UNITY_EDITOR
        MouseMovement();

#elif UNITY_ANDROID
       
        TouchMovement();
#endif

    }

    public void ToggleThrusters()
    {
        if (currentControl == PhysicControls.direct)
        {
            currentControl = PhysicControls.thrusters;
            print("Thrusters in use");
            touchText.GetComponent<Text>().text = "Thrusters";
        }
        else
        {
            currentControl = PhysicControls.direct;
            print("Direct controls in use");
            touchText.GetComponent<Text>().text = "Direct";
        }
    }

    void HandleRotationRight()
    {
        print(playerTrans.localEulerAngles.z.ToString());
        if (playerTrans.localEulerAngles.z == 0 || playerTrans.localEulerAngles.z > 0 && playerTrans.localEulerAngles.z >= 340 || playerTrans.localEulerAngles.z < 33)
            player.transform.Rotate(0, 0, -0.5f);
    }
    void HandleRotationLeft()
    {
        print(playerTrans.localEulerAngles.z.ToString());
        if (player.transform.localEulerAngles.z == 0 || player.transform.localEulerAngles.z <= 20 || player.transform.localEulerAngles.z > 317)
            player.transform.Rotate(0, 0, 0.5f);
    }

    void LevelRotation()
    {
        if (isTouchingGround == false)
        {
            if (player.transform.localEulerAngles.z < 180)
            {
                player.transform.Rotate(0, 0, -1.2f);

            }
            else if (player.transform.localEulerAngles.z > 180)
            {
                player.transform.Rotate(0, 0, 1.2f);
            }
        }

    }
    void MouseMovement()
    {
        if (Input.GetMouseButton(0) && Input.mousePosition.x > Screen.width / 2) //mouse clicked right side
        {
            if (currentControl == PhysicControls.direct)
            {
                rbPlayer.AddForce(new Vector2(forceX, forceY), ForceMode2D.Force);
                HandleRotationRight();
            }
            else // PhysicControls are Thrusters
            {
                rbPlayer.AddForce(-leftThrust.transform.up * 15.4f);
                HandleRotationRight();
            }
        }
        else if (Input.GetMouseButton(0) && Input.mousePosition.x < Screen.width / 2) // Mouse clicked left side
        {
            if (currentControl == PhysicControls.direct)
            {
                rbPlayer.AddForce(new Vector2(-forceX, forceY), ForceMode2D.Force);
                HandleRotationLeft();
            }
            else
            {
                rbPlayer.AddForce(-rightThrust.transform.up * 15.4f);
                HandleRotationLeft();
            }
        }
        else // If not clicking on mouse, handle rotation anyways
        {
            LevelRotation();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            rbPlayer.freezeRotation = false;
            print("Touching ground");
            isTouchingGround = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            rbPlayer.freezeRotation = true;
            print("Left the ground");
            isTouchingGround = false;
        }
    }
    void TouchMovement()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                if (currentControl == PhysicControls.direct)
                {
                    if (Input.GetTouch(0).position.x > Screen.width / 2) // Right side
                    {
                        rbPlayer.AddForce(new Vector2(forceX, forceY), ForceMode2D.Force);
                        HandleRotationRight();
                    }
                    else if (Input.GetTouch(0).position.x < Screen.width / 2) // Left side
                    {
                        rbPlayer.AddForce(new Vector2(-forceX, forceY), ForceMode2D.Force);
                        HandleRotationLeft();
                    }
                }
                else if (currentControl == PhysicControls.thrusters)
                {
                    if (Input.GetTouch(0).position.x > Screen.width / 2) // right side of the screen
                    {
                        rbPlayer.AddForce(-rightThrust.transform.up * 15.4f);
                        HandleRotationRight();
                    }
                    else if (Input.GetTouch(0).position.x < Screen.width / 2) // left side of the screen
                    {
                        rbPlayer.AddForce(-leftThrust.transform.up * 15.4f);
                        HandleRotationLeft();
                    }
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            if (Input.GetTouch(1).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Stationary)
            {
                if (Input.GetTouch(0).position.x > Screen.width / 2 && Input.GetTouch(1).position.x < Screen.width / 2 || Input.GetTouch(1).position.x > Screen.width / 2 && Input.GetTouch(0).position.x < Screen.width / 2)
                {
                    rbPlayer.AddForce(new Vector2(0, forceY*2), ForceMode2D.Force);
                }
            }
        }
        else
        {
            LevelRotation();
        }
    }
}
