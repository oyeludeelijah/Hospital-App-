import React from 'react';
import './App.css';
import { AuthProvider } from './contexts/AuthContext';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import ApiTest from './components/ApiTest';
import Login from './pages/Auth/Login';
import Register from './pages/Auth/Register';
import { useAuth } from './contexts/AuthContext';

// Protected route component
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const auth = useAuth();
  
  if (!auth.isAuthenticated) {
    return <Navigate to="/login" />;
  }
  
  return <>{children}</>;
};

// Main app layout with navigation
const AppLayout = ({ children }: { children: React.ReactNode }) => {
  const auth = useAuth();
  const handleLogout = () => {
    auth.logout();
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>Hospital Management System</h1>
        <nav style={{ display: 'flex', justifyContent: 'center', gap: '20px', marginTop: '10px' }}>
          <a href="/" style={{ color: 'white', textDecoration: 'none' }}>Home</a>
          {auth.isAuthenticated ? (
            <>
              <a href="/patients" style={{ color: 'white', textDecoration: 'none' }}>Patients</a>
              <a href="/doctors" style={{ color: 'white', textDecoration: 'none' }}>Doctors</a>
              <a href="/appointments" style={{ color: 'white', textDecoration: 'none' }}>Appointments</a>
              <button 
                onClick={handleLogout}
                style={{
                  background: 'none',
                  border: 'none',
                  color: 'white',
                  cursor: 'pointer',
                  fontSize: '16px'
                }}
              >
                Logout
              </button>
            </>
          ) : (
            <>
              <a href="/login" style={{ color: 'white', textDecoration: 'none' }}>Login</a>
              <a href="/register" style={{ color: 'white', textDecoration: 'none' }}>Register</a>
            </>
          )}
        </nav>
      </header>
      <main>
        {children}
      </main>
    </div>
  );
};

// App component with routing
function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/" element={
            <AppLayout>
              <div style={{ padding: '20px' }}>
                <h2>Welcome to the Hospital Management System</h2>
                <p>This system helps manage hospital operations efficiently.</p>
                <p>The system includes:</p>
                <ul style={{ textAlign: 'left', maxWidth: '500px', margin: '0 auto' }}>
                  <li>Patient Management</li>
                  <li>Doctor Management</li>
                  <li>Appointment Scheduling</li>
                  <li>Medical Records</li>
                  <li>Billing System</li>
                </ul>
                <div className="api-test-section" style={{ marginTop: '30px' }}>
                  <ApiTest />
                </div>
              </div>
            </AppLayout>
          } />
          <Route path="/patients" element={
            <ProtectedRoute>
              <AppLayout>
                <div style={{ padding: '20px' }}>
                  <h2>Patient Management</h2>
                  <p>Coming soon...</p>
                </div>
              </AppLayout>
            </ProtectedRoute>
          } />
          <Route path="/doctors" element={
            <ProtectedRoute>
              <AppLayout>
                <div style={{ padding: '20px' }}>
                  <h2>Doctor Management</h2>
                  <p>Coming soon...</p>
                </div>
              </AppLayout>
            </ProtectedRoute>
          } />
          <Route path="/appointments" element={
            <ProtectedRoute>
              <AppLayout>
                <div style={{ padding: '20px' }}>
                  <h2>Appointment Scheduling</h2>
                  <p>Coming soon...</p>
                </div>
              </AppLayout>
            </ProtectedRoute>
          } />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
