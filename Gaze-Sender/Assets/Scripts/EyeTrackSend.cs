using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using extOSC;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct EyeDataMarshallingStructure
{
    public Vector3 HeadValue;
    public Vector3 HitpointValue;
}

public class EyeTrackSend : MonoBehaviour
{
    public GameObject HitpointViz;
    public GameObject HeadViz;

    private OSCTransmitter Transmitter;
    private string _address;

    private void Start()
    {
        Transmitter = GameObject.FindObjectOfType<OSCTransmitter>();
        _address = "eyeData";
    }

    private void Update()
    {
        var eyeGazeProvider = CoreServices.InputSystem?.EyeGazeProvider;
        if (eyeGazeProvider != null)
        {
            HeadViz.transform.position = eyeGazeProvider.GazeOrigin;
            RaycastHit hitInfo;
            Physics.Raycast(eyeGazeProvider.GazeOrigin, eyeGazeProvider.GazeDirection, out hitInfo);
            HitpointViz.transform.position = hitInfo.point;

            SendEyeData(HeadViz.transform.localPosition, HitpointViz.transform.localPosition);
        }
    }

    private void SendEyeData(Vector3 headPos, Vector3 hitPos)
    {
        if (headPos == Vector3.zero || hitPos == Vector3.zero) return;

        var message = new OSCMessage(_address);
        var trasformStructure = new EyeDataMarshallingStructure();

        trasformStructure.HeadValue = headPos;
        trasformStructure.HitpointValue = hitPos;

        var bytes = OSCUtilities.StructToByte(trasformStructure);
        message.AddValue(OSCValue.Blob(bytes));

        Transmitter.Send(message);
    }
}
