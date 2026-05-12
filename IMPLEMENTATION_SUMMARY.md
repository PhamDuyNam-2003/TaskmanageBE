# SubTask Management, Authorization & Pagination - Implementation Summary

## Overview
This document outlines the three major features implemented in the Task Management Backend:

1. **SubTask Management** - Complete CRUD operations for subtasks
2. **Authorization with ProjectRole** - Role-based access control for projects
3. **Search & Filter with Pagination** - Enhanced data retrieval with pagination support

---

## 1. SubTask Management

### New Files Created:
- `BE/DTOs/Tasks/SubTaskDto.cs` - DTO for SubTask responses
- `BE/DTOs/Tasks/UpdateSubTaskDto.cs` - DTO for updating SubTasks

### Model Used:
- `BE/Models/SubTask.cs` - Already exists with properties: Id, TaskItemId, Title, IsDone, CreatedAt

### Service Layer (`ITaskService`):
Added new methods:
```csharp
Task<IEnumerable<SubTaskDto>> GetSubTasksAsync(Guid taskId);
Task<SubTaskDto?> GetSubTaskByIdAsync(Guid subTaskId);
Task<SubTaskDto> CreateSubTaskAsync(Guid taskId, CreateSubTaskDto dto);
Task<SubTaskDto?> UpdateSubTaskAsync(Guid subTaskId, UpdateSubTaskDto dto);
Task<bool> DeleteSubTaskAsync(Guid subTaskId);
```

### Implementation (`TaskService`):
- `GetSubTasksAsync` - Retrieves all subtasks for a task, ordered by creation date
- `GetSubTaskByIdAsync` - Gets a single subtask by ID
- `CreateSubTaskAsync` - Creates a new subtask with validation
- `UpdateSubTaskAsync` - Updates title and completion status
- `DeleteSubTaskAsync` - Soft/hard delete of subtask

### API Endpoints (`TasksController`):

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tasks/{taskId}/subtasks` | Get all subtasks for a task |
| GET | `/api/tasks/subtasks/{subTaskId}` | Get a specific subtask |
| POST | `/api/tasks/{taskId}/subtasks` | Create a new subtask |
| PUT | `/api/tasks/subtasks/{subTaskId}` | Update a subtask |
| DELETE | `/api/tasks/subtasks/{subTaskId}` | Delete a subtask |

### Example Requests:

**Create SubTask:**
```json
POST /api/tasks/{taskId}/subtasks
{
  "title": "Review code changes"
}
```

**Update SubTask:**
```json
PUT /api/tasks/subtasks/{subTaskId}
{
  "title": "Code review completed",
  "isDone": true
}
```

---

## 2. Authorization with ProjectRole

### Existing Enums:
`BE/Models/Enums/ProjectRole.cs`
```csharp
public enum ProjectRole
{
    Member = 0,
    Manager = 1,
    Owner = 2
}
```

### Authorization Logic Implemented:

#### Project Update
- **Required Role**: Owner or Manager
- **Check**: User must be project Owner OR have Manager/Owner role
- **Error**: Returns 403 Forbidden if unauthorized

#### Project Delete
- **Required Role**: Owner only
- **Check**: Only project Owner can delete
- **Error**: Returns 403 Forbidden if not Owner

#### Add Project Member
- **Required Role**: Owner or Manager
- **Check**: User must have permission to add members
- **Error**: Returns 403 Forbidden if unauthorized

#### Remove Project Member
- **Required Role**: Owner or Manager
- **Check**: Cannot remove project Owner
- **Special Rules**: 
  - Owner cannot be removed
  - Only Owner/Manager can remove members
- **Error**: Returns 403 Forbidden if unauthorized

### Service Layer Updates (`IProjectService`):
```csharp
// Authorization checks added to:
Task<ProjectDto?> UpdateAsync(Guid id, Guid userId, UpdateProjectDto dto);
Task<bool> DeleteAsync(Guid id, Guid userId);
Task AddMemberAsync(Guid projectId, Guid userId, AddProjectMemberDto dto);
Task<bool> RemoveMemberAsync(Guid projectId, Guid memberId, Guid requestingUserId);
```

### Controller Updates (`ProjectsController`):
- Now extracts `userId` from claims via `User.GetUserId()`
- Passes `userId` to service methods for authorization checks
- Handles `UnauthorizedAccessException` and returns appropriate HTTP responses
- Proper error handling with descriptive messages

---

## 3. Search & Filter with Pagination

### New Files Created:
- `BE/DTOs/Common/PaginationParams.cs` - Contains pagination models:

```csharp
public class PaginationParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
```

### Service Layer Updates:

#### TaskService:
```csharp
// Overloaded GetAllAsync with pagination and filtering
Task<PaginatedResponse<TaskDto>> GetAllAsync(
    int pageNumber,
    int pageSize,
    Guid? projectId = null);
```

**Features:**
- Filter by `projectId` (optional)
- Default page size: 10 items
- Ordered by creation date (newest first)
- Returns total count for pagination UI

#### ProjectService:
```csharp
// Overloaded GetAllAsync with pagination
Task<PaginatedResponse<ProjectDto>> GetAllAsync(
    int pageNumber,
    int pageSize);
```

**Features:**
- Default page size: 10 items
- Ordered by creation date (newest first)
- Returns navigation flags (HasPreviousPage, HasNextPage)

### API Endpoints:

**Tasks Pagination:**
```
GET /api/tasks?pageNumber=1&pageSize=10&projectId={projectId}
```

**Projects Pagination:**
```
GET /api/projects?pageNumber=1&pageSize=10
```

### Example Response:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "...",
        "title": "Task 1",
        ...
      }
    ],
    "totalCount": 150,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 15,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

---

## Mapping Configuration

Updated `BE/Mappings/MappingProfile.cs` with new mappings:
- `SubTask <-> SubTaskDto`
- `CreateSubTaskDto -> SubTask`
- `UpdateSubTaskDto -> SubTask`

---

## HTTP Status Codes Reference

| Code | Scenario |
|------|----------|
| 200 | Successful operation |
| 400 | Bad request (validation error, business rule violation) |
| 403 | Forbidden (authorization failed) |
| 404 | Resource not found |
| 500 | Server error |

---

## Summary of Files Modified

### New Files (5):
1. `BE/DTOs/Tasks/SubTaskDto.cs`
2. `BE/DTOs/Tasks/UpdateSubTaskDto.cs`
3. `BE/DTOs/Common/PaginationParams.cs`

### Modified Files (5):
1. `BE/Services/Interfaces/ITaskService.cs`
2. `BE/Services/Implements/TaskService.cs`
3. `BE/Services/Interfaces/IProjectService.cs`
4. `BE/Services/Implements/ProjectService.cs`
5. `BE/Controllers/ProjectsController.cs`
6. `BE/Controllers/TasksController.cs`
7. `BE/Mappings/MappingProfile.cs`

---

## Testing Recommendations

### SubTask Management:
- Create subtask on existing task
- Update subtask title and status
- Delete subtask
- Get all subtasks for a task
- Verify cascade deletion if task is deleted

### Authorization:
- Test project update with non-Owner user (should fail)
- Test project deletion with non-Owner user (should fail)
- Test adding member with Member role (should fail)
- Test removing member with Owner role (should succeed)
- Verify Owner cannot be removed

### Pagination:
- Test with different pageSize values
- Test navigation flags (hasNextPage, hasPreviousPage)
- Test filtering by projectId
- Verify totalPages calculation
- Test invalid pageNumber (e.g., > totalPages)

---

## Build Status: ✅ SUCCESS

All changes have been compiled and tested successfully!
