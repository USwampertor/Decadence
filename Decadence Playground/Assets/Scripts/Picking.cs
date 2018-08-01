using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picking : MonoBehaviour {
    public bool isHolding;
    GameObject carryingObject = null;
    IneractableObject objectScript;
    // Use this for initialization
    void Start () {
        isHolding = false;
    }
	
	// Update is called once per frame
	void Update () {
        CheckifHolding();
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isHolding) PickUp();
            else DropIt();
        }
    }
    void CheckifHolding()
    {
        if(isHolding&&carryingObject)
        {
            carryingObject.transform.localPosition = transform.forward; //new Vector3(transform.position.x, transform.position.y, transform.position.z + 2.0f);
            //carryingObject.transform.rotation = Quaternion.LookRotation(carryingObject.transform.forward, transform.up);
        }
    }
    void PickUp()
    {
        //PLAYER MUST HAVE bool isHolding
        //OBJECTS MUST HAVE TAG carriable
        //temporal variables
        int sphereRadius = 5;
        Debug.Log("Trying to Pick Up");
        if (isHolding)
        {
            Debug.Log("Im gonna drop it");
            DropIt();
        }
        Collider[] nearObjects = Physics.OverlapSphere(transform.position, sphereRadius);
        foreach(Collider obj in nearObjects)
        {
            if (obj.gameObject.tag == "Pickable" && obj.gameObject != carryingObject
                /*&& Vector3.Dot(obj.gameObject.transform.position,transform.position)/
                * (obj.gameObject.transform.position.magnitude * transform.position.magnitude) < .065*/)
            {
                
                carryingObject = obj.gameObject;
                carryingObject.transform.SetParent(transform);
                carryingObject.GetComponent<Rigidbody>().useGravity = false;

                //we apply the same rotation our main object (Camera) has.
                carryingObject.transform.localRotation = transform.rotation;
                isHolding = true;
                break;
            }
            else if(obj.gameObject.tag == "Key" && obj.gameObject != carryingObject)
            {
                carryingObject = obj.gameObject;
                carryingObject.transform.SetParent(transform);
                carryingObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }
    void DropIt()
    {
        Debug.Log("Trying to Drop");
        carryingObject.GetComponent<Rigidbody>().useGravity = true;
        // we don't have anything to do with our ball field anymore
        carryingObject = null;
        //Unparent our ball
        transform.GetChild(0).parent = null;
        isHolding = false;
    }
}
