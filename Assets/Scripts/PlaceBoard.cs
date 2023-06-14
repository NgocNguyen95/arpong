using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceBoard : MonoBehaviour
{
    public GameObject cursor;
    public ARRaycastManager arRaycastManager;
    public ARPlaneManager arPlaneManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ARPlane _plane;


    private void OnEnable()
    {
        _plane = null;
    }


    private void OnDisable()
    {
        _plane = null;
    }


    void Update()
    {
        // Update cursor position
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            _plane = arPlaneManager.GetPlane(hits[0].trackableId);
            if (_plane == null)
            {
                cursor.SetActive(false);
                return;
            }

            if (_plane.alignment != PlaneAlignment.HorizontalUp)
            {
                cursor.SetActive(false);
                return;
            }

            Pose hitPose = hits[0].pose;
            cursor.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            cursor.SetActive(true);
        }
        else
        {
            cursor.SetActive(false);
        }
    }
}