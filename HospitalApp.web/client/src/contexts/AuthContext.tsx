import React, { createContext, useContext, useState, useEffect } from 'react';

interface AuthState {
  isAuthenticated: boolean;
  token: string | null;
  user: {
    id: number;
    username: string;
    role: string;
  } | null;
}

interface AuthContextType extends AuthState {
  login: (token: string, userId: number, username: string, role: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [authState, setAuthState] = useState<AuthState>(() => {
    // Initialize from localStorage
    const token = localStorage.getItem('token');
    const userStr = localStorage.getItem('user');
    const user = userStr ? JSON.parse(userStr) : null;
    
    return {
      isAuthenticated: !!token,
      token,
      user
    };
  });

  // Update localStorage when auth state changes
  useEffect(() => {
    if (authState.token) {
      localStorage.setItem('token', authState.token);
      localStorage.setItem('user', JSON.stringify(authState.user));
    } else {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
    }
  }, [authState]);

  const login = (token: string, userId: number, username: string, role: string) => {
    setAuthState({
      isAuthenticated: true,
      token,
      user: {
        id: userId,
        username,
        role
      }
    });
  };

  const logout = () => {
    setAuthState({
      isAuthenticated: false,
      token: null,
      user: null
    });
  };

  return (
    <AuthContext.Provider value={{ ...authState, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
