using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Readingcsv : MonoBehaviour
{
    public List<Posdata> position = new List<Posdata>();
    public List<Hiplist> hipposition = new List<Hiplist>();
    public List<Kneelist> kneeposition = new List<Kneelist>();
    public List<Ankllist> anklposition = new List<Ankllist>();
    
    // Start is called before the first frame update
    void Start ()
    {
        TextAsset csvposdata = Resources.Load<TextAsset>("gait");
        string[] data = csvposdata.text.Split(new char[] { '\n' });
        Debug.Log(data.Length);
        for (int i = 1; i < data.Length - 1; i++)
        { 
            string[] row = data[i].Split(new char[] { ',' });
                Posdata p = new Posdata();
                float.TryParse(row[0], out p.time);
                float.TryParse(row[1], out p.hip_pos_x);
                float.TryParse(row[2], out p.hip_pos_y);
                float.TryParse(row[3], out p.hip_pos_z);
                float.TryParse(row[4], out p.knee_pos_x);
                float.TryParse(row[5], out p.knee_pos_y);
                float.TryParse(row[6], out p.knee_pos_z);
                float.TryParse(row[7], out p.ankl_pos_x);
                float.TryParse(row[8], out p.ankl_pos_y);
                float.TryParse(row[9], out p.ankl_pos_z);
                position.Add(p);
        }
        // foreach (Posdata p in position) {
        //     Debug.Log(p.time);
        //     // Debug.Log(p.ankl_pos_x);
        //     Hiplist h = new Hiplist();
        //     hipposition.Add(
        //         new Hiplist() {
        //             hpos = new Vector3(p.hip_pos_x * 10 , p.hip_pos_y * 10 , p.hip_pos_z * 10)
        //         }
        //     );
        //     Kneelist k = new Kneelist();
        //     kneeposition.Add(
        //         new Kneelist() {
        //             kpos = new Vector3(p.knee_pos_x * 10, p.knee_pos_y * 10, p.knee_pos_z * 10)
        //         }
        //     );
        //     Ankllist a = new Ankllist();
        //     anklposition.Add(
        //         new Ankllist() {
        //             apos = new Vector3(p.ankl_pos_x * 10, p.ankl_pos_y * 10, p.ankl_pos_z * 10)
        //         }
        //     );       
        // }
        // foreach (Ankllist a in anklposition) {
        //     Debug.Log("apos" + a.apos.ToString("F5"));
        // }
        // foreach (Kneelist k in kneeposition) {
        //     Debug.Log("kpos" + k.kpos.ToString("F5"));
        // }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
