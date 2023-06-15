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

    public bool Moving = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    float time = 0.5f;
    void Update()
    {
		// if(time >= 0){
		// 	time -= Time.deltaTime;
		// 	return;
		//}else{
		    if (Moving) return;
            TryHipSim();
            Debug.Log(Moving);
		//}
       //Do Something else while clock counting down        

    }
    public void TryHipSim() 
    {
        if (Moving) return;
        if (positionManager.loaded == false && Moving ==false)
        {
            //Start moving hip
            StartCoroutine(MoveHipsim());
            
        }
        if (positionManager.loaded == true) 
        {
            MoveHippros();
        }
        else
        {
            Debug.Log("Loaded null exception");
        }
    }
    float FindHipY(Transform groundpos, Transform footpos)
    {
        float loadedhipY = groundpos.position.y - footpos.position.y;
        //change here for height determination
        return loadedhipY;
    }
    IEnumerator MoveHipsim() 
    {
        Moving = true;
        float moveDuration;
        moveDuration = 60 / positionManager.cadence;
        Debug.Log(positionManager.cadence + "cad");
        float timeelapsed = 0f;
        //Move hip function when the able leg is on the ground
        //use stepDistance to find period of sine curve
        Vector3 startpos = hipmodel.transform.position;

        do
        {
            timeelapsed += Time.deltaTime;
            float normalizedTime = timeelapsed / moveDuration;
            Vector3 localPos = hipmodel.transform.position;
            localPos.y = Mathf.Lerp(
                Mathf.Lerp(startpos.y, hipdip, normalizedTime),
                Mathf.Lerp(hipdip, startpos.y, normalizedTime),
                normalizedTime
            );
            //quadratic bezier curve made by nested linear interpolation, returns to startpos
            //must alter bezier curve accordingly to gait
            Debug.Log("movinghipsim");
            //version using a sine curve
            //localPos.y += hipdip * Mathf.Sin(normalizedTime * 2 * Mathf.PI);

            hipmodel.transform.position = localPos;
            yield return null;
        } while (timeelapsed < moveDuration);

        Moving = false;
    }
    void MoveHippros() 
    {
        //move hip fuction when the prosthetic is on the ground.
        Vector3 localPos = hipmodel.transform.position;
        localPos.y = FindHipY(ground, prostheticFoot) + offsetfromgnd;
        hipmodel.transform.position = localPos;   
        //may add interpolation function is resultant animation is janky
        Debug.Log(localPos.y + "posy");

        //move target body as well according to hip
        Vector3 targetPos = targetmodel.transform.position;
        targetPos.y = FindHipY(ground, prostheticFoot) + offsetfromgnd;
        targetmodel.transform.position = targetPos;   
    }

    // void LateUpdate()
    // {
    //     //insert animation related functions here so environment is deveoped-> animation -> frame rendered
    // }

}
