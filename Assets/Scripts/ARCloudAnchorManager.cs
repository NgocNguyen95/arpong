using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class CloudAnchorCreatedEvent : UnityEvent<Transform> { }


public class ARCloudAnchorManager : MonoBehaviour
{
    private static ARCloudAnchorManager _instance;

    [SerializeField] Camera arCamera = null;

    [SerializeField] float resolveAnchorPassedTimeout = 10.0f;

    private ARAnchorManager _arAnchorManager;

    private ARAnchor _pendingHostAnchor;

    private ARCloudAnchor _cloudAnchor;

    private HostCloudAnchorPromise _hostPromise = null;

    private HostCloudAnchorResult _hostResult = null;

    private IEnumerator _hostCoroutine = null;

    private string _anchorToResolve;

    private bool _anchorHostInProgress = false;

    private bool _anchorResolveInProgress = false;

    private float _safeToResolvePassed = 0;

    private CloudAnchorCreatedEvent _cloudAnchorCreatedEvent = null;

    private FeatureMapQuality _previousFeatureMapQuality;


    public static ARCloudAnchorManager Instance
    {
        get { return _instance; }
    }


    private void Awake()
    {
        _instance = this;
        _cloudAnchorCreatedEvent = new CloudAnchorCreatedEvent();
    }


    private void OnEnable()
    {
        _pendingHostAnchor = null;
        _hostPromise = null;
        _hostCoroutine = null;
        _hostResult = null;
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
    }


    private void Update()
    {
        HostingAnchor();
    }


    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.localPosition,
            arCamera.transform.localRotation);
    }

    public void QueueAnchor(ARAnchor anchor)
    {
        _pendingHostAnchor = anchor;
    }


    private void HostingAnchor()
    {
        if (_pendingHostAnchor == null)
        {
            return;
        }

        if (_hostPromise != null || _hostResult != null)
        {
            return;
        }

        if (_pendingHostAnchor.pending)
        {
            StartCoroutine(CheckAnchorAddedCoroutine());
        }

        FeatureMapQuality quality = _arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());

        if (_previousFeatureMapQuality != quality)
        {
            Debug.Log($"[{nameof(ARCloudAnchorManager)}] {nameof(HostingAnchor)} Feature map quality: {quality}");
            _previousFeatureMapQuality = quality;
        }

        if (quality != FeatureMapQuality.Good)
        {
            return;
        }

        Debug.Log($"[{nameof(ARCloudAnchorManager)}] {nameof(HostingAnchor)} good mapping quality, create cloud anchor");

        var promise = _arAnchorManager.HostCloudAnchorAsync(_pendingHostAnchor, 1);

        _hostPromise = promise;
        _hostCoroutine = HostAnchor();
        StartCoroutine(_hostCoroutine);
    }


    private IEnumerator CheckAnchorAddedCoroutine()
    {
        Debug.Log($"[{nameof(ARCloudAnchorManager)}] {nameof(CheckAnchorAddedCoroutine)} pending");
        yield return new WaitUntil(() => !_pendingHostAnchor.pending);
    }


    private IEnumerator HostAnchor()
    {
        yield return _hostPromise;
        _hostResult = _hostPromise.Result;
        _hostPromise = null;

        if (_hostResult.CloudAnchorState == CloudAnchorState.Success)
        {
            OnCloudAnchorHostedFinished(true, _hostResult.CloudAnchorId);
        }
        else
        {
            OnCloudAnchorHostedFinished(false, _hostResult.CloudAnchorState.ToString());
        }
    }


    private void OnCloudAnchorHostedFinished(bool isSuccess, string response = null)
    {
        if (isSuccess)
        {
            Debug.Log($"[{nameof(ARCloudAnchorManager)}] {nameof(OnCloudAnchorHostedFinished)} Success, ID: {response}");
        }
        else
        {
            Debug.Log($"[{nameof(ARCloudAnchorManager)}] {nameof(OnCloudAnchorHostedFinished)} Failed, error: {response}");
        }
    }
}
