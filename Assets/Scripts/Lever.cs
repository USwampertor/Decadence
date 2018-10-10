using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {
    private bool bActive;
    public GameObject door;
    Rigidbody rb;
    new Renderer renderer;
    Collider boxColl;

	// Use this for initialization
	void Start () {
        //Consigue los componentes.
        rb = door.GetComponent<Rigidbody>();
        renderer = door.GetComponent<Renderer>();
        boxColl = door.GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    public void ActivateLever()
    {
        //Si la palanca está activada desactiva el renderer y el collider del objeto con el que está enlazada, en caso contrario los activa.
        if (bActive)
        {
            //rb.velocity = new Vector3(0, 0, 1);
            renderer.enabled = false;
            boxColl.enabled = false;
        }
        else
        {
            //rb.velocity = Vector3.zero;
            renderer.enabled = true;
            boxColl.enabled = true;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.tag=="Player")
    //    {
    //        Debug.Log("Toqué la palanca.");
    //    }
    //}

    private void OnCollisionStay(Collision collision)
    {
        //Si el jugador está en contacto con la palanca y se presiona el botón de acción, la palanca se activa.
        if(collision.gameObject.tag=="Player" && Input.GetButtonDown("Action"))
        {
            //Debug.Log("Se usó la palanca.");
            bActive = !bActive;
        }
    }
}