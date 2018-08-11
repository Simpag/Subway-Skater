using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour {

    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05f;

    //
    private bool isRunning = false;

    //Animation
    private Animator anim;

    //Movement
    private CharacterController controller;
    private float jumpForce = 4.5f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private int desiredLane = 1; // 0 = left, 1 = middle, 2 = right

    //Speed modifier
    private float originalSpeed = 7.0f;
    private float speed;
    private float speedIncreaseLastTick;
    private float speedIncreaseDelay = 2.5f;
    private float speedIncreaseAmount = 0.1f;

    private void Start()
    {
        speed = originalSpeed;

        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isRunning) //Don't run the update loop if the game isnt started
            return;

        if(Time.time - speedIncreaseLastTick > speedIncreaseDelay)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;

            //Change modifier score text
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }

        //Gather inputs on which lane we should be
        //Move Left
        if (MobileInput.Instance.SwipeLeft)
            MoveLane(false);

        //Move Right
        if (MobileInput.Instance.SwipeRight)
            MoveLane(true);

        //Calculate where we should be
        //Start off in the middle lane
        Vector3 targetPosition = transform.position.z * Vector3.forward;

        if (desiredLane == 0)  
            targetPosition += Vector3.left * LANE_DISTANCE;
        else if (desiredLane == 2)
            targetPosition += Vector3.right * LANE_DISTANCE;

        //Calculate move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

        //Y position
        if (isGrounded) //Grounded
        {
            verticalVelocity = -0.1f;

            if (MobileInput.Instance.SwipeUp)
            {
                //Jump
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if (MobileInput.Instance.SwipeDown)
            {
                //Slide
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        }
        else
        {
            //Wont fall down far so termal velocity will never be reached
            verticalVelocity -= (gravity * Time.deltaTime);

            //Swipe down, fall faster
            if (MobileInput.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce;
            }
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        //Move the pengu
        controller.Move(moveVector * Time.deltaTime);

        //Rotate the pengu
        Vector3 dir = controller.velocity;
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
        }
    }

    private void StartSliding()
    {
        anim.SetBool("Sliding", true);
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
    }

    private void StopSliding()
    {
        anim.SetBool("Sliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }

    private void MoveLane (bool goingRight)
    {
        //If going right, add one, else (left) sub one
        desiredLane += (goingRight) ? 1 : -1;

        //Clamp between lanes
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(
            new Vector3(    //Origin
                controller.bounds.center.x,
                (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f,
                controller.bounds.center.z),
            Vector3.down);   //Direction

        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

    public void StartRunning()
    {
        isRunning = true;
        anim.SetTrigger("StartRunning");
    }

    private void Crash()
    {
        anim.SetTrigger("Death");
        isRunning = false;
        GameManager.Instance.OnDeath();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;
        }
    }
}
