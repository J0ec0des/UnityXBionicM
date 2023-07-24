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
    
    //init target objects that are manipulated by transform 
    public GameObject knee;
    
    public GameObject ankl;

    public Readingcsv script;

    public Vector3 kneetarget, ankltarget;


    //Initializing static variable that will be used accross scripts and methods.
    public static float cadence; //init cadence value for global access
    public static bool loaded = true; //init loaded bool for global access

    public static float stepDistance; //init step distance var for global access
    public float currentxpos;

    public static float footcurrentxpos; //init current x-position of proshetic foot for global access

    public static float ankly; //init current y-position of proshetic foot for global access


    // Start is called before the first frame update, coroutine
    IEnumerator Start()
    {
        script = GetComponent<Readingcsv>();
        yield return new WaitForEndOfFrame();
        StartCoroutine(PreDelay());
        yield return new WaitForSeconds(1.77f);
        foreach (Posdata data in script.position) {

            /*
            //hip.transform.localPosition = new Vector3(data.hip_pos_x * 10, data.hip_pos_z * 10, data.hip_pos_y * 10 ); 
            //removed due to values being 0.

            // knee.transform.localPosition = new Vector3(data.knee_pos_x * 10, data.knee_pos_z * 10, data.knee_pos_y * 10);
            // ankl.transform.localPosition = new Vector3(data.ankl_pos_x * 10, data.ankl_pos_z * 10, data.ankl_pos_y * 10);
            //Debug.Log(data.time.ToString("F4") + hip.transform.position.ToString("F5"));
            
            //making an interpolated version of the transform function to account for the 30ms latency of prosthetics' BLE:
            //first using current vector3 function types:
            // kneetarget = new Vector3(data.knee_pos_x * 10, data.knee_pos_z * 10, data.knee_pos_y * 10);
            // ankltarget = new Vector3(data.ankl_pos_x * 10, data.ankl_pos_z * 10, data.ankl_pos_y * 10);
            */

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

            float globalknee_angl = data.hip_angl + data.knee_angl;
            float B = (Mathf.PI / 2) - data.hip_abduction;
            float thighA = (Mathf.PI / 2) - data.hip_angl;
            float shinA = (Mathf.PI / 2) - globalknee_angl;

            //Angle to Vector3 conversion calculations 
            float knee_x = -1f * Scalemanager.thigh_length * Mathf.Cos(thighA) * Mathf.Sin(B) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(thighA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float knee_z = Scalemanager.thigh_length * Mathf.Cos(B) * Mathf.Sin(thighA) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(thighA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float knee_y = -1f * Scalemanager.thigh_length * Mathf.Sin(B) * Mathf.Sin(thighA) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(thighA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float ankl_x = -1f * Scalemanager.shin_length * Mathf.Cos(shinA) * Mathf.Sin(B) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(shinA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float ankl_z = Scalemanager.shin_length * Mathf.Cos(B) * Mathf.Sin(shinA) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(shinA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float ankl_y = -1f * Scalemanager.shin_length * Mathf.Sin(B) * Mathf.Sin(shinA) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(shinA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));


            //saving values as Vector3
            kneetarget = new Vector3(knee_x, knee_y, knee_z);
            ankltarget = new Vector3(knee_x + ankl_x, knee_y + ankl_y, knee_z + ankl_z);
            
            //Making and setting values as they will look at the very future.
            ankly = knee_y + ankl_y;
            footcurrentxpos = knee_x + ankl_x;
            //Init clock for proper time scale calculation based on how often one as been running.
            float time = 0;
            Vector3 startposankl = ankl.transform.localPosition;
            Vector3 startposknee = knee.transform.localPosition;
            while (time < (data.interval * 0.001f))
            {
                //actions to take during the interval between each data value parsed and applied
                ankl.transform.localPosition = Vector3.Lerp(startposankl, ankltarget, time / (data.interval * 0.001f));
                knee.transform.localPosition = Vector3.Lerp(startposknee, kneetarget, time / (data.interval * 0.001f));
                time += Time.deltaTime; //updating time
                yield return null;
            }

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
            }

        }
    }
    IEnumerator PreDelay()
    {
        //coroutine that reads the list before the function that assigns object transform reads the list.
        foreach (Posdata data in script.position) {
            //altering values for easier calculations
            float globalknee_angl = data.hip_angl + data.knee_angl;
            float B = (Mathf.PI / 2) - data.hip_abduction;
            float thighA = (Mathf.PI / 2) - data.hip_angl;
            float shinA = (Mathf.PI / 2) - globalknee_angl;
            float knee_x = -1f * Scalemanager.thigh_length * Mathf.Cos(thighA) * Mathf.Sin(B) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(thighA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            float ankl_x = -1f * Scalemanager.shin_length * Mathf.Cos(shinA) * Mathf.Sin(B) / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(B), 2f) * Mathf.Pow(Mathf.Sin(shinA), 2f) + Mathf.Pow(Mathf.Sin(B), 2f)));
            positionManager.cadence = data.cadence;

            // setting loaded booleans for globals usel
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
        transform.Translate(Vector3.right * (GetBodySpeed() * 1.15f) * Time.deltaTime);
        //adding speed to character model;

        //move object back to world 0 for saving mem *removed due to bug
        // if (GetBodySpeed() <= 3f && transform.position.x > 50)
        // {
        //     transform.position = new Vector3 (0, 0.28f, 0);
        // }
    }

    public float lastpos;
    private void GetFootLength() 
    {
        stepDistance = (lastpos - currentxpos) / 2;
    }
    public float GetBodySpeed()
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
