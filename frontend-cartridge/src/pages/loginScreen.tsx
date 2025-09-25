import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import  { Toaster } from 'react-hot-toast';
import { useStarknetConnect } from '../connect/useStarknetConnect.ts';
//import desktopBg from 'react-scripts';
//import mobileBg from '../../../assets/login-mobile.webp';
interface LoginScreenProps {
  onLoginSuccess: () => void;
}

// Helper function to truncate hash

export const LoginScreen: React.FC<LoginScreenProps> = ({ onLoginSuccess }) => {
  const { status, handleConnect, hasTriedConnect } = useStarknetConnect();
  //const { 
  //  txHash, 
  //  txStatus, 
  //  initializePlayer, 
  //  playerExists,
  //  completed
  //} = useSpawnPlayer();
  //const storePlayer = useAppStore(state => state.player);

 // const [isMobile, setIsMobile] = useState<boolean>(window.innerWidth <= 768);
 // const position = isMobile ? 'bottom-center' : 'top-right';
    const position = 'bottom-center';
  // Trigger player initialization on wallet connect
  useEffect(() => {
    if (status === 'connected' && hasTriedConnect) {
      console.log("Wallet connected, initializing player...");
      
    }
  }, [status, hasTriedConnect]);

  // Redirect on player exists or initialization completion

    // If player exists or initialization completed successfully, redirect

  // Transaction toast and success toast
  
  // Responsive toast positioning
  //useEffect(() => {
  //  //const handleResize = () => setIsMobile(window.innerWidth <= 768);
  //  window.addEventListener('resize', handleResize);
  //  return () => window.removeEventListener('resize', handleResize);
  //}, []);

  return (
    <div
      className="min-h-screen w-full flex items-center justify-center bg-cover bg-center">
      <motion.div
        initial={{ scale: 0.8, opacity: 0 }}
        animate={{ scale: 1, opacity: 1 }}
        transition={{ duration: 0.3 }}
        className="relative z-10 flex flex-col items-center"
      >
        <h1 className="text-4xl md:text-5xl font-luckiest text-white mb-8 drop-shadow-lg text-center">
          Arcane COE
        </h1>
        <button
          onClick={handleConnect}
          disabled={status !== 'disconnected'}
          className="btn-cr-yellow text-xl px-8 py-4 font-bold tracking-wide rounded-[10px] shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Connect
        </button>
      </motion.div>

      {/* React Hot Toast container with Tailwind styles */}
      <Toaster
        position={position}
        toastOptions={{
          className: 'font-luckiest bg-cream text-dark border border-dark rounded-[5px] shadow-lg p-4',
          success: { duration: 1500 },
        }}
      />
    </div>
  );
};