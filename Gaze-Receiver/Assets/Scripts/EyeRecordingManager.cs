using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class EyeData
{
    public Vector3 HeadPos;
    public Vector3 HitpointPos;

    public EyeData(Vector3 newHeadPos, Vector3 newHitpointPos)
    {
        HeadPos = newHeadPos;
        HitpointPos = newHitpointPos;
    }
}

public class Heatmap
{
    public Vector3 heatPoint;
    public float heatIntensity;

    public Heatmap(Vector3 newHeatPoint, float newHeatIntensity)
    {
        heatPoint = newHeatPoint;
        heatIntensity = newHeatIntensity;
    }
}

public class EyeRecordingManager : MonoBehaviour
{
    public List<EyeData> eyedata = new List<EyeData>();
    public List<Heatmap> heatmap = new List<Heatmap>();

    public GameObject HeadViz;
    public GameObject HitpointViz;
    public GameObject HololensGO;
    public GameObject Origin;

    public GameObject SphereHeatPrefab;
    public GameObject ColorHeatPrefab;

    private List<GameObject> SphereCollection = new List<GameObject>();

    public Material GrayMat, TransparentMat;

    private MeshRenderer HololensRenderer;

    private Gradient gradient;
    private GradientColorKey[] colorKey;
    private GradientAlphaKey[] alphaKey;

    private int frameCount;
    private const float neighborThreshold = 0.001f;

    public enum EyeRecordingState
    {
        Idle, Record, Playback
    }
    public EyeRecordingState EyeRecordState
    {
        get; set;
    }
    private void ChangeState(EyeRecordingState newState)
    {
        if (EyeRecordState != newState) EyeRecordState = newState;
    }

    private void Start()
    {
        ClearEyeGazeData();
        HololensRenderer = HololensGO.GetComponent<MeshRenderer>();

        gradient = new Gradient();

        // set from what key it changes
        colorKey = new GradientColorKey[3];
        colorKey[0].color = Color.green;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.yellow;
        colorKey[1].time = 0.5f;
        colorKey[2].color = Color.red;
        colorKey[2].time = 1.0f;

        // always opaque
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);
    }

    private void FixedUpdate()
    {
        if (EyeRecordState == EyeRecordingState.Idle) return;

        if (EyeRecordState == EyeRecordingState.Record)
        {
            eyedata.Add(new EyeData(HeadViz.transform.position, HitpointViz.transform.position));
        }
        if (EyeRecordState == EyeRecordingState.Playback)
        {
            if (frameCount >= eyedata.Count)
            {
                ChangeState(EyeRecordingState.Idle);
                HololensRenderer.material = TransparentMat;
                return;
            } 

            HeadViz.transform.position = eyedata[frameCount].HeadPos;
            HitpointViz.transform.position = eyedata[frameCount].HitpointPos;
            frameCount++;
        }
    }

    public void RecordEyeGaze()
    {
        ChangeState(EyeRecordingState.Record);
    }
    public void PlaybackEyeGaze()
    {
        frameCount = 0;
        ChangeState(EyeRecordingState.Playback);
        HololensRenderer.material = GrayMat;
    }
    public void StopRecordEyeGaze()
    {
        ChangeState(EyeRecordingState.Idle);
    }
    public void ClearEyeGazeData()
    {
        frameCount = 0;
        eyedata.Clear();
        ChangeState(EyeRecordingState.Idle);
    }

    public void GenerateHeatmapPoints()
    {
        Vector3 prevPos = Vector3.zero;
        float dist = 0;
        int intensity = 0;
        heatmap.Clear();

        //get the hitpos for heatmap
        for (int i = 0; i < eyedata.Count; i += 1)
        {
            if (i == 0) prevPos = eyedata[i].HitpointPos;

            dist = Vector3.Distance(eyedata[i].HitpointPos, prevPos);

            if (dist >= neighborThreshold)
            {
                heatmap.Add(new Heatmap(prevPos, intensity));
                prevPos = eyedata[i].HitpointPos;
                intensity = 0;
            }
            else
            {
                if (intensity < 20) intensity++;
            }
        }
    }

    public void DebugHeatmapPoints()
    {
        foreach (Heatmap heats in heatmap)
        {
            Debug.Log("pos: " + heats.heatPoint.ToString() + " intensity: " + heats.heatIntensity.ToString()) ;
        }
    }

    public void ShowSphereHeatmap()
    {
        float maxIntensity = 0;
        int sphereCounter = 1;
        Vector3 prevPos = new Vector3();
        SphereCollection.Clear();

        foreach (Heatmap heats in heatmap)
        {
            if (heats.heatIntensity > maxIntensity) maxIntensity = heats.heatIntensity;
        }

        for (int i = 0; i < heatmap.Count; i++)
        {
            float sphereScale = heatmap[i].heatIntensity / maxIntensity;

            //if point is not focused enough, ignore
            if (sphereScale < 0.3) continue;

            //if too near each other, don't generate new sphere
            float dist = Vector3.Distance(heatmap[i].heatPoint, prevPos);
            if (dist < 0.1) continue;

            SphereCollection.Add(Instantiate(SphereHeatPrefab, heatmap[i].heatPoint, Quaternion.identity));

            prevPos = heatmap[i].heatPoint;

            SphereHeatPrefab.GetComponentInChildren<TextMeshPro>().text = sphereCounter.ToString();
            sphereCounter++;
        }

        var firstItem = SphereCollection.First();
        firstItem.GetComponentInChildren<TextMeshPro>().text = "0";
    }

    public void DestroySphereHeatmap()
    {
        foreach (GameObject go in SphereCollection)
        {
            Destroy(go);
        }
    }
}
