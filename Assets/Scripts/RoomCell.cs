using System;
using TMPro;
using UnityEngine;

public class RoomCell : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] TMP_Text _roomIdText;
    [SerializeField] TMP_Text _cloudAnchorIdText;
    [SerializeField] TMP_Text _availableStateText;

    [Header("Buttons")]
    [SerializeField] GameObject _joinRoomButton;

    private bool _isExpired = false;
    private string _cloudAnchorId;


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
        _cloudAnchorId = cloudAnchorId;

        SetAvailableState(createdTime, expireTime);
    }


    public void OnClickJoinRoomButton()
    {
        if (_isExpired) 
            return;

        if (_cloudAnchorId == "" || _cloudAnchorId == null)
            return;

        ARCloudAnchorManager.Instance.QueueResolveAnchor(_cloudAnchorId);
        EventManager.Instance.joinRoomEvent.Invoke();
    }


    private void SetAvailableState(DateTime createdTime, DateTime expireTime)
    {
        if (DateTime.UtcNow > expireTime)
        {
            _availableStateText.text = "Expired";
            _joinRoomButton.SetActive(false);
            _isExpired = true;
            return;
        }

        _joinRoomButton.SetActive(true);

        TimeSpan span = DateTime.UtcNow - createdTime;
        string str = span.Hours == 0 ? span.Minutes == 0 ? "Just now" :
                string.Format("{0}m ago", span.Minutes) : string.Format("{0}h ago", span.Hours);
        _availableStateText.text = "Created " + str;
    }


    private void Clear()
    {
        _roomIdText.text = "";
        _cloudAnchorIdText.text = "";
        _availableStateText.text = "";
    }
}
