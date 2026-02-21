using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

// MonoBehaviourPunCallbacks is a Photon class that gives us callback methods
// for network events (connected, joined room, player entered, etc.).
// Think of it like inheriting from a base class that has pre-built event hooks —
// similar to how FastAPI has @app.on_event("startup") handlers.
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Connection UI")]
    // [SerializeField] lets us drag-and-drop UI elements from the Unity Editor
    // into these fields. We'll wire these up after building the UI.
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button connectButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Lobby UI")]
    [SerializeField] private GameObject lobbyPanel;        // The panel shown after connecting
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Transform roomListContent;    // Parent object for room list items
    [SerializeField] private GameObject roomListItemPrefab; // Prefab for each room entry

    [Header("Room UI")]
    [SerializeField] private GameObject roomPanel;         // The panel shown when inside a room
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private Transform playerListContent;  // Parent object for player list items
    [SerializeField] private GameObject playerListItemPrefab; // Prefab for each player entry
    [SerializeField] private Button startGameButton;       // Only visible to the host
    [SerializeField] private Button leaveRoomButton;

    [Header("Connection Panel")]
    [SerializeField] private GameObject connectionPanel;   // The initial connect panel

    // Keeps track of available rooms so we can update the room list UI.
    // Dictionary = Python dict. Key is room name, value is room info.
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    // Max players per room — we'll use 10 to match our game design (4-10 players).
    private const int MAX_PLAYERS = 10;

    void Start()
    {
        // Start with only the connection panel visible.
        connectionPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);

        // Wire up button click events via code.
        // In Python terms: button.on_click = self.connect_to_photon
        connectButton.onClick.AddListener(ConnectToPhoton);
        createRoomButton.onClick.AddListener(CreateRoom);
        startGameButton.onClick.AddListener(StartGame);
        leaveRoomButton.onClick.AddListener(LeaveRoom);

        statusText.text = "Enter your name and connect.";
    }

    // --- CONNECTION ---

    // Called when the player clicks "Connect".
    private void ConnectToPhoton()
    {
        string playerName = playerNameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            statusText.text = "Please enter a name!";
            return;
        }

        // Save the player's name so Photon uses it across the network.
        // Other players will see this name.
        PhotonNetwork.NickName = playerName;

        statusText.text = "Connecting...";
        connectButton.interactable = false;

        // Connect to Photon using the settings from PhotonServerSettings asset
        // (where your App ID is stored). This is an async operation —
        // Photon will call OnConnectedToMaster() when it's done.
        PhotonNetwork.ConnectUsingSettings();
    }

    // Photon callback — fires when we successfully connect to the Photon master server.
    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected! Joining lobby...";

        // After connecting to master, we join the default lobby.
        // The lobby is where we can see available rooms.
        PhotonNetwork.JoinLobby();
    }

    // Photon callback — fires when we successfully join the lobby.
    public override void OnJoinedLobby()
    {
        statusText.text = "In Lobby. Create or join a room.";
        connectionPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    // Photon callback — fires if connection fails.
    public override void OnDisconnected(DisconnectCause cause)
    {
        statusText.text = $"Disconnected: {cause}";
        connectButton.interactable = true;
        connectionPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
    }

    // --- ROOM CREATION / JOINING ---

    // Called when the player clicks "Create Room".
    private void CreateRoom()
    {
        string roomName = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            statusText.text = "Please enter a room name!";
            return;
        }

        // RoomOptions lets us configure the room.
        // MaxPlayers caps how many people can join.
        // IsVisible = true means the room shows up in the room list.
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = MAX_PLAYERS,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomName, options);
        statusText.text = "Creating room...";
    }

    // Called when a player clicks a room in the room list to join it.
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        statusText.text = "Joining room...";
    }

    // Photon callback — fires when we successfully join (or create) a room.
    public override void OnJoinedRoom()
    {
        statusText.text = $"Joined room: {PhotonNetwork.CurrentRoom.Name}";
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        // Update the player list and start button visibility.
        UpdatePlayerList();
        UpdateStartButton();
    }

    // Photon callback — fires if room creation fails (e.g., name already taken).
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = $"Room creation failed: {message}";
    }

    // Photon callback — fires if joining a room fails.
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = $"Join failed: {message}";
    }

    // --- ROOM LIST ---

    // Photon callback — fires whenever the room list changes while we're in the lobby.
    // This gives us a DELTA (changes only), not the full list, so we merge into our cache.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Update our cached room list with the changes.
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                cachedRoomList.Remove(room.Name);
            }
            else
            {
                cachedRoomList[room.Name] = room;
            }
        }

        UpdateRoomListUI();
    }

    // Rebuilds the room list UI from the cached data.
    private void UpdateRoomListUI()
    {
        // Clear old list items (destroy all children of the content parent).
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        // Create a new UI item for each room.
        foreach (var kvp in cachedRoomList)
        {
            RoomInfo room = kvp.Value;
            if (!room.IsOpen || !room.IsVisible || room.PlayerCount >= room.MaxPlayers)
                continue;

            // Instantiate = create a copy of the prefab. Like calling a factory function.
            GameObject item = Instantiate(roomListItemPrefab, roomListContent);
            TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"{room.Name}  ({room.PlayerCount}/{room.MaxPlayers})";

            // Wire up the button to join this specific room.
            // The lambda captures the room name — similar to Python closures.
            Button button = item.GetComponent<Button>();
            string roomName = room.Name;
            button.onClick.AddListener(() => JoinRoom(roomName));
        }
    }

    // --- PLAYER LIST (inside a room) ---

    // Photon callback — fires when another player enters the room.
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayerList();
        UpdateStartButton();
    }

    // Photon callback — fires when another player leaves the room.
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayerList();
        UpdateStartButton();
    }

    // Rebuilds the player list UI showing everyone in the current room.
    private void UpdatePlayerList()
    {
        // Clear old list.
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // PhotonNetwork.PlayerList is an array of all players in the room.
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            GameObject item = Instantiate(playerListItemPrefab, playerListContent);
            TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();

            // Mark the host with a crown indicator.
            string hostTag = player.IsMasterClient ? " [HOST]" : "";
            text.text = $"{player.NickName}{hostTag}";
        }
    }

    // Only the host (MasterClient) can see the Start Game button.
    private void UpdateStartButton()
    {
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    // --- LEAVE / START ---

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // Photon callback — fires when we leave a room.
    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        cachedRoomList.Clear();
        statusText.text = "In Lobby. Create or join a room.";
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Lock the room so no one else can join mid-game.
            PhotonNetwork.CurrentRoom.IsOpen = false;

            // LoadLevel syncs the scene load across all clients.
            // All players in the room will load "Game" scene together.
            // We'll create the Game scene in Step 1.2.
            PhotonNetwork.LoadLevel("Game");
        }
    }
}
