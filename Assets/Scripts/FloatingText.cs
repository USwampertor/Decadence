using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour {

    public float destroyTime;
    Vector3 offset = new Vector3(0, 2, 0);
	// Use this for initialization
	void Start () {
        destroyTime = 3;
        Destroy(gameObject, destroyTime);

        transform.localPosition += offset;
	}
	
}
