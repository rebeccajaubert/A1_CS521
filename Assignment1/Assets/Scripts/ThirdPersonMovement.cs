
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{

    public CharacterController controller;
    public Transform cam;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    float gravity = 9.8f;
    float verticalVelocity = 0f;
    float jumpSpeed = 4f;

    // Update is called once per frame
    void Update()
    {
       

        float horizontal = Input.GetAxisRaw("Vertical");
        float vertical = -Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        
        verticalVelocity -= gravity * Time.deltaTime; 
      //  if (controller.isGrounded) verticalVelocity = 0f;
        Vector3 gravityVector = new Vector3(0f, verticalVelocity, 0f);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(-direction.z, direction.x) * Mathf.Rad2Deg + cam.eulerAngles.y; // add cam angle so that player in direction of camera
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //to smooth player s turn
            transform.rotation = Quaternion.Euler(0f, angle, 0f); //player rotate 

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; //move in direction of camera
            if (Input.GetKeyDown("space"))
            { // unless it jumps:
                verticalVelocity = jumpSpeed;
                gravityVector.y = verticalVelocity;
            }
            controller.Move( (gravityVector.normalized + moveDir.normalized) * speed * Time.deltaTime);
        }
        
        if (Input.GetKeyDown("space"))
        { // unless it jumps:
            verticalVelocity = jumpSpeed;
            gravityVector.y = verticalVelocity;
            controller.Move(gravityVector);
        }
        if (!Input.anyKey)
        {
            //controller.Move(gravityVector *50);
            controller.SimpleMove(gravityVector*100);
        }

    }

}
