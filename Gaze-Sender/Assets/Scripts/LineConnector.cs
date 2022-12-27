using UnityEngine;

public class LineConnector : MonoBehaviour
{
    public GameObject HitpointViz;
    public GameObject HeadViz;

    private LineRenderer lr;

    private void Start()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.widthMultiplier = 0.01f;
        lr.startColor = Color.red;
        lr.endColor = Color.red;
    }

    private void Update()
    {
        if (HeadViz == null || HitpointViz == null) return;
        lr.SetPosition(0, HeadViz.transform.position);
        lr.SetPosition(1, HitpointViz.transform.position);
    }
}
