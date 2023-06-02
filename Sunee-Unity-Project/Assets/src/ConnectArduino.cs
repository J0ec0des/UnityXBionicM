using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ConnectArduino : MonoBehaviour
{
    SerialPort arddatastream = new SerialPort("/dev/cu.usbserial-110", 9600);
    public string recievedstring;
    public GameObject sphere; 
    public float randposx;
    public float randposy;
    public float randposz;
    public float randnum1;
    public float randnum2;
    public float randnum3;
    public float randnum4;

    // Start is called before the first frame update
    void Start()
    {
        arddatastream.Open();
    }
    
    // Update is called once per frame
    void Update()
    {
        recievedstring = arddatastream.ReadLine();
        string[] row = recievedstring.Split(',');
        Debug.Log(recievedstring);
        float.TryParse(row[0], out randposx);
        float.TryParse(row[1], out randposy);
        float.TryParse(row[2], out randposz);
        sphere.transform.position = new Vector3(randposx, randposy, randposz);
        float.TryParse(row[3], out randnum1);
        Debug.Log("randum1" + randnum1);
        float.TryParse(row[4], out randnum2);
        Debug.Log("randum2" + randnum2);
        float.TryParse(row[5], out randnum3);
        Debug.Log("randum3" + randnum3);
        float.TryParse(row[6], out randnum4);
        Debug.Log("randum4" + randnum4);
    }
}
