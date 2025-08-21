using Dojo;
using Dojo.Starknet;
using UnityEngine;
 
public class AccountMaker : MonoBehaviour
{
    public string rpcUrl;
    public string masterAddress;
    public string masterPrivateKey;

    void Start()
    {
        var provider = new JsonRpcClient(rpcUrl);
        var signer = new SigningKey(masterPrivateKey);
        var account = new Account(provider, signer, new FieldElement(masterAddress));
    }
}