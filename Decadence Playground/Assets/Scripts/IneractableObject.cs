using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IneractableObject : MonoBehaviour {

    public enum SIZE
    {
        SMALL = 0,
        MED = 1,
        BIG = 2,
        BRK = 3
    };

    public float weight;
    public SIZE size;
    bool breakable = false;
        // Use this for initialization
	void Start () {
        gameObject.tag = "Interactable";
        SetSize();

    }
	
	// Update is called once per frame
	void Update () {
   
    }

    void SetSize()
    {
        if (breakable)
        {
            size = SIZE.BRK;
        }
        else if (weight < 10)
        {
            size = SIZE.SMALL;

        }
        else if (weight > 10 && weight < 30)
        {
            size = SIZE.MED;

        }
        else if (weight > 30 && weight < 60)
        {
            size = SIZE.BIG;
        }
    }
}
