using Google.XR.ARCoreExtensions;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public class ARCloudAnchorManager : MonoBehaviour
{
    private static ARCloudAnchorManager _instance;

    [SerializeField] Camera arCamera = null;

    [SerializeField] ARAnchorManager _arAnchorManager;

    private ARAnchor _pendingHostAnchor;

    private HostCloudAnchorPromise _hostPromise = null;

    private HostCloudAnchorResult _hostResult = null;

    private IEnumerator _hostCoroutine = null;

    private ResolveCloudAnchorPromise _resolvePromise = null;

    private ResolveCloudAnchorResult _resolveResult = null;

    private IEnumerator _resolveCoroutine = null;

    private string _anchorToResolve;

    private bool _anchorHostInProgress = false;

    private bool _anchorResolveInProgress = false;

    private FeatureMapQuality _previousFeatureMapQuality;

    public TransformEvent CloudAnchorResolvedEvent;
    public GameEvent CloudAnchorHostEvent;

    public static ARCloudAnchorManager Instance
    {
        get { return _instance; }
    }


    private void Awake()
    {
        _instance = this;
    }


    private void OnEnable()
    {
        _pendingHostAnchor = null;
        _hostPromise = null;
        _hostCoroutine = null;
        _hostResult = null;

        _anchorToResolve = "";
        _resolvePromise = null;
        _resolveResult = null;
        _resolveCoroutine = null;
    }


    private void OnDisable()
    {
        if (_pendingHostAnchor != null)
        {
            _pendingHostAnchor = null;
        }

        if (_hostCoroutine != null)
        {
            StopCoroutine(_hostCoroutine);
            _hostCoroutine = null;
        }

        if (_hostPromise != null)
        {
            _hostPromise.Cancel();
            _hostPromise = null;
        }

        _hostResult = null;

        _anchorToResolve = "";

        if (_resolveCoroutine != null)
        {
            StopCoroutine(_resolveCoroutine);
            _resolveCoroutine = null;
        }

        if (_resolvePromise != null)
        {
            _resolvePromise.Cancel();
            _resolvePromise = null;
        }

        _resolveResult = null;
    }


    private void Update()
    {
        if (_anchorHostInProgress)
        {
            HostingAnchor();
            return;
        }

        if (_anchorResolveInProgress)
        {
            ResolvingAnchor();
            return;
        }
    }


    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.localPosition,
            arCamera.transform.localRotation);
    }

    public void QueueAnchor(ARAnchor anchor)
    {
        _pendingHostAnchor = anchor;
        _hostResult = null;
        _anchorHostInProgress = true;
    }


    private void HostingAnchor()
    {
        if (_pendingHostAnchor == null)
            return;

        if (_hostPromise != null || _hostResult != null)
            return;

        if (_pendingHostAnchor.pending)
        {
            Debug.Log($"[{nameof(ARCloudAnchorManager)}] {nameof(HostingAnchor)} anchor pending");
            return;
        }

        FeatureMapQuality quality = _arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());

        if (_previousFeatureMapQuality != quality)
        {
            Debug.Log($"[{nameof(ARCloudAnchorManager)}] {nameof(HostingAnchor)} Feature map quality: {quality}");
            _previousFeatureMapQuality = quality;
        }

        if (quality != FeatureMapQuality.Good)
            return;

        Debug.Log($"[{nameof(ARCloudAnchorManager)}] {nameof(HostingAnchor)} good mapping quality, create cloud anchor");

        var promise = _arAnchorManager.HostCloudAnchorAsync(_pendingHostAnchor, 1);

        _hostPromise = promise;
        _hostCoroutine = HostAnchor();
        StartCoroutine(_hostCoroutine);
    }


    private IEnumerator HostAnchor()
    {
        yield return _hostPromise;
        _hostResult = _hostPromise.Result;
        _hostPromise = null;

        if (_hostResult.CloudAnchorState == CloudAnchorState.Success)
        {
            OnCloudAnchorHostedFinished(true, _hostResult.CloudAnchorId);
            CloudAnchorHostEvent.Raise();
        }
        else
        {
            OnCloudAnchorHostedFinished(false, _hostResult.CloudAnchorState.ToString());
        }
    }


    public void QueueResolveAnchor(string cloudAnchorId)
    {
        _anchorToResolve = cloudAnchorId;
        _resolveResult = null;
        _anchorResolveInProgress = true;
    }


    private void ResolvingAnchor()
    {
        if (_anchorToResolve == null)
            return;

        if (_resolvePromise != null || _resolveResult != null) 
            return;

        if (ARSession.state != ARSessionState.SessionTracking)
            return;

        Debug.Log($"[{nameof(ARCloudAnchorManager)}] {nameof(ResolvingAnchor)} Id: {_anchorToResolve}");

        var promise = _arAnchorManager.ResolveCloudAnchorAsync(_anchorToResolve);

        _resolvePromise = promise;
        _resolveCoroutine = ResolveAnchor();
        StartCoroutine(_resolveCoroutine);
    }


    private IEnumerator ResolveAnchor()
    {
        yield return _resolvePromise;
        _resolveResult = _resolvePromise.Result;
        _resolvePromise = null;

        if (_resolveResult.CloudAnchorState == CloudAnchorState.Success)
        {
            CloudAnchorResolvedEvent.Raise(_resolveResult.Anchor.transform);
            OnCloudAnchorResolveFinished(true, _anchorToResolve);
        }
        else
        {
            OnCloudAnchorResolveFinished(false, _anchorToResolve, _resolveResult.CloudAnchorState.ToString());
        }
    }


    private void OnCloudAnchorHostedFinished(bool isSuccess, string response = null)
    {
        _anchorHostInProgress = false;

        if (isSuccess)
        {
            Debug.Log($"[{nameof(ARCloudAnchorManager)}] {nameof(OnCloudAnchorHostedFinished)} Success, ID: {response}");
            FirestoreManager.Instance.AddCloudAnchor(response);
        }
        else
        {
            Debug.LogError($"[{nameof(ARCloudAnchorManager)}] {nameof(OnCloudAnchorHostedFinished)} Failed, error: {response}");
        }
    }


    private void OnCloudAnchorResolveFinished(bool isSuccess, string cloudAnchorId, string response = null)
    {
        _anchorResolveInProgress = false;

        if (isSuccess)
        {
            Debug.Log($"[{nameof(ARCloudAnchorManager)}] {nameof(OnCloudAnchorResolveFinished)} Success, ID: {cloudAnchorId}");
        }
        else
        {
            Debug.LogError($"[{nameof(ARCloudAnchorManager)}] {nameof(OnCloudAnchorResolveFinished)} Failed, Id: {cloudAnchorId}, error: {response}");
        }
    }
}
