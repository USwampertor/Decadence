using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject player;
    public float offsetX = -35;
    public float offsetZ = -40;
    public float offsetY = 30;
    public float maximunDistances = 2;
    public float playerVelocity = 10;

    public float movementX;
    public float movementY;
    public float movementZ;

    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        movementX = (player.transform.position.x + offsetX - this.transform.position.x) / maximunDistances;
        movementY = (player.transform.position.y + offsetY - this.transform.position.y) / maximunDistances;
        movementZ = (player.transform.position.z + offsetZ - this.transform.position.z) / maximunDistances;

        this.transform.position += new Vector3((movementX * playerVelocity * Time.deltaTime), (movementY * playerVelocity * Time.deltaTime), (movementZ * playerVelocity * Time.deltaTime));
    }
}

