using M2MqttUnity;
using System.Globalization;
using UnityEngine;

public class AppController : MonoBehaviour
{
    public BaseClient baseClient;
    public AnchorScript m_anchorScript;
    public EyeRecordingManager m_eyeRecordingManager;
    public ObjectPositionRecorder m_positionRecorder;
    public HeatmapController m_heatmapController;

    private float heatColor, heatSize, heatDistance;

    private void OnEnable()
    {
        baseClient.RegisterTopicHandler("M2MQTT/recording", HandleRecording);
        baseClient.RegisterTopicHandler("M2MQTT/calibration", HandleCalibration);
        baseClient.RegisterTopicHandler("M2MQTT/heatmap", HandleHeatmap);
        baseClient.RegisterTopicHandler("M2MQTT/heatColor", HandleHeatColor);
        baseClient.RegisterTopicHandler("M2MQTT/heatSize", HandleHeatSize);
        baseClient.RegisterTopicHandler("M2MQTT/heatDistance", HandleHeatDistance);
    }

    private void OnDisable()
    {
        baseClient.UnregisterTopicHandler("M2MQTT/recording", HandleRecording);
        baseClient.RegisterTopicHandler("M2MQTT/calibration", HandleCalibration);
        baseClient.RegisterTopicHandler("M2MQTT/heatmap", HandleHeatmap);
        baseClient.UnregisterTopicHandler("M2MQTT/heatColor", HandleHeatColor);
        baseClient.UnregisterTopicHandler("M2MQTT/heatSize", HandleHeatSize);
        baseClient.UnregisterTopicHandler("M2MQTT/heatDistance", HandleHeatDistance);
    }

    private void HandleCalibration(string topic, string message)
    {
        if (message == "calibrateReceiver") m_anchorScript.StartVuforiaCamera();
    }

    private void HandleRecording(string topic, string message)
    {
        if (topic != "M2MQTT/recording") return;

        if (message == "record")
        {
            m_eyeRecordingManager.RecordEyeGaze();
            m_positionRecorder.RecordEvents = true;
        }

        if (message == "stop")
        {
            m_eyeRecordingManager.StopRecordEyeGaze();
            m_positionRecorder.RecordEvents = false;
        }

        if (message == "playback") m_eyeRecordingManager.PlaybackEyeGaze();
        
        if (message == "clear") m_eyeRecordingManager.ClearEyeGazeData();
    }

    private void HandleHeatmap(string topic, string message)
    {
        if (message == "generateHeatmap")
        {
            m_eyeRecordingManager.GenerateHeatmapPoints();
            m_heatmapController.LoadEvents();
        }
        if (message == "showSphereHeatmap")
        { 
            m_eyeRecordingManager.ShowSphereHeatmap(); 
        }
        if (message == "showColorHeatmap")
        {
            m_heatmapController.InitializeParticleSystem();
            m_heatmapController.AddSelectedEventsToHeatmap();
        }
        if (message == "destroySphereHeatmap") 
        {
            m_eyeRecordingManager.DestroySphereHeatmap(); 
        }
        if (message == "destroyColorHeatmap") 
        {
            m_heatmapController.ResetHeatmap();
        }
    }

    private void HandleHeatColor(string topic, string message)
    {
        if (topic != "M2MQTT/heatColor") return;

        bool success = float.TryParse(message, NumberStyles.Float, CultureInfo.InvariantCulture, out heatColor);
        if (success) m_heatmapController.settings.colorMultiplier = heatColor;
    }

    private void HandleHeatSize(string topic, string message)
    {
        if (topic != "M2MQTT/heatSize") return;

        bool success = float.TryParse(message, NumberStyles.Float, CultureInfo.InvariantCulture, out heatSize);
        if (success) m_heatmapController.settings.particleSize = heatSize;
    }

    private void HandleHeatDistance(string topic, string message)
    {
        if (topic != "M2MQTT/heatDistance") return;

        bool success = float.TryParse(message, NumberStyles.Float, CultureInfo.InvariantCulture, out heatDistance);
        if (success) m_heatmapController.settings.maxColoringDistance = heatDistance;
    }

}
