import React, { useState, useEffect } from 'react';
import { api } from '../services/api';

/**
 * ApiStatus component that shows whether the app is using mock data or real API
 */
const ApiStatus: React.FC = () => {
  const [status, setStatus] = useState<'checking' | 'mock' | 'real'>('checking');
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // Check if we're in production or development
    const isProduction = window.location.hostname !== 'localhost';
    
    if (isProduction) {
      // In production, we're definitely using mock data
      setStatus('mock');
      setError(null);
    } else {
      // In development, try to make a test API call to see if real API is available
      api.get('/auth/test')
        .then(response => {
          setStatus('real');
          setError(null);
        })
        .catch(err => {
          // If there's an error, we're either using mock data or there's a connection issue
          const isConnectionError = err.message?.includes('Network Error') || 
                                   err.message?.includes('Connection refused');
          
          if (isConnectionError) {
            setStatus('real');
            setError('API connection error. Please start your backend API server.');
          } else {
            // Non-connection errors might be because we're getting mock data
            setStatus('mock');
            setError(null);
          }
        });
    }
  }, []);

  return (
    <div style={{ 
      position: 'fixed', 
      bottom: '10px', 
      right: '10px', 
      padding: '8px 12px',
      backgroundColor: status === 'mock' ? '#fff3e0' : status === 'real' ? '#e8f5e9' : '#f5f5f5',
      border: `1px solid ${status === 'mock' ? '#ffb74d' : status === 'real' ? '#81c784' : '#e0e0e0'}`,
      borderRadius: '4px',
      fontSize: '12px',
      fontWeight: 500,
      color: '#424242',
      zIndex: 9999,
      boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
      display: 'flex',
      alignItems: 'center',
      gap: '6px'
    }}>
      <div style={{
        width: '8px',
        height: '8px',
        borderRadius: '50%',
        backgroundColor: status === 'checking' ? '#bdbdbd' : 
                         status === 'mock' ? '#ff9800' : 
                         error ? '#f44336' : '#4caf50'
      }} />
      {status === 'checking' && 'Checking API...'}
      {status === 'mock' && 'Using Demo Data'}
      {status === 'real' && !error && 'Connected to API'}
      {status === 'real' && error && 'API Connection Error'}
      {error && <div style={{ color: '#d32f2f', marginTop: '4px', fontSize: '10px' }}>{error}</div>}
    </div>
  );
};

export default ApiStatus;
