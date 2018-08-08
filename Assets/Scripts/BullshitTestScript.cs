using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If you want to test some random shit withouth fucking up the OG scripts, this is the place to do it.
/// </summary>


public class BullshitTestScript : MonoBehaviour {
    public GameObject targetGObj;
    public bool targeting = false;

    // Use this for initialization
    void Start () {
		
	}
	

	// Update is called once per frame
	void FixedUpdate() {
        if (Input.GetMouseButton(0))
        {
            transform.LookAt(targetGObj.transform);
            targeting = true;
        }
        LookAtMouse();
    }

    public float speed;

    void LookAtMouse()
    {
        // Generate a plane that intersects the transform's position with an upwards normal.
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        // Generate a ray from the cursor position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects the plane.
        // This will be the point that the object must look towards to be looking at the mouse.
        // Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
        //   then find the point along that ray that meets that distance.  This will be the point
        //   to look at.
        float hitdist = 0.0f;
        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(ray, out hitdist))
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 targetPoint = ray.GetPoint(hitdist);
            transform.LookAt(targetPoint);
            // Determine the target rotation.  This is the rotation if the transform looks at the target point.
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
             

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }
    }

}
