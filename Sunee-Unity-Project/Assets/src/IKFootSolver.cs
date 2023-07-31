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
    float speed = 1;
    //speed of the able leg. Note: once speed of the prosthetic leg can be retrieved via ble, Use getcomponent to retrieve speed of the prosthetic foot. is it the same as the able leg?
    [SerializeField] IKFootSolver otherFoot = default;
    //initializing gameobject of the prosthetic foot for recognition by the script
    float stepDistance;
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
    Vector3 oldPosition, currentPosition, newPosition, midPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;
    float heel;

    //reference for inverse kinematics component
    public TwoBoneIKConstraint constraint;
    public GameObject toeik;
    //init ik constraint as a gameobject
    //can be deleted/made small?



    public GameObject rightfoot;
    public Transform leftfoot;

    //toe ik and toe ik targets
    public GameObject toeiktarget;
    public Transform toe;
    public Transform toehint;
    [SerializeField] private Vector3 _forward = Vector3.forward;

    [SerializeField] Vector3 toeoffsetfromfoot;

    // Start is called before the first frame update
    void Start()
    {
        //initializing foot spacing and other initial positions related to foot
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = midPosition = transform.position;
        currentNormal = newNormal = oldNormal = -transform.forward;
        lerp = 1; 

        constraint = toeik.GetComponent<TwoBoneIKConstraint>(); //init of toe ik constraint for access
    }

    Quaternion approxLookRotation(Vector3 approximateForward, Vector3 exactUp) {
        //function to make object face a certain approximate direction with an assigned up vector
        Quaternion zToUp = Quaternion.LookRotation(exactUp, -approximateForward);
        Quaternion yToz = Quaternion.Euler(90, 0, 0);   //adding static offset
        return zToUp * yToz;
    }

    // Update is called once per frame
    void Update()
    {
        //setting transform
        transform.position = currentPosition;
        transform.rotation = approxLookRotation(body.forward, currentNormal) * Quaternion.Euler(110, 0, 0);

        speed = 1.56f * positionManager.cadence / 60f;
        //updating steplength as value is changed in positionManager script
        stepLength = positionManager.stepDistance * 1.56f;
        stepDistance = (float)(positionManager.stepDistance / 2f);
        Ray ray = new Ray(body.position + (body.right * footSpacing) + (body.up * stepHeight), Vector3.down);
        //shooting raycast downward to determine current able leg offset from hip position    
        if (Physics.Raycast(ray, out RaycastHit info, 100, terrainLayer.value)) {
            Debug.Log(newPosition.x-info.point.x+"difference");
            //conditions for initiating a step
            //change here to add loaded bool for stepping condition, change step length to step distance
            if (Vector3.Distance(newPosition, info.point) > stepDistance * 1.5f && lerp >= 1 && newPosition.x-info.point.x < 0) {
                if (Vector3.Distance(newPosition, info.point) > stepDistance * 1.7f && HipMover.Moving == false && positionManager.loaded == true && positionManager.footcurrentxpos < 2.5 && positionManager.footcurrentxpos > -1) {
                    //when conditions are met to move abled leg
                    lerp = 0;
                    int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                    newPosition = info.point + (body.forward * stepLength * direction) + footOffset;
                    newNormal = info.normal;
                }
                else {
                    midPosition = oldPosition;
                    RaiseHeel(Vector3.Distance(newPosition, info.point));
                }
            }
        }
        else
        {
            //ray debugging
            Debug.Log("Did not Hit" + body.position + (body.right * footSpacing));
        }

        if (lerp < 1) {
            //moving abled leg and rotationg objects when abled leg is moving
            Vector3 tempPosition = Vector3.Lerp(midPosition, newPosition, lerp);

            tempPosition.y += StepHeight(lerp) * Scalemanager.height_normalized * 12f * positionManager.stepDistance / 3.9f;
            currentPosition = tempPosition;
            Vector3 tempNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            //tempNormal.x += Mathf.Sin(Mathf.Deg2Rad * deg)/*Mathf.Sin(lerp * 2 * Mathf.PI) * stepHeight;*/;
            tempNormal.y = Mathf.Cos(Mathf.Deg2Rad * -Footdir(lerp));
            tempNormal.x = -Mathf.Sin(Mathf.Deg2Rad * -Footdir(lerp));
            currentNormal = tempNormal;
            lerp += Time.deltaTime * speed;
            //constraint.data.targetPositionWeight = 0f;

            //constraint.data.targetPositionWeight += 1f - Mathf.Sin(lerp *  Mathf.PI) * 5f; 
            //constraint.data.targetPositionWeight = -6f * Mathf.Pow(lerp, 2) + 1f;
            if (lerp > 0.16) {
                constraint.data.targetPositionWeight = 0;
            }
            //changing weight of inverse kinematics for toe so that rotation is function is applied once the foot leaves the ground
        }
        else {
            oldPosition = newPosition;
            oldNormal = newNormal;
            constraint.data.targetPositionWeight = 1f;
            toeiktarget.transform.position = oldPosition + toeoffsetfromfoot;
        }
    }
    void RaiseHeel(float magnitude) 
    {
        float prevpos = this.transform.position.y;
        //currentPosition.y = oldPosition.y + (speed / 3f) * (magnitude - stepDistance);
        if (currentPosition.y < oldPosition.y + Scalemanager.height_normalized * 12f * positionManager.stepDistance / 3.9f * StepHeight(0f)) 
        {
            currentPosition.y = Mathf.MoveTowards(prevpos, (float)((oldPosition.y + Scalemanager.height_normalized * 12f * positionManager.stepDistance / 3.8f) * StepHeight(0f)) ,(1.4f* speed * positionManager.stepDistance * Time.deltaTime));
        }
        else {
            Debug.Log("raisingheel broken");
        }
        currentPosition.x = midPosition.x = oldPosition.x + (0.2f * speed * (magnitude - stepDistance));
        currentNormal.x = oldNormal.x + (0.2f * (magnitude - stepDistance));
    }

    public bool IsMoving() {
        return lerp < 1;
    }

    //Parametrized regression models implementaiton
    float StepHeight(float lerp) 
    {
         //parametric according to a normalized version of lerp
        //float posnorm = (float)(lerp * 0.55 + 0.05); //normalizing lerp var for regression model
        float temposy;
        // if (posnorm < 0.20)
        // {
        //     temposy =(float)( 
        //         -0.0003347*Mathf.Pow(posnorm*100, 2) 
        //         + 0.0154286*Mathf.Pow(posnorm*100, 1)  
        //         - 0.00236440
        //     );
        // }
        // else if (posnorm < 0.40)
        // {
        //     temposy =(float)( 
        //         0.0002538*Mathf.Pow(posnorm*100, 2)  
        //         - 0.0226260*Mathf.Pow(posnorm*100, 1)  
        //         + 0.5193032
        //     );
        // }
        // else
        // {
        //     temposy =(float)( 
        //         -0.0004432*Mathf.Pow(posnorm*100, 2) 
        //         + 0.0440576*Mathf.Pow(posnorm*100, 1) 
        //         - 1.0432218
        //     );
        // }
        float posnorm = (float)(lerp * 0.44 + 0.16);
        if (posnorm < 0.36)
        {
            temposy =(float)( 
                -0.0008687*Mathf.Pow(posnorm*100, 2) 
                + 0.0469357*Mathf.Pow(posnorm*100, 1)  
                - 0.4427061
            );
        }
        else if (posnorm < 0.50)
        {
            temposy =(float)( 
                0.0006703*Mathf.Pow(posnorm*100, 2)  
                - 0.0646528*Mathf.Pow(posnorm*100, 1)  
                + 1.5823719
            );
        }
        else
        {
            temposy =(float)( 
                -0.0008296*Mathf.Pow(posnorm*100, 2) 
                + 0.0913990*Mathf.Pow(posnorm*100, 1) 
                - 2.4840992
            );
        }
        return temposy;
    }
    float Footdir(float lerp)
    {
        float deg;
        float norm = 64f + lerp * 36f; //normalizing lerp var for regression model
        //parametric model of ankle angle during step
        if (norm < 74) {
            deg =(float)(
                0.6885119*Mathf.Pow(norm, 1)  
                - 9.5340476
            );

        }
        else if (norm < 88) {
            deg =(float)(
                -0.1562798*Mathf.Pow(norm, 2) 
                + 22.5995833*Mathf.Pow(norm, 1) 
                - 775.8403571

            );

        }
        else {
            deg =(float)(
                0.2430060*Mathf.Pow(norm, 2) 
                - 47.2838690*Mathf.Pow(norm, 1) 
                + 2282.2692857
            );
        }
        return deg;
    }
}
