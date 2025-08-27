// using Dojo;
// using Dojo.Starknet;
// using UnityEngine;
// using System.Threading.Tasks;

// public class PlayerStateSync : MonoBehaviour
// {
//     public string rpcUrl = "https://starknet-sepolia.public.blastapi.io";
//     public string worldAddress = "<WORLD_ADDRESS>";
//     public string accountAddress = "<ACCOUNT_ADDRESS>";
//     public string privateKey = "<PRIVATE_KEY>";

//     /private World world;
//     private Account account;

//     async void Start()
//     {
//         var provider = new JsonRpcClient(rpcUrl);
//         var signer = new SigningKey(privateKey);
//         account = new Account(provider, signer, new FieldElement(accountAddress));

//         world = await World.FromAddress(provider, new FieldElement(worldAddress));

//         Debug.Log("Connected to Dojo World!");
        
//         await FetchPlayerState();
//     }

//     async Task FetchPlayerState()
//     {
//         // Fetch all player states (or your own player)
//         var playerStates = await world.Entities.Query<dojo_starter_PlayerState>();

//         foreach (var player in playerStates)
//         {
//             Debug.Log($"Player {player.player} is {player.state} at ({player.position.x}, {player.position.y})");
//         }
//     }
// }
