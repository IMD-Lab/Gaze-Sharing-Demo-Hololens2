using UnityEngine;

public class LineConnector : MonoBehaviour
{
    public GameObject HitpointViz;
    public GameObject HeadViz;
    public Material lineMaterial;

    private LineRenderer lr;

    private void Start()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startWidth = 0.001f;
        lr.endWidth = 0.05f;
    }

    private void Update()
    {
        if (HeadViz == null || HitpointViz == null) return;
        lr.SetPosition(0, HeadViz.transform.position);
        lr.SetPosition(1, HitpointViz.transform.position);
    }
}
