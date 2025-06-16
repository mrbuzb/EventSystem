export interface User {
  id: number;
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  phoneNumber: string;
  role: string;
}

export interface UserCreateDto {
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  password: string;
  phoneNumber: string;
}

export interface UserLoginDto {
  userName: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
}

export interface RefreshRequestDto {
  accessToken: string;
  refreshToken: string;
}

export interface Event {
  id: number;
  type: EventType;
  title: string;
  date: string;
  location: string;
  description: string;
  capacity: number;
  currentAttendees: number;
  isSubscribed?: boolean;
  createdBy: string;
  guestUsers?: GuestUser[];
}

export interface EventCreateDto {
  type: EventType;
  title: string;
  date: string;
  location: string;
  description: string;
  capasity: number; // Note: API has typo
  guestUsers?: GuestUserDto[];
}

export interface EventUpdateDto {
  id: number;
  type: EventType;
  title: string;
  date: string;
  location: string;
  description: string;
  capasity: number; // Note: API has typo
}

export interface GuestUserDto {
  email: string;
  firstName: string;
}

export interface GuestUser {
  email: string;
  firstName: string;
}

export enum EventType {
  Private = 0,
  Public = 1
}

export interface Role {
  id: number;
  name: string;
}

export interface ApiResponse<T = any> {
  success: boolean;
  data: T;
  message?: string;
}