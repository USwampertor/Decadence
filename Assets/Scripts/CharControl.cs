using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharControl : MonoBehaviour {
    public enum PLAYER_STATE
    {
        WALKING,
        CLIMBING,
        GRABING,
    }
    private enum PLAYER_INTERCTION
    {
        DEFAULT,
       
        PICKING
    }

    [SerializeField]
    float minSpeed, maxSpeed, acceleration = .5f, currentSpeed, jumpHeight = 9f, jumpSpeed = 9f;

    public bool isHolding, isFloored, isGravityOn, isClimbing, onRange;
    GameObject carryingObject = null;
    IneractableObject objectScript;
    Vector3 gravity;
    Rigidbody rb;
    public GameObject m_interactObject;
    public bool hasKey = false;
    Vector3 resetPosition;
    Interact grabScript;
    public GameObject m_pickPanel;
    public GameObject m_dragPanel;
    int floorMask;
    float camRayLenght = 100;

    Vector3 forward, right, playerRight, playerForward;

    public PLAYER_STATE movState;
    PLAYER_INTERCTION interactState;

    [SerializeField]
    bool jump = false;
    // Use this for initialization

    void Start () {
        forward = Camera.main.transform.forward;
        grabScript = GetComponent<Interact>();
        minSpeed = 20;
        maxSpeed = 30;
        forward.y = 0;
        forward.Normalize();
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        rb = GetComponent<Rigidbody>();
        isHolding = false;
        movState = PLAYER_STATE.WALKING;
        interactState = PLAYER_INTERCTION.DEFAULT;
        currentSpeed = minSpeed;
        gravity = Vector3.down;

        resetPosition = rb.position;
        //objectScript = carryingObject.GetComponent<IneractableObject>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Checks if position needs to be reset
        Debug.DrawRay(transform.position, forward, Color.green);
        ResetPosition();

        if (!grabScript.m_isGrabbing)
        {
            LookAtmouse();
        }
        if (!isFloored)
            rb.velocity = new Vector3(0, -9.8f, 0);

        //if (isHolding && carryingObject)
        //{
        //    carryingObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2.0f);
        //}
        
        if (movState == PLAYER_STATE.WALKING)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                    Move();

            else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 && Input.GetAxis("RightStickX") != 0 || Input.GetAxis("RightStickY") != 0)
                MoveJoystick();
            //MoveDualShock();
        }
        else if(movState==PLAYER_STATE.CLIMBING)
        {
            if (Input.GetAxis("Vertical") != 0)
                ClimbStairs();

            //else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 && Input.GetAxis("RightStickX") != 0 || Input.GetAxis("RightStickY") != 0)
            //    MoveJoystick();
        }
        else if (movState== PLAYER_STATE.GRABING)
        {
            MoveWhileGrabing();
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
        Vector3 rightMovement = right * Input.GetAxis("Horizontal");
        Vector3 upMovement = forward  * Input.GetAxis("Vertical");

        
        Vector3 heading = rightMovement + upMovement;

        heading.Normalize();
        heading *= currentSpeed * Time.deltaTime;

        transform.position += heading;
        //transform.position += upMovement;
    }

    void MoveWhileGrabing()
    {
        playerRight = transform.right;
        playerForward = transform.forward;
        currentSpeed = 5;

        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 rightMovement = playerRight * Input.GetAxis("Horizontal");
        Vector3 upMovement = playerForward * Input.GetAxis("Vertical");


        //        Vector3 heading = rightMovement + upMovement;
        Vector3 heading = direction;

        heading.Normalize();
        heading *= currentSpeed * Time.deltaTime;

        transform.position += heading;
        //transform.position += upMovement;
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

        //rb.AddForce(heading * currentSpeed * Input.GetAxis("Vertical"));
        heading.Normalize();
        transform.position += heading;
        //transform.position += upMovement;
       
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
        // Generate a plane that intersects the transform's position with an upwards normal.
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        // Generate a ray from the cursor position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        // Determine the point where the cursor ray intersects the plane.
        // This will be the point that the object must look towards to be looking at the mouse.
        // Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
        //   then find the point along that ray that meets that distance.  This will be the point
        //   to look at.
        float hitdist = 0.0f;
        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(ray, out hitdist))
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 targetPoint = ray.GetPoint(hitdist);
            //transform.LookAt(targetPoint);
            // Determine the target rotation.  This is the rotation if the transform looks at the target point.
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);


            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentSpeed * Time.deltaTime);
        }
    }

    void ClimbStairs()
    {
        Vector3 upMovement = new Vector3(0, 1, 0) * currentSpeed     * Time.deltaTime * Input.GetAxis("Vertical");

        transform.position += upMovement;
        isFloored = true;
    }


    void ResetPosition()
    {
        if (rb.position.y < -2.0f || Input.GetKeyDown(KeyCode.R))
        {
            rb.position = resetPosition;
        }
    }

    void CheckPanel(GameObject gameObj)
    {
        if (gameObj.tag == "Pickable")
            m_pickPanel.SetActive(true);
        else if(gameObj.tag == "Grabable")
            m_dragPanel.SetActive(true);


    }
    void CheckPanelDeactivate(GameObject gameObj)
    {
        if (gameObj.tag == "Pickable")
            m_pickPanel.SetActive(false);
        else if (gameObj.tag == "Grabable")
            m_dragPanel.SetActive(false);


    }
    /// <summary>
    /// Collisions 
    /// </summary>


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
        if (collision.gameObject.tag == "Stairs" && Input.GetButtonDown("Action"))
        {
            Debug.Log("Voy a subir escaleras.");
            if (!isClimbing)
            {
                movState = PLAYER_STATE.CLIMBING;
                rb.useGravity = false;
                isFloored = true;
            }
            else
            {
                isFloored = false;
                movState = PLAYER_STATE.WALKING;
                rb.useGravity = true;
            }
            
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Stairs")
        {
            Debug.Log("Ya me quité de las escaleras.");
            movState = PLAYER_STATE.WALKING;
            rb.useGravity = true;
        }
        else if (collision.gameObject.tag == "Floor")
        {
            isFloored = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Grabable" || other.gameObject.tag == "Door" || other.gameObject.tag == "Pickable" || other.gameObject.tag == "Key" || other.gameObject.tag == "Interactable")
        { 
            onRange = true;
            m_interactObject = other.gameObject;
            CheckPanel(m_interactObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Grabable" || other.gameObject.tag == "Door" || other.gameObject.tag == "Pickable" || other.gameObject.tag == "Key" || other.gameObject.tag == "Interactable")
        {
            onRange = false;
            m_interactObject = null;
            CheckPanelDeactivate(other.gameObject);

        }

    }
}
