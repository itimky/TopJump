using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    private int minSwipeDist = 35;
    private int maxSwipeTime = 4;
    //    private float tapSec = 1f;
    //    private float tapMaxSwipeDist = 50;

    public bool IsTouchSupported { get; private set; }

    private float fingerStartTime = 0.0f;
    private Vector2 fingerStartPos = Vector2.zero;

    private bool isSwiping = false;


    //    public static bool Tap { get; private set; }

    public static bool Back { get; private set; }

    public static bool Swipe { get; private set; }

    public static MoveDirection SwipeDirection { get; private set; }


    //    private bool isTapping;

    void Start()
    {
//        Game.RegisterPausableObject(this);
        fingerStartTime = 0.0f;
        if (SystemInfo.deviceType == DeviceType.Handheld)
            IsTouchSupported = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (IsTouchSupported)
            ProcessTouch();
        else
            ProcessInput();

        if (Input.GetKeyDown(KeyCode.Escape))
            Back = true;
        else
            Back = false;
    }


    private void ProcessInput()
    {        
        if (Input.GetButtonDown("Horizontal"))
        {
            var axis = Input.GetAxis("Horizontal");
            if (axis > 0)
                SwipeDirection = MoveDirection.Right;
            else
                SwipeDirection = MoveDirection.Left;          
        }
        else
            SwipeDirection = MoveDirection.None;
        
//        if (Input.GetButtonDown("Jump"))
//            Tap = true;
//        else
//            Tap = false;
    }


    private void ProcessTouch()
    {
//        Tap = false;

        if (Input.touchCount == 0)
            return;
             
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                        //this is a new touch
                    isSwiping = true;
//                    isTapping = true;
                    fingerStartTime = Time.time;
                    fingerStartPos = touch.position;
                    break;
                case TouchPhase.Canceled:
                        //The touch is being canceled
                    isSwiping = false;
//                    isTapping = false;
                    break;
                case TouchPhase.Moved:
                    if (isSwiping)
                        SetSwipe(touch);
                    break;
                case TouchPhase.Ended:
//                    if (isTapping)
//                        SetTap(touch);
//                    isTapping = false;
                    isSwiping = false;
                    break;                        
            }
        }
    }

    private Vector2 GetSwipeType(Vector2 position)
    {
        Vector2 direction = position - fingerStartPos;
        Vector2 swipeType = Vector2.zero;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // the swipe is horizontal:
            swipeType = Vector2.right * Mathf.Sign(direction.x);
        }
        else
        {
            // the swipe is vertical:
            swipeType = Vector2.up * Mathf.Sign(direction.y);
        }

        return swipeType;
    }
    //
    //    private void SetTap(Touch touch)
    //    {
    //        if (touch.tapCount == 1)
    //            Tap = true;
    ////        float gestureTime = Time.time - fingerStartTime;
    ////        float gestureDist = (touch.position - fingerStartPos).magnitude;
    ////
    ////        if (gestureTime < tapSec && gestureDist < tapMaxSwipeDist)
    ////            Tap = true;
    //    }

    private void SetSwipe(Touch touch)
    {        
        float gestureTime = Time.time - fingerStartTime;
        float gestureDist = (touch.position - fingerStartPos).magnitude;

        if (gestureTime < maxSwipeTime && gestureDist > minSwipeDist)
        {
            var swipeType = GetSwipeType(touch.position);                           
            if (swipeType.x != 0.0f)
            {
                isSwiping = false;
                if (swipeType.x > 0.0f)
                    SwipeDirection = MoveDirection.Right;
                else
                    SwipeDirection = MoveDirection.Left;                              
            }                       
        }
        else
            SwipeDirection = MoveDirection.None;
    }
}

