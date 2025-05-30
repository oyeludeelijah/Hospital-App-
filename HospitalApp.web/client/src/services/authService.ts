import axios from 'axios';

// Create a configured axios instance with explicitly verified backend URL
const API_BASE_URL = 'http://localhost:5005';
console.log('Using API base URL:', API_BASE_URL);

const api = axios.create({
  baseURL: `${API_BASE_URL}/api`,
  timeout: 15000, // Increased timeout for slower connections
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
});

// Add request/response interceptors for debugging
api.interceptors.request.use(request => {
  console.log('Starting Request:', request.url, request.data);
  return request;
});

api.interceptors.response.use(
  response => {
    console.log('Response:', response.data);
    return response;
  },
  error => {
    console.error('API Error:', error.message);
    if (error.response) {
      console.error('Response data:', error.response.data);
      console.error('Response status:', error.response.status);
    } else if (error.request) {
      console.error('No response received, request:', error.request);
    }
    return Promise.reject(error);
  }
);

const API_URL = '/auth';

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: number; // 0 = Admin, 1 = Doctor, 2 = Receptionist
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  token: string;
  userId: number;
  username: string;
  role: number;
}

export const register = async (userData: RegisterRequest): Promise<AuthResponse> => {
  try {
    // Log the request data for debugging
    console.log('Sending registration data:', userData);
    
    // Use our configured axios instance
    const response = await api.post(`${API_URL}/register`, userData);
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (error.response) {
        // Server responded with an error
        const errorMessage = error.response.data.message || 'Registration failed';
        console.error('Server error during registration:', errorMessage);
        throw new Error(errorMessage);
      } else if (error.request) {
        // No response received
        console.error('No response received from server during registration');
        throw new Error('Server did not respond. Please check if the API is running.');
      }
    }
    // Generic error
    console.error('Registration error:', error);
    throw new Error('Network error during registration. Please try again later.');
  }
};

export const login = async (userData: LoginRequest): Promise<AuthResponse> => {
  try {
    // Log the request data for debugging
    console.log('Sending login data:', userData);
    
    // Use our configured axios instance
    const response = await api.post(`${API_URL}/login`, userData);
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (error.response) {
        // Server responded with an error
        const errorMessage = error.response.data.message || 'Login failed';
        console.error('Server error during login:', errorMessage);
        throw new Error(errorMessage);
      } else if (error.request) {
        // No response received
        console.error('No response received from server during login');
        throw new Error('Server did not respond. Please check if the API is running.');
      }
    }
    // Generic error
    console.error('Login error:', error);
    throw new Error('Network error during login. Please try again later.');
  }
};
