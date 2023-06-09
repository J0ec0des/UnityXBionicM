using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the script for managing hip movement

public class HipMover : MonoBehaviour
{
    public GameObject hipmodel;

    public Transform ground;
    public Transform prostheticFoot;

    [SerializeField] float offsetfromgnd = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TryHipSim();
    }
    public void TryHipSim() 
    {
        if (Posdata.loaded == false)
        {
            //Start moving hip
            StartCoroutine(MoveHipsim());
        }
        if (Posdata.loaded == true) 
        {
            MoveHippros();
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
        float timeelapsed = 0;
        //Move hip function when the able leg is on the ground
        //use stepDistance to find period of sine curve
        Vector3 startpos = hipmodel.transform.localPosition;
        Quaternion startRot = hipmodel.transform.localRotation;
        //transform.position = slerp();
        yield return null;
        Debug.Log("movinghipsim");
    }
    void MoveHippros() 
    {
        //move hip fuction when the prosthetic is on the ground.
        hipmodel.transform.localPosition += new Vector3(0, FindHipY(ground, prostheticFoot) + offsetfromgnd, 0);
        //may add interpolation function is resultant animation is janky
        Debug.Log("movinghippros");
    }

    void LateUpdate()
    {
        //insert animation related functions here so environment is deveoped-> animation -> frame rendered
    }

}
