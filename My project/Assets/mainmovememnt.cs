using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainmovememnt : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float scale = 5;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale += new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.forward
        
    }
}
