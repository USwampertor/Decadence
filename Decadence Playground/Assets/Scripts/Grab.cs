using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour {

    public bool isHolding, isGrabbing = false;
    private Quaternion beforeGrabRotation;
    GameObject carryingObject = null;
    public GameObject FloatingKeyText;
    public GameObject doorText;
    IneractableObject objectScript;
    CharControl playerScript;
    Lever leverScript;
    Vector3 offset = new Vector3(0, 0, 4);

    //public GameObject player;
    // Use this for initialization
    void Start()
    {
        isHolding = false;
        playerScript = GetComponent<CharControl>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckifHolding();
        ///TENDRIAS QUE HACER EN UNA MAQUINA DE ESTADOS COMO PUEDE MOVERSE
        ///PERO COMO SE QUE AHORITA NO TENEMOS TIEMPO PARA ESO
        ///HAS UN UPDATE PARA EL MOVIMIENTO DE CUANDO ESTAS SIN AGARRAR LAS COSAS Y EN CHECKIFHOLDING PONDRE EL UPDATE
        ///PARA CUANDO ESTES AGARRANDO ALGO
        if (Input.GetKeyDown(KeyCode.F))
        {
             if(!isHolding)
                PickUp();
             else if (isHolding)
                DropIt();

        }
        //else if (Input.GetKey(KeyCode.G) && isHolding)
    }
    void CheckifHolding()
    {
        if (isHolding && carryingObject)
        {
            //carryingObject.transform.position = transform.forward;
            //transform.rotation = beforeGrabRotation;
            // _InputDir = joystick.GetInputDirection();
            // Joy Stick input inputX = _InputDir.x; inputY = _InputDir.y; if (Mathf.Abs (inputX) > Mathf.Abs (inputY)) { inputY = 0; } else { inputX = 0; }
        }
    }
    void PickUp()
    {
        int sphereRadius = 5;
        Debug.Log("Trying to Pick Up");
        //if (isHolding)
        //{
        //    Debug.Log("Im gonna drop it");
        //    DropIt();
        //}
        Collider[] nearObjects = Physics.OverlapSphere(transform.position, sphereRadius);
        foreach (Collider obj in nearObjects)
        {
            Vector3 difference = (obj.transform.position - transform.position);
            switch(obj.gameObject.tag)
            {
                case "Pickable":
                    {
                        //Generating vector to make the dot product
                        Vector3 frontPoint = transform.position + (transform.forward * 2.0f);
                        Vector3 front = frontPoint - transform.position;
                        //Generated the vector for the dot product

                        if ((Vector3.Dot(difference, front) / front.sqrMagnitude) > .25f && (Vector3.Dot(difference, front) / front.sqrMagnitude) < 2.0f)
                        {
                            carryingObject = obj.gameObject;
                            carryingObject.transform.SetParent(transform);
                            beforeGrabRotation = transform.rotation;
                            //carryingObject.GetComponent<Rigidbody>().useGravity = false;
                            //we apply the same rotation our main object (Camera) has.
                            //carryingObject.transform.localRotation = transform.rotation;
                            isHolding = true;
                            break;
                        }
                    }
                    break;

                case "Key":
                    {
                        playerScript.hasKey = true;
                        isHolding = true;
                        carryingObject = obj.gameObject;
                        carryingObject.transform.SetParent(transform);
                        carryingObject.transform.localPosition = transform.forward;
                        Instantiate(FloatingKeyText, transform.position, Quaternion.Euler(0, 45, 0), transform);
                        carryingObject.tag = null;
                        //Destroy(carryingObject);
                    }
                    break;
                case "Grabable":
                    {
                        isHolding = true;
                        carryingObject = obj.gameObject;
                        
                        isGrabbing = true;
                        transform.LookAt(carryingObject.transform);
                        carryingObject.transform.SetParent(transform);
                        carryingObject.transform.localPosition = transform.forward *2;
                        //carryingObject.transform.localPosition = transform.position + offset;
                        playerScript.movState = CharControl.PLAYER_STATE.GRABING;
                    }
                    break;
                case "Door":
                    {
                        carryingObject = obj.gameObject;
                        if (playerScript.hasKey)
                        {
                            carryingObject.GetComponent<Renderer>().enabled = false;
                            carryingObject.GetComponent<BoxCollider>().enabled = false;

                        }
                        else
                        {
                             Instantiate(doorText, transform.position, Quaternion.Euler(0, 45, 0), transform);
                        }
                    }
                    break;
                case "Interactable":
                    {
                        carryingObject = obj.gameObject;
                        leverScript = carryingObject.GetComponent<Lever>();
                        leverScript.ActivateLever();
                    }
                    break;
            }
            //if (obj.gameObject.tag == "Pickable" && obj.gameObject != carryingObject)
            //{
            //    //Generating vector to make the dot product
            //    Vector3 frontPoint = transform.position + (transform.forward * 2.0f);
            //    Vector3 front = frontPoint - transform.position;
            //    //Generated the vector for the dot product

            //    if ((Vector3.Dot(difference, front) / front.sqrMagnitude) > .25f && (Vector3.Dot(difference, front) / front.sqrMagnitude) < 2.0f)
            //    {
            //        carryingObject = obj.gameObject;
            //        carryingObject.transform.SetParent(transform);
            //        beforeGrabRotation = transform.rotation;
            //        //carryingObject.GetComponent<Rigidbody>().useGravity = false;
            //        //we apply the same rotation our main object (Camera) has.
            //        //carryingObject.transform.localRotation = transform.rotation;
            //        isHolding = true;
            //        break;
            //    }
            //}
            //else if (obj.gameObject.tag == "Key" && obj.gameObject != carryingObject)
            //{
            //    playerScript.hasKey = true;
            //    carryingObject = obj.gameObject;
            //    Instantiate(FloatingKeyText, transform.position, Quaternion.Euler(0, 45, 0), transform);
            //    Destroy(carryingObject);
            //}
            //else if (obj.gameObject.tag == "Grabable" && obj.gameObject != carryingObject)
            //{
            //    isHolding = true;
            //    carryingObject = obj.gameObject;
            //    carryingObject.transform.SetParent(transform);
            //    playerScript.movState = CharControl.PLAYER_STATE.GRABING;
            //}
            //else if (obj.gameObject.tag == "Grabable" && obj.gameObject != carryingObject)
            //{
            //    isHolding = true;
            //    carryingObject = obj.gameObject;
            //    carryingObject.transform.SetParent(transform);
            //    playerScript.movState = CharControl.PLAYER_STATE.GRABING;
            //}

        }
    }
    void DropIt()
    {
        //Debug.Log("Trying to Drop");
        //carryingObject.GetComponent<Rigidbody>().useGravity = true;
        // we don't have anything to do with our ball field anymore
        
        //Unparent our ball
        carryingObject.transform.parent = null;
        isHolding = false;
        isGrabbing = false;
        playerScript.hasKey = false;
        carryingObject.transform.parent = null;
        carryingObject = null;
        playerScript.movState = CharControl.PLAYER_STATE.WALKING;
         
    }
}
