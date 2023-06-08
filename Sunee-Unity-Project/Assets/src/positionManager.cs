using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posdata
{
   public float time,hip_pos_x,hip_pos_y,hip_pos_z,knee_pos_x,knee_pos_y,knee_pos_z,ankl_pos_x,ankl_pos_y,ankl_pos_z;

   //Add for Experimental script 1
   //public float hip_angl, hip_abduction, knee_angl;
   //[SerializeField]public float thigh_length = 4;
   //[SerializeField]public float shin_length = 4; <-variable value that must be solved

   //Add for Experimental script 2
   //public float hip_speed;
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
    public GameObject hip;
    
    public GameObject knee;
    
    public GameObject ankl;
    public Readingcsv script;

    public Vector3 kneetarget, ankltarget;

    
    // public List<Hiplist> position = new List<Hiplist>();
    // public List<Kneelist> kneeposition = new List<Kneelist>();
    // public List<Ankllist> anklposition = new List<Ankllist>();

    public float bodyspeed = 4;
    //placeholder for body speed variable, will change according to speed determination by prosthetic
    // delete value when implementing speed variation(experimental script 2)


    // Start is called before the first frame update
    
    IEnumerator Start()
    {
        script = GetComponent<Readingcsv>();
        yield return new WaitForEndOfFrame();
        foreach (Posdata data in script.position) {
            //hip.transform.localPosition = new Vector3(data.hip_pos_x * 10, data.hip_pos_z * 10, data.hip_pos_y * 10 ); 
            //removed due to values being 0.

            // knee.transform.localPosition = new Vector3(data.knee_pos_x * 10, data.knee_pos_z * 10, data.knee_pos_y * 10);
            // ankl.transform.localPosition = new Vector3(data.ankl_pos_x * 10, data.ankl_pos_z * 10, data.ankl_pos_y * 10);
            //Debug.Log(data.time.ToString("F4") + hip.transform.position.ToString("F5"));
            
            //making an interpolated version of the transform function to account for the 30ms latency of prosthetics' BLE:
            //first using current vector3 function types:
            kneetarget = new Vector3(data.knee_pos_x * 10, data.knee_pos_z * 10, data.knee_pos_y * 10);
            ankltarget = new Vector3(data.ankl_pos_x * 10, data.ankl_pos_z * 10, data.ankl_pos_y * 10);

            //lerp function
            float time = 0;
            while (time < 0.005f)
            {
                ankl.transform.localPosition = Vector3.Lerp(ankl.transform.localPosition, ankltarget, time / 0.005f);
                knee.transform.localPosition = Vector3.Lerp(knee.transform.localPosition, kneetarget, time / 0.005f);
                time += Time.deltaTime;
                yield return null;
                //Debug.Log("lerped");
            }

            //Experimental script1(angle to Vector3 positions)
            //float globalknee_angl = data.hip_angl + data.knee_angl;
            // float knee_x = data.thigh_length * cos(data.hip_angl) * sin(data.hip_abduction) / (Math.pow(cos(data.hip_abduction)) * Math.Pow(sin(data.hip_angl)) + Math.Pow(sin(data.hip_abduction)));
            // float knee_z = data.thigh_length * cos(data.hip_abduction) * sin(data.hip_angl) / (Math.pow(cos(data.hip_abduction)) * Math.Pow(sin(data.hip_angl)) + Math.Pow(sin(data.hip_abduction)));
            // float knee_y = data.thigh_length * sin(data.hip_abduction) * sin(data.hip_angl) / (Math.pow(cos(data.hip_abduction)) * Math.Pow(sin(data.hip_angl)) + Math.Pow(sin(data.hip_abduction)));
            // float ankl_x = data.shin_length * cos(globalknee_angl) * sin(data.hip_abduction) / (Math.pow(cos(data.hip_abduction)) * Math.Pow(sin(globalknee_angl)) + Math.Pow(sin(data.hip_abduction)));
            // float ankl_z = data.shin_length * cos(data.hip_abduction) * sin(globalknee_angl) / (Math.pow(cos(data.hip_abduction)) * Math.Pow(sin(globalknee_angl)) + Math.Pow(sin(data.hip_abduction)));
            // float ankl_y = data.shin_length * sin(data.hip_abduction) * sin(globalknee_angl) / (Math.pow(cos(data.hip_abduction)) * Math.Pow(sin(globalknee_angl)) + Math.Pow(sin(data.hip_abduction)));
            //knee.transform.localPosition = new Vector3(knee_x, knee_y, knee_z);
            //ankl.transform.localPosition = new Vector3(knee_x + ankl_x, knee_z + ankl_z, knee_y + ankl_y);

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
            }
            if (Vector3.Distance(ankl.transform.localPosition, ankltarget) > 0.001f)
            {
                //force move
                ankl.transform.localPosition = ankltarget;
                //Debug.Log("catch exception");
            }


        }
    }
    
    // Update is called once per frame
    void Update()
    {
       transform.Translate(Vector3.right * bodyspeed * Time.deltaTime);
        //adding speed to character model
    }
}
