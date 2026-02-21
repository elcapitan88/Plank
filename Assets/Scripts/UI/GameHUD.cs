using UnityEngine;
using TMPro;
using Photon.Pun;

// Displays the current game state, timer, and role on screen.
public class GameHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI roleText;

    // Reference to the local player's PlayerRole (found at runtime).
    private PlayerRole localPlayerRole;

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += UpdateStateDisplay;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= UpdateStateDisplay;
    }

    void Update()
    {
        UpdateTimerDisplay();
        UpdateRoleDisplay();
    }

    private void UpdateTimerDisplay()
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

    private void UpdateRoleDisplay()
    {
        if (roleText == null) return;

        // Find the local player's PlayerRole if we haven't yet.
        // We search each frame until we find it because the player spawns
        // asynchronously via Photon.
        if (localPlayerRole == null)
        {
            PlayerRole[] allRoles = FindObjectsByType<PlayerRole>(FindObjectsSortMode.None);
            foreach (PlayerRole pr in allRoles)
            {
                if (pr.photonView.IsMine)
                {
                    localPlayerRole = pr;
                    break;
                }
            }
        }

        if (localPlayerRole == null) return;

        // Show the role with a color indicator.
        if (localPlayerRole.MyRole == Role.Mutineer)
        {
            roleText.text = "MUTINEER";
            roleText.color = new Color(1f, 0.3f, 0.3f); // Red
        }
        else if (localPlayerRole.MyRole == Role.Crew)
        {
            roleText.text = "CREW";
            roleText.color = new Color(0.3f, 0.8f, 1f); // Light blue
        }
        else
        {
            roleText.text = "";
        }
    }

    private void UpdateStateDisplay(GameState newState)
    {
        if (stateText == null) return;

        switch (newState)
        {
            case GameState.Playing:
                stateText.text = "";
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
