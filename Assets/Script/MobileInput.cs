using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour {

    private const float DEADZONE = 100.0f;

    public static MobileInput Instance { set; get; }

    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private Vector2 swipeDelta, startTouch;

    public bool Tap { get { return tap;  } }
    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //Reset all variables
        ResetVariables();

        //Check for inputs

        #region Standalone Inputs

        if (Input.GetMouseButtonDown(0)) //Left click
        {
            tap = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            startTouch = swipeDelta = Vector2.zero;
        }

        #endregion

        #region Mobile Inputs

        if (Input.touches.Length != 0) //Atleast one touch
        {
            if (Input.touches[0].phase == TouchPhase.Began) //Only read first touch
            {
                tap = true;
                startTouch = Input.mousePosition;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) //If the touch is released or touch ended
            {
                startTouch = swipeDelta = Vector2.zero;
            }
        }

        #endregion

        //Calculate swipe distance
        swipeDelta = Vector2.zero;
        if (startTouch != Vector2.zero) //If a touch has started
        {
            //Check mobile touch
            if (Input.touches.Length != 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
            //Check standalone touch
            else if (Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }

        //Check if the swipe is beyond the deadzone, make sure that it was not a tap
        if (swipeDelta.magnitude > DEADZONE)
        {
            //Confirmed swipe

            //Calculate direction
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                //Left or right swipe
                if (x < 0)
                {
                    swipeLeft = true;
                }
                else
                {
                    swipeRight = true;
                }
            }
            else
            {
                //Up or downwards swipe
                if (y < 0)
                {
                    swipeDown = true;
                }
                else
                {
                    swipeUp = true;
                }
            }

            startTouch = swipeDelta = Vector2.zero; //Do not want two swipes within one fram of eachother
        }

    }

    private void ResetVariables()
    {
        tap = false;
        swipeLeft = false;
        swipeRight = false;
        swipeUp = false;
        swipeDown = false;
    }
}
