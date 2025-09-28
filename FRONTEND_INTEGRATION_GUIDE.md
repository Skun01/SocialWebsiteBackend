# Social Website Backend - Frontend Integration Guide

## 📋 Table of Contents
1. [Overview](#overview)
2. [Base Configuration](#base-configuration)
3. [Authentication System](#authentication-system)
4. [API Endpoints](#api-endpoints)
5. [Data Models](#data-models)
6. [SignalR Real-time Features](#signalr-real-time-features)
7. [Error Handling](#error-handling)
8. [File Upload Guidelines](#file-upload-guidelines)
9. [Frontend Implementation Examples](#frontend-implementation-examples)

## 🎯 Overview

This is a comprehensive guide for frontend developers to integrate with the Social Website Backend API. The backend is built with .NET 9, uses JWT authentication, role-based authorization, and SignalR for real-time features.

### Key Features
- **Authentication**: JWT-based with email verification
- **Authorization**: Role-based (User, Moderator, Admin)
- **Real-time**: SignalR for chat and notifications
- **File Upload**: Support for images and files
- **Social Features**: Posts, comments, likes, friendships
- **Chat System**: Private conversations with real-time messaging
- **Notifications**: Real-time notification system

## ⚙️ Base Configuration

### API Base URL
```
Development: http://localhost:5000
Production: [Your production URL]
```

### API Version
All endpoints are prefixed with `/v1`

### CORS Configuration
The backend accepts requests from:
- `http://localhost:5173` (Vite default)
- `http://localhost:3000` (React default)

### Content Types
- **JSON**: `application/json`
- **Form Data**: `multipart/form-data` (for file uploads)

## 🔐 Authentication System

### JWT Token Structure
The backend uses JWT tokens with the following claims:
- `sub` (NameIdentifier): User ID
- `email`: User email
- `role`: User role (User, Moderator, Admin)
- `exp`: Expiration time

### Authentication Flow
1. **Register** → Email verification required
2. **Login** → Receive JWT token
3. **Include token** in Authorization header: `Bearer {token}`

### Token Storage Recommendation
Store JWT token securely (localStorage, sessionStorage, or httpOnly cookies)

## 🚀 API Endpoints

### 1. Authentication Endpoints (`/v1/auth`)

#### POST `/v1/auth/register`
Register a new user account.

**Request Body:**
```json
{
  "userName": "johndoe",
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "1990-01-01T00:00:00Z",
  "gender": "Male",
  "profilePictureUrl": "https://example.com/avatar.jpg"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Registration successful, please check email to verify",
  "data": null,
  "traceId": "trace-id"
}
```

#### POST `/v1/auth/login`
Login with email and password.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "traceId": "trace-id"
}
```

#### GET `/v1/auth/confirm-email?token={token}`
Verify email address with token from email.

#### GET `/v1/auth/me`
Get current user information (requires authentication).

**Headers:**
```
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "user-guid",
    "userName": "johndoe",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "dateOfBirth": "1990-01-01T00:00:00Z",
    "gender": "Male",
    "profilePictureUrl": "http://localhost:5000/images/avatar.jpg",
    "isEmailVerified": true,
    "role": "User"
  }
}
```

#### POST `/v1/auth/forgot-password`
Request password reset email.

**Request Body:**
```json
{
  "email": "user@example.com"
}
```

#### POST `/v1/auth/reset-password`
Reset password with token from email.

**Request Body:**
```json
{
  "publicId": "user-guid",
  "token": "reset-token",
  "newPassword": "NewSecurePassword123!"
}
```

### 2. User Management (`/v1/users`)

#### GET `/v1/users/search`
Search for users.

**Query Parameters:**
- `name`: Search term for user name
- `sortBy`: Sort field (default: "name")
- `pageNumber`: Page number
- `pageSize`: Items per page (default: 10, max: 30)

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "user-guid",
        "userName": "johndoe",
        "firstName": "John",
        "lastName": "Doe",
        "email": "john@example.com",
        "dateOfBirth": "1990-01-01T00:00:00Z",
        "gender": "Male",
        "profilePictureUrl": "http://localhost:5000/images/avatar.jpg",
        "isEmailVerified": true,
        "role": "User"
      }
    ],
    "totalCount": 1,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

#### POST `/v1/users/{userId}/avatar`
Upload user avatar (requires authentication).

**Headers:**
```
Authorization: Bearer {token}
Content-Type: multipart/form-data
```

**Form Data:**
- `file`: Image file (JPG, PNG, GIF)

**Response:**
```json
{
  "success": true,
  "message": "Avatar uploaded successfully",
  "data": {
    "url": "http://localhost:5000/images/new-avatar.jpg"
  }
}
```

#### GET `/v1/users/{id}`
Get user by ID.

#### GET `/v1/users/{userId}/posts`
Get posts by specific user.

**Query Parameters:**
- `cursor`: Cursor for pagination
- `pageSize`: Number of posts (default: 10)

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "post-guid",
        "authorId": "user-guid",
        "authorName": "John Doe",
        "authorAvatar": "http://localhost:5000/images/avatar.jpg",
        "content": "This is a post content",
        "privacy": "Public",
        "likeCount": 5,
        "isLikedByMe": false,
        "commentCount": 3,
        "createdAt": "2024-01-01T00:00:00Z",
        "updatedAt": "2024-01-01T00:00:00Z",
        "files": []
      }
    ],
    "hasNextPage": true,
    "nextCursor": "next-cursor-value"
  }
}
```

#### GET `/v1/users` (Moderator+)
Get all users.

#### POST `/v1/users` (Admin only)
Create new user.

#### PUT `/v1/users/{id}` (Admin only)
Update user.

#### DELETE `/v1/users/{id}` (Admin only)
Delete user.

### 3. Posts (`/v1/posts`)

#### GET `/v1/posts`
Get posts with pagination.

**Query Parameters:**
- `cursor`: Cursor for pagination
- `pageSize`: Number of posts (default: 10)

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "post-guid",
        "authorId": "user-guid",
        "authorName": "John Doe",
        "authorAvatar": "http://localhost:5000/images/avatar.jpg",
        "content": "This is a post content",
        "privacy": "Public",
        "likeCount": 5,
        "isLikedByMe": false,
        "commentCount": 3,
        "createdAt": "2024-01-01T00:00:00Z",
        "updatedAt": "2024-01-01T00:00:00Z",
        "files": [
          {
            "id": "file-guid",
            "fileName": "image.jpg",
            "fileUrl": "http://localhost:5000/PostAsset/image.jpg",
            "assetType": "Image"
          }
        ]
      }
    ],
    "hasNextPage": true,
    "nextCursor": "next-cursor-value"
  }
}
```

#### POST `/v1/posts`
Create a new post (requires authentication).

**Headers:**
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "content": "This is my new post!",
  "privacy": "Public"
}
```

**Privacy Options:**
- `Public`: Visible to everyone
- `Friends`: Visible to friends only
- `OnlyMe`: Visible to author only

#### GET `/v1/posts/{postId}`
Get specific post by ID.

#### PUT `/v1/posts/{postId}`
Update post (requires authentication, author only).

#### DELETE `/v1/posts/{postId}`
Delete post (requires authentication, author only).

#### PUT `/v1/posts/{postId}/privacy`
Change post privacy.

**Request Body:**
```json
{
  "privacy": "Friends"
}
```

**Available Privacy Values:**
- `Public` (0)
- `Friends` (1) 
- `OnlyMe` (2)

#### POST `/v1/posts/{postId}/files`
Add files to post.

**Headers:**
```
Content-Type: multipart/form-data
```

**Form Data:**
- `files`: Multiple files

#### DELETE `/v1/posts/{postId}/files/{fileId}`
Remove file from post.

#### POST `/v1/posts/{postId}/likes`
Like a post (requires authentication).

#### DELETE `/v1/posts/{postId}/likes`
Unlike a post (requires authentication).

#### GET `/v1/posts/{postId}/comments`
Get post comments.

#### POST `/v1/posts/{postId}/comments`
Add comment to post (requires authentication).

**Request Body:**
```json
{
  "content": "This is a comment"
}
```

### 4. Comments (`/v1/comments`)

#### GET `/v1/comments/{commentId}/replies`
Get replies to a comment.

#### POST `/v1/comments/{commentId}/replies`
Reply to a comment (requires authentication).

**Request Body:**
```json
{
  "content": "This is a reply"
}
```

#### PUT `/v1/comments/{commentId}`
Update comment (requires authentication, author only).

**Request Body:**
```json
{
  "content": "Updated comment content"
}
```

#### DELETE `/v1/comments/{commentId}`
Delete comment (requires authentication, author only).

#### POST `/v1/comments/{commentId}/likes`
Like a comment (requires authentication).

#### DELETE `/v1/comments/{commentId}/likes`
Unlike a comment (requires authentication).

### 5. Chat System (`/v1/chat`)

#### GET `/v1/chat/conversations`
Get user's conversations (requires authentication).

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": "conversation-guid",
      "type": "Private",
      "participants": [
        {
          "id": "user-guid",
          "firstName": "John",
          "lastName": "Doe",
          "avatarUrl": "http://localhost:5000/images/avatar.jpg"
        }
      ],
      "lastMessage": {
        "id": "message-guid",
        "content": "Hello there!",
        "sentAt": "2024-01-01T00:00:00Z",
        "sender": {
          "id": "user-guid",
          "firstName": "John",
          "lastName": "Doe"
        }
      },
      "unreadCount": 2
    }
  ]
}
```

#### POST `/v1/chat/conversations`
Create new conversation (requires authentication).

**Request Body:**
```json
{
  "participantIds": ["user-guid-1", "user-guid-2"],
  "type": "Private"
}
```

#### GET `/v1/chat/conversations/{conversationId}/messages`
Get conversation messages (requires authentication).

**Query Parameters:**
- `cursor`: Cursor for pagination
- `pageSize`: Number of messages

#### POST `/v1/chat/conversations/{conversationId}/messages`
Send message (requires authentication).

**Request Body:**
```json
{
  "content": "Hello, how are you?"
}
```

### 6. Friendships (`/v1/friendships`)

#### GET `/v1/friendships`
Get user's friends list (requires authentication).

#### GET `/v1/friendships/requests/received`
Get received friend requests (requires authentication).

#### GET `/v1/friendships/requests/sent`
Get sent friend requests (requires authentication).

#### POST `/v1/friendships/request/{receiverId}`
Send friend request (requires authentication).

#### PUT `/v1/friendships/accept/{senderId}`
Accept friend request (requires authentication).

#### DELETE `/v1/friendships/decline/{senderId}`
Decline friend request (requires authentication).

#### DELETE `/v1/friendships/{friendId}`
Remove friend (requires authentication).

#### GET `/v1/friendships/status/{userId}`
Get friendship status between current user and target user (requires authentication).

**Response:**
```json
{
  "success": true,
  "data": {
    "status": "Accepted",
    "message": "You are friends"
  }
}
```

**Status Values:**
- `Pending` (0): Friend request is pending
- `Accepted` (1): Users are friends
- `Rejected` (2): No friendship relationship or declined

### 7. Notifications (`/v1/notifications`)

#### GET `/v1/notifications`
Get user notifications (requires authentication).

**Query Parameters:**
- `cursor`: Cursor for pagination
- `pageSize`: Number of notifications

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "notification-guid",
        "type": "Like",
        "message": "John Doe liked your post",
        "isRead": false,
        "createdAt": "2024-01-01T00:00:00Z",
        "relatedEntityId": "post-guid",
        "actor": {
          "id": "user-guid",
          "firstName": "John",
          "lastName": "Doe",
          "avatarUrl": "http://localhost:5000/images/avatar.jpg"
        }
      }
    ],
    "hasNextPage": true,
    "nextCursor": "next-cursor"
  }
}
```

#### GET `/v1/notifications/unread`
Get unread notification count (requires authentication).

#### PUT `/v1/notifications/{notificationId}`
Mark notification as read (requires authentication).

#### PUT `/v1/notifications/read-all`
Mark all notifications as read (requires authentication).

#### DELETE `/v1/notifications/{notificationId}`
Delete notification (requires authentication).

### 8. Admin Endpoints (`/v1/admin`)

#### PUT `/v1/admin/users/{userId}/role` (Admin only)
Set user role.

**Request Body:**
```json
{
  "role": "Moderator"
}
```

**Available Roles:**
- `User` (0): Regular user
- `Moderator` (1): Can moderate content
- `Admin` (2): Full administrative access

**Available Gender Values:**
- `Unknown` (0): Not specified
- `Male` (1): Male
- `Female` (2): Female
- `Other` (3): Other

## 📊 Data Models

### User Model
```typescript
interface User {
  id: string;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  dateOfBirth?: string;
  gender: 'Unknown' | 'Male' | 'Female' | 'Other';
  profilePictureUrl?: string;
  isEmailVerified: boolean;
  role: 'User' | 'Moderator' | 'Admin';
}
```

### Post Model
```typescript
interface Post {
  id: string;
  authorId: string;
  authorName: string;
  authorAvatar: string;
  content: string;
  privacy: 'Public' | 'Friends' | 'OnlyMe';
  likeCount: number;
  isLikedByMe: boolean;
  commentCount: number;
  files: PostFile[];
  createdAt: string;
  updatedAt: string;
}

interface PostFile {
  id: string;
  fileName: string;
  fileUrl: string;
  assetType: 'Image' | 'Video' | 'Document';
}
```

### Comment Model
```typescript
interface Comment {
  id: string;
  content: string;
  createdAt: string;
  updatedAt: string;
  likesCount: number;
  isLikedByCurrentUser: boolean;
  author: User;
  replies?: Comment[];
  parentCommentId?: string;
}
```

### Conversation Model
```typescript
interface Conversation {
  id: string;
  type: 'Private' | 'Group';
  participants: User[];
  lastMessage?: Message;
  unreadCount: number;
  createdAt: string;
}

interface Message {
  id: string;
  content: string;
  sentAt: string;
  sender: User;
  conversationId: string;
}
```

### Notification Model
```typescript
interface Notification {
  id: string;
  type: 'Like' | 'Comment' | 'FriendRequest' | 'Message';
  message: string;
  isRead: boolean;
  createdAt: string;
  relatedEntityId?: string;
  actor: User;
}
```

### Friendship Model
```typescript
interface Friendship {
  id: string;
  requester: User;
  receiver: User;
  status: 'Pending' | 'Accepted' | 'Declined';
  createdAt: string;
  updatedAt: string;
}
```

## 🔄 SignalR Real-time Features

### Connection Setup
```javascript
import * as signalR from "@microsoft/signalr";

// Chat Hub
const chatConnection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/chatHub", {
    accessTokenFactory: () => localStorage.getItem("token")
  })
  .build();

// Notification Hub
const notificationConnection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/notificationHub", {
    accessTokenFactory: () => localStorage.getItem("token")
  })
  .build();

// Feed Hub (Real-time Post Updates)
const feedConnection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/feedHub", {
    accessTokenFactory: () => localStorage.getItem("token")
  })
  .build();
```

### Chat Hub Events

#### Receiving Messages
```javascript
chatConnection.on("ReceiveMessage", (message) => {
  console.log("New message:", message);
  // Update UI with new message
});
```

#### Message Model for SignalR
```typescript
interface SignalRMessage {
  id: string;
  content: string;
  sentAt: string;
  conversationId: string;
  sender: {
    id: string;
    firstName: string;
    lastName: string;
    avatarUrl?: string;
  };
}
```

### Notification Hub Events

#### Receiving Notifications
```javascript
notificationConnection.on("ReceiveNotification", (notification) => {
  console.log("New notification:", notification);
  // Update notification count and list
});
```

### Feed Hub Events

#### Receiving Real-time Post Updates
```javascript
// New post created
feedConnection.on("NewPost", (post) => {
  console.log("New post:", post);
  // Add new post to feed UI
});

// Post updated
feedConnection.on("PostUpdated", (post) => {
  console.log("Post updated:", post);
  // Update post in feed UI
});

// Post deleted
feedConnection.on("PostDeleted", (data) => {
  console.log("Post deleted:", data.postId);
  // Remove post from feed UI
});
```

#### Connection Management
```javascript
// Start connections
await chatConnection.start();
await notificationConnection.start();
await feedConnection.start();

// Handle disconnection
chatConnection.onclose(() => {
  console.log("Chat connection closed");
  // Implement reconnection logic
});

feedConnection.onclose(() => {
  console.log("Feed connection closed");
  // Implement reconnection logic
});
```

## ❌ Error Handling

### Standard Error Response
```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    {
      "field": "Email",
      "message": "Email is required"
    }
  ],
  "traceId": "trace-id"
}
```

### HTTP Status Codes
- `200`: Success
- `400`: Bad Request (validation errors)
- `401`: Unauthorized (authentication required)
- `403`: Forbidden (insufficient permissions)
- `404`: Not Found
- `500`: Internal Server Error

### Common Error Scenarios

#### Authentication Errors
```json
{
  "success": false,
  "message": "User not authenticated",
  "traceId": "trace-id"
}
```

#### Validation Errors
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    {
      "field": "Email",
      "message": "Email is required"
    },
    {
      "field": "Password",
      "message": "Password must be at least 8 characters"
    }
  ],
  "traceId": "trace-id"
}
```

#### Permission Errors
```json
{
  "success": false,
  "message": "Insufficient permissions",
  "traceId": "trace-id"
}
```

## 📁 File Upload Guidelines

### Supported File Types
- **Images**: JPG, JPEG, PNG, GIF, BMP, WEBP
- **Documents**: PDF, DOC, DOCX, TXT
- **Videos**: MP4, AVI, MOV (if implemented)

### File Size Limits
- **Avatar**: Max 5MB
- **Post Files**: Max 10MB per file
- **Multiple Files**: Max 50MB total per post

### Upload Implementation
```javascript
// Avatar upload
const uploadAvatar = async (userId, file) => {
  const formData = new FormData();
  formData.append('file', file);
  
  const response = await fetch(`/v1/users/${userId}/avatar`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`
    },
    body: formData
  });
  
  return response.json();
};

// Post files upload
const uploadPostFiles = async (postId, files) => {
  const formData = new FormData();
  files.forEach(file => formData.append('files', file));
  
  const response = await fetch(`/v1/posts/${postId}/files`, {
    method: 'POST',
    body: formData
  });
  
  return response.json();
};
```

## 💻 Frontend Implementation Examples

### Authentication Service
```typescript
class AuthService {
  private baseUrl = 'http://localhost:5000/v1/auth';
  
  async login(email: string, password: string) {
    const response = await fetch(`${this.baseUrl}/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ email, password })
    });
    
    const data = await response.json();
    
    if (data.success) {
      localStorage.setItem('token', data.data.accessToken);
    }
    
    return data;
  }
  
  async getCurrentUser() {
    const token = localStorage.getItem('token');
    
    const response = await fetch(`${this.baseUrl}/me`, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });
    
    return response.json();
  }
  
  logout() {
    localStorage.removeItem('token');
  }
}
```

### API Client with Interceptors
```typescript
class ApiClient {
  private baseUrl = 'http://localhost:5000/v1';
  
  private async request(endpoint: string, options: RequestInit = {}) {
    const token = localStorage.getItem('token');
    
    const config: RequestInit = {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` }),
        ...options.headers
      }
    };
    
    const response = await fetch(`${this.baseUrl}${endpoint}`, config);
    
    if (response.status === 401) {
      // Handle token expiration
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    
    return response.json();
  }
  
  get(endpoint: string) {
    return this.request(endpoint);
  }
  
  post(endpoint: string, data: any) {
    return this.request(endpoint, {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }
  
  put(endpoint: string, data: any) {
    return this.request(endpoint, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  }
  
  delete(endpoint: string) {
    return this.request(endpoint, {
      method: 'DELETE'
    });
  }
}
```

### React Hook for Posts
```typescript
import { useState, useEffect } from 'react';

const usePosts = () => {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(false);
  const [cursor, setCursor] = useState(null);
  const [hasMore, setHasMore] = useState(true);
  
  const apiClient = new ApiClient();
  
  const loadPosts = async (reset = false) => {
    setLoading(true);
    
    try {
      const params = new URLSearchParams();
      if (cursor && !reset) params.append('cursor', cursor);
      params.append('limit', '10');
      
      const response = await apiClient.get(`/posts?${params}`);
      
      if (response.success) {
        const newPosts = response.data.items;
        setPosts(prev => reset ? newPosts : [...prev, ...newPosts]);
        setCursor(response.data.nextCursor);
        setHasMore(response.data.hasNextPage);
      }
    } catch (error) {
      console.error('Error loading posts:', error);
    } finally {
      setLoading(false);
    }
  };
  
  const createPost = async (content: string, privacy: string) => {
    try {
      const response = await apiClient.post('/posts', { content, privacy });
      
      if (response.success) {
        // Refresh posts
        loadPosts(true);
      }
      
      return response;
    } catch (error) {
      console.error('Error creating post:', error);
      throw error;
    }
  };
  
  const likePost = async (postId: string) => {
    try {
      const response = await apiClient.post(`/posts/${postId}/likes`);
      
      if (response.success) {
        // Update post in state
        setPosts(prev => prev.map(post => 
          post.id === postId 
            ? { ...post, isLikedByMe: true, likeCount: post.likeCount + 1 }
            : post
        ));
      }
      
      return response;
    } catch (error) {
      console.error('Error liking post:', error);
    }
  };
  
  useEffect(() => {
    loadPosts(true);
  }, []);
  
  return {
    posts,
    loading,
    hasMore,
    loadPosts,
    createPost,
    likePost
  };
};
```

### SignalR Chat Implementation
```typescript
import * as signalR from "@microsoft/signalr";

class ChatService {
  private connection: signalR.HubConnection;
  private messageHandlers: ((message: any) => void)[] = [];
  
  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/chatHub", {
        accessTokenFactory: () => localStorage.getItem("token") || ""
      })
      .withAutomaticReconnect()
      .build();
    
    this.setupEventHandlers();
  }
  
  private setupEventHandlers() {
    this.connection.on("ReceiveMessage", (message) => {
      this.messageHandlers.forEach(handler => handler(message));
    });
  }
  
  async start() {
    try {
      await this.connection.start();
      console.log("Chat connection started");
    } catch (error) {
      console.error("Error starting chat connection:", error);
    }
  }
  
  async stop() {
    await this.connection.stop();
  }
  
  onMessage(handler: (message: any) => void) {
    this.messageHandlers.push(handler);
  }
  
  removeMessageHandler(handler: (message: any) => void) {
    const index = this.messageHandlers.indexOf(handler);
    if (index > -1) {
      this.messageHandlers.splice(index, 1);
    }
  }
}
```

## 🔧 Development Tips

### 1. Environment Variables
Create environment variables for different stages:
```javascript
const config = {
  development: {
    apiUrl: 'http://localhost:5000/v1',
    signalRUrl: 'http://localhost:5000'
  },
  production: {
    apiUrl: 'https://your-api.com/v1',
    signalRUrl: 'https://your-api.com'
  }
};
```

### 2. Error Boundary
Implement error boundaries for better error handling:
```typescript
class ApiErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false };
  }
  
  static getDerivedStateFromError(error) {
    return { hasError: true };
  }
  
  componentDidCatch(error, errorInfo) {
    console.error('API Error:', error, errorInfo);
  }
  
  render() {
    if (this.state.hasError) {
      return <div>Something went wrong with the API.</div>;
    }
    
    return this.props.children;
  }
}
```

### 3. Loading States
Implement proper loading states for better UX:
```typescript
const LoadingSpinner = () => (
  <div className="loading-spinner">
    <div className="spinner"></div>
    <p>Loading...</p>
  </div>
);
```

### 4. Pagination Helper
```typescript
const usePagination = (fetchFunction) => {
  const [items, setItems] = useState([]);
  const [cursor, setCursor] = useState(null);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);
  
  const loadMore = async () => {
    if (loading || !hasMore) return;
    
    setLoading(true);
    try {
      const response = await fetchFunction(cursor);
      if (response.success) {
        setItems(prev => [...prev, ...response.data.items]);
        setCursor(response.data.nextCursor);
        setHasMore(response.data.hasNextPage);
      }
    } catch (error) {
      console.error('Pagination error:', error);
    } finally {
      setLoading(false);
    }
  };
  
  return { items, loadMore, hasMore, loading };
};
```

## 🚀 Getting Started Checklist

### Backend Setup
- [ ] Backend is running on `http://localhost:5000`
- [ ] Database is configured and migrations applied
- [ ] CORS is configured for your frontend URL
- [ ] JWT configuration is set up

### Frontend Implementation
- [ ] Install SignalR client: `npm install @microsoft/signalr`
- [ ] Set up API client with authentication
- [ ] Implement authentication flow
- [ ] Set up SignalR connections
- [ ] Implement error handling
- [ ] Add loading states
- [ ] Test file upload functionality

### Testing
- [ ] Test authentication flow
- [ ] Test CRUD operations for posts
- [ ] Test real-time chat functionality
- [ ] Test notification system
- [ ] Test file upload
- [ ] Test error scenarios

## 📞 Support

If you encounter any issues or need clarification on any endpoint, please refer to:
1. The Swagger documentation at `http://localhost:5000/swagger` (development only)
2. Check the backend logs for detailed error information
3. Verify JWT token is valid and not expired
4. Ensure proper CORS configuration

---

**Last Updated:** 2024-01-01  
**API Version:** v1  
**Backend Framework:** .NET 9  
**Database:** SQL Server
