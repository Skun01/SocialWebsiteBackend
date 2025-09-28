# 🔐 AUTHORIZATION SUMMARY - Social Website Backend

## **Role Hierarchy**
```
Admin (2) > Moderator (1) > User (0)
```

## **Authorization Logic**
- **Higher roles inherit lower role permissions**
- **Admin** can access all Moderator and User endpoints
- **Moderator** can access all User endpoints
- **User** can only access User-level endpoints

---

## **📋 ENDPOINT PERMISSIONS BREAKDOWN**

### **🔓 PUBLIC ENDPOINTS (No Authentication Required)**
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/v1/auth/login` | User login |
| POST | `/v1/auth/register` | User registration |
| GET | `/v1/auth/confirm-email` | Email verification |
| POST | `/v1/auth/forgot-password` | Request password reset |
| POST | `/v1/auth/reset-password` | Reset password |
| GET | `/v1/users/{id}` | Get user by ID (public profile) |
| GET | `/v1/posts/{postId}/comments` | Get post comments |

### **👤 USER LEVEL (Requires Authentication)**
| Method | Endpoint | Description | Access |
|--------|----------|-------------|---------|
| GET | `/v1/auth/me` | Get current user info | User+ |
| GET | `/v1/auth/debug-claims` | Debug user claims | User+ |
| POST | `/v1/posts` | Create new post | User+ |
| GET | `/v1/posts/{postId}` | Get post details | User+ |
| PUT | `/v1/posts/{postId}` | Update own post | User+ |
| DELETE | `/v1/posts/{postId}` | Delete own post | User+ |
| PUT | `/v1/posts/{postId}/privacy` | Change post privacy | User+ |
| POST | `/v1/posts/{postId}/files` | Add files to post | User+ |
| DELETE | `/v1/posts/{postId}/files/{fileId}` | Remove files from post | User+ |
| POST | `/v1/posts/{postId}/likes` | Like/unlike post | User+ |
| DELETE | `/v1/posts/{postId}/likes` | Unlike post | User+ |
| POST | `/v1/posts/{postId}/comments` | Comment on post | User+ |
| POST | `/v1/users/{userId}/avatar` | Upload user avatar | User+ |
| GET | `/v1/users/search` | Search users | User+ |

### **🛡️ MODERATOR LEVEL (Moderator + Admin)**
| Method | Endpoint | Description | Access |
|--------|----------|-------------|---------|
| GET | `/v1/users` | Get all users | Moderator+ |

### **👑 ADMIN LEVEL (Admin Only)**
| Method | Endpoint | Description | Access |
|--------|----------|-------------|---------|
| POST | `/v1/users` | Create new user | Admin |
| PUT | `/v1/users/{id}` | Update any user | Admin |
| DELETE | `/v1/users/{id}` | Delete any user | Admin |
| PUT | `/v1/admin/users/{userId}/role` | Change user role | Admin |

### **🔒 AUTHENTICATED ENDPOINTS (All Logged-in Users)**
| Method | Endpoint | Description |
|--------|----------|-------------|
| **Notifications** | | |
| GET | `/v1/notifications` | Get user notifications |
| GET | `/v1/notifications/unread` | Get unread count |
| PUT | `/v1/notifications/{id}` | Mark as read |
| PUT | `/v1/notifications/read-all` | Mark all as read |
| DELETE | `/v1/notifications/{id}` | Delete notification |
| **Friendships** | | |
| GET | `/v1/friendships` | Get friends list |
| GET | `/v1/friendships/requests/received` | Get received requests |
| GET | `/v1/friendships/requests/sent` | Get sent requests |
| POST | `/v1/friendships/request/{userId}` | Send friend request |
| PUT | `/v1/friendships/accept/{userId}` | Accept friend request |
| DELETE | `/v1/friendships/decline/{userId}` | Decline friend request |
| DELETE | `/v1/friendships/{friendId}` | Remove friend |
| **Chat** | | |
| POST | `/v1/chat/conversations` | Create conversation |
| POST | `/v1/chat/conversations/{id}/messages` | Send message |
| GET | `/v1/chat/conversations/{id}/messages` | Get messages |
| GET | `/v1/chat/conversations` | Get conversations |
| **Comments** | | |
| GET | `/v1/comments/{id}/replies` | Get comment replies |
| POST | `/v1/comments/{id}/replies` | Reply to comment |
| PUT | `/v1/comments/{id}` | Update own comment |
| DELETE | `/v1/comments/{id}` | Delete own comment |
| POST | `/v1/comments/{id}/likes` | Like comment |
| DELETE | `/v1/comments/{id}/likes` | Unlike comment |

---

## **🚨 POTENTIAL ISSUES TO CHECK**

### **1. Admin Cannot Access Moderator Endpoints**
**Issue**: Admin user getting "Required minimum role: Moderator" error

**Possible Causes**:
1. JWT token missing role claim
2. Role claim value incorrect
3. Authorization handler logic error

**Debug Steps**:
1. Login as admin and call `/v1/auth/debug-claims`
2. Check if "role" claim exists and has value "Admin"
3. Verify RoleAuthorizationHandler logic

### **2. Missing Authorization on Some Endpoints**
Some endpoints might need role restrictions:
- User profile updates should only allow self or admin
- Post/comment deletion should only allow owner or moderator+

### **3. Recommended Authorization Improvements**

```csharp
// Add owner-based authorization for posts/comments
public static TBuilder RequireOwnerOrModerator<TBuilder>(this TBuilder builder)
    where TBuilder : IEndpointConventionBuilder
{
    // Custom authorization logic
}
```

---

## **🧪 TESTING AUTHORIZATION**

### **Test Admin Access**:
1. Login as admin: `POST /v1/auth/login`
2. Get token and test these endpoints:
   - `GET /v1/auth/debug-claims` (should show role: Admin)
   - `GET /v1/users` (should work - Moderator+)
   - `PUT /v1/admin/users/{id}/role` (should work - Admin only)

### **Test Role Hierarchy**:
- Admin should access all Moderator and User endpoints
- Moderator should access all User endpoints  
- User should only access User-level endpoints

---

## **📝 NOTES**

1. **Role Values**: User=0, Moderator=1, Admin=2
2. **Authorization Logic**: `userRole >= requirement.MinimumRole`
3. **JWT Claim**: Role stored as "role" claim in JWT token
4. **Default Admin**: username="admin", password="123456789tT"
