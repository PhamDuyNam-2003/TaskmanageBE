# Quick Reference Guide - API Endpoints

## SubTask Management Endpoints

### Get All SubTasks for a Task
```
GET /api/tasks/{taskId}/subtasks
Authorization: Bearer {token}
```
Response: `IEnumerable<SubTaskDto>`

### Get Single SubTask
```
GET /api/tasks/subtasks/{subTaskId}
Authorization: Bearer {token}
```
Response: `SubTaskDto`

### Create SubTask
```
POST /api/tasks/{taskId}/subtasks
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Implement authentication"
}
```
Response: `SubTaskDto`

### Update SubTask
```
PUT /api/tasks/subtasks/{subTaskId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Implement JWT authentication",
  "isDone": false
}
```
Response: `SubTaskDto`

### Delete SubTask
```
DELETE /api/tasks/subtasks/{subTaskId}
Authorization: Bearer {token}
```
Response: Success message

---

## Task Management with Pagination

### Get All Tasks with Pagination
```
GET /api/tasks?pageNumber=1&pageSize=10&projectId={projectId}
Authorization: Bearer {token}
```

Query Parameters:
- `pageNumber` (optional, default: 1)
- `pageSize` (optional, default: 10)
- `projectId` (optional, filter by project)

Response:
```json
{
  "success": true,
  "data": {
    "items": [...],
    "totalCount": 100,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 10,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

---

## Project Management with Authorization

### Update Project (Owner/Manager Only)
```
PUT /api/projects/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Updated Project Name",
  "description": "Updated description"
}
```
**Authorization**: Owner or Manager role
Response: `ProjectDto` or 403 Forbidden

### Delete Project (Owner Only)
```
DELETE /api/projects/{id}
Authorization: Bearer {token}
```
**Authorization**: Only Owner
Response: Success message or 403 Forbidden

### Add Project Member (Owner/Manager Only)
```
POST /api/projects/{id}/members
Authorization: Bearer {token}
Content-Type: application/json

{
  "userId": "user-guid",
  "role": 0  // 0=Member, 1=Manager, 2=Owner
}
```
**Authorization**: Owner or Manager role
Response: Success message or 403 Forbidden

### Remove Project Member (Owner/Manager Only)
```
DELETE /api/projects/{id}/members/{memberId}
Authorization: Bearer {token}
```
**Authorization**: Owner or Manager role
**Restriction**: Cannot remove Owner
Response: Success message or 403 Forbidden

---

## Get Projects with Pagination

### Get All Projects
```
GET /api/projects?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

Query Parameters:
- `pageNumber` (optional, default: 1)
- `pageSize` (optional, default: 10)

Response:
```json
{
  "success": true,
  "data": {
    "items": [...],
    "totalCount": 50,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

---

## Error Handling

### Authorization Errors
- **403 Forbidden**: User lacks required role/permission
- Message: "Bạn không có quyền [action] [resource]"

### Validation Errors
- **400 Bad Request**: Invalid input or business rule violation
- Includes specific error message

### Not Found Errors
- **404 Not Found**: Resource doesn't exist
- Message: "[Resource] không tồn tại"

### Server Errors
- **500 Internal Server Error**: Unexpected server error

---

## Authorization Levels

| Action | Required Role | Can Bypass |
|--------|---------------|-----------|
| Update Project | Owner/Manager | - |
| Delete Project | Owner | - |
| Add Member | Owner/Manager | - |
| Remove Member | Owner/Manager | - |
| Create Task | Any Member | - |
| Update Task | Creator/Assignee | - |
| Delete Task | Creator/Owner | - |

---

## Code Examples

### C# Client Example (Create SubTask)
```csharp
var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

var dto = new CreateSubTaskDto { Title = "Review PR" };
var json = JsonSerializer.Serialize(dto);
var content = new StringContent(json, Encoding.UTF8, "application/json");

var response = await client.PostAsync(
    $"https://api.example.com/api/tasks/{taskId}/subtasks", 
    content);

var subtask = await response.Content.ReadAsAsync<SubTaskDto>();
```

### JavaScript/Fetch Example (Get Tasks with Pagination)
```javascript
const response = await fetch(
  'https://api.example.com/api/tasks?pageNumber=1&pageSize=20',
  {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  }
);

const result = await response.json();
console.log(result.data.items);
console.log(`Page ${result.data.pageNumber} of ${result.data.totalPages}`);
```

---

## Database Entities

### SubTask
- Id: Guid
- TaskItemId: Guid (Foreign Key to TaskItem)
- Title: string
- IsDone: bool
- CreatedAt: DateTime

### ProjectMember
- ProjectId: Guid
- UserId: Guid
- Role: ProjectRole (Member=0, Manager=1, Owner=2)
- JoinedAt: DateTime
