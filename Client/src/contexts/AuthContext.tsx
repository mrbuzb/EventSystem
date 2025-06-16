import React, { createContext, useContext, useEffect, useState, ReactNode } from 'react';
import { User, LoginResponse, UserLoginDto, UserCreateDto } from '../types/api';
import { apiService } from '../services/api';
import { jwtDecode } from 'jwt-decode';

interface DecodedToken {
  UserId: string;
  FirstName: string;
  LastName: string;
  role: string;
  // qo‘shimcha kerakli maydonlar
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

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

interface DecodedToken {
  UserId: string;
  FirstName: string;
  LastName: string;
  PhoneNumber: string;
  unique_name: string;
  role: string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": string;
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
    role: decoded.role,
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
          console.error('Error parsing token:', error);
          localStorage.clear();
        }
      }

      setLoading(false);
    };

    initAuth();
  }, []);

  const login = async (credentials: UserLoginDto) => {
    try {
      const response: LoginResponse = await apiService.login(credentials);

      localStorage.setItem('accessToken', response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);

      const userFromToken = parseUserFromToken(response.accessToken);
      localStorage.setItem('user', JSON.stringify(userFromToken));
      setUser(userFromToken);
    } catch (error) {
      throw error;
    }
  };

  const logout = () => {
    apiService.logout();
    localStorage.clear();
    setUser(null);
  };

  const signUp = async (userData: UserCreateDto) => {
    try {
      await apiService.signUp(userData);
    } catch (error) {
      throw error;
    }
  };

  const sendVerificationCode = async (email: string) => {
    try {
      await apiService.sendCode(email);
    } catch (error) {
      throw error;
    }
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
  const isAdmin = user?.role === 'Admin';

  const value: AuthContextType = {
    user,
    loading,
    login,
    logout,
    signUp,
    sendVerificationCode,
    confirmCode,
    isAuthenticated,
    isAdmin,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
