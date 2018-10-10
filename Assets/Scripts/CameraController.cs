using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

  public GameObject player;
  public float offsetX;
  public float offsetZ;
  public float offsetY;
  float maximunDistances = 2;
  float playerVelocity = 10;
  public float movementX;
  public float movementY;
  public float movementZ;
  bool isReversed;

    
  // Use this for initialization
  void Start()
  {
    isReversed = false;
    offsetX = 0;
    offsetZ = -15;
    offsetY = 30;
    Camera.main.transform.eulerAngles = new Vector3(45, 0, 0);

  }

  // Update is called once per frame
  void Update()
  {

    movementX = (player.transform.position.x + offsetX - this.transform.position.x) / maximunDistances;
    movementY = (player.transform.position.y + offsetY - this.transform.position.y) / maximunDistances;
    movementZ = (player.transform.position.z + offsetZ - this.transform.position.z) / maximunDistances;
    transform.position += new Vector3((movementX * playerVelocity * Time.deltaTime), (movementY * playerVelocity * Time.deltaTime), (movementZ * playerVelocity * Time.deltaTime));

    if (Input.mousePosition.y < player.transform.position.y) {
      StartCoroutine(rotateCamera());
    }
    Debug.Log("Mouse Rotation: " + Input.mousePosition);

  }

  IEnumerator rotateCamera() {
    yield return new WaitForSeconds(1.5f);

    if (isReversed) {
      
      Camera.main.transform.eulerAngles = new Vector3(45, 0, 0);
      offsetZ = -15;
      isReversed = false;
    }
    else {
      Camera.main.transform.eulerAngles = new Vector3(45, 180, 0);
      offsetZ = 15;
      isReversed = true;

    }

    yield return null;
  }
}

