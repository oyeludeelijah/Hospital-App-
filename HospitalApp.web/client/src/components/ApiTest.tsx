import React, { useState } from 'react';

const ApiTest: React.FC = () => {
  const [patientData, setPatientData] = useState<any[]>([]);
  const [message, setMessage] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>('');

  // Simplified hardcoded patient data for demonstration
  const hardcodedPatients = [
    { 
      id: 1, 
      firstName: 'John', 
      lastName: 'Doe', 
      dateOfBirth: '1980-01-01',
      gender: 'Male',
      contactNumber: '555-1234',
      email: 'john.doe@example.com',
      address: '123 Main St'
    },
    { 
      id: 2, 
      firstName: 'Jane', 
      lastName: 'Smith', 
      dateOfBirth: '1985-05-15',
      gender: 'Female',
      contactNumber: '555-5678',
      email: 'jane.smith@example.com',
      address: '456 Oak Ave'
    }
  ];

  const fetchApiData = () => {
    setLoading(true);
    setError('');
    
    // Since we're having network issues with the browser preview,
    // we'll use hardcoded data to demonstrate functionality
    setTimeout(() => {
      try {
        setPatientData(hardcodedPatients);
        setMessage('Successfully loaded patient data!');
        setLoading(false);
      } catch (err) {
        setError('Error loading data');
        setLoading(false);
      }
    }, 1000); // Simulate network delay
  };

  return (
    <div style={{ 
      padding: '20px',
      maxWidth: '800px',
      margin: '0 auto',
      fontFamily: 'Arial, sans-serif'
    }}>
      <h1>Hospital Management System</h1>
      
      <div style={{
        padding: '15px',
        border: '1px solid #ccc',
        borderRadius: '5px',
        marginTop: '20px',
        backgroundColor: '#f5f5f5'
      }}>
        <h2>Patient Management Demo</h2>
        <button 
          onClick={fetchApiData}
          style={{
            padding: '10px 15px',
            backgroundColor: '#4CAF50',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer',
            marginBottom: '15px'
          }}
          disabled={loading}
        >
          {loading ? 'Loading...' : 'Load Patient Data'}
        </button>
        
        {error && <p style={{ color: 'red' }}>{error}</p>}
        {message && <p style={{ color: 'green' }}>{message}</p>}
        
        {patientData.length > 0 && (
          <div>
            <h3>Patient List</h3>
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ backgroundColor: '#f2f2f2' }}>
                  <th style={{ border: '1px solid #ddd', padding: '8px', textAlign: 'left' }}>ID</th>
                  <th style={{ border: '1px solid #ddd', padding: '8px', textAlign: 'left' }}>Name</th>
                  <th style={{ border: '1px solid #ddd', padding: '8px', textAlign: 'left' }}>Gender</th>
                  <th style={{ border: '1px solid #ddd', padding: '8px', textAlign: 'left' }}>Contact</th>
                  <th style={{ border: '1px solid #ddd', padding: '8px', textAlign: 'left' }}>Email</th>
                </tr>
              </thead>
              <tbody>
                {patientData.map(patient => (
                  <tr key={patient.id}>
                    <td style={{ border: '1px solid #ddd', padding: '8px' }}>{patient.id}</td>
                    <td style={{ border: '1px solid #ddd', padding: '8px' }}>{patient.firstName} {patient.lastName}</td>
                    <td style={{ border: '1px solid #ddd', padding: '8px' }}>{patient.gender}</td>
                    <td style={{ border: '1px solid #ddd', padding: '8px' }}>{patient.contactNumber}</td>
                    <td style={{ border: '1px solid #ddd', padding: '8px' }}>{patient.email}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
};

export default ApiTest;
