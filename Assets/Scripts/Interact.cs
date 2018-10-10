using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour {

    public bool m_isHolding, m_isGrabbing = false;
    private Quaternion m_beforeGrabRotation;
    GameObject m_GOcarryingObject = null, m_GOInteractedObject = null;
    public GameObject m_GOFloatingKeyText;
    public GameObject m_GODoorText;
    public GameObject m_GOHand;
    IneractableObject m_objectScript;
    float biItem= 5, smallItem = 2;
    CharControl m_playerScript;
    Lever m_leverScript;
    Rigidbody m_carryingObjectRB;
    // Use this for initialization
    void Start () {
        m_isHolding = false;
        m_playerScript = GetComponent<CharControl>();
	}
	
	// Update is called once per frame
	void Update ()
    {


        if (Input.GetButtonDown("Action"))
        {
 
            m_GOInteractedObject = m_playerScript.m_interactObject;
            OnInteraction(m_GOInteractedObject);
 
        }


    }

    void OnInteraction(GameObject interactableObject)
    {
        
        //Vector3 front = frontPoint - transform.position;
        if (!m_isHolding)
        {
          if (m_GOInteractedObject != null)
          {
            switch (m_GOInteractedObject.tag)
            {
<<<<<<< HEAD
              case "Pickable":
                {
                  Vector3 frontPoint = transform.position + transform.forward * 4;
                  m_GOcarryingObject = m_GOInteractedObject;
                  m_carryingObjectRB = m_GOcarryingObject.GetComponent<Rigidbody>();
                  m_GOcarryingObject.transform.SetParent(transform);
                  m_GOcarryingObject.transform.position = frontPoint /*m_GOHand.transform.position*/;
                  m_carryingObjectRB.freezeRotation = true;
                  m_carryingObjectRB.constraints = RigidbodyConstraints.FreezePosition;
                  Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), m_GOcarryingObject.GetComponent<Collider>());
                  m_carryingObjectRB.useGravity = false;
                  m_isHolding = true;
=======
                case "Pickable":
                    {
                        Vector3 frontPoint = transform.position + transform.forward * 4;
                        m_GOcarryingObject = m_GOInteractedObject;
                        m_carryingObjectRB = m_GOcarryingObject.GetComponent<Rigidbody>();
                        m_GOcarryingObject.transform.SetParent(transform);
                        m_GOcarryingObject.transform.position = frontPoint /*m_GOHand.transform.position*/;
                        m_carryingObjectRB.freezeRotation = true;
                        m_carryingObjectRB.constraints = RigidbodyConstraints.FreezePosition;
                        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), m_GOcarryingObject.GetComponent<Collider>());
                        m_carryingObjectRB.useGravity = false;
                        m_isHolding = true;                     
                     
                        
                    }
                    break;
>>>>>>> e5bbf24562dab3a475a39bede6cd452be7750ea4

                  //m_ract
                }
                break;

              case "Key":
                {
                  Vector3 frontPoint = transform.position + transform.forward * smallItem;
                  m_isHolding = true;
                  m_playerScript.hasKey = true;
                  m_GOcarryingObject = m_GOInteractedObject;
                  m_carryingObjectRB = m_GOcarryingObject.GetComponent<Rigidbody>();
                  m_GOcarryingObject.transform.SetParent(transform);
                  m_GOcarryingObject.transform.position = frontPoint;
                  m_carryingObjectRB.freezeRotation = true;
                  m_carryingObjectRB.constraints = RigidbodyConstraints.FreezePosition;
                  m_carryingObjectRB.useGravity = false;
                  m_playerScript.m_interactObject = null;
                  Instantiate(m_GOFloatingKeyText, transform.position, Quaternion.Euler(0, 45, 0), transform);
                  //Destroy(carryingObject);
                }
                break;
              case "Grabable":
                {
                  Vector3 frontPoint = transform.position + transform.forward * biItem;
                  m_isHolding = true;
                  m_isGrabbing = true;
                  m_GOcarryingObject = m_GOInteractedObject;
                  m_carryingObjectRB = m_GOcarryingObject.GetComponent<Rigidbody>();
                  transform.LookAt(m_GOcarryingObject.transform);

                  m_GOcarryingObject.transform.SetParent(transform);
                  //m_GOcarryingObject.transform.position = frontPoint;
                  m_carryingObjectRB.freezeRotation = true;
                  m_carryingObjectRB.constraints = RigidbodyConstraints.FreezePosition;
                  m_carryingObjectRB.useGravity = false;
                  m_playerScript.movState = CharControl.PLAYER_STATE.GRABING;
                }
                break;
              case "Door":
                {
                  Instantiate(m_GODoorText, transform.position, Quaternion.Euler(0, 45, 0), transform);
                }
                break;
              case "Interactable":
                {

                  m_leverScript = m_GOInteractedObject.GetComponent<Lever>();
                  m_leverScript.ActivateLever();
                }
                break;
            }
          }
        }
        else if (m_isHolding)
        {
            if (m_GOcarryingObject.tag == "Key")
            {
                if (m_GOInteractedObject)
                {
                    if (m_GOInteractedObject.tag == "Door")
                    {
                        m_GOInteractedObject.GetComponent<Renderer>().enabled = false;
                        m_GOInteractedObject.GetComponent<BoxCollider>().enabled = false;
                        Destroy(m_GOcarryingObject);
                        m_isHolding = false;
                    }
                    else

                        DropIt();
                }
                else
                {
                    DropIt();
                }

            }
            else
            {
                DropIt();
                m_playerScript.movState = CharControl.PLAYER_STATE.WALKING;
            }
        }
        

    }

    void DropIt()
    {
        m_GOcarryingObject.transform.parent = null;
        m_isHolding = false;
        m_isGrabbing = false;
        m_GOcarryingObject = null;
        m_carryingObjectRB.freezeRotation = false;
        m_carryingObjectRB.useGravity = true;
        m_carryingObjectRB.constraints = RigidbodyConstraints.None;
    }
}
