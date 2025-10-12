import React from "react";
//import WalletConnectPage from "./connect/connect";
import StarknetProvider from "./connect/starknetProvider.tsx";
import { LoginScreen } from "./pages/loginScreen.tsx";

function App() {
  return (
    
      <StarknetProvider >
        <LoginScreen>

        </LoginScreen>
      </StarknetProvider>

  );
}

export default App;
