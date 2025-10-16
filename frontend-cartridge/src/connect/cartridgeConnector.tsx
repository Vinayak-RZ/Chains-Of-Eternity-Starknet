import { Connector } from "@starknet-react/core";
import { ControllerConnector } from "@cartridge/connector";
import { ControllerOptions } from "@cartridge/controller";
import { SessionPolicies } from "@cartridge/presets";

const CONTRACT_ADDRESS_GAME = "0x03718c8a4b39b2d6a204454080cc8491c8ee0cbbc7faffde89d128d44411dc23";

const policies: SessionPolicies = {
  contracts: {
    [CONTRACT_ADDRESS_GAME]: {
      methods: [
        { name: "spawn_player", entrypoint: "spawn_player" },
        { name: "spawn_enemy", entrypoint: "spawn_enemy" },
        { name: "enemy_damaged", entrypoint: "enemy_damaged" },
        { name: "enemy_killed", entrypoint: "enemy_killed" },
        { name: "update_player_state", entrypoint: "update_player_state" },
        { name: "create_spell", entrypoint: "create_spell" },
        { name: "fire_spell", entrypoint: "fire_spell" },
        { name: "player_attacked", entrypoint: "player_attacked" },
        { name: "equip_spell", entrypoint: "equip_spell" },
        { name: "take_damage", entrypoint: "take_damage" },
      ],
    },
  },
};

const options: ControllerOptions = {
  chains: [
    {
      rpcUrl: "https://api.cartridge.gg/x/starknet/sepolia",
    },
  ],
  policies,
  defaultChainId: "0x534e5f5345504f4c4941",
  namespace: "arcane_starter",
  slot: "arcane_slot",
};

const cartridgeConnector = new ControllerConnector(options) as never as Connector;

export default cartridgeConnector;
