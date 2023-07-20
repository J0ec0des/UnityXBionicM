using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrainmanager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float terrainOffset = 0.2f;
    public GameObject toe;

    void Start()
    {
        Vector3 targetPos = this.transform.position;
        targetPos.y = toe.transform.position.y - terrainOffset;
        this.transform.position = targetPos;   
    }
}
