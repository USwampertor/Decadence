using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour {

    public bool isHolding;
    private Quaternion beforeGrabRotation;
    GameObject carryingObject = null;
    IneractableObject objectScript;
    // Use this for initialization
    void Start()
    {
        isHolding = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckifHolding();
        ///TENDRIAS QUE HACER EN UNA MAQUINA DE ESTADOS COMO PUEDE MOVERSE
        ///PERO COMO SE QUE AHORITA NO TENEMOS TIEMPO PARA ESO
        ///HAS UN UPDATE PARA EL MOVIMIENTO DE CUANDO ESTAS SIN AGARRAR LAS COSAS Y EN CHECKIFHOLDING PONDRE EL UPDATE
        ///PARA CUANDO ESTES AGARRANDO ALGO
        if (Input.GetKeyDown(KeyCode.G))  PickUp();
        if (Input.GetKeyUp(KeyCode.G)) DropIt();
    }
    void CheckifHolding()
    {
        if (isHolding && carryingObject)
        {
            carryingObject.transform.localPosition = transform.forward;
            transform.rotation = beforeGrabRotation;
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
            if (obj.gameObject.tag == "Pickable" && obj.gameObject != carryingObject)
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
        }
    }
    void DropIt()
    {
        if(isHolding)
        {
            //Debug.Log("Trying to Drop");
            //carryingObject.GetComponent<Rigidbody>().useGravity = true;
            // we don't have anything to do with our ball field anymore
            carryingObject = null;
            //Unparent our ball
            transform.GetChild(0).parent = null;
            isHolding = false;
        }
        
    }
}
