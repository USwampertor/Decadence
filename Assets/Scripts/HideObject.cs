using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObject : MonoBehaviour {

    [SerializeField]
    RaycastHit[] hits;

    Vector3 direction;
    public GameObject character;
    [SerializeField]
    Renderer r;
    float m_fTransparency = 0.4f;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Saca la dirección de la cámara al personaje.
        direction = character.transform.position - transform.position;

        // Crea un array de GameObjects en el que guarda todas las paredes y les pone el alpha en 1.0
        GameObject[] hides = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject hide in hides)
        {
            r = hide.GetComponent<Renderer>();
            r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 1.0f);
        }

        // Mete en un array todas las paredes que hay entre el personaje y la cámara, y les baja el alpha a 0.4
        hits = Physics.RaycastAll(transform.position, direction, direction.magnitude, 9);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Wall"))
            {
                r = hit.collider.gameObject.GetComponent<Renderer>();
                r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, m_fTransparency);
            }
        }

        //if (Physics.Raycast(transform.position, direction, out hit))
        //{
        //    if (hit.collider.CompareTag("Wall"))
        //    {
        //        Debug.Log("Toqué una pared");
        //        r = hit.collider.gameObject.GetComponent<Renderer>();
        //        r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0.2f);
        //    }
        //}
    }
}
