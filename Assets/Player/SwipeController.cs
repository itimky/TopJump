using UnityEngine;
using System;
using System.Collections;

public class SwipeController : Pausable
{
    public float minSwipeDist;
    public float maxSwipeTime;

    public bool IsTouchSupported { get;	private set; }

    private float fingerStartTime = 0.0f;
    private Vector2 fingerStartPos = Vector2.zero;
	
    private bool isSwipe = false;

    void Start()
    {
        Game.RegisterPausableObject(this);
        fingerStartTime = 0.0f;
        if (SystemInfo.deviceType == DeviceType.Handheld)
            IsTouchSupported = true;
    }

    void Update()
    {
        MoveDirection direction;
        if (IsTouchSupported)
            direction = GetSwipeDirection();
        else
            direction = GetInputDireciton();
            
        if (direction != MoveDirection.None)
            GetComponent<MoveController>().QueueMove(direction);

        if (Input.GetKeyDown(KeyCode.Escape))
            Game.Pause();
    }

    private MoveDirection GetInputDireciton()
    {
        if (Input.GetButtonDown("Horizontal"))
        {
            var axis = Input.GetAxis("Horizontal");
            if (axis > 0)
                return MoveDirection.Right;
            else
                return MoveDirection.Left;			
        }

        return MoveDirection.None;
    }

    private MoveDirection GetSwipeDirection()
    {
        if (Input.touchCount > 0)
        {			
            foreach (Touch touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
						//this is a new touch
                        isSwipe = true;
                        fingerStartTime = Time.time;
                        fingerStartPos = touch.position;
                        return MoveDirection.None;
						
                    case TouchPhase.Canceled:
						//The touch is being canceled
                        isSwipe = false;
                        return MoveDirection.None;
						
                    case TouchPhase.Moved:
                        if (!isSwipe)
                            return MoveDirection.None;
                        float gestureTime = Time.time - fingerStartTime;
                        float gestureDist = (touch.position - fingerStartPos).magnitude;
						
                        if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist)
                        {
                            var swipeType = GetSwipeType(touch.position);							
                            if (swipeType.x != 0.0f)
                            {
                                isSwipe = false;
                                if (swipeType.x > 0.0f)
                                    return MoveDirection.Right;
                                else
                                    return MoveDirection.Left;								
                            }							
                            //if(swipeType.y != 0.0f ) {
                            //	if(swipeType.y > 0.0f)
                            //		Swipe(SwipeDirection.Up);
                            //	else
                            //		Swipe(SwipeDirection.Down);
                            //
                            //}							
                        }
						
                        return MoveDirection.None;
                }
            }
        }

        return MoveDirection.None;
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
}