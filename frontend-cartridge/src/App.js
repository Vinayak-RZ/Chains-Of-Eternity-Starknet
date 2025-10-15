import React from "react";
//import WalletConnectPage from "./connect/connect";
import StarknetProvider from "./connect/starknetProvider.tsx";
import { LoginScreen } from "./pages/loginScreen.tsx";

function App() {
  return (
    <div style={{ textAlign: "center", marginTop: "50px" }}>
      <h1>My Unity + Cartridge Game</h1>
      <StarknetProvider >
        <LoginScreen>

        </LoginScreen>
      </StarknetProvider>
    </div>
  );
}

export default App;
