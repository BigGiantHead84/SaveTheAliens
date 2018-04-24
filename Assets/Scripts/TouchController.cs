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
    CircleCollider2D groundCheck;
    public float forceX;
    public float forceY;
    private Vector2 fp; // first finger position
    private Vector2 lp; // last finger position
    private float angle;
    private float swipeDistanceX;
    private float swipeDistanceY;

    public PhysicControls forceControls;
    public ControlMode currentControls = ControlMode.screen;

    GameObject touchText;
    GameObject[] buttonControls;

    bool isTouchingGround = false;

    public enum ControlMode
    {
        screen, buttons, dpad
    }

    public enum PhysicControls
    {
        direct, thrusters
    }

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        playerTrans = player.GetComponent<Transform>();
        groundCheck = GetComponent<CircleCollider2D>();
        touchText = GameObject.Find("TouchCText");


        buttonControls = GameObject.FindGameObjectsWithTag("ButtonControls");
        ShowHideOnScreenControls();

        leftThrust = GameObject.FindWithTag("Thruster_left");
        rightThrust = GameObject.FindWithTag("Thruster_right");

        rbPlayer = player.GetComponent<Rigidbody2D>();

        if (forceControls == PhysicControls.direct)
        {
            Debug.Log("Direct, disabling thrusters...");
            leftThrust.SetActive(false);
            rightThrust.SetActive(false);
            HandleDebugTextsOnScreen();
        }
        else if (forceControls == PhysicControls.thrusters)
        {
            HandleDebugTextsOnScreen();
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

    void HandleDebugTextsOnScreen()
    {
        touchText.GetComponent<Text>().text = "Force is controlled by: " + forceControls.ToString();
    }
    public void ToggleThrusters()
    {
        if (forceControls == PhysicControls.direct)
        {
            forceControls = PhysicControls.thrusters;
            HandleDebugTextsOnScreen();
        }
        else
        {
            forceControls = PhysicControls.direct;
            HandleDebugTextsOnScreen();
        }
    }

    void ShowHideOnScreenControls()
    {
        if (currentControls == ControlMode.buttons)
        {
            foreach (GameObject onScreenButton in buttonControls)
            {
                onScreenButton.SetActive(true);
                onScreenButton.GetComponent<Image>().enabled = true;

            }
        }
        else if (currentControls == ControlMode.screen)
        {
            foreach (GameObject onScreenButton in buttonControls)
            {
                onScreenButton.SetActive(false);
                onScreenButton.GetComponent<Image>().enabled = false;

            }
        }
    }
    public void ToggleControls()
    {
        if (currentControls == ControlMode.screen)
        {
            currentControls = ControlMode.buttons;
            ShowHideOnScreenControls();

        }
        else
        {
            currentControls = ControlMode.screen;
            ShowHideOnScreenControls();
        }
    }
    public void ForceRight()
    {
        if (forceControls == PhysicControls.direct)
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
    public void ForceLeft()
    {
        if (forceControls == PhysicControls.direct)
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
    public void ForceUp()
    {
        rbPlayer.AddForce(new Vector2(0, forceY * 2), ForceMode2D.Force);
    }

    public void HandleRotationRight()
    {
        print("Rotation z: " +playerTrans.localEulerAngles.z.ToString());
        if (playerTrans.localEulerAngles.z == 0 || playerTrans.localEulerAngles.z > 0 && playerTrans.localEulerAngles.z >= 340 || playerTrans.localEulerAngles.z < 33)
            player.transform.Rotate(0, 0, -0.5f);
    }
    public void HandleRotationLeft()
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

    private void HandleOnScreenTouchControls()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary)
            {

                if (Input.GetTouch(0).position.x > Screen.width / 2) // Right side
                {
                    ForceRight();
                }
                else if (Input.GetTouch(0).position.x < Screen.width / 2) // Left side
                {
                    ForceLeft();
                }

            }
        }
        else if (Input.touchCount == 2)
        {
            if (Input.GetTouch(1).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Stationary)
            {
                if (Input.GetTouch(0).position.x > Screen.width / 2 && Input.GetTouch(1).position.x < Screen.width / 2 || Input.GetTouch(1).position.x > Screen.width / 2 && Input.GetTouch(0).position.x < Screen.width / 2)
                {
                    ForceUp();
                }
            }
        }
        else
        {
            LevelRotation();
        }
    }

    private void HandleButtonControls()
    {
        return;
    }
    private void HandleOnScreenMouseControls()
    {
        if (Input.GetMouseButton(0) && Input.mousePosition.x > Screen.width / 2) //mouse clicked right side
        {
            ForceRight();
        }
        else if (Input.GetMouseButton(0) && Input.mousePosition.x < Screen.width / 2) // Mouse clicked left side
        {
            ForceLeft();
        }
        else
        {
            LevelRotation();
        }
    }

    void TouchMovement()
    {
        if (currentControls == ControlMode.screen)
        {
            HandleOnScreenTouchControls();
        }
        else if (currentControls == ControlMode.buttons)
        {
            HandleButtonControls();
        }

    }
    void MouseMovement()
    {
        if (currentControls == ControlMode.screen)
        {
            HandleOnScreenMouseControls();
        }
        else if (currentControls == ControlMode.buttons)
        {
            HandleButtonControls();
        }

       
    }
}
