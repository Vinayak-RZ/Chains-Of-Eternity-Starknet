import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { Toaster } from 'react-hot-toast';
import { useStarknetConnect } from '../connect/useStarknetConnect.ts';
import './loginScreen.css';

interface LoginScreenProps {
  onLoginSuccess: () => void;
}

interface Particle {
  id: number;
  x: number;
  y: number;
  size: number;
  duration: number;
  delay: number;
}

export const LoginScreen: React.FC<LoginScreenProps> = ({ onLoginSuccess }) => {
  const { status, handleConnect, hasTriedConnect } = useStarknetConnect();
  const [particles, setParticles] = useState<Particle[]>([]);
  
  const position = 'bottom-center';

  // Generate floating particles
  useEffect(() => {
    const newParticles: Particle[] = Array.from({ length: 30 }, (_, i) => ({
      id: i,
      x: Math.random() * 100,
      y: Math.random() * 100,
      size: Math.random() * 4 + 2,
      duration: Math.random() * 10 + 15,
      delay: Math.random() * 5
    }));
    setParticles(newParticles);
  }, []);

  useEffect(() => {
    if (status === 'connected' && hasTriedConnect) {
      console.log("Wallet connected, initializing player...");
    }
  }, [status, hasTriedConnect]);

  return (
    <div className="login-container">
      {/* Animated background gradient orbs */}
      <div className="background-orbs">
        <motion.div
          animate={{
            scale: [1, 1.2, 1],
            x: [0, 100, 0],
            y: [0, -50, 0],
          }}
          transition={{
            duration: 20,
            repeat: Infinity,
            ease: "easeInOut"
          }}
          className="orb orb-1"
        />
        <motion.div
          animate={{
            scale: [1, 1.3, 1],
            x: [0, -80, 0],
            y: [0, 100, 0],
          }}
          transition={{
            duration: 25,
            repeat: Infinity,
            ease: "easeInOut"
          }}
          className="orb orb-2"
        />
        <motion.div
          animate={{
            scale: [1, 1.1, 1],
            x: [0, -50, 0],
            y: [0, 80, 0],
          }}
          transition={{
            duration: 18,
            repeat: Infinity,
            ease: "easeInOut"
          }}
          className="orb orb-3"
        />
      </div>

      {/* Floating particles */}
      {particles.map((particle) => (
        <motion.div
          key={particle.id}
          className="particle"
          style={{
            left: `${particle.x}%`,
            top: `${particle.y}%`,
            width: particle.size,
            height: particle.size,
          }}
          animate={{
            y: [0, -100, 0],
            opacity: [0, 1, 0],
          }}
          transition={{
            duration: particle.duration,
            repeat: Infinity,
            delay: particle.delay,
            ease: "easeInOut"
          }}
        />
      ))}

      {/* Grid pattern overlay */}
      <div className="grid-overlay" />

      {/* Main content */}
      <motion.div
        initial={{ scale: 0.8, opacity: 0, y: 50 }}
        animate={{ scale: 1, opacity: 1, y: 0 }}
        transition={{ duration: 0.6, ease: "easeOut" }}
        className="main-content"
      >
        {/* Glowing title container */}
        <motion.div 
          className="title-container"
          animate={{ 
            filter: [
              "drop-shadow(0 0 20px rgba(139, 92, 246, 0.5))",
              "drop-shadow(0 0 40px rgba(139, 92, 246, 0.8))",
              "drop-shadow(0 0 20px rgba(139, 92, 246, 0.5))"
            ]
          }}
          transition={{ duration: 3, repeat: Infinity, ease: "easeInOut" }}
        >
          <h1 className="title">ARCANE</h1>
          <motion.p 
            className="subtitle"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.3, duration: 0.8 }}
          >
            CHRONICLES OF ETERNITY
          </motion.p>
        </motion.div>

        {/* Decorative lines */}
        <div className="decorative-lines">
          <motion.div 
            className="line line-left"
            initial={{ width: 0 }}
            animate={{ width: 64 }}
            transition={{ delay: 0.5, duration: 0.8 }}
          />
          <motion.div
            animate={{ rotate: 360 }}
            transition={{ duration: 20, repeat: Infinity, ease: "linear" }}
            className="rotating-dot"
          />
          <motion.div 
            className="line line-right"
            initial={{ width: 0 }}
            animate={{ width: 64 }}
            transition={{ delay: 0.5, duration: 0.8 }}
          />
        </div>

        {/* Connect button */}
        <motion.button
          onClick={handleConnect}
          disabled={status !== 'disconnected'}
          className={`connect-button ${status !== 'disconnected' ? 'disabled' : ''}`}
          whileHover={{ scale: status !== 'disconnected' ? 1 : 1.05 }}
          whileTap={{ scale: status !== 'disconnected' ? 1 : 0.98 }}
        >
          {/* Button background gradient */}
          <div className="button-bg" />
          
          {/* Animated border */}
          <motion.div
            className="button-shine"
            animate={{
              x: ["-100%", "200%"],
            }}
            transition={{
              duration: 1.5,
              repeat: Infinity,
              ease: "linear"
            }}
          />
          
          {/* Inner glow */}
          <div className="button-glow" />
          
          <span className="button-content">
            {status === 'connected' ? (
              <>
                <motion.span
                  animate={{ scale: [1, 1.2, 1] }}
                  transition={{ duration: 1, repeat: Infinity }}
                  className="status-dot connected"
                />
                CONNECTED
              </>
            ) : status === 'connecting' ? (
              <>
                <motion.span
                  animate={{ rotate: 360 }}
                  transition={{ duration: 1, repeat: Infinity, ease: "linear" }}
                  className="spinner"
                />
                CONNECTING...
              </>
            ) : (
              'ENTER REALM'
            )}
          </span>
        </motion.button>

        {/* Status message */}
        <motion.p
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 0.8 }}
          className="status-message"
        >
          Connect your wallet to begin your journey
        </motion.p>
      </motion.div>

      {/* Corner decorations */}
      <div className="corner-decoration corner-top-left" />
      <div className="corner-decoration corner-top-right" />
      <div className="corner-decoration corner-bottom-left" />
      <div className="corner-decoration corner-bottom-right" />

      {/* Toast notifications */}
      <Toaster
        position={position}
        toastOptions={{
          className: 'custom-toast',
          success: { duration: 1500 },
        }}
      />
    </div>
  );
};