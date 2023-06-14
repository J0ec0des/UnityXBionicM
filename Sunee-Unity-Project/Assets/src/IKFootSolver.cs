using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the script governing the position of the prediction foot(able leg)

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    //recognization of the terrain as the ground(collidable obejct)
    [SerializeField] Transform body = default;
    //init gameobject this script is attached to will be the body that is altered(moved)
    [SerializeField] float speed = 1;
    //speed of the able leg. Note: once speed of the prosthetic leg can be retrieved via ble, Use getcomponent to retrieve speed of the prosthetic foot. is it the same as the able leg?
    [SerializeField] IKFootSolver otherFoot = default;
    //initializing gameobject of the prosthetic foot for recognition by the script
    [SerializeField] float stepDistance = 4;
    //step distance is the offset that the able leg must have from the hip position to initiate step
    [SerializeField] float stepLength = 4;
    //step length is how far the foot will take a step after step initiation
    //change step length and step distance according to prosthetic value
    [SerializeField] float stepHeight = 1;
    // change step height to variable value. Note: considering whether step height of abe leg will change according to step length/distance. 
    [SerializeField] Vector3 footOffset = default;
    //initial offset between the two feet
    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

    public GameObject rightfoot;

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = rightfoot.transform.position;
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        //currentNormal = newNormal = oldNormal = -transform.forward;
        lerp = 1; 
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = currentPosition;
        //transform.forward = -currentNormal;
        //Debug.Log(transform.forward.ToString("F5"));

        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);
        //shooting raycast downward to determine current able leg offset from hip position    
        if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value)) {

            //conditions for initiating a step
            //change here to add loaded bool for stepping condition, change step length to step distance
            if (Vector3.Distance(newPosition, info.point) > stepDistance && lerp >= 1) {
                lerp = 0;
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (body.forward * stepLength * direction) + footOffset;
                //newNormal = info.normal;
                Debug.Log("ray is hit");
            }
        }

        if (lerp < 1) {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            currentPosition = tempPosition;
            //currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else {
            oldPosition = newPosition;
            //oldNormal = newNormal;
        }
    }

    public bool IsMoving() {
        return lerp < 1;
    }
}
