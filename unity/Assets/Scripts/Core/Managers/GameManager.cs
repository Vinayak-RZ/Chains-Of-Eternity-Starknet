using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private StateMachine<GameManager> gameStateMachine;

    void Awake()
    {
        // Enforce singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

        gameStateMachine = new StateMachine<GameManager>();
    }

    void Start()
    {
        gameStateMachine.Initialize(new InGameState(this, gameStateMachine));
    }

    void Update()
    {
        gameStateMachine.HandleInput();
        gameStateMachine.LogicUpdate();
    }

    void FixedUpdate()
    {
        gameStateMachine.PhysicsUpdate();
    }

    public void ShowGameplayUI()
    {
        Debug.Log("Showing gameplay UI");
        // e.g., HUD.SetActive(true);
    }

    public void ShowInventoryUI()
    {
        Debug.Log("Showing inventory UI");
        // e.g., InventoryUI.SetActive(true);
    }

    public void HideInventoryUI()
    {
        Debug.Log("Hiding inventory UI");
    }

    public void ShowPauseMenu()
    {
        Debug.Log("Showing pause menu");
        // e.g., PauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        Debug.Log("Hiding pause menu");
        // e.g., PauseMenu.SetActive(false);
    }
}
