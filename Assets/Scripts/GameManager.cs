using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _arPongTable;

    [SerializeField] PlaceBoard _placeBoard;

    [SerializeField] Button _createRoomButton;
    [SerializeField] Toggle _enterRoomButton;
    [SerializeField] Button _placeBoardButton;

    [SerializeField] GameObject _roomScrollView;
    [SerializeField] GameObject _roomCellPrefab;


    private void Awake()
    {
        AddListeners();
    }


    private void OnDisable()
    {
        RemoveListeners();
    }


    private void AddListeners()
    {
        EventManager.Instance.joinRoomEvent.AddListener(HandleJoinRoomEvent);
    }


    private void RemoveListeners()
    {
        EventManager.Instance.joinRoomEvent.RemoveListener(HandleJoinRoomEvent);
    }


    public void InitBoard(Transform cloudAnchorTransform)
    {
        _arPongTable.SetActive(true);
        _arPongTable.transform.SetPositionAndRotation(cloudAnchorTransform.position, cloudAnchorTransform.rotation);
    }


    public void OnClickCreateRoomButton()
    {
        _placeBoard.enabled = true;
        _placeBoardButton.interactable = true;
    }


    public void OnClickEnterRoom(bool isOn)
    {
        if (!isOn)
        {
            int roomCellCount = _roomScrollView.GetComponent<ScrollRect>().content.childCount;

            for (int i = 0; i < roomCellCount; i++)
            {
                Destroy(_roomScrollView.GetComponent<ScrollRect>().content.GetChild(i).gameObject);
            }
            _roomScrollView.SetActive(false);
            return;
        }

        _roomScrollView.SetActive(true);
        FirestoreManager.Instance.GetCloudAnchors((cloudAnchors) =>
        {
            int i = 0;
            foreach (var cloudAnchor in cloudAnchors)
            {
                i++;
                var roomCell = Instantiate(_roomCellPrefab, _roomScrollView.GetComponent<ScrollRect>().content);
                roomCell.GetComponent<RoomCell>().Init("Room " + i, cloudAnchor.CloudAnchorId, cloudAnchor.CreateTime, cloudAnchor.ExpireTime);
            }
        });
    }


    public void OnClickPlaceBoardButton()
    {
        _placeBoard.enabled = false;
        EventManager.Instance.boardPlacedEvent.Invoke();
        _placeBoardButton.interactable = false;
        _createRoomButton.interactable = false;
    }


    public void HandleJoinRoomEvent()
    {
        _enterRoomButton.isOn = false;
    }
}
