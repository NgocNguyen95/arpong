using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _arPongTable;

    [SerializeField] PlaceBoard _placeBoard;

    [SerializeField] Button _createRoomButton;
    [SerializeField] Toggle _enterRoomButton;
    [SerializeField] Button _resolveCloudAnchorButton;
    [SerializeField] Button _placeBoardButton;

    [SerializeField] TMP_InputField _cloudAnchorIdInputField;


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
        EventManager.Instance.cloudAnchorResolvedEvent.AddListener(InitBoard);
    }


    private void RemoveListeners()
    {
        EventManager.Instance.cloudAnchorResolvedEvent.RemoveListener(InitBoard);
    }


    private void InitBoard(Transform cloudAnchorTransform)
    {
        _arPongTable.SetActive(true);
        _arPongTable.transform.SetPositionAndRotation(cloudAnchorTransform.position, cloudAnchorTransform.rotation);
    }


    public void OnClickCreateRoomButton()
    {
        _placeBoard.enabled = true;
        _placeBoardButton.interactable = true;
    }


    public void OnClickPlaceBoardButton()
    {
        _placeBoard.enabled = false;
        EventManager.Instance.boardPlacedEvent.Invoke();
        _placeBoardButton.interactable = false;
        _createRoomButton.interactable = false;
    }


    public void OnClickResolveCloudAnchorButton()
    {
        if (_cloudAnchorIdInputField.text == "")
            return;

        string cloudAnchorId = _cloudAnchorIdInputField.text;
        ARCloudAnchorManager.Instance.QueueResolveAnchor(cloudAnchorId);

        _cloudAnchorIdInputField.text = "";
        _enterRoomButton.isOn = false;
    }
}
