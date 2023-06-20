using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posdata
{
   public float time,hip_pos_x,hip_pos_y,hip_pos_z,knee_pos_x,knee_pos_y,knee_pos_z,ankl_pos_x,ankl_pos_y,ankl_pos_z;

   public float interval,sampling;
   public int stance; //may use in the future 

   //Add for Experimental script 1
   public float hip_angl, hip_abduction, knee_angl;

   //Add for Experimental script 2
   public float hip_speed;

    // initializing cadence and loaded
    public float cadence;
    public int loaded;
}
public class Hiplist {
    public Vector3 hpos;
}
public class Kneelist {
    public Vector3 kpos;
}
public class Ankllist {
    public Vector3 apos;
}
public class positionManager : MonoBehaviour
{
    //likely unnecessry
    public GameObject hip;
    
    public GameObject knee;
    
    public GameObject ankl;

    public Readingcsv script;

    public Vector3 kneetarget, ankltarget;

    public static float cadence;
    public static bool loaded = true;

    //Add for experimental script 1:
    //[SerializeField]public float thigh_length = 4;
    //[SerializeField]public float shin_length = 4; <-variable value that must be solved

    
    // public List<Hiplist> position = new List<Hiplist>();
    // public List<Kneelist> kneeposition = new List<Kneelist>();
    // public List<Ankllist> anklposition = new List<Ankllist>();

    public float bodyspeed = 4;
    //placeholder for body speed variable, will change according to speed determination by prosthetic
    // delete value when implementing speed variation(experimental script 2)

    public static float stepDistance;
    public float currentxpos;

    public static float ankly;


    // Start is called before the first frame update, coroutine
    IEnumerator Start()
    {
        script = GetComponent<Readingcsv>();
        yield return new WaitForEndOfFrame();
        StartCoroutine(PreDelay());
        yield return new WaitForSeconds(2.5f);
        foreach (Posdata data in script.position) {
            //hip.transform.localPosition = new Vector3(data.hip_pos_x * 10, data.hip_pos_z * 10, data.hip_pos_y * 10 ); 
            //removed due to values being 0.

            // knee.transform.localPosition = new Vector3(data.knee_pos_x * 10, data.knee_pos_z * 10, data.knee_pos_y * 10);
            // ankl.transform.localPosition = new Vector3(data.ankl_pos_x * 10, data.ankl_pos_z * 10, data.ankl_pos_y * 10);
            //Debug.Log(data.time.ToString("F4") + hip.transform.position.ToString("F5"));
            
            //making an interpolated version of the transform function to account for the 30ms latency of prosthetics' BLE:
            //first using current vector3 function types:
            // kneetarget = new Vector3(data.knee_pos_x * 10, data.knee_pos_z * 10, data.knee_pos_y * 10);
            // ankltarget = new Vector3(data.ankl_pos_x * 10, data.ankl_pos_z * 10, data.ankl_pos_y * 10);

            //Moved to predelay func
            //positionManager.cadence = data.cadence;
            if (data.loaded == 1) 
            {
                positionManager.loaded = true;
            }
            else if (data.loaded == 0) 
            {
                positionManager.loaded = false;
            }
            else 
            {
                Debug.Log("exeptionerror");
            }
            Debug.Log(positionManager.loaded + "cadpos");
            float globalknee_angl = data.hip_angl + data.knee_angl;
            float B = (Mathf.PI / 2) - data.hip_abduction;
            float thighA = (Mathf.PI / 2) - data.hip_angl;
            float shinA = (Mathf.PI / 2) - globalknee_angl;

            //Experimental script1(angle to Vector3 positions)
            float knee_x = -1f * Scalemanager.thigh_length * Mathf.Cos(thighA) * Mathf.Sin(B) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(thighA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float knee_z = Scalemanager.thigh_length * Mathf.Cos(B) * Mathf.Sin(thighA) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(thighA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float knee_y = -1f * Scalemanager.thigh_length * Mathf.Sin(B) * Mathf.Sin(thighA) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(thighA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float ankl_x = -1f * Scalemanager.shin_length * Mathf.Cos(shinA) * Mathf.Sin(B) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(shinA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float ankl_z = Scalemanager.shin_length * Mathf.Cos(B) * Mathf.Sin(shinA) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(shinA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float ankl_y = -1f * Scalemanager.shin_length * Mathf.Sin(B) * Mathf.Sin(shinA) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(shinA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));

            kneetarget = new Vector3(knee_x, knee_y, knee_z);
            ankltarget = new Vector3(knee_x + ankl_x, knee_y + ankl_y, knee_z + ankl_z);
            
            ankly = knee_y + ankl_y;

            float time = 0;
            while (time < (data.interval * 0.001f))
            {
                ankl.transform.localPosition = Vector3.Lerp(ankl.transform.localPosition, ankltarget, time / data.interval);
                knee.transform.localPosition = Vector3.Lerp(knee.transform.localPosition, kneetarget, time / data.interval);
                time += Time.deltaTime;
                yield return null;
                Debug.Log("force lerped");
            }


            //Experimental script2(speed variation)
            //speed = data.hip_speed;

            //yield return new WaitForSeconds(0.005f);
            //change upper value for refresh rate of serial connection
            //potential and/or necessity for automation once serial connication logistics are updated
            //*temporarily removed for lerp function

            // Check if the position of the target and actual are approximately equal.
            if (Vector3.Distance(knee.transform.localPosition, kneetarget) > 0.001f)
            {
                //force move
                knee.transform.localPosition = kneetarget;
                Debug.Log("force moved");
            }
            if (Vector3.Distance(ankl.transform.localPosition, ankltarget) > 0.001f)
            {
                //force move
                ankl.transform.localPosition = ankltarget;
                //Debug.Log("catch exception");
            }

        }
    }
    IEnumerator PreDelay()
    {
        foreach (Posdata data in script.position) {
            float globalknee_angl = data.hip_angl + data.knee_angl;
            float B = (Mathf.PI / 2) - data.hip_abduction;
            float thighA = (Mathf.PI / 2) - data.hip_angl;
            float shinA = (Mathf.PI / 2) - globalknee_angl;
            float knee_x = -1f * Scalemanager.thigh_length * Mathf.Cos(thighA) * Mathf.Sin(B) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(thighA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float ankl_x = -1f * Scalemanager.shin_length * Mathf.Cos(shinA) * Mathf.Sin(B) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(shinA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            positionManager.cadence = data.cadence;
            if (data.loaded == 1 && loadbool == false) 
            {
                loadbool = true;
            }
            else if (data.loaded == 0 && loadbool == true) 
            {
                loadbool = false;
            }
            else 
            {
                Debug.Log("exeptionerror predelay");
            }
        //saving xpos as value
        currentxpos = knee_x + ankl_x;
        yield return new WaitForSeconds(data.interval * 0.001f);
        }
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        transform.Translate(Vector3.right * (GetBodySpeed() + 0.1f) * Time.deltaTime);
        //adding speed to character model
        
    }

    public float lastpos;
    private void GetFootLength() 
    {
        stepDistance = (lastpos - currentxpos) / 2;
    }
    float GetBodySpeed()
    {
        float speed = stepDistance * positionManager.cadence / 60;
        return speed;
    }

    // //check if loaded is changed
    private bool _loadbool;
    public bool loadbool
    {
        get { return _loadbool; }
        set
        {
            //triggering x-value recorder
            if (_loadbool == false && value == true)
            {
                lastpos = currentxpos;
            }
            if (_loadbool == true && value == false)
            {
                GetFootLength();
                Debug.Log("boolchanged" + stepDistance);
            }
            _loadbool = value;
        }
    }
}
