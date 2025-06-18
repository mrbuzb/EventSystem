import React, { createContext, useContext, useEffect, useState, ReactNode } from 'react';
import { User, LoginResponse, UserLoginDto, UserCreateDto } from '../types/api';
import { apiService } from '../services/api';
import { jwtDecode } from 'jwt-decode';

interface DecodedToken {
  UserId: string;
  FirstName: string;
  LastName: string;
  PhoneNumber: string;
  unique_name: string;
  role: string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": string;
}

interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (credentials: UserLoginDto) => Promise<void>;
  logout: () => void;
  signUp: (userData: UserCreateDto) => Promise<void>;
  sendVerificationCode: (email: string) => Promise<void>;
  confirmCode: (code: string, email: string) => Promise<boolean>;
  isAuthenticated: boolean;
  isAdmin: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

const parseUserFromToken = (token: string): User => {
  const decoded = jwtDecode<DecodedToken>(token);
  return {
    id: decoded.UserId,
    firstName: decoded.FirstName,
    lastName: decoded.LastName,
    phone: decoded.PhoneNumber,
    username: decoded.unique_name,
    email: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
    role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
  };
};

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
  const initAuth = () => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      try {
        const userFromToken = parseUserFromToken(token);
        setUser(userFromToken);
      } catch (error) {
        console.error('Invalid token:', error);
        localStorage.clear();
      }
    }
    setLoading(false);
  };

  initAuth();
}, []);

useEffect(() => {
  console.log({ user, isAuthenticated, isAdmin });
  console.log(user?.role);
}, [user]);


  const login = async (credentials: UserLoginDto) => {
    const response: LoginResponse = await apiService.login(credentials);

    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('refreshToken', response.refreshToken);

    const userFromToken = parseUserFromToken(response.accessToken);
    setUser(userFromToken);
  };

  const logout = () => {
    apiService.logout(); // optional backend call
    localStorage.clear();
    setUser(null);
  };

  const signUp = async (userData: UserCreateDto) => {
    await apiService.signUp(userData);
  };

  const sendVerificationCode = async (email: string) => {
    await apiService.sendCode(email);
  };

  const confirmCode = async (code: string, email: string): Promise<boolean> => {
    try {
      const result = await apiService.confirmCode(code, email);
      return result === true || result?.success === true || result?.result === true;
    } catch (error) {
      console.error('Confirm code error:', error);
      return false;
    }
  };

  const isAuthenticated = !!user;
  
  const isAdmin = user?.role === 'Admin' || 'SuperAdmin';

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        login,
        logout,
        signUp,
        sendVerificationCode,
        confirmCode,
        isAuthenticated,
        isAdmin,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
