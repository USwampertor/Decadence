using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    private Vector3 vMyPosition;
    public GameObject gObjective;
    public float fSeekRadius, fFleeRadius, fApproachRadius, fMagnitude;
    public Vector3 vWander, circle, vLastForce;
    public bool
        bLeave,
        bArrive,
        bPursuit,
        bWander,
        bAvoid,
        bSeek,
        bFlee,
        bDirection,
        bCohesion,
        bDistance,
        bFlock,
        bObstacle,
        bPathFollowing,
        bSurroundEnemy;
    public float timetochange = 0, fmag;
    private float fLenght, neighborRadius = 8.0f,innervalue;
    public GameObject[] beaconList;
    private int iActualBeacon = 0, iLastBeacon;
    // Use this for initialization
    void Start()
    {
        //beaconList = GameObject.FindGameObjectsWithTag("beacon");
        //int b = beaconList.Length;
        //for (int I = 1; I < b+1; ++I)
        //{
        //    beaconList[I-1] = GameObject.Find("Cube (" + I + ")");
        //}
        //float a = Random.Range(-1, 1);
        //if (a < 0) innervalue = -1;
        //else innervalue = 1;
        //gameObject.tag = "agent";
        vWander = new Vector3();
        vLastForce = Vector3.zero;
        //fLenght = GetComponent<Renderer>().bounds.extents.magnitude;
    }
        // Update is called once per frame
    void Update()
    {
        vMyPosition = transform.position;
        Vector3 objPos = gObjective.transform.position;
        //Initialize forcevector
        Vector3 vFinal = vLastForce;

        Vector3 vObjectivesVelocity = gObjective.GetComponent<Rigidbody>().velocity;
        if (bFlee) vFinal += Run(vMyPosition, objPos, fMagnitude);
        if (bSeek) vFinal += Seek(vMyPosition, objPos, fMagnitude);
        if (bArrive) vFinal += Approach(vMyPosition, objPos, fMagnitude, fApproachRadius);
        if (bAvoid) vFinal += Avoid(vMyPosition, objPos, vObjectivesVelocity, fMagnitude, fSeekRadius, fFleeRadius);
        if (bLeave) vFinal += RunAway(vMyPosition, objPos, fMagnitude, fSeekRadius, fFleeRadius);
        if (bPursuit) vFinal += Pursuit(vMyPosition, objPos, vObjectivesVelocity, fMagnitude);
        if (bObstacle) vFinal += ObstacleAvoidance(vMyPosition);
        if (bFlock) vFinal += Flock();
        if (bCohesion) vFinal += Cohesion() * fMagnitude;
        if (bDirection) vFinal += Direction() * fMagnitude;
        if (bDistance) vFinal += Distance() * fMagnitude;
        if (bSurroundEnemy) vFinal += SurroundEnemy(vMyPosition, objPos, fMagnitude, 10.0f);
        if (bPathFollowing) vFinal += PathFollow(vMyPosition);
        if (bWander && timetochange <= 0.0f)
        {
            vWander = Wander(vMyPosition, fMagnitude);
            timetochange = Random.Range(0.5f, 1.0f);
            
        }
        if(bWander) Debug.DrawLine(vMyPosition, circle, Color.yellow);
        if(!bWander) vWander = Vector3.zero;
        vFinal += vWander;

        float tmp = vFinal.magnitude;
        vFinal.y = 0.0f;
        vFinal = vFinal.normalized * tmp;
        //vFinal *= Time.deltaTime;
        vFinal *= 0.92561f;
        vFinal = Vector3.ClampMagnitude(vFinal, 1);
        timetochange -= Time.deltaTime;
        Debug.DrawRay(transform.position, transform.forward, Color.green);
        Debug.DrawRay(transform.position, vFinal * 10.0f, Color.red);
        Vector3 forward = (transform.forward).normalized;
        transform.Translate((vFinal), Space.World);
        //transform.position += vFinal/* * Time.deltaTime*/;
        transform.rotation = Quaternion.LookRotation((vFinal+transform.forward).normalized, transform.up);
        fmag = vFinal.magnitude;
        vLastForce = vFinal;
    }
    //Creates a vector towards the Objective
    public Vector3 Seek(Vector3 vPosition, Vector3 vObjective, float magnitude)
    {
        //Initializes vector
        Vector3 forceV = new Vector3();
        //sets vector to where object will start moving towards
        forceV = vObjective - vPosition;
        //Normalize the vector
        forceV.Normalize();
        forceV *= magnitude;
        //Just Debugging the final direction
        Debug.DrawRay(vPosition, forceV * 10, Color.blue);
        return forceV;
    }
    //Creates a vector against the Objective
    public Vector3 Run(Vector3 vPosition, Vector3 vObjective, float magnitude)
    {
        //Initialize vector
        Vector3 forceV = new Vector3();
        //Sets vector to where object will start fleeing from
        forceV = vPosition - vObjective;
        //Normalize the vector
        forceV.Normalize();
        forceV *= magnitude;
        return forceV;
    }
    public Vector3 Wander(Vector3 vPosition, float magnitude)
    {

        float lenght = 5.0f;
        float vision = Mathf.PI * .9f;
        float worldangle = Mathf.Atan(vPosition.z / vPosition.x);
        Vector3 vOffSet = transform.position + transform.forward*lenght;
        float angle = Random.Range(worldangle - vision, worldangle + vision);
        Vector3 RandomObjective = new Vector3(Mathf.Cos(angle) * 3.0f, 0, Mathf.Sin(angle) * 3.0f) + vOffSet;
        Vector3 vForce = Seek(vPosition, RandomObjective, magnitude / 2);
        circle = RandomObjective;
        return vForce;
    }
    public Vector3 Approach(Vector3 vPosition, Vector3 vObjective, float magnitude, float innerradio)
    {
        float distance = (vObjective - vPosition).magnitude;
        Debug.Log(distance);
        if(distance<=fSeekRadius)
        {
            Vector3 forceV = Seek(vPosition, vObjective, magnitude);
            float scale = 0.0f;
            scale = Mathf.Min(distance / innerradio, 1.0f);
            return forceV * scale;
        }
        return Vector3.zero;
    }
    public Vector3 Pursuit(Vector3 vPosition, Vector3 vObjective, Vector3 velocity, float magnitude)
    {
        float offset = 10.0f;
        Vector3 vPredictPosition = new Vector3();
        offset = Mathf.Min(offset, (vPosition - vObjective).magnitude);
        
        vPredictPosition = vObjective + (velocity * offset*3);
        //if ((vPosition - vObjective).magnitude <= vPredictPosition.magnitude)
        //{
        //    offset = (vPosition - vObjective).magnitude;
        //    vPredictPosition = vObjective + (velocity * offset);
        //}
        //else offset = 10.0f;
        Debug.DrawLine(vPosition, vPredictPosition,Color.cyan);
        Vector3 forceV = Seek(vPosition, vPredictPosition, magnitude);
        return forceV;
    }
    public Vector3 Avoid(Vector3 vPosition, Vector3 vObjective, Vector3 velocity, float magnitude, float innerradio, float outerradio)
    {
        float offset2 = 10.0f;
        Vector3 vPredictPosition = new Vector3();
        offset2 = Mathf.Min(offset2, (vPosition - vObjective).magnitude);
        vPredictPosition = vObjective + (velocity * offset2*3);
        //if ((vPosition - vObjective).magnitude <= vPredictPosition.magnitude)
        //{
        //    offset2 = (vPosition - vObjective).magnitude;
        //    vPredictPosition = vObjective + (velocity * offset2);
        //}
        //else offset2 = 10.0f;
        Debug.DrawLine(vPosition, vPredictPosition, Color.cyan);
        Vector3 forceV = RunAway(vPosition, vPredictPosition, magnitude, innerradio, outerradio);
        return forceV;
    }

    public Vector3 RunAway(Vector3 vPosition, Vector3 vObjective, float magnitude, float innerradio, float outerradio)
    {
        Vector3 forceV = Run(vPosition, vObjective, magnitude);
        float scale = 1.0f;
        if ((vPosition - vObjective).magnitude >= innerradio)
        {
            float distance = (vObjective - vPosition).magnitude - innerradio;
            distance = outerradio - distance;
            scale = Mathf.Min(distance / outerradio, 1.0f);
            if (scale <= .001f)
                scale = 0;
        }
        return forceV * scale;

    }

    public void CreateCollisionBox(Vector3 vPosition, List<Vector3> Corners ,List<Vector3> Box,float length,  Color boxcolor)
    {
        Corners.Clear();
        Box.Clear();
        float fFrontOffset = length, fBackOffset = length/2;
        Vector3 fLeftNear, fLeftFar, fRightNear, fRightFar;
        
        fLeftFar = vPosition + transform.forward * fFrontOffset - transform.right;
        fRightFar = vPosition + transform.forward * fFrontOffset + transform.right;
        fLeftNear = vPosition - transform.forward * fBackOffset - transform.right;
        fRightNear = vPosition - transform.forward * fBackOffset + transform.right;

        Corners.Add(fLeftFar);
        Corners.Add(fLeftNear);
        Corners.Add(fRightNear);
        Corners.Add(fRightFar);

        Vector3 vA, vB, vC, vD;
        vA = fRightFar - fLeftFar;
        vB = fLeftFar - fLeftNear;
        vC = fLeftNear - fRightNear;
        vD = fRightNear - fRightFar;

        Box.Add(vA);
        Box.Add(vB);
        Box.Add(vC);
        Box.Add(vD);

        Debug.DrawRay(fLeftFar, vA, boxcolor);
        Debug.DrawRay(fLeftNear, vB, boxcolor);
        Debug.DrawRay(fRightNear, vC, boxcolor);
        Debug.DrawRay(fRightFar, vD, boxcolor);
    }

    public Vector3 ObstacleAvoidance(Vector3 vPosition)
    {
        Vector3 vBackForce = new Vector3();
        List<Vector3> corners, box;corners = new List<Vector3>();box = new List<Vector3>();
        CreateCollisionBox(vPosition,corners, box,4.0f,Color.grey);
        GameObject[] ObstacleList;
        ObstacleList = GameObject.FindGameObjectsWithTag("Untagged");
        foreach(GameObject obstacle in ObstacleList)
        {
            if((vMyPosition-obstacle.transform.position).magnitude<=9.0f)
            {
                
                Vector3 obstaclePos = obstacle.transform.position;
                float obstaclerad  = obstacle.GetComponent<Renderer>().bounds.extents.magnitude;
                //Vector3 alpha = new Vector3(), betta = new Vector3(), gamma = new Vector3(), delta = new Vector3();
                //float a = -1, b = -1, c = -1, d = -1;
                //alpha = obstaclePos - corners[0];betta = obstaclePos - corners[1];gamma = obstaclePos - corners[2];delta = obstaclePos - corners[3];
                //a = Vector3.Dot(alpha, box[0]) / box[0].sqrMagnitude;b = Vector3.Dot(betta, box[1]) / box[1].sqrMagnitude;c = Vector3.Dot(gamma, box[2]) / box[2].sqrMagnitude;d = Vector3.Dot(delta, box[3]) / box[3].sqrMagnitude;
                for( int i = 0; i < 4;++i)
                {
                    Vector3 shoot = obstaclePos - corners[i];
                    float dot = Vector3.Dot(shoot, box[i]) / box[i].sqrMagnitude;
                    vBackForce += CheckifInside(box[i], shoot, obstaclePos, corners[i], dot, obstaclerad);
                }
                //vBackForce += CheckifInside(box[1], betta, obstaclePos, corners[1], b, obstaclerad);vBackForce += CheckifInside(box[2], gamma, obstaclePos, corners[2], c, obstaclerad);vBackForce += CheckifInside(box[3], delta, obstaclePos, corners[3], d, obstaclerad);
            }
        }
    return vBackForce;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(circle, .25f);
    }
    Vector3 CheckifInside(Vector3 side, Vector3 towardsObject, Vector3 obstaclePosition, Vector3 corner, float dotfactor,float obstacleradius)
    {
        Vector3 result = new Vector3(0, 0, 0);
        if (dotfactor>0&&dotfactor<1)
        {
            Vector3 reason = new Vector3();
            reason = side * dotfactor;
            Vector3 bforce = new Vector3();
            bforce = reason - towardsObject;
            if (bforce.magnitude<=(obstacleradius/2))
            {
                Debug.DrawRay(corner, side, Color.white);
                float randomangle = Random.Range(-3.14f/2, 3.14f/2);
                result = Quaternion.AngleAxis(innervalue * 50.0f, transform.up)*(reason - towardsObject).normalized * fMagnitude*10f;
            }
        }

        return result;
    }
    Vector3 Flock()
    {
        return (Cohesion() + Distance() + Direction()).normalized*fMagnitude;
    }
    Vector3 Cohesion()
    {
        GameObject[] agents = GetTotalAgents();
        Vector3 
            GeneralPoint = new Vector3(0, 0, 0),
            vtowardsCenter = new Vector3(0, 0, 0);
        int neighbors=0;
        foreach(GameObject agent in agents)
        {
            if((agent.transform.position - vMyPosition).magnitude!=0&&
                (agent.transform.position - vMyPosition).magnitude<=neighborRadius)
            {
                GeneralPoint += agent.transform.position;
                ++neighbors;
            }
        }
        if(neighbors!=0)
        {
            GeneralPoint /= neighbors;
            vtowardsCenter=(GeneralPoint - vMyPosition).normalized/**fMagnitude*/;
        }
        return vtowardsCenter;
    }
    Vector3 Distance()
    {
        GameObject[] agents = GetTotalAgents();
        Vector3
            GeneralAvoidance = new Vector3(0, 0, 0);
        int neighbors = 0;
        foreach (GameObject agent in agents)
        {
            if ((agent.transform.position - vMyPosition).magnitude != 0 &&
                (agent.transform.position - vMyPosition).magnitude <= neighborRadius)
            {
                GeneralAvoidance += (agent.transform.position-vMyPosition);
                ++neighbors;
            }
        }
        if (neighbors != 0)
        {
            GeneralAvoidance /= neighbors;
            GeneralAvoidance *= -1.0f;
            GeneralAvoidance.Normalize();
            //GeneralAvoidance *= fMagnitude;
        }
        return GeneralAvoidance;
    }
    Vector3 Direction()
    {
        GameObject[] agents = GetTotalAgents();
        Vector3
            GeneralDirection = new Vector3(0, 0, 0);
        int neighbors = 0;
        foreach (GameObject agent in agents)
        {
            if ((agent.transform.position - vMyPosition).magnitude != 0 &&
                (agent.transform.position - vMyPosition).magnitude <= neighborRadius)
            {
                GeneralDirection += agent.transform.forward;
                ++neighbors;
            }
        }
        if (neighbors != 0)
        {
            GeneralDirection /= neighbors;
            GeneralDirection.Normalize();
            //GeneralDirection *= fMagnitude;
        }
        return GeneralDirection;
    }
    Vector3 PathFollow(Vector3 vPosition)
    {
        Vector3 beaconPosition = beaconList[iActualBeacon].transform.position;
        iLastBeacon = iActualBeacon;
        if ((beaconPosition-vPosition).magnitude < 5.0f) iActualBeacon++;
        if (iActualBeacon >= beaconList.Length) iActualBeacon = 0;

        Vector3 beaconVector = beaconList[iLastBeacon].transform.position - beaconList[iActualBeacon].transform.position;
        Vector3 beacontoPosition = vPosition - beaconPosition;
        float a = Vector3.Dot(beacontoPosition, beaconVector) / beaconVector.sqrMagnitude;
        beaconVector *= a;
        Vector3 nearestposition = vPosition + beaconVector;
        
        Vector3 vForce = Seek(vPosition, beaconPosition, fMagnitude);
        vForce += Seek(vPosition, nearestposition, fMagnitude);
        return vForce;
    }
    Vector3 SurroundEnemy(Vector3 vPosition, Vector3 vObjective, float magnitude, float enemyradius)
    {
        //The object will Arrive to a position offset units from the object. Now, what we wanna do is that if the object is already surrounded
        //in that part, the agent move towards left or right, until he arrives to the point we want
        Vector3 vForce = new Vector3(0, 0, 0);
        GameObject[] agents = GetTotalAgents();
        Vector3 vV = vPosition - vObjective;
        Vector3 vW = vV.normalized * 10.0f;
        Vector3 vPoint = vW - vV + vPosition;
        vForce += Approach(vPosition, vPoint, fMagnitude, 1.5f);
        
        foreach (GameObject agent in agents)
        {
            Vector3 difference = (agent.transform.position - vMyPosition);
            if (difference.magnitude != 0   &&
                difference.magnitude <= 3.0f)
            {
                Vector3 frontPoint = vPosition + (transform.forward * 3.0f);
                Vector3 front = frontPoint - vPosition;
                
                if (( Vector3.Dot(difference, front) / front.sqrMagnitude)>.25f&& (Vector3.Dot(difference, front) / front.sqrMagnitude)<2.0f)
                {
                    vForce += Quaternion.AngleAxis(innervalue*90.0f, transform.up) * RunAway(vPosition, vPoint, fMagnitude, 3.5f, 4.0f)*2;
                }
            }
        }
        if ((vPoint - vPosition).magnitude <= .5f) return Vector3.zero;
        return vForce;
    }
    GameObject[] GetTotalAgents()
    {
        GameObject[] agentList = GameObject.FindGameObjectsWithTag("agent");
        return agentList;
    }
}
