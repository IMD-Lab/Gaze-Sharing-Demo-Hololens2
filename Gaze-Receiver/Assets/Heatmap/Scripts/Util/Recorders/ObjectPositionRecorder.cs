using UnityEngine;

public class ObjectPositionRecorder : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToRecord = null;
    [SerializeField]
    private string dataPath;
    [SerializeField]
    private string eventName;
    [SerializeField]
    private bool createFileIfNonFound;
    [SerializeField]
    private float recordInterval = 0.5F;
    [SerializeField]
    private bool recordEvents = true;

    private IEventWriter eventWriter;

    private float timer = 0F;

    public bool RecordEvents
    {
        get { return recordEvents; } set { recordEvents = value; }
    }

    void Start()
    {
        dataPath = Application.persistentDataPath + "/heatmap.txt";
        //Debug.Log(dataPath);

        eventWriter = new JSONEvenWriter(dataPath, createFileIfNonFound);

        recordEvents = false;
    }

    void Update()
    {
        if (recordEvents)
        {
            if (timer > recordInterval)
            {
                RecordGameobjectPosition();

                timer = 0F;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    private void RecordGameobjectPosition()
    {
        BaseEvent baseEvent = PrepareData();

        eventWriter.SaveEventInstance(baseEvent);
    }


    private BaseEvent PrepareData()
    {
        return new BaseEvent(eventName, objectToRecord.transform.position);
    }

    private bool IsObjectOnScene()
    {
        if (objectToRecord == null) return false;

        return (objectToRecord.scene.name != null);
    }
}
