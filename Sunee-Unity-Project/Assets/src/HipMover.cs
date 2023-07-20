using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the script for managing hip movement

public class HipMover : MonoBehaviour
{
    public GameObject hipmodel;
    public GameObject targetmodel;


    public Transform ground;
    public Transform prostheticFoot;

    [SerializeField] float offsetfromgnd = 1f;
    //[SerializeField] float hipdip = 0.85f;
    [SerializeField] float hipfromthigh = 0.15f; 

    public static bool Moving = false;
    public bool movingpros = false;
    public Quaternion startrot;

    public float magnitude;
    // Start is called before the first frame update
    void Start()
    {
        startrot = hipmodel.transform.rotation;
    }

    // Update is called once per frame
    float time = 2.8f;
	void Update () {

		if(time >= 0){
			time -= Time.deltaTime;
		}else{
		    TryHipSim();
            magnitude = (float)(positionManager.stepDistance / 2.855);
		}
    }
    public void TryHipSim() 
    {
        //function for checking which hip to move
        if (Moving) return;
        if (positionManager.loaded == false && Moving == false && positionManager.ankly > -5.8 * Scalemanager.height_normalized) //change 5.8 after further gait analysis and debugging
        {
            //Start moving hip(abled leg is loaded)
            StartCoroutine(MoveHipsim());
            
        }
        if (positionManager.loaded == true) 
        {
            //Start moving hip(prosthetic is loaded)
            StartCoroutine(MoveHippros());
            if (movingpros == false)
            {
                StartCoroutine(Moveprosrot());
            }
        }
        else
        {
            Debug.Log("Loaded null exception");
            //exeption thrown when neither conditions are met, most likely when prosthetic is loaded but no horizontal movement
        }
    }
    float FindHipY(Transform groundpos, Transform footpos)
    {
        //finding the y-value of the hip from the distance from foot position of pprosthetic to ground
        //i.e. when loaded, foot position y-val = ground y-val
        float loadedhipY = groundpos.position.y - footpos.position.y;
        //change here for height determination
        return loadedhipY;
    }
    IEnumerator MoveHipsim() 
    {
        Moving = true;
        float moveDuration = 60 / positionManager.cadence;
        if (moveDuration > 5f) {
            Moving = false;
            Debug.Log("break was called");
            yield break;
        } 
        Debug.Log(moveDuration + "dura" + positionManager.cadence + "cad");
        float timeelapsed = 0f;
        //Move hip function when the able leg is on the ground
        //use stepDistance to find period of sine curve
        Vector3 startpos = hipmodel.transform.position;
        Vector3 targetstartPos = targetmodel.transform.position;
        Quaternion endrot = startrot * Quaternion.Euler(0, magnitude * -7.8f, 0); //setting the goal of that the rotation of the hip will end up on
        Quaternion begRot = hipmodel.transform.rotation; // setting up initial rotaiton of the hip at execution for constant linear interpolation
        do
        {
            if (positionManager.loaded == true) 
            {
                StartCoroutine(MoveHippros());
                Moving = false;
                Debug.Log("broken");
                yield break;
            }
            timeelapsed += Time.deltaTime;
            Vector3 localPos = hipmodel.transform.position;
            Vector3 localtarpos = targetmodel.transform.position;
            /*localPos.y = Mathf.Lerp(
                Mathf.Lerp(startpos.y, hipdip, normalizedTime),
                Mathf.Lerp(hipdip, startpos.y, normalizedTime),
                normalizedTime
            ); */ //quadratic bezier curve made by nested linear interpolation, returns to startpos
            //must alter bezier curve accordingly to gait

            float normalizedTime = timeelapsed / moveDuration;

            Quaternion localRot = Quaternion.Lerp(begRot, endrot, normalizedTime);
            //executing rot interpolation

            //version using a sine curve
            //localPos.y += hipdip * Mathf.Sin(normalizedTime * 2 * Mathf.PI);
            localPos.y = Gethipheight(normalizedTime) * positionManager.stepDistance / 3.9f + startpos.y + 0.17f;
            localtarpos.y = Gethipheight(normalizedTime) * positionManager.stepDistance / 3.9f + targetstartPos.y + 0.17f;

            hipmodel.transform.position = localPos;
            hipmodel.transform.rotation = localRot;
            targetmodel.transform.position = localtarpos;
            Debug.Log("movinghipsim");
            yield return null;
        } while (timeelapsed < moveDuration);
        Moving = false;
    }

    IEnumerator MoveHippros() 
    {
        //move hip fuction when the prosthetic is on the ground.
        Vector3 localPos = hipmodel.transform.position;
        //localPos.y = FindHipY(ground, prostheticFoot) + offsetfromgnd;
        localPos.y = ground.position.y - positionManager.ankly + offsetfromgnd;
        hipmodel.transform.position = Vector3.MoveTowards(hipmodel.transform.position, localPos, 100f);    
        Debug.Log(localPos.y + "cadposy");

        //move target body as well according to hip
        Vector3 targetPos = targetmodel.transform.position;
        //targetPos.y = FindHipY(ground, prostheticFoot) + offsetfromgnd;
        targetPos.y = ground.position.y - positionManager.ankly + offsetfromgnd;
        targetmodel.transform.position = Vector3.MoveTowards(targetmodel.transform.position, targetPos, 100f); 
        yield return null; 
    }
    IEnumerator Moveprosrot()
    {
        movingpros = true;
        float moveDuration = 60 / positionManager.cadence;
        if (moveDuration > 5f) {
            movingpros = false;
            Debug.Log("break was called");
            yield break;
        } 
        float timeelapsed = 0f;
        Quaternion endrot = startrot * Quaternion.Euler(0, 7.8f, 0); //setting rotation goal as the opposite from the simulated version
        Quaternion begRot = hipmodel.transform.rotation;
        do
        {
            if (positionManager.loaded == false) 
            {
                StartCoroutine(MoveHipsim());
                movingpros = false;
                Debug.Log("brokenhip");
                yield break;
            }
            timeelapsed += Time.deltaTime;
            float normalizedTime = timeelapsed / moveDuration;

            Quaternion localRot = Quaternion.Lerp(begRot, endrot, normalizedTime);
            hipmodel.transform.rotation = localRot;
            Debug.Log("movingprosrot");
            yield return null;
        } while (timeelapsed < moveDuration);
        movingpros = false;
    }
    float Gethipheight(float normalizedTime)
    {
        float pos;
        float Time = normalizedTime * 0.62f;
        //parametric according to a normalized version of lerp it goa attention
        if (normalizedTime * 0.62 < 0.4)
        {
            pos = hipfromthigh + offsetfromgnd + (float)(Scalemanager.height_normalized * 12f * (
                - 0.0000063*Mathf.Pow((float)(100 * Time), 3) 
                + 0.0004173*Mathf.Pow((float)(100 * Time), 2) 
                - 0.0059269*Mathf.Pow((float)(100 * Time), 1) 
                + 0.4607267 - 0.491
            ));
        }
        else
        {
            pos = hipfromthigh + offsetfromgnd + (float)(Scalemanager.height_normalized * 12f * (
                - 0.0002851*Mathf.Pow((float)(100 * Time), 2) 
                + 0.0267342*Mathf.Pow((float)(100 * Time), 1) 
                - 0.1322132 - 0.491
            ));
        }
        return pos;
    }
}
