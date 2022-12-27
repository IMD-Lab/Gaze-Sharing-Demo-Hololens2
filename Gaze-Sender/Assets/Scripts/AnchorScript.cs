using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.OpenXR.ARSubsystems;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Vuforia;

// AnchorScript implementation from https://github.com/peted70/LocalAnchors

[RequireComponent(typeof(ARAnchorManager))]
public class AnchorScript : MonoBehaviour
{
    [SerializeField]
    private bool calibOnStart;

    private ARAnchorManager _arAnchorManager;
    private XRAnchorStore _anchorStore;
    private readonly string AnchorName = "AnchorSampleAnchor";

    // This is the one gameObject that will be anchored in this sample. If a new anchor position is created
    // then we will destry this and create a new one so there is only ever one.
    private GameObject _currentAnchor;

    public GameObject placingGameObject;
    public GameObject anchorPrefab;
    public GameObject actualContent;
    public GameObject cubeVuforia;

    async void Start()
    {
        if(!calibOnStart) VuforiaApplication.Instance.OnVuforiaStarted += StopVuforiaCamera;

        _arAnchorManager = GetComponent<ARAnchorManager>();
        if (!_arAnchorManager.enabled || _arAnchorManager.subsystem == null)
        {
            Debug.Log($"ARAnchorManager not enabled or available; sample anchor functionality will not be enabled.");
            return;
        }
        _arAnchorManager.anchorsChanged += AnchorsChanged;

        _anchorStore = await _arAnchorManager.subsystem.LoadAnchorStoreAsync();

        if (_anchorStore == null)
        {
            Debug.Log("XRAnchorStore not available, sample anchor persistence functionality will not be enabled.");
            return;
        }

        var persistedAnchor = _anchorStore.PersistedAnchorNames.Where(a => a == AnchorName).SingleOrDefault();
        if (persistedAnchor != null)
        {
            _arAnchorManager.anchorPrefab = anchorPrefab;

            var anchorId = _anchorStore.LoadAnchor(persistedAnchor);

            Debug.Log($"Loaded persistent anchor {anchorId}");
        }
    }

    private void AnchorsChanged(ARAnchorsChangedEventArgs obj)
    {
        Debug.Log($"AnchorsChanged added: {obj.added.Count} removed: {obj.removed.Count} updated: {obj.updated.Count}");

        if (obj.added.Count > 0)
        {
            // maybe need to parent here...
            Debug.Log($"add count = {obj.added.Count}");
            foreach (var a in obj.added)
            {
                Debug.Log($"added {a.trackableId}");
                _currentAnchor = a.gameObject;
                actualContent.SetActive(true);
                actualContent.transform.SetParent(_currentAnchor.transform, false);
            }
        }

        if (obj.removed.Count > 0)
        {
            Debug.Log($"removed count = {obj.removed.Count}");
            foreach (var a in obj.removed)
            {
                Debug.Log($"removed {a.trackableId}");
                _currentAnchor = null;
            }
        }
    }

    GameObject AnchorContent(Vector3 position, Quaternion rotation, GameObject prefab)
    {
        // Create an instance of the prefab
        var instance = Instantiate(prefab, position, rotation);

        var anchor = instance.GetComponent<ARAnchor>();

        // Add an ARAnchor component if it doesn't have one already.
        if (anchor == null)
        {
            anchor = instance.AddComponent<ARAnchor>();
        }

        anchor.destroyOnRemoval = true;

        return instance;
    }

    public void AddAnchor()
    {
        Debug.Log($"Add Anchor called");

        StopVuforiaCamera();

        if (_currentAnchor != null)
        {
            Destroy(_currentAnchor);
        }

        _currentAnchor = AnchorContent(placingGameObject.transform.position, placingGameObject.transform.rotation, anchorPrefab);

        actualContent.SetActive(true);
        actualContent.transform.SetParent(_currentAnchor.transform, false);

        var anchor = _currentAnchor.GetComponent<ARAnchor>();

        Debug.Log($"Add Anchor trackable id = {anchor.trackableId}");

        if (_anchorStore.PersistedAnchorNames.Contains(AnchorName))
        {
            _anchorStore.UnpersistAnchor(AnchorName);
        }

        // persist the anchor...
        _anchorStore.TryPersistAnchor(anchor.trackableId, AnchorName);

        
    }

    public void StartVuforiaCamera()
    {
        if (!VuforiaBehaviour.Instance.isActiveAndEnabled)
        {
            VuforiaBehaviour.Instance.enabled = true;
            cubeVuforia.SetActive(true);
        }
    }

    public void StopVuforiaCamera()
    {
        if (VuforiaBehaviour.Instance.isActiveAndEnabled)
        {
            VuforiaBehaviour.Instance.enabled = false;
            cubeVuforia.SetActive(false);
        }
    }
}
