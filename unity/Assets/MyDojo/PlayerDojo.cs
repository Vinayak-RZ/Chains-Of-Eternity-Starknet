using UnityEngine;
using Dojo;
using System.Threading.Tasks;

public class PlayerDojo : MonoBehaviour
{
    private float timer = 0f;

    public PlayerStats playerstats; // Reference to the PlayerStats script
    public Player player;           // Reference to the Player script
    public PlayerFSMState currentState;


    private async Task CallUpdatePlayerState()
    {
        try
        {
            // This runs asynchronously without blocking gameplay
            var result = await DojoActions.UpdatePlayerState(
                state: new PlayerFSMState.Idle(),
                pos_x: (ushort)transform.position.x,
                pos_y: (ushort)transform.position.y,
                facing_dir: (byte)(player.IsFacingRight ? 1 : 0),
                velocity: (short)player.CurrentVelocity.magnitude
            );

            Debug.Log($"✅ Updated Player State. Tx: {result.Inner}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error updating player state: {ex.Message}");
        }
    }

    private void Awake()
    {
        playerstats = GetComponent<PlayerStats>();
        player = GetComponent<Player>();
    }

// private PlayerFSMState GetCurrentState(string state)
// {
//     player.currentState = state switch
//     {
//         "PlayerIdleState" => new PlayerFSMState.Idle(),
//         "PlayerMoveState" => new PlayerFSMState.Running(),
//         "PlayerDashState" => new PlayerFSMState.Dashing(),
//         "PlayerAttackState" => new PlayerFSMState.Attacking(),
//         "PlayerStunState" => new PlayerFSMState.Dead(),
//         _ => player.currentState
//     };
//     return player.currentState;
// }


    private async void Start()
    {
        // Wait until DojoManager is initialized
        while (!DojoManager.Instance.IsInitialized)
            await Task.Yield();

        Debug.Log("✅ DojoManager initialized, PlayerDojo ready.");
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 0.5f)
        {
            timer = 0f;
            _ = CallUpdatePlayerState(); // Fire and forget
        }
    }
}
