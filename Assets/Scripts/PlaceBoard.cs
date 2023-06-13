using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceBoard : MonoBehaviour
{
    public GameObject cursor;
    public ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        // Update cursor position
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes))
        {
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