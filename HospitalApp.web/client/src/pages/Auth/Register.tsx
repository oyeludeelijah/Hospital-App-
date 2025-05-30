import React, { useState } from 'react';
import { register } from '../../services/authService';
import { useAuth } from '../../contexts/AuthContext';
import { useNavigate, Link } from 'react-router-dom';

const Register: React.FC = () => {
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    role: 1 // Default to Doctor role (0=Admin, 1=Doctor, 2=Nurse, 3=Patient)
  });
  
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'role' ? parseInt(value) : value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Simple validation
    if (formData.password !== formData.confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    if (formData.password.length < 6) {
      setError('Password must be at least 6 characters');
      return;
    }
    
    try {
      setError('');
      setLoading(true);
      
      const response = await register({
        username: formData.username,
        email: formData.email,
        password: formData.password,
        firstName: formData.firstName,
        lastName: formData.lastName,
        role: formData.role
      });
      
      if (response.success) {
        login(
          response.token,
          response.userId,
          response.username,
          response.role.toString() // Convert number to string
        );
        // Redirect to home page after successful registration
        console.log('Registration successful, redirecting to home page');
        navigate('/')
      } else {
        setError(response.message);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to register');
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
      marginTop: '40px', // Add margin to position the form
    }}>
      <div style={{ 
        width: '500px',
        padding: '30px', 
        borderRadius: '8px', 
        boxShadow: '0 4px 8px rgba(0,0,0,0.1)',
        backgroundColor: 'white',
      }}>
        <h2 style={{ textAlign: 'center', marginBottom: '24px' }}>Register</h2>
        
        {error && (
          <div style={{ padding: '10px', backgroundColor: '#ffebee', color: '#d32f2f', borderRadius: '4px', marginBottom: '20px' }}>
            {error}
          </div>
        )}
        
        <form onSubmit={handleSubmit}>
          <div style={{ display: 'flex', gap: '20px', marginBottom: '16px' }}>
            <div style={{ flex: 1 }}>
              <label htmlFor="firstName" style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>
                First Name
              </label>
              <input
                type="text"
                id="firstName"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                required
                style={{ 
                  width: '100%', 
                  padding: '10px', 
                  borderRadius: '4px', 
                  border: '1px solid #ccc',
                  boxSizing: 'border-box' 
                }}
              />
            </div>
            
            <div style={{ flex: 1 }}>
              <label htmlFor="lastName" style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>
                Last Name
              </label>
              <input
                type="text"
                id="lastName"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                required
                style={{ 
                  width: '100%', 
                  padding: '10px', 
                  borderRadius: '4px', 
                  border: '1px solid #ccc',
                  boxSizing: 'border-box'
                }}
              />
            </div>
          </div>
          
          <div style={{ marginBottom: '16px' }}>
            <label htmlFor="username" style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>
              Username
            </label>
            <input
              type="text"
              id="username"
              name="username"
              value={formData.username}
              onChange={handleChange}
              required
              style={{ 
                width: '100%', 
                padding: '10px', 
                borderRadius: '4px', 
                border: '1px solid #ccc',
                boxSizing: 'border-box'
              }}
            />
          </div>
          
          <div style={{ marginBottom: '16px' }}>
            <label htmlFor="email" style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>
              Email
            </label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              style={{ 
                width: '100%', 
                padding: '10px', 
                borderRadius: '4px', 
                border: '1px solid #ccc',
                boxSizing: 'border-box'
              }}
            />
          </div>
          
          <div style={{ marginBottom: '16px' }}>
            <label htmlFor="role" style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>
              Role
            </label>
            <select
              id="role"
              name="role"
              value={formData.role}
              onChange={handleChange}
              required
              style={{ 
                width: '100%', 
                padding: '10px', 
                borderRadius: '4px', 
                border: '1px solid #ccc',
                boxSizing: 'border-box'
              }}
            >
              <option value="0">Admin</option>
              <option value="1">Doctor</option>
              <option value="2">Nurse</option>
              <option value="3">Patient</option>
            </select>
          </div>
          
          <div style={{ marginBottom: '16px' }}>
            <label htmlFor="password" style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>
              Password
            </label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              required
              style={{ 
                width: '100%', 
                padding: '10px', 
                borderRadius: '4px', 
                border: '1px solid #ccc',
                boxSizing: 'border-box'
              }}
            />
          </div>
          
          <div style={{ marginBottom: '24px' }}>
            <label htmlFor="confirmPassword" style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>
              Confirm Password
            </label>
            <input
              type="password"
              id="confirmPassword"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
              style={{ 
                width: '100%', 
                padding: '10px', 
                borderRadius: '4px', 
                border: '1px solid #ccc',
                boxSizing: 'border-box'
              }}
            />
          </div>
          
          <button
            type="submit"
            disabled={loading}
            style={{
              width: '100%',
              padding: '12px',
              backgroundColor: '#1976d2',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              cursor: loading ? 'not-allowed' : 'pointer',
              opacity: loading ? 0.7 : 1,
              boxSizing: 'border-box'
            }}
          >
            {loading ? 'Registering...' : 'Register'}
          </button>
        </form>
        
        <div style={{ marginTop: '20px', textAlign: 'center' }}>
          Already have an account? <Link to="/login">Login</Link>
        </div>
      </div>
    </div>
  );
};

export default Register;
