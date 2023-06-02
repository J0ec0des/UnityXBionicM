using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementmanager : MonoBehaviour
{
    [SerializeField] private GameObject magneticField;
    [SerializeField] private float degreesPerSecond = 90;
    [SerializeField] private float radius = 2;
    [SerializeField] private GameObject sphere;

    // Start is called before the first frame update
    void Start()
    {
        transform.position += new Vector3(radius, 0, 0);
        sphere.transform.localScale += new Vector3(1/(radius * radius), 1/(radius * radius), 1/(radius * radius));
    }

    // Update is called once per frame
    void Update()
    {
         transform.RotateAround(magneticField.transform.position, Vector3.forward, degreesPerSecond * Time.deltaTime);
    }
}
