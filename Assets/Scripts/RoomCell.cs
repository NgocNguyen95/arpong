using System;
using TMPro;
using UnityEngine;

public class RoomCell : MonoBehaviour
{
    [SerializeField] TMP_Text _roomIdText;
    [SerializeField] TMP_Text _cloudAnchorIdText;
    [SerializeField] TMP_Text _availableStateText;


    private void OnEnable()
    {
        Clear();
    }


    private void OnDisable()
    {
        Clear();
    }


    public void Init(string roomId, string cloudAnchorId, DateTime createdTime, DateTime expireTime)
    {
        _roomIdText.text = "Room ID: " + roomId;
        _cloudAnchorIdText.text = "Cloud anchor ID: " + cloudAnchorId;

        SetAvailableState(createdTime, expireTime);
    }


    private void SetAvailableState(DateTime createdTime, DateTime expireTime)
    {
        if (DateTime.Now > expireTime)
        {
            _availableStateText.text = "Expired";
            return;
        }

        TimeSpan span = DateTime.Now - createdTime;
        string str = span.Hours == 0 ? span.Minutes == 0 ? "Just now" :
                string.Format("{0}m ago", span.Minutes) : string.Format("{0}h ago", span.Hours);
        _availableStateText.text = "Created" + str;
    }


    private void Clear()
    {
        _roomIdText.text = "";
        _cloudAnchorIdText.text = "";
        _availableStateText.text = "";
    }
}
