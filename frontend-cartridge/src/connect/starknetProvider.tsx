import type { PropsWithChildren } from "react";
import { sepolia, mainnet } from "@starknet-react/chains";
import {
    jsonRpcProvider,
    StarknetConfig,
    starkscan,
} from "@starknet-react/core";
import cartridgeConnector from "./cartridgeConnector.tsx";

export default function StarknetProvider({ children }: PropsWithChildren) {
    //const { VITE_PUBLIC_DEPLOY_TYPE } = import.meta.env;

    // Get RPC URL based on environment
    const getRpcUrl = () => {
        switch ("sepolia") {
            case "sepolia":
                return "https://api.cartridge.gg/x/starknet/sepolia";
            default:
                return "https://api.cartridge.gg/x/starknet/sepolia"; 
        }
    };

    // Create provider with the correct RPC URL
    const provider = jsonRpcProvider({
        rpc: () => ({ nodeUrl: getRpcUrl() }),
    });

    const VITE_PUBLIC_DEPLOY_TYPE = "sepolia"; // Replace with actual environment variable if needed

    // Determine which chain to use
    const chains = VITE_PUBLIC_DEPLOY_TYPE
        ? [mainnet] 
        : [sepolia];

    return (
        <StarknetConfig
            autoConnect
            chains={chains}
            connectors={[cartridgeConnector]}
            explorer={starkscan}
            provider={provider}
        >
            {children}
        </StarknetConfig>
    );
}