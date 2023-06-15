using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scalemanager : MonoBehaviour
{
    public GameObject charactermodel;
    [SerializeField]float height = 170f;
    public static float height_normalized;
    [SerializeField]float conversionconstant = 1.35f;

    public static float thigh_length;
    public static float shin_length;
    // Start is called before the first frame update
    void Awake()
    {
        height_normalized = height * conversionconstant / 170f;
        thigh_length = height_normalized * 0.245f * 12f;
        shin_length = height_normalized * 0.246f * 12f;
        charactermodel.transform.localScale = new Vector3(height_normalized, height_normalized, height_normalized);
    }
}
