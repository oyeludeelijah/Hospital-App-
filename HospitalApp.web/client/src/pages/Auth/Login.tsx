import React, { useState } from 'react';
import { login } from '../../services/authService';
import { useAuth } from '../../contexts/AuthContext';
import { useNavigate, Link } from 'react-router-dom';

const Login: React.FC = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login: authLogin } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      setError('');
      setLoading(true);
      
      const response = await login({ username, password });
      
      if (response.success) {
        authLogin(
          response.token,
          response.userId,
          response.username,
          response.role.toString() // Convert number to string
        );
        // Redirect to home page after successful login
        console.log('Login successful, redirecting to home page');
        navigate('/')
      } else {
        setError(response.message);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to login');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      height: 'calc(100vh - 120px)', // Adjust for header height
    }}>
      <div style={{ 
        width: '400px',
        padding: '30px', 
        borderRadius: '8px', 
        boxShadow: '0 4px 8px rgba(0,0,0,0.1)',
        backgroundColor: 'white',
      }}>
        <h2 style={{ textAlign: 'center', marginBottom: '24px' }}>Login</h2>
        
        {error && (
          <div style={{ padding: '10px', backgroundColor: '#ffebee', color: '#d32f2f', borderRadius: '4px', marginBottom: '20px' }}>
            {error}
          </div>
        )}
        
        <form onSubmit={handleSubmit}>
          <div style={{ marginBottom: '16px' }}>
            <label htmlFor="username" style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>
              Username
            </label>
            <input
              type="text"
              id="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              style={{ 
                width: '100%', 
                padding: '10px', 
                borderRadius: '4px', 
                border: '1px solid #ccc',
                boxSizing: 'border-box' // This ensures padding is included in width calculation
              }}
            />
          </div>
          
          <div style={{ marginBottom: '16px' }}>
            <label htmlFor="password" style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>
              Password
            </label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              style={{ 
                width: '100%', 
                padding: '10px', 
                borderRadius: '4px', 
                border: '1px solid #ccc',
                boxSizing: 'border-box' // This ensures padding is included in width calculation
              }}
            />
          </div>
          
          <button 
            type="submit" 
            disabled={loading}
            style={{ 
              width: '100%', 
              padding: '10px', 
              backgroundColor: '#1976d2', 
              color: 'white', 
              border: 'none', 
              borderRadius: '4px', 
              fontSize: '16px',
              cursor: loading ? 'not-allowed' : 'pointer',
              opacity: loading ? 0.7 : 1
            }}
          >
            Login
          </button>
        </form>
        
        <div style={{ marginTop: '20px', textAlign: 'center' }}>
          Don't have an account? <Link to="/register">Register</Link>
        </div>
      </div>
    </div>
  );
};

export default Login;
