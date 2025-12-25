import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import Login from './pages/Login';
import Devices from './pages/Devices';
import Downloads from './pages/Downloads';
import Settings from './pages/Settings';
import { DeviceProvider } from './context/DeviceContext';
import { AuthProvider, useAuth } from './context/AuthContext';

const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated } = useAuth();
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  return <>{children}</>;
};

const AppRoutes = () => {
  const { isAuthenticated } = useAuth();

  return (
    <Routes>
      <Route path="/login" element={!isAuthenticated ? <Login /> : <Navigate to="/" replace />} />

      <Route path="/" element={
        <ProtectedRoute>
          <DeviceProvider>
            <Layout />
          </DeviceProvider>
        </ProtectedRoute>
      }>
        <Route index element={<Dashboard />} />
        <Route path="devices" element={<Devices />} />
        <Route path="downloads" element={<Downloads />} />
        <Route path="settings" element={<Settings />} />
        <Route path="console" element={<div className="p-6">Console View (See Dashboard)</div>} />
      </Route>
    </Routes>
  );
};

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
