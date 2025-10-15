import React, { useState, useEffect } from "react";
import Controller from "@cartridge/controller";

export default function WalletConnect() {
  const [controller, setController] = useState(null);
  const [session, setSession] = useState(null);

  useEffect(() => {
    const init = async () => {
      const c = await Controller({
        redirectUri: window.location.origin,
      });
      setController(c);

      // Check if returning from login
      const s = await c.connect();
      if (s) {
        setSession(s);
        console.log("Connected session:", s);

        // Send session token to backend using fetch
        await fetch("http://localhost:3000/api/store-session", {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({ sessionToken: s.session_key })
        });
      }
    };

    init();
  }, []);

  const handleExecuteTx = async () => {
    if (!session) return;

    const txPayload = [
      {
        contractAddress: "0xYourDojoWorldAddress",
        entrypoint: "attack_enemy",
        calldata: ["0x1", "0x2"]
      }
    ];

    const response = await fetch("http://localhost:3001/api/execute", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        address: session.address,
        txPayload
      })
    });

    const data = await response.json();
    console.log("Transaction result:", data);
  };

  return (
    <div>
      {session ? (
        <>
          <p>Connected as: {session.address}</p>
          <button onClick={handleExecuteTx}>Execute Dojo TX</button>
        </>
      ) : (
        <button onClick={() => controller?.connect()}>
          Connect Cartridge Wallet
        </button>
      )}
    </div>
  );
}
