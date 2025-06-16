import axios, { AxiosInstance, AxiosResponse } from 'axios';
import { 
  UserCreateDto, 
  UserLoginDto, 
  LoginResponse, 
  RefreshRequestDto,
  EventCreateDto,
  EventUpdateDto,
  Event,
  User,
  Role
} from '../types/api';

const API_BASE_URL = 'http://localhost:5065/'; // Update with your backend URL

class ApiService {
  private api: AxiosInstance;

  constructor() {
    this.api = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    // Request interceptor to add auth token
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('accessToken');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor to handle token refresh
    this.api.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config;

        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true;

          try {
            await this.refreshToken();
            const token = localStorage.getItem('accessToken');
            originalRequest.headers.Authorization = `Bearer ${token}`;
            return this.api(originalRequest);
          } catch (refreshError) {
            this.logout();
            window.location.href = '/login';
            return Promise.reject(refreshError);
          }
        }

        return Promise.reject(error);
      }
    );
  }

  // Auth endpoints
  async signUp(userData: UserCreateDto): Promise<any> {
    const response = await this.api.post('/api/auth/sign-up', userData);
    return response.data;
  }

  async sendCode(email: string): Promise<any> {
    const response = await this.api.post(`/api/auth/send-code?email=${encodeURIComponent(email)}`);
    return response.data;
  }

  async confirmCode(code: string, email: string): Promise<boolean> {
  const response = await this.api.post(
    `/api/auth/confirm-code?code=${encodeURIComponent(code)}&email=${encodeURIComponent(email)}`
  );
console.log('Confirm response:', response.data);

  // Backend true qaytarayapti, lekin string bo‘lishi mumkin
  return response.data === true || response.data === "true";
}


  async login(credentials: UserLoginDto): Promise<LoginResponse> {
    const response = await this.api.post('/api/auth/login', credentials);
    return response.data;
  }

  async refreshToken(): Promise<LoginResponse> {
    const accessToken = localStorage.getItem('accessToken');
    const refreshToken = localStorage.getItem('refreshToken');
    
    if (!accessToken || !refreshToken) {
      throw new Error('No tokens available');
    }

    const response = await this.api.put('/api/auth/refresh-token', {
      accessToken,
      refreshToken
    } as RefreshRequestDto);

    const newTokens = response.data;
    localStorage.setItem('accessToken', newTokens.accessToken);
    localStorage.setItem('refreshToken', newTokens.refreshToken);
    
    return newTokens;
  }

  async logout(): Promise<void> {
    const refreshToken = localStorage.getItem('refreshToken');
    if (refreshToken) {
      try {
        await this.api.delete(`/api/auth/log-out?refreshToken=${encodeURIComponent(refreshToken)}`);
      } catch (error) {
        console.error('Logout error:', error);
      }
    }
    
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  }

  // Event endpoints
  async getAllPublicEvents(): Promise<Event[]> {
    const response = await this.api.get('/api/event/get-all-public-events');
    return response.data;
  }

  async getAllEvents(): Promise<Event[]> {
    const response = await this.api.get('/api/event/get-all-events');
    return response.data;
  }

  async getSubscribedEvents(): Promise<Event[]> {
  const response = await this.api.get('/api/event/get-all-subscribed-events');
  return response.data;
}


  async getAllGuestedEvents(): Promise<Event[]> {
    const response = await this.api.get('/api/event/get-all-guested-events');
    return response.data;
  }

  async getEventById(eventId: number): Promise<Event> {
    const response = await this.api.get(`/api/event/get-event-by-id?eventId=${eventId}`);
    return response.data;
  }

  async addEvent(eventData: EventCreateDto): Promise<any> {
    const response = await this.api.post('/api/event/add-event', eventData);
    return response.data;
  }

  async updateEvent(eventData: EventUpdateDto): Promise<any> {
    const response = await this.api.put('/api/event/update-event', eventData);
    return response.data;
  }

  async deleteEvent(eventId: number): Promise<any> {
    const response = await this.api.delete(`/api/event/delete-event?eventId=${eventId}`);
    return response.data;
  }

  async subscribeEvent(eventId: number): Promise<any> {
    const response = await this.api.post(`/api/event/subscribe-event?eventId=${eventId}`);
    return response.data;
  }

  async unsubscribeEvent(eventId: number): Promise<any> {
    const response = await this.api.delete(`/api/event/unsubscribe-event?eventId=${eventId}`);
    return response.data;
  }

  // Admin endpoints
  async getAllUsersByRole(role: string): Promise<User[]> {
    const response = await this.api.get(`/api/admin/get-all-users-by-role?role=${encodeURIComponent(role)}`);
    return response.data;
  }

  async deleteUser(userId: number): Promise<any> {
    const response = await this.api.delete(`/api/admin/delete?userId=${userId}`);
    return response.data;
  }

  async updateUserRole(userId: number, userRole: string): Promise<any> {
    const response = await this.api.patch(`/api/admin/updateRole?userId=${userId}&userRole=${encodeURIComponent(userRole)}`);
    return response.data;
  }

  // Role endpoints
  async getAllRoles(): Promise<Role[]> {
    const response = await this.api.get('/api/role/get-all-roles');
    return response.data;
  }
}

export const apiService = new ApiService();