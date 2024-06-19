using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent (typeof(BoxCollider))]
public class BeamSword : MonoBehaviour
{
    // Start is called before the first frame update
    public float length = 1f;
    public float width = 0.1f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void create()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        lineRenderer.positionCount = 2;
        Vector3[] points = new Vector3[2];
        points[0] = new Vector3(0, 0, 0);
        points[1] = new Vector3(0, 0, length);
        boxCollider.center = new Vector3 (0, 0, length/2);
        boxCollider.size = new Vector3(0.1f,0.1f ,length);
        lineRenderer.SetPositions(points);
    }
}
