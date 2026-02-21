// An enum is like a Python Enum — a fixed set of named values.
// This defines every possible state the game can be in.
// The GameManager transitions between these states and syncs
// the current state to all players via Photon Custom Properties.
public enum GameState
{
    WaitingToStart,  // In lobby or loading — game hasn't begun yet
    Playing,         // Players are moving, doing tasks, killing
    Discussion,      // A body was reported or bell was rung — everyone talks
    Voting,          // Players vote on who to eject
    VoteResult,      // Showing the vote result (ejected or skipped)
    GameOver         // A team has won — show results
}
