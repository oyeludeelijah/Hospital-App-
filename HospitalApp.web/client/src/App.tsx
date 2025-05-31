import React from 'react';
import './App.css';
import { AuthProvider } from './contexts/AuthContext';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Login from './pages/Auth/Login';
import Register from './pages/Auth/Register';
import Patients from './pages/Patients/Patients';
import Doctors from './pages/Doctors/Doctors';
import Appointments from './pages/Appointments/Appointments';
import Billing from './pages/Billing/Billing';
import Departments from './pages/Departments/Departments';
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

  // Get current path to highlight active navigation item
  const pathname = window.location.pathname;

  return (
    <div className="App">
      <header className="App-header">
        <h1>Hospital Management System</h1>
        <div className="nav-container">
          <div className="nav-links">
            <a 
              href="/" 
              className={`nav-link ${pathname === '/' ? 'active' : ''}`}
            >
              Home
            </a>
            {auth.isAuthenticated ? (
              <>
                <a 
                  href="/patients" 
                  className={`nav-link ${pathname === '/patients' ? 'active' : ''}`}
                >
                  Patients
                </a>
                <a 
                  href="/doctors" 
                  className={`nav-link ${pathname === '/doctors' ? 'active' : ''}`}
                >
                  Doctors
                </a>
                <a 
                  href="/appointments" 
                  className={`nav-link ${pathname === '/appointments' ? 'active' : ''}`}
                >
                  Appointments
                </a>
                <a 
                  href="/billing" 
                  className={`nav-link ${pathname === '/billing' ? 'active' : ''}`}
                >
                  Billing
                </a>
                <a 
                  href="/departments" 
                  className={`nav-link ${pathname === '/departments' ? 'active' : ''}`}
                >
                  Departments
                </a>
                <button 
                  onClick={handleLogout}
                  className="logout-btn"
                >
                  Logout
                </button>
              </>
            ) : (
              <>
                <a 
                  href="/login" 
                  className={`nav-link ${pathname === '/login' ? 'active' : ''}`}
                >
                  Login
                </a>
                <a 
                  href="/register" 
                  className={`nav-link ${pathname === '/register' ? 'active' : ''}`}
                >
                  Register
                </a>
              </>
            )}
          </div>
        </div>
      </header>
      <main>
        {children}
      </main>
      <footer className="footer">
        <div className="footer-content">
          <div className="footer-section">
            <h4>Contact</h4>
            <p>support@hospitalapp.com</p>
          </div>
          <div className="footer-section">
            <p>&copy; 2025 Hospital Management System</p>
          </div>
          <div className="footer-section">
            <p>Version 1.0.2</p>
          </div>
        </div>
      </footer>
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
              <div className="welcome-section">
                <h1 className="welcome-heading">Welcome to the Hospital Management System</h1>
                <p className="welcome-text">This system helps manage hospital operations efficiently.</p>
                <div className="divider"></div>
                
                <h2>System Features</h2>
                <table className="features-table">
                  <thead>
                    <tr>
                      <th>Feature</th>
                      <th>Description</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td className="feature-name">Patient Management</td>
                      <td className="feature-description">Register patients, maintain medical histories, and track patient admissions and discharges</td>
                    </tr>
                    <tr>
                      <td className="feature-name">Doctor Management</td>
                      <td className="feature-description">Organize doctor profiles, specialties, and department assignments</td>
                    </tr>
                    <tr>
                      <td className="feature-name">Department Management</td>
                      <td className="feature-description">Create and manage hospital departments, track associated doctors and resource allocation</td>
                    </tr>
                    <tr>
                      <td className="feature-name">Appointment Scheduling</td>
                      <td className="feature-description">Book, reschedule, and manage patient appointments with calendar integration</td>
                    </tr>
                    <tr>
                      <td className="feature-name">Medical Records</td>
                      <td className="feature-description">Access and update digital health records, prescriptions, and test results</td>
                    </tr>
                    <tr>
                      <td className="feature-name">Billing System</td>
                      <td className="feature-description">Generate invoices, process payments, and manage insurance claims</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </AppLayout>
          } />
          <Route path="/patients" element={
            <ProtectedRoute>
              <AppLayout>
                <Patients />
              </AppLayout>
            </ProtectedRoute>
          } />
          <Route path="/doctors" element={
            <ProtectedRoute>
              <AppLayout>
                <Doctors />
              </AppLayout>
            </ProtectedRoute>
          } />
          <Route path="/appointments" element={
            <ProtectedRoute>
              <AppLayout>
                <Appointments />
              </AppLayout>
            </ProtectedRoute>
          } />
          <Route path="/billing" element={
            <ProtectedRoute>
              <AppLayout>
                <Billing />
              </AppLayout>
            </ProtectedRoute>
          } />
          <Route path="/departments" element={
            <ProtectedRoute>
              <AppLayout>
                <Departments />
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
