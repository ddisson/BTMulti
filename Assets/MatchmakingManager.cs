using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField nicknameInputField;
    public TextMeshProUGUI nicknameDisplayText;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            nicknameDisplayText.text = $"Current Nickname: {PhotonNetwork.NickName}";
        }
    }

    public void SetPlayerNickname()
    {
        string nickname = nicknameInputField.text;
        if (!string.IsNullOrEmpty(nickname))
        {
            PhotonNetwork.NickName = nickname;
            nicknameDisplayText.text = $"Current Nickname: {PhotonNetwork.NickName}";
        }
        else
        {
            Debug.LogWarning("Nickname cannot be empty!");
        }
    }

    public void FindGame()
    {
        SetPlayerNickname();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel("Battle Scene");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player joined: " + newPlayer.NickName);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel("Battle Scene");
        }
    }
}

