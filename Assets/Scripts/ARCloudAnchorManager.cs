using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class CloudAnchorCreatedEvent : UnityEvent<Transform> { }


public class ARCloudAnchorManager : MonoBehaviour
{
    [SerializeField] Camera arCamera = null;

    [SerializeField] float resolveAnchorPassedTimeout = 10.0f;

    private ARAnchorManager _arAnchorManager;

    private ARAnchor _pendingHostAnchor;

    private ARCloudAnchor _cloudAnchor;

    private string _anchorToResolve;

    private bool _anchorHostInProgress = false;

    private bool _anchorResolveInProgress = false;

    private float _safeToResolvePassed = 0;

    private CloudAnchorCreatedEvent _cloudAnchorCreatedEvent = null;


    private void Awake()
    {
        _cloudAnchorCreatedEvent = new CloudAnchorCreatedEvent();
    }


    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.position,
            arCamera.transform.rotation);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
