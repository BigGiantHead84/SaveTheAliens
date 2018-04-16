using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchController : MonoBehaviour
{
    GameObject player;
    Animator playerAnim;
    Animation playerAnimation;
    AnimationState pAnimState;

    GameObject leftThrust;
    GameObject rightThrust;
    Rigidbody2D rbPlayer;
    public float forceX;
    public float forceY;
    private Vector2 fp; // first finger position
    private Vector2 lp; // last finger position
    private float angle;
    private float swipeDistanceX;
    private float swipeDistanceY;
    public PhysicControls currentControl;
    

    public enum PhysicControls
    {
        direct, thrusters
    }

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        print("Quoaternion identity rotation: " + player.transform.localEulerAngles);
        playerAnim = player.GetComponent<Animator>();
        playerAnimation = player.GetComponent<Animation>();

        leftThrust = GameObject.FindWithTag("Thruster_left");
        rightThrust = GameObject.FindWithTag("Thruster_right");

        rbPlayer = player.GetComponent<Rigidbody2D>();

        if (currentControl == PhysicControls.direct)
        {
            Debug.Log("Direct, disabling thrusters...");
            leftThrust.SetActive(false);
            rightThrust.SetActive(false);
        }
        else if (currentControl == PhysicControls.thrusters)
        {
            Debug.Log("Thrusters, enabling thruster-mode");
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
        print("Euler z rotation of Player: " + player.transform.localEulerAngles.z);
#if UNITY_EDITOR
        MouseMovement();

#elif UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                if (currentControl == PhysicControls.direct)
                {
                    if (Input.GetTouch(0).position.x > Screen.width / 2)
                    {
                        rbPlayer.AddForce(new Vector2(forceX, forceY), ForceMode2D.Force);
                    }
                    else if (Input.GetTouch(0).position.x < Screen.width / 2)
                    {
                        rbPlayer.AddForce(new Vector2(-forceX, forceY), ForceMode2D.Force);
                    }
                }
                else if (currentControl == PhysicControls.thrusters)
                {
                    if (Input.GetTouch(0).position.x > Screen.width / 2) // right side of the screen
                    {
                        rbPlayer.AddForceAtPosition(-leftThrust.transform.up * 8.4f, leftThrust.transform.position);
                    }
                    else if (Input.GetTouch(0).position.x < Screen.width / 2) // left side of the screen
                    {
                        rbPlayer.AddForceAtPosition(-rightThrust.transform.up * 8.4f, rightThrust.transform.position);
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
                    rbPlayer.AddForce(new Vector2(0, forceY), ForceMode2D.Force);
                }
            }
        }
        else
        {
            return;
        }

#endif

        }

    void HanleRotation()
    {

    }
    void MouseMovement()
    {
        if (Input.GetMouseButton(0) && Input.mousePosition.x > Screen.width / 2) //mouse clicked right side
        {
            print("Right side");
            if (currentControl == PhysicControls.direct)
            {
                rbPlayer.AddForce(new Vector2(forceX, forceY), ForceMode2D.Force);

                if (player.transform.localEulerAngles.z == 0 || player.transform.localEulerAngles.z >= 320 || player.transform.localEulerAngles.z < 30)
                    player.transform.Rotate(0, 0, -0.2f);
            }
            else // PhysicControls are Thrusters
            {
                rbPlayer.AddForce(-leftThrust.transform.up * 15.4f);
            }
        }
        else if (Input.GetMouseButton(0) && Input.mousePosition.x < Screen.width / 2) // Mouse clicked left side
        {
            print("Left side");
            if (currentControl == PhysicControls.direct)
            {
                rbPlayer.AddForce(new Vector2(-forceX, forceY), ForceMode2D.Force);
                if (player.transform.localEulerAngles.z == 0 || player.transform.localEulerAngles.z < 30 || player.transform.localEulerAngles.z >= 320)
                    player.transform.Rotate(0, 0, 0.2f);
            }
            else
            {
                rbPlayer.AddForce(-rightThrust.transform.up * 15.4f);
            }
        }
        else // If not clicking on mouse
        {
            if (player.transform.localEulerAngles.z > 0 && player.transform.localEulerAngles.z < 32)
                player.transform.Rotate(0, 0, -0.2f);
            else if (player.transform.localEulerAngles.z > 0 && player.transform.localEulerAngles.z > 318)
                player.transform.Rotate(0, 0, 0.2f);
        }
    }
    void TouchMovement()
    {

    }
    }
