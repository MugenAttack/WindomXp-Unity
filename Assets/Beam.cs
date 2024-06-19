using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Beam : MonoBehaviour
{
    LineRenderer lineRenderer;
    public bool updateBeam = true;
    public float Speed = 0.1f;
    public float segmentLengthLimit = 2f;
    public float beamLengthLimit = 20f;
    List<Vector3> points;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        points = new List<Vector3>();
        points.Add(transform.position);
        points.Add(transform.position + (transform.forward * Speed));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Speed > segmentLengthLimit || Vector3.Distance(points[0],points[points.Count - 1]) > beamLengthLimit)
            updateBeam = false;

        if (Vector3.Distance(points[points.Count - 2], points[points.Count - 1]) < segmentLengthLimit)
            points[points.Count - 1] += transform.forward * Speed;
        else
        {
            points.Add(points[points.Count - 1] + transform.forward * Speed);
        }

        
        lineRenderer.SetPositions(points.ToArray());
        lineRenderer.positionCount = points.Count;
    }
}
