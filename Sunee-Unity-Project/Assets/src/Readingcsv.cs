using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is for reading the csv file given by the prosthetic

public class Readingcsv : MonoBehaviour
{
    public List<Posdata> position = new List<Posdata>();
    
    // Start is called before the first frame update
    void Start ()
    {
        TextAsset csvposdata = Resources.Load<TextAsset>("monitor2");
        //init file
        string[] data = csvposdata.text.Split(new char[] { '\n' });
        //parsing in terms of all values between comma characters
        Debug.Log(data.Length);
        //debug
        for (int i = 1; i < data.Length - 1; i++)
        { 
            string[] row = data[i].Split(new char[] { ',' });
                Posdata p = new Posdata();
                //saving each value as a new list
                //change each value every time data value definitions are changed
                //former parsing script when position values were already given
                // float.TryParse(row[0], out p.time);
                // float.TryParse(row[1], out p.hip_pos_x);
                // float.TryParse(row[2], out p.hip_pos_y);
                // float.TryParse(row[3], out p.hip_pos_z);
                // float.TryParse(row[4], out p.knee_pos_x);
                // float.TryParse(row[5], out p.knee_pos_y);
                // float.TryParse(row[6], out p.knee_pos_z);
                // float.TryParse(row[7], out p.ankl_pos_x);
                // float.TryParse(row[8], out p.ankl_pos_y);
                // float.TryParse(row[9], out p.ankl_pos_z);

                //new script for reading angles
                float.TryParse(row[0], out p.time);
                float.TryParse(row[1], out p.interval);
                float.TryParse(row[2], out p.sampling);
                
                float.TryParse(row[4], out p.hip_angl);
                float.TryParse(row[5], out p.hip_abduction);
                float.TryParse(row[6], out p.knee_angl);
                float.TryParse(row[7], out p.cadence);
                int.TryParse(row[8], out p.loaded);
                int.TryParse(row[9], out p.stance);
                



                position.Add(p);
        }
    }
}
