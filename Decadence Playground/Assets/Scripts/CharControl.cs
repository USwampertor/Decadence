using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControl : MonoBehaviour {
    private enum PLAYER_STATE
    {
        WALKING,
        CLIMBING
    }

    [SerializeField]
    float minSpeed = 15, maxSpeed = 30, acceleration = .5f, currentSpeed, jumpHeight = 9f, jumpSpeed = 9f;

    public bool isHolding, isFloored, isGravityOn, isClimbing;
    GameObject carryingObject = null;
    IneractableObject objectScript;
    Vector3 gravity;
    Rigidbody rb;

    Vector3 resetPosition;
    
    int floorMask;
    float camRayLenght = 100;

    Vector3 forward, right;

    PLAYER_STATE state;

    [SerializeField]
    bool jump = false;
    // Use this for initialization

    void Start () {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        rb = GetComponent<Rigidbody>();
        isHolding = false;
        state = PLAYER_STATE.WALKING;
        currentSpeed = minSpeed;
        gravity = Vector3.down;

        resetPosition = rb.position;
        //objectScript = carryingObject.GetComponent<IneractableObject>();
    }
	
	// Update is called once per frame
	void Update () {
        //Checks if position needs to be reset
        ResetPosition();

        //transform.position -= transform.up * Time.deltaTime * jumpSpeed;
        if(!isFloored)
            rb.velocity = new Vector3(0, -5, 0);

        if (isHolding && carryingObject)
        {
            carryingObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2.0f);
        }
        
        //LookAtmouse();

        if (Input.GetButtonDown("Jump") && !jump)
            StartCoroutine(Jump());
        //else if (Input.GetButtonDown("Action"))
        //    PickUp();
        if (state == PLAYER_STATE.WALKING)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                    Move();

            else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 && Input.GetAxis("RightStickX") != 0 || Input.GetAxis("RightStickY") != 0)
                MoveJoystick();
            //MoveDualShock();
        }
        if(state==PLAYER_STATE.CLIMBING)
        {
            if (Input.GetAxis("Vertical") != 0)
                ClimbStairs();

            //else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 && Input.GetAxis("RightStickX") != 0 || Input.GetAxis("RightStickY") != 0)
            //    MoveJoystick();
        }
    }

    void Move()
    {
        //currentSpeed = Mathf.SmoothStep(currentSpeed, maxSpeed, 2 * Time.deltaTime);
        if (Input.GetButton("Sprint"))
            currentSpeed = maxSpeed;
        else
            currentSpeed = minSpeed;
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 rightMovement = right * currentSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        Vector3 upMovement = forward * currentSpeed * Time.deltaTime * Input.GetAxis("Vertical");

        
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        if (heading!=Vector3.zero)
        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }

    void MovePeroBien()
    {
        //currentSpeed = Mathf.SmoothStep(currentSpeed, maxSpeed, 2 * Time.deltaTime);
        if (Input.GetButton("Sprint"))
            currentSpeed = maxSpeed;
        else
        {
            if(Input.GetAxis("Accelerate") > 0 || Input.GetAxis("Accelerate") < 0)
            {

                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += acceleration;
                    Debug.Log("Velocidad: " + currentSpeed);
                }
                else
                {
                    currentSpeed += maxSpeed;
                }
            }
            else if (Input.GetAxis("Accelerate") == 0)
            {
                currentSpeed = minSpeed;
                Debug.Log("Velocidad sin aceleración: " + currentSpeed);
            }
        }
        //currentSpeed = minSpeed;
        
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 rightMovement = right * currentSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        Vector3 upMovement = forward * currentSpeed * Time.deltaTime * Input.GetAxis("Vertical");


        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        if (heading != Vector3.zero)
            transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }

    void MoveDualShock()
    {
       

        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 rightMovement = right * currentSpeed * Time.deltaTime * Input.GetAxisRaw("Horizontal");
        Vector3 upMovement = forward * currentSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");
        Vector3 lookDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    void MoveJoystick()
    {


        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 rightMovement = right * currentSpeed * Time.deltaTime * Input.GetAxisRaw("Horizontal");
        Vector3 upMovement = forward * currentSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");
        Vector3 lookDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        rb.AddForce(heading * currentSpeed * Input.GetAxis("Vertical"));

        //input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //float t = input.magnitude;
        //if (t > 0)
        //    rb.AddForce(input * t);

        //transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
        //transform.rotation = Quaternion.LookRotation(lookDirection);
    }
   

    IEnumerator Jump()
    {
        float ogHeight = transform.position.y;
        float maxHeight = ogHeight + jumpHeight;
        rb.useGravity = false;
        //isGravityOn = false;

        jump = true;
        
        yield return null;

        while(transform.position.y < maxHeight  )
        {
            transform.position += transform.up * Time.deltaTime * jumpSpeed;
            yield return null;
        }

        while (transform.position.y > ogHeight)
        {
            //transform.position -= transform.up * Time.deltaTime * jumpSpeed;
            isFloored = false;
            yield return null;

        }

        rb.useGravity = true;
        isGravityOn = true;
        jump = false;

        yield return null;

    }

    void LookAtmouse()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, camRayLenght, floorMask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;

            playerToMouse.y = 0f;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

            rb.MoveRotation(newRotation);
        }

    }

    void ClimbStairs()
    {
        Vector3 upMovement = new Vector3(0, 1, 0) * currentSpeed     * Time.deltaTime * Input.GetAxis("Vertical");

        transform.position += upMovement;
    }

    //void Interact()
    //{
    //    int sphereRadius = 5;
    //    //
    //    if (isHolding)
    //    {
    //        carryingObject.GetComponent<Rigidbody>().useGravity = true;
    //        // we don't have anything to do with our ball field anymore
    //        carryingObject = null;
    //        //Unparent our ball
    //        transform.GetChild(0).parent = null;
    //        isHolding = false;
    //    }
    //    else
    //    {
    //        Collider[] nearObjects = Physics.OverlapSphere(transform.position, sphereRadius);
    //        foreach (Collider obj in nearObjects)
    //        {
    //            //objectScript = obj.GetComponent<IneractableObject>();
    //            if (obj.GetComponent<IneractableObject>().size == IneractableObject.SIZE.SMALL)
    //                PickUp();
    //            else if (obj.GetComponent<IneractableObject>().size == IneractableObject.SIZE.MED)
    //                Drag();
    //            else if (obj.GetComponent<IneractableObject>().size == IneractableObject.SIZE.BRK)
    //                BreakObject();
    //        }
    //    }
    //}

    //void PickUp()
    //{
    //    int sphereRadius = 5;
    //    //
    //    if (isHolding)
    //    {
    //        carryingObject.GetComponent<Rigidbody>().useGravity = true;
    //        // we don't have anything to do with our ball field anymore
    //        carryingObject = null;
    //        //Unparent our ball
    //        transform.GetChild(0).parent = null;
    //        isHolding = false;
    //    }
    //    else
    //    {
    //        Collider[] nearObjects = Physics.OverlapSphere(transform.position, sphereRadius);
    //        foreach (Collider obj in nearObjects)
    //        {

    //            if (obj.gameObject.tag == "Interactable")
    //            {
    //                carryingObject = obj.gameObject;
    //                carryingObject.transform.SetParent(transform);
    //                carryingObject.GetComponent<Rigidbody>().useGravity = false;

    //                //we apply the same rotation our main object (Camera) has.
    //                carryingObject.transform.localRotation = transform.rotation;
    //                isHolding = true;
    //            }
    //        }
    //    }
    //}

    void Drag()
    {

    }

    void BreakObject()
    {

    }

    void ResetPosition()
    {
        if (rb.position.y < -2.0f || Input.GetKeyDown(KeyCode.R))
        {
            rb.position = resetPosition;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Stairs")
        {
            Debug.Log("Encontré escaleras.");
        }
        else if (collision.gameObject.tag == "Floor")
        {
            isFloored = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Stairs" && Input.GetButtonDown("Action") && state == PLAYER_STATE.WALKING)
        {
            Debug.Log("Voy a subir escaleras.");
            if (!isClimbing)
            {
                state = PLAYER_STATE.CLIMBING;
                isGravityOn = false;
                rb.useGravity = false;
            }
            else
            {
                state = PLAYER_STATE.WALKING;
                isGravityOn = false;
                rb.useGravity = true;
            }
            
        }
        else if (state == PLAYER_STATE.CLIMBING)
        {
            if (Input.GetButtonDown("Action"))
            {
                state = PLAYER_STATE.WALKING;
                rb.useGravity = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Stairs")
        {
            Debug.Log("Ya me quité de las escaleras.");
            state = PLAYER_STATE.WALKING;
            isGravityOn = true;
            rb.useGravity = true;
        }
        else if (collision.gameObject.tag == "Floor")
        {
            isFloored = false;
        }
    }
}
