using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class BattleConsole : MonoBehaviourPunCallbacks
{
    public static BattleConsole instance;

    [SerializeField] public TextMeshProUGUI consoleText;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public const byte BattleConsoleEventCode = 0; // Or any other unique number


    public void AddLine(string line)
    {
        consoleText.text += "\n" + line;

        // Raise a Photon event to update all other players' consoles
        object[] content = new object[] { line };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // Updated to send to others only
        PhotonNetwork.RaiseEvent(BattleConsoleEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }


    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        GameManager.OnSinglePlayerConnected += UpdateSinglePlayerConnectedText;
        GameManager.OnBothPlayersConnected += UpdateBothPlayersConnectedText;
        Debug.Log("[BattleConsole] Subscribed to GameManager events.");
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        GameManager.OnSinglePlayerConnected -= UpdateSinglePlayerConnectedText;
        GameManager.OnBothPlayersConnected -= UpdateBothPlayersConnectedText;
    }

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == BattleConsoleEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string line = (string)data[0];
            consoleText.text += "\n" + line;
        }
    }

    void UpdateSinglePlayerConnectedText()
    {
        consoleText.text = "Waiting for the opponent...";
    }

    void UpdateBothPlayersConnectedText()
    {
        consoleText.text = "Both players are ready to fight";
    }
}




