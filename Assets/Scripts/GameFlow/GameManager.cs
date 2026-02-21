using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

// GameManager is a singleton — only one exists in the scene, and any script
// can access it via GameManager.Instance. In Python terms, it's like a module-level
// global object that everyone imports and uses.
//
// It runs on ALL clients but only the MasterClient (host) drives state changes.
// State is synced via Photon Room Custom Properties — like a shared dictionary
// that all players can read but only the host writes to.
public class GameManager : MonoBehaviourPunCallbacks
{
    // Singleton pattern — lets any script call GameManager.Instance.DoSomething()
    public static GameManager Instance { get; private set; }

    // Current game state — other scripts read this to know what phase we're in.
    public GameState CurrentState { get; private set; } = GameState.WaitingToStart;

    // Event that fires whenever the state changes.
    // Other scripts subscribe to this to react to state transitions.
    // In Python terms: like a signal/callback list.
    public event Action<GameState> OnStateChanged;

    // Timer for discussion and voting phases.
    public float StateTimer { get; private set; }

    // Configurable durations (in seconds).
    [Header("Timing")]
    [SerializeField] private float discussionTime = 30f;
    [SerializeField] private float votingTime = 20f;
    [SerializeField] private float voteResultTime = 5f;

    // Key used in Photon Room Properties to store the game state.
    // Custom Properties are Photon's way of syncing arbitrary data to all clients.
    private const string STATE_KEY = "GameState";
    private const string TIMER_KEY = "StateTimerEnd";

    void Awake()
    {
        // Singleton setup — if one already exists, destroy this duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Only the host starts the game automatically when the scene loads.
        if (PhotonNetwork.IsMasterClient)
        {
            SetState(GameState.Playing);
        }
    }

    void Update()
    {
        UpdateTimer();
    }

    // --- STATE MANAGEMENT ---

    // Changes the game state. Only called on the MasterClient (host).
    // Syncs to all clients via Photon Room Custom Properties.
    public void SetState(GameState newState)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Calculate when the timer should expire (as a Photon server timestamp).
        // PhotonNetwork.ServerTimestamp is synced across all clients, so everyone
        // agrees on when the timer ends — like using UTC timestamps in a web app.
        float duration = GetStateDuration(newState);
        int timerEnd = 0;
        if (duration > 0)
        {
            timerEnd = PhotonNetwork.ServerTimestamp + (int)(duration * 1000f);
        }

        // Set the room properties. This triggers OnRoomPropertiesUpdate on all clients.
        Hashtable props = new Hashtable
        {
            { STATE_KEY, (int)newState },
            { TIMER_KEY, timerEnd }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    // Photon callback — fires on ALL clients when room properties change.
    // This is how non-host clients learn about state changes.
    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        if (changedProps.ContainsKey(STATE_KEY))
        {
            GameState newState = (GameState)(int)changedProps[STATE_KEY];
            CurrentState = newState;

            // Notify all listeners that the state changed.
            OnStateChanged?.Invoke(newState);

            Debug.Log($"[GameManager] State changed to: {newState}");
        }
    }

    // Returns how long each state lasts (0 = no timer).
    private float GetStateDuration(GameState state)
    {
        switch (state)
        {
            case GameState.Discussion: return discussionTime;
            case GameState.Voting: return votingTime;
            case GameState.VoteResult: return voteResultTime;
            default: return 0f;
        }
    }

    // --- TIMER ---

    private void UpdateTimer()
    {
        // Only timed states need a countdown.
        if (CurrentState != GameState.Discussion &&
            CurrentState != GameState.Voting &&
            CurrentState != GameState.VoteResult)
        {
            StateTimer = 0f;
            return;
        }

        // Read the timer end from room properties.
        if (PhotonNetwork.CurrentRoom == null) return;
        object timerEndObj;
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TIMER_KEY, out timerEndObj))
            return;

        int timerEnd = (int)timerEndObj;
        int remaining = timerEnd - PhotonNetwork.ServerTimestamp;
        StateTimer = Mathf.Max(0f, remaining / 1000f);

        // When the timer runs out, the host advances to the next state.
        if (StateTimer <= 0f && PhotonNetwork.IsMasterClient)
        {
            AdvanceState();
        }
    }

    // Moves to the next logical state when a timer expires.
    private void AdvanceState()
    {
        switch (CurrentState)
        {
            case GameState.Discussion:
                SetState(GameState.Voting);
                break;
            case GameState.Voting:
                // VotingManager will handle tallying votes and calling SetState.
                // For now, go straight to VoteResult.
                SetState(GameState.VoteResult);
                break;
            case GameState.VoteResult:
                // Check win conditions, then back to Playing.
                SetState(GameState.Playing);
                break;
        }
    }

    // --- PUBLIC METHODS (called by other systems) ---

    // Called when a body is reported or the bell is rung.
    public void StartDiscussion()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (CurrentState != GameState.Playing) return;

        SetState(GameState.Discussion);
    }

    // Called when a win condition is met.
    public void EndGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        SetState(GameState.GameOver);
    }

    // --- HOST MIGRATION ---

    // If the host disconnects, Photon assigns a new MasterClient.
    // This callback fires on the new host so they can take over.
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        Debug.Log($"[GameManager] New host: {newMasterClient.NickName}");
    }
}
