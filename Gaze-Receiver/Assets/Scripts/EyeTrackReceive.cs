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

public class EyeTrackReceive : MonoBehaviour
{
    public GameObject HitpointViz;
    public GameObject HeadViz;
    public EyeRecordingManager _eyeRecordingManager;

    private OSCReceiver Receiver;
    private string _address;

    private void Start()
    {
        _address = "eyeData";
        Receiver = GameObject.FindObjectOfType<OSCReceiver>();
        Receiver.Bind(_address, HandleEyeData);
    }

    private void HandleEyeData(OSCMessage message)
    {
        if (_eyeRecordingManager.EyeRecordState == EyeRecordingManager.EyeRecordingState.Playback) return;

        byte[] bytes;

        if (!message.ToBlob(out bytes))
            return;

        var trasformStructure = OSCUtilities.ByteToStruct<EyeDataMarshallingStructure>(bytes);

        HeadViz.transform.localPosition = trasformStructure.HeadValue;
        HitpointViz.transform.localPosition = trasformStructure.HitpointValue;
    }
}
