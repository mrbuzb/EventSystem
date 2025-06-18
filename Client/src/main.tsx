import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import App from './App.tsx';
import './index.css';
import { AuthProvider } from './contexts/AuthContext';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    {/* 🟢 AuthProvider bilan App ni o'rab oldik */}
    <AuthProvider>
      <App />
    </AuthProvider>
  </StrictMode>
);
