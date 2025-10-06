using UnityEngine;
using Dojo.Starknet;
using Dojo;
using System.Threading.Tasks;
using System;

public class DojoManager : MonoBehaviour
{
    public static DojoManager Instance { get; private set; }

    [Header("Starknet Settings")]
    public string rpcUrl = "https://starknet-sepolia.g.alchemy.com/starknet/version/rpc/v0_9/WiJmx2NG7dz1PEyksSPP2";
    public string accountAddress = "<YOUR_ACCOUNT_ADDRESS>";
    public string privateKey = "<YOUR_PRIVATE_KEY>";
    public string actionsContract = "<ACTIONS_CONTRACT_ADDRESS>";

    public Account Account { get; private set; }
    public Actions Actions { get; private set; }

    public bool IsInitialized { get; private set; } = false;

    async void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            Debug.Log("🔗 Connecting to Starknet...");
            var provider = new JsonRpcClient(rpcUrl);
            var signer = new SigningKey(privateKey);
            Account = new Account(provider, signer, new FieldElement(accountAddress));

            Actions = gameObject.AddComponent<Actions>();
            Actions.contractAddress = actionsContract;

            IsInitialized = true;
            Debug.Log("✅ DojoManager initialized successfully!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ DojoManager initialization failed: {ex.Message}");
        }
    }
}
