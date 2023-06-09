using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the script for managing hip movement

public class HipMover : MonoBehaviour
{
    public GameObject hipmodel;

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
        if (Posdata.loaded = false)
        {
            //Start moving hip
            StartCoroutine(MoveHipsim());
        }
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
    }
    IEnumerator MoveHippros() 
    {
        //move hip fuction when the prosthetic is on the ground.
        
        yield return null;
    }

    void LateUpdate()
    {
        //insert animation related functions here so environment is deveoped-> animation -> frame rendered
    }

}
