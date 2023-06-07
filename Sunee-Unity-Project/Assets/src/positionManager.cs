using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posdata
{
   public float time,hip_pos_x,hip_pos_y,hip_pos_z,knee_pos_x,knee_pos_y,knee_pos_z,ankl_pos_x,ankl_pos_y,ankl_pos_z;

   //Add for Experimental script 1
   //public float hip_angl, hip_phi, knee_angl, knee_phi;
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

    private Vector3 kneetarget, ankltarget;
    
    // public List<Hiplist> position = new List<Hiplist>();
    // public List<Kneelist> kneeposition = new List<Kneelist>();
    // public List<Ankllist> anklposition = new List<Ankllist>();

    public float bodyspeed = 4;
    //placeholder for body speed variable, will change according to speed determination by prosthetic
    // delete value when implementing speed variation(experimental script 2)

    [SerializeField] float interpolationspeed = 1000;

    // Start is called before the first frame update
    
    IEnumerator Start()
    {
        script = GetComponent<Readingcsv>();
        yield return new WaitForEndOfFrame();
        foreach (Posdata data in script.position) {
            //hip.transform.localPosition = new Vector3(data.hip_pos_x * 10, data.hip_pos_z * 10, data.hip_pos_y * 10 ); 
            //deleted due to values being 0.

            // knee.transform.localPosition = new Vector3(data.knee_pos_x * 10, data.knee_pos_z * 10, data.knee_pos_y * 10);
            // ankl.transform.localPosition = new Vector3(data.ankl_pos_x * 10, data.ankl_pos_z * 10, data.ankl_pos_y * 10);
            //Debug.Log(data.time.ToString("F4") + hip.transform.position.ToString("F5"));
            
            //making an interpolated version of the transform function to account for the 30ms latency of prosthetics' BLE:
            //first using current vector3 function types:
            kneetarget = new Vector3(data.knee_pos_x * 10, data.knee_pos_z * 10, data.knee_pos_y * 10);
            ankltarget = new Vector3(data.ankl_pos_x * 10, data.ankl_pos_z * 10, data.ankl_pos_y * 10);
            var step = interpolationspeed * Time.deltaTime;
            //interpolationspeed may become a data value given by the prosthetic leg but mostly unnecessary
            knee.transform.localPosition = Vector3.MoveTowards(transform.localPosition, kneetarget, step);
            ankl.transform.localPosition = Vector3.MoveTowards(transform.localPosition, ankltarget, step);
            
            // Check if the position of the target and actual are approximately equal.
            if (Vector3.Distance(knee.transform.localPosition, kneetarget) > 0.01f)
            {
                //force move
                knee.transform.localPosition = new Vector3(data.knee_pos_x * 10, data.knee_pos_z * 10, data.knee_pos_y * 10);
            }
            if (Vector3.Distance(ankl.transform.localPosition, ankltarget) > 0.01f)
            {
                //force move
                ankl.transform.localPosition = new Vector3(data.ankl_pos_x * 10, data.ankl_pos_z * 10, data.ankl_pos_y * 10);
            }
            
            


            //Experimental script1(angle to Vector3 positions)
            // float knee_x = data.thigh_length * cos(data.hip_angl) * sin(data.hip_phi) / (Math.pow(cos(data.hip_phi)) * Math.Pow(sin(data.hip_angl)) + Math.Pow(sin(data.hip_phi)));
            // float knee_z = data.thigh_length * cos(data.hip_phi) * sin(data.hip_angl) / (Math.pow(cos(data.hip_phi)) * Math.Pow(sin(data.hip_angl)) + Math.Pow(sin(data.hip_phi)));
            // float knee_y = data.thigh_length * sin(data.hip_phi) * sin(data.hip_angl) / (Math.pow(cos(data.hip_phi)) * Math.Pow(sin(data.hip_angl)) + Math.Pow(sin(data.hip_phi)));
            // float ankl_x = data.shin_length * cos(data.knee_angl) * sin(data.knee_phi) / (Math.pow(cos(data.knee_phi)) * Math.Pow(sin(data.knee_angl)) + Math.Pow(sin(data.knee_phi)));
            // float ankl_z = data.shin_length * cos(data.knee_phi) * sin(data.knee_angl) / (Math.pow(cos(data.knee_phi)) * Math.Pow(sin(data.knee_angl)) + Math.Pow(sin(data.knee_phi)));
            // float ankl_y = data.shin_length * sin(data.knee_phi) * sin(data.knee_angl) / (Math.pow(cos(data.knee_phi)) * Math.Pow(sin(data.knee_angl)) + Math.Pow(sin(data.knee_phi)));
            //knee.transform.localPosition = new Vector3(knee_x, knee_y, knee_z);
            //ankl.transform.localPosition = new Vector3(knee_x + ankl_x, knee_z + ankl_z, knee_y + ankl_y);

            //Experimental script2(speed varation)
            //speed = data.hip_speed;

            yield return new WaitForSeconds(0.005f);
            //change upper value for refresh rate of serial connection
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * bodyspeed * Time.deltaTime);
        //adding speed to character model
        
    }
}
