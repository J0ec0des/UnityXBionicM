using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//referencing inverse kinematics package
using UnityEngine.Animations.Rigging;

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
    //spacing between the foot and the center of body 

    //vars for stepping, olf, current, and target
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

    //reference for inverse kinematics component
    public TwoBoneIKConstraint constraint;
    public GameObject toeik;


    public GameObject rightfoot;

    public GameObject toeiktarget;

    [SerializeField] Vector3 toeoffsetfromfoot;

    // Start is called before the first frame update
    void Start()
    {
        //initializing foot spacing and other initial positions related to foot
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = -transform.forward;
        lerp = 1; 

        constraint = toeik.GetComponent<TwoBoneIKConstraint>(); //init of toe ik constraint for access
    }

    Quaternion approxLookRotation(Vector3 approximateForward, Vector3 exactUp) {
        //function to make object face a certain approximate direction with an assigned up vector
        Quaternion zToUp = Quaternion.LookRotation(exactUp, -approximateForward);
        Quaternion yToz = Quaternion.Euler(110, 0, 0);   //adding static offset
        return zToUp * yToz;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(positionManager.loaded + "loaded?" + positionManager.footcurrentxpos);
        //updating steplength as value is changed in positionManager script
        stepLength = positionManager.stepDistance;
        //setting transform
        transform.position = currentPosition;
        transform.rotation = approxLookRotation(body.forward, currentNormal) * Quaternion.Euler(105, 0, 0);
        //setting transform for toe iktarget
        toeiktarget.transform.position = currentPosition + toeoffsetfromfoot;
        Ray ray = new Ray(body.position + (body.right * footSpacing) + (body.up * stepHeight), Vector3.down);
        //shooting raycast downward to determine current able leg offset from hip position    
        if (Physics.Raycast(ray, out RaycastHit info, 100, terrainLayer.value)) {
            //conditions for initiating a step
            //change here to add loaded bool for stepping condition, change step length to step distance
            if (Vector3.Distance(newPosition, info.point) > stepDistance && lerp >= 1 && HipMover.Moving == false && positionManager.loaded == true && positionManager.footcurrentxpos < 2 && positionManager.footcurrentxpos > -0.5) {
                //when conditions are met to move abled leg
                lerp = 0;
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (body.forward * stepLength * direction) + footOffset;
                newNormal = info.normal;
            }
        }
        else
        {
            //ray debugging
            Debug.DrawRay(body.position + (body.right * footSpacing), Vector3.down, Color.red);
            Debug.Log("Did not Hit" + body.position + (body.right * footSpacing));
        }

        if (lerp < 1) {
            //moving abled leg and rotationg objects when abled leg is moving
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            currentPosition = tempPosition;
            Vector3 tempNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            tempNormal.x += Mathf.Sin(lerp * 2 * Mathf.PI) * stepHeight;
            currentNormal = tempNormal;
            lerp += Time.deltaTime * speed;
            constraint.data.targetPositionWeight += 1f - Mathf.Sin(lerp *  Mathf.PI) * 5f; 
            //changing weight of inverse kinematics for toe so that rotation is function is applied once the foot leaves the ground
        }
        else {
            oldPosition = newPosition;
            oldNormal = newNormal;
            constraint.data.targetPositionWeight = 1f;
        }
    }

    public bool IsMoving() {
        return lerp < 1;
    }
}
