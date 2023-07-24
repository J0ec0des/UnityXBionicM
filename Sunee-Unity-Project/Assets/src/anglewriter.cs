using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class anglewriter : MonoBehaviour
{
    public Transform ankle;
    public Transform thigh;
    public Transform hip;
    public List<string> lines = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Writeangle());
    }

    // Update is called once per frame
    IEnumerator Writeangle() 
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1.77f);
        while (true)
        {
            Debug.Log("appended");
            float timeelapsed = Time.time;
            float hipangle = Vector3.Angle(new Vector3 (0,-1,0), thigh.transform.up);
            float kneeangle = Vector3.Angle(thigh.transform.up, transform.up); 
            float ankleangle = Vector3.Angle(transform.up, ankle.transform.up);
            float posx = ankle.transform.position.x - hip.position.x;
            float posy = ankle.transform.position.y - hip.position.y;
            lines.Add(timeelapsed.ToString("F4")+","+hipangle.ToString("F4")+","+kneeangle.ToString("F4")+","+ankleangle.ToString("F4")+","+posx.ToString("F4")+","+posy.ToString("F4"));
            Writecsv(lines);
            yield return new WaitForSeconds(0.03f);
        }
        yield return null;
    }

    private void Writecsv(IEnumerable<string> lines)
    {
        using (StreamWriter sw = new StreamWriter(@"Assets/Resources/data.csv", false, Encoding.GetEncoding("shift-jis")))
        {
            foreach (string line in lines)
            {   
                sw.WriteLine(line);
            }
            sw.Close();
        }
    }
}
