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
    [SerializeField] float hipdip = 0.85f;

    public static bool Moving = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    float time = 2.8f;
	void Update () {

		if(time >= 0){
			time -= Time.deltaTime;
		}else{
		    TryHipSim();
            Debug.Log("ankly" + positionManager.ankly);
		}
    }
    public void TryHipSim() 
    {
        //function for checking which hip to move
        if (Moving) return;
        if (positionManager.loaded == false && Moving == false && positionManager.ankly > -7.8) //this condition is iffy, must change to variable value to account for height differences
        {
            //Start moving hip(abled leg is loaded)
            StartCoroutine(MoveHipsim());
            
        }
        if (positionManager.loaded == true) 
        {
            //Start moving hip(prosthetic is loaded)
            StartCoroutine(MoveHippros());
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
            float normalizedTime = timeelapsed / moveDuration;
            Vector3 localPos = hipmodel.transform.position;
            localPos.y = Mathf.Lerp(
                Mathf.Lerp(startpos.y, hipdip, normalizedTime),
                Mathf.Lerp(hipdip, startpos.y, normalizedTime),
                normalizedTime
            );  //quadratic bezier curve made by nested linear interpolation, returns to startpos
            //must alter bezier curve accordingly to gait
            Debug.Log("movinghipsim");

            //version using a sine curve
            //localPos.y += hipdip * Mathf.Sin(normalizedTime * 2 * Mathf.PI);

            hipmodel.transform.position = localPos;
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
        yield return new WaitForEndOfFrame(); 
    }
}
