using Dojo;
using Dojo.Starknet;
using UnityEngine;
using System.Threading.Tasks;


public class BurnerAccount : MonoBehaviour
{
    public string rpcUrl;
    public string masterAddress;
    public string masterPrivateKey;

    async void Start()
    {
        Account burnerAccount = await CreateBurnerAccount(rpcUrl, masterAddress, masterPrivateKey);
    }

    private async Task<Account> CreateBurnerAccount(string rpcUrl, string masterAddress, string masterPrivateKey)
    {
        var provider = new JsonRpcClient(rpcUrl);
        var signer = new SigningKey(masterPrivateKey);
        var account = new Account(provider, signer, new FieldElement(masterAddress));

        BurnerManager burnerManager = new BurnerManager(provider, account);
        return await burnerManager.DeployBurner();
    }
}