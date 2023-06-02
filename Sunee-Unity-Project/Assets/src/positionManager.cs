using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posdata
{
   public float time,hip_pos_x,hip_pos_y,hip_pos_z,knee_pos_x,knee_pos_y,knee_pos_z,ankl_pos_x,ankl_pos_y,ankl_pos_z;
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
    
    // public List<Hiplist> position = new List<Hiplist>();
    // public List<Kneelist> kneeposition = new List<Kneelist>();
    // public List<Ankllist> anklposition = new List<Ankllist>();

    public float speed = 4;

    // Start is called before the first frame update
    
    IEnumerator Start()
    {
        script = GetComponent<Readingcsv>();
        yield return new WaitForEndOfFrame();
        // foreach (Posdata p in script.position) {
        //     Debug.Log(p.time.ToString("F4"));
        // }
        // while (Posdata.time == Time.time) {
        //     hip.transform.Translate(hip_pos_x, hip_pos_y, hip_pos_z);
        //     knee.transform.Translate(knee_pos_x, knee_pos_y, knee_pos_z);
        //     ankl.transform.Translate(ankl_pos_x, ankl_pos_y, ankl_pos_z);
        // }

        // foreach (Posdata data in script.position) {
        //     Debug.Log("positiontime" + data.time.ToString("F4"));
        //     hip.transform.position = script.hipposition.hpos;
        //     knee.transform.position = script.k.kpos;
        //     ankl.transform.position = script.a.apos;   
        //     foreach (Kneelist i in script.kneeposition) {
        //         Debug.Log("kpos" + i.kpos.ToString("F5"));
        //     }     
        // }

        foreach (Posdata data in script.position) {
            hip.transform.localPosition = new Vector3(data.hip_pos_x * 10, data.hip_pos_z * 10, data.hip_pos_y * 10 );
            knee.transform.localPosition = new Vector3(data.knee_pos_x * 10, data.knee_pos_z * 10, data.knee_pos_y * 10);
            ankl.transform.localPosition = new Vector3(data.ankl_pos_x * 10, data.ankl_pos_z * 10, data.ankl_pos_y * 10);
            //Debug.Log(data.time.ToString("F4") + hip.transform.position.ToString("F5"));
            yield return new WaitForSeconds(0.005f);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
