// WalletConnectPage.jsx
import React, { useState } from "react";
import { ControllerConnector } from "@cartridge/connector";

export default function WalletConnectPage() {
  const [wallet, setWallet] = useState(null);
  const [account, setAccount] = useState(null);

  // Initialize ControllerConnector and request connection
  const connectWallet = async () => {
    try {
      const connector = new ControllerConnector();
      const walletStandard = await connector.controller.connect();

     // await walletStandard.enable(); // Request wallet connection
      const accounts = await walletStandard.getAccounts();
      setWallet(walletStandard);
      setAccount(accounts[0]?.address || null);
    } catch (err) {
      console.error("Wallet connection failed:", err);
    }
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-900 text-white p-4">
      <h1 className="text-2xl font-bold mb-6">Connect Your Wallet</h1>
      {!account ? (
        <button
          onClick={connectWallet}
          className="px-6 py-3 bg-teal-500 hover:bg-teal-400 rounded-lg font-semibold transition"
        >
          Connect Wallet
        </button>
      ) : (
        <div className="text-center">
          <p className="mb-2">Wallet connected!</p>
          <p className="break-all font-mono">{account}</p>
        </div>
      )}
    </div>
  );
}
