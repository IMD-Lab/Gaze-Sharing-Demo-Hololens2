using M2MqttUnity;
using System.Globalization;
using UnityEngine;

public class AppController : MonoBehaviour
{
    public BaseClient baseClient;
    public AnchorScript m_anchorScript;

    private void OnEnable()
    {
        baseClient.RegisterTopicHandler("M2MQTT/calibration", HandleCalibration);
    }

    private void OnDisable()
    {
        baseClient.RegisterTopicHandler("M2MQTT/calibration", HandleCalibration);
    }


    private void HandleCalibration(string topic, string message)
    {
        if (message == "calibrateSender") m_anchorScript.StartVuforiaCamera();
    }

}
