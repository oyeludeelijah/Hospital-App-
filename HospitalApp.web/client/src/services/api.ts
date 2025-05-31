import axios from "axios";
import { AxiosResponse, InternalAxiosRequestConfig } from 'axios';

// Determine if the app is running in production or development
// Check both hostname and port (Vercel uses localhost:3000 during development but a different host in production)
const isProduction = !window.location.hostname.includes('localhost') || 
                   window.location.href.includes('vercel.app');

// Create a custom API instance that directly uses mock data in production
export const api = {
  get: async (endpoint: string) => {
    if (isProduction) {
      // In production, return mock data directly
      return Promise.resolve(mockApiResponse('get', endpoint, {}));
    } else {
      // In development, make a real API call
      return axios.get(`http://localhost:5005/api${endpoint}`, {
        headers: { "Content-Type": "application/json" }
      });
    }
  },
  post: async (endpoint: string, data: any) => {
    if (isProduction) {
      // In production, return mock data directly
      return Promise.resolve(mockApiResponse('post', endpoint, data));
    } else {
      // In development, make a real API call
      return axios.post(`http://localhost:5005/api${endpoint}`, data, {
        headers: { "Content-Type": "application/json" }
      });
    }
  },
  put: async (endpoint: string, data: any) => {
    if (isProduction) {
      // In production, return mock data directly
      return Promise.resolve(mockApiResponse('put', endpoint, data));
    } else {
      // In development, make a real API call
      return axios.put(`http://localhost:5005/api${endpoint}`, data, {
        headers: { "Content-Type": "application/json" }
      });
    }
  },
  delete: async (endpoint: string) => {
    if (isProduction) {
      // In production, return mock data directly
      return Promise.resolve(mockApiResponse('delete', endpoint, {}));
    } else {
      // In development, make a real API call
      return axios.delete(`http://localhost:5005/api${endpoint}`, {
        headers: { "Content-Type": "application/json" }
      });
    }
  }
};

// Mock API response generator
function mockApiResponse(method: string | undefined, url: string | undefined, requestData: any): AxiosResponse {
  // Mock data for various API endpoints
  const mockData = {
    // Auth endpoints
    '/api/auth/login': {
      token: 'mock-jwt-token',
      user: {
        id: 1,
        username: 'demouser',
        email: 'demo@example.com',
        role: 'Admin'
      }
    },
    '/api/auth/register': {
      token: 'mock-jwt-token',
      user: {
        id: 1,
        username: requestData?.username || 'newuser',
        email: requestData?.email || 'new@example.com',
        role: requestData?.role || 'User'
      }
    },
    
    // Patients endpoints
    '/api/patients': [
      {
        id: 1,
        firstName: 'Samuel',
        lastName: 'Ajani',
        dateOfBirth: '1990-05-15',
        gender: 'Male',
        contactNumber: '444-444-888',
        email: 'ajani@gmail.com',
        address: '9, Balat, France, Earth'
      },
      {
        id: 2,
        firstName: 'Maria',
        lastName: 'Rodriguez',
        dateOfBirth: '1985-10-20',
        gender: 'Female',
        contactNumber: '555-123-4567',
        email: 'maria@example.com',
        address: '123 Main St, Springfield'
      }
    ],
    
    // Doctors endpoints
    '/api/doctors': [
      {
        id: 1,
        firstName: 'John',
        lastName: 'Smith',
        specialization: 'Cardiology',
        contactNumber: '555-1234',
        email: 'john.smith@hospital.com',
        department: 'Cardiology'
      },
      {
        id: 2,
        firstName: 'Sarah',
        lastName: 'Johnson',
        specialization: 'Pediatrics',
        contactNumber: '555-5678',
        email: 'sarah.johnson@hospital.com',
        department: 'Pediatrics'
      }
    ],
    
    // Appointments endpoints
    '/api/appointments': [
      {
        id: 1,
        patientName: 'Samuel Ajani',
        doctorName: 'John Smith',
        date: '2025-06-15',
        time: '10:00 AM',
        status: 'Scheduled',
        notes: 'Regular checkup'
      },
      {
        id: 2,
        patientName: 'Maria Rodriguez',
        doctorName: 'Sarah Johnson',
        date: '2025-06-20',
        time: '2:30 PM',
        status: 'Confirmed',
        notes: 'Follow-up appointment'
      }
    ],
    
    // Departments endpoints
    '/api/departments': [
      {
        id: 1,
        name: 'Cardiology',
        description: 'Heart and cardiovascular system',
        doctorCount: 1
      },
      {
        id: 2,
        name: 'Pediatrics',
        description: 'Medical care for infants, children',
        doctorCount: 1
      },
      {
        id: 3,
        name: 'Orthopedics',
        description: 'Musculoskeletal system - bones',
        doctorCount: 0
      }
    ],
    
    // Billing endpoints
    '/api/billing': [
      {
        id: 1,
        patientName: 'Samuel Ajani',
        date: '2025-05-15',
        amount: 150.00,
        status: 'Paid',
        description: 'Consultation fee'
      },
      {
        id: 2,
        patientName: 'Maria Rodriguez',
        date: '2025-05-20',
        amount: 75.50,
        status: 'Pending',
        description: 'Lab tests'
      }
    ]
  };
  
  // Determine which mock data to return based on the URL and method
  const endpoint = Object.keys(mockData).find(key => 
    url?.includes(key) || key.includes(url?.replace('/api/', '') || '')
  );
  
  let responseData = endpoint ? mockData[endpoint as keyof typeof mockData] : { message: 'Not found' };
  
  // Handle POST requests for creating new items
  if (method === 'post' && url && responseData instanceof Array) {
    const newItem = {
      ...requestData,
      id: responseData.length + 1
    };
    responseData = newItem;
  }
  
  // Handle PUT requests for updating items
  if (method === 'put' && url && responseData instanceof Array) {
    // Extract ID from URL (e.g., /api/patients/1)
    const idMatch = url.match(/\/([^/]+)\/([0-9]+)$/);
    const id = idMatch ? parseInt(idMatch[2]) : null;
    
    if (id) {
      responseData = {
        ...requestData,
        id
      };
    }
  }
  
  // Handle DELETE requests
  if (method === 'delete') {
    responseData = { message: 'Deleted successfully' };
  }
  
  // Return a mocked axios response object
  return {
    data: responseData,
    status: 200,
    statusText: 'OK',
    headers: {},
    config: {
      headers: {}
    } as InternalAxiosRequestConfig,
    request: {}
  } as AxiosResponse;
}
