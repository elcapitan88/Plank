using UnityEngine;
using TMPro;

// Displays the current game state and timer on screen.
// This is a simple debug HUD — we'll make it prettier later.
public class GameHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI timerText;

    void Start()
    {
        // Subscribe to state changes.
        // When GameManager changes state, UpdateStateDisplay gets called.
        // This is the Observer pattern — like adding an event listener in JS.
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += UpdateStateDisplay;
    }

    void OnDestroy()
    {
        // Always unsubscribe when this object is destroyed to prevent
        // "dangling reference" errors. Like removing an event listener.
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= UpdateStateDisplay;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (timerText == null) return;

        float timer = GameManager.Instance.StateTimer;
        if (timer > 0f)
        {
            timerText.text = Mathf.CeilToInt(timer).ToString();
            timerText.gameObject.SetActive(true);
        }
        else
        {
            timerText.gameObject.SetActive(false);
        }
    }

    private void UpdateStateDisplay(GameState newState)
    {
        if (stateText == null) return;

        switch (newState)
        {
            case GameState.Playing:
                stateText.text = "";  // Hide during normal gameplay
                break;
            case GameState.Discussion:
                stateText.text = "DISCUSSION";
                break;
            case GameState.Voting:
                stateText.text = "VOTE NOW";
                break;
            case GameState.VoteResult:
                stateText.text = "RESULTS";
                break;
            case GameState.GameOver:
                stateText.text = "GAME OVER";
                break;
            default:
                stateText.text = "";
                break;
        }
    }
}
