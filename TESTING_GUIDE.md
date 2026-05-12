# Testing Guide - SubTask Management, Authorization & Pagination

## Test Scenarios & Commands

### Prerequisite
- Backend running on `https://localhost:5001` (or your configured URL)
- Valid JWT token obtained from login endpoint
- Postman, Insomnia, or cURL for API testing

---

## Part 1: SubTask Management Tests

### Test 1.1: Create SubTask
**Scenario**: User creates a subtask for an existing task

**Request**:
```http
POST /api/tasks/{taskId}/subtasks
Authorization: Bearer {your_token}
Content-Type: application/json

{
  "title": "Implement authentication module"
}
```

**Expected Response** (200 OK):
```json
{
  "success": true,
  "message": "Tạo subtask thành công",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "taskItemId": "123e4567-e89b-12d3-a456-426614174000",
    "title": "Implement authentication module",
    "isDone": false,
    "createdAt": "2024-12-09T10:30:00Z"
  }
}
```

**Edge Cases to Test**:
- ❌ Task ID doesn't exist → 404 Not Found
- ❌ Empty title → Should still create (or add validation)
- ✅ Duplicate titles → Should allow (different tasks might have similar subtasks)

---

### Test 1.2: Get SubTasks for Task
**Scenario**: User retrieves all subtasks for a specific task

**Request**:
```http
GET /api/tasks/{taskId}/subtasks
Authorization: Bearer {your_token}
```

**Expected Response** (200 OK):
```json
{
  "success": true,
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "taskItemId": "123e4567-e89b-12d3-a456-426614174000",
      "title": "Implement authentication module",
      "isDone": false,
      "createdAt": "2024-12-09T10:30:00Z"
    },
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "taskItemId": "123e4567-e89b-12d3-a456-426614174000",
      "title": "Write unit tests",
      "isDone": true,
      "createdAt": "2024-12-09T10:35:00Z"
    }
  ]
}
```

**Order**: Should be ordered by creation date (newest first)

---

### Test 1.3: Get Single SubTask
**Scenario**: User retrieves a specific subtask

**Request**:
```http
GET /api/tasks/subtasks/{subTaskId}
Authorization: Bearer {your_token}
```

**Expected Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "taskItemId": "123e4567-e89b-12d3-a456-426614174000",
    "title": "Implement authentication module",
    "isDone": false,
    "createdAt": "2024-12-09T10:30:00Z"
  }
}
```

**Edge Cases**:
- ❌ Non-existent SubTask ID → 404 Not Found

---

### Test 1.4: Update SubTask
**Scenario**: User updates a subtask's title and completion status

**Request**:
```http
PUT /api/tasks/subtasks/{subTaskId}
Authorization: Bearer {your_token}
Content-Type: application/json

{
  "title": "Implement JWT authentication module",
  "isDone": true
}
```

**Expected Response** (200 OK):
```json
{
  "success": true,
  "message": "Cập nhật subtask thành công",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "taskItemId": "123e4567-e89b-12d3-a456-426614174000",
    "title": "Implement JWT authentication module",
    "isDone": true,
    "createdAt": "2024-12-09T10:30:00Z"
  }
}
```

**Edge Cases**:
- ❌ Non-existent SubTask → 404 Not Found
- ✅ Update only isDone → Should update
- ✅ Update only title → Should update

---

### Test 1.5: Delete SubTask
**Scenario**: User deletes a subtask

**Request**:
```http
DELETE /api/tasks/subtasks/{subTaskId}
Authorization: Bearer {your_token}
```

**Expected Response** (200 OK):
```json
{
  "success": true,
  "message": "Xóa subtask thành công"
}
```

**Edge Cases**:
- ❌ Non-existent SubTask → 404 Not Found
- ✅ Delete multiple times → Second delete should fail

---

## Part 2: Authorization Tests

### Test 2.1: Update Project - Authorization Check
**Setup**: 
- Create Project as User A
- Make User B a Member (role: 0)
- Make User C a Manager (role: 1)

**Test 2.1a: Member tries to update**
```http
PUT /api/projects/{projectId}
Authorization: Bearer {userB_token}
Content-Type: application/json

{
  "name": "Updated Name",
  "description": "Updated Description"
}
```
**Expected**: 403 Forbidden

**Test 2.1b: Manager tries to update**
```http
PUT /api/projects/{projectId}
Authorization: Bearer {userC_token}
Content-Type: application/json

{
  "name": "Updated Name",
  "description": "Updated Description"
}
```
**Expected**: 200 OK (successful update)

**Test 2.1c: Owner tries to update**
```http
PUT /api/projects/{projectId}
Authorization: Bearer {userA_token}
Content-Type: application/json

{
  "name": "Updated Name",
  "description": "Updated Description"
}
```
**Expected**: 200 OK (successful update)

---

### Test 2.2: Delete Project - Authorization Check
**Test 2.2a: Member tries to delete**
```http
DELETE /api/projects/{projectId}
Authorization: Bearer {userB_token}
```
**Expected**: 403 Forbidden

**Test 2.2b: Manager tries to delete**
```http
DELETE /api/projects/{projectId}
Authorization: Bearer {userC_token}
```
**Expected**: 403 Forbidden

**Test 2.2c: Owner tries to delete**
```http
DELETE /api/projects/{projectId}
Authorization: Bearer {userA_token}
```
**Expected**: 200 OK (project deleted)

---

### Test 2.3: Add Member - Authorization Check
**Test 2.3a: Member tries to add another member**
```http
POST /api/projects/{projectId}/members
Authorization: Bearer {userB_token}
Content-Type: application/json

{
  "userId": "{userD_id}",
  "role": 0
}
```
**Expected**: 403 Forbidden

**Test 2.3b: Manager tries to add member**
```http
POST /api/projects/{projectId}/members
Authorization: Bearer {userC_token}
Content-Type: application/json

{
  "userId": "{userD_id}",
  "role": 0
}
```
**Expected**: 200 OK (member added)

---

### Test 2.4: Remove Member - Authorization Check
**Setup**: User D is already a member (role: 0)

**Test 2.4a: Member tries to remove another member**
```http
DELETE /api/projects/{projectId}/members/{userD_id}
Authorization: Bearer {userB_token}
```
**Expected**: 403 Forbidden

**Test 2.4b: Manager tries to remove member**
```http
DELETE /api/projects/{projectId}/members/{userD_id}
Authorization: Bearer {userC_token}
```
**Expected**: 200 OK (member removed)

**Test 2.4c: Try to remove Owner**
```http
DELETE /api/projects/{projectId}/members/{userA_id}
Authorization: Bearer {userC_token}
```
**Expected**: 400 Bad Request with message "Không thể xóa chủ sở hữu project"

---

## Part 3: Pagination & Filtering Tests

### Test 3.1: Get Tasks with Pagination (Default)
**Scenario**: User requests tasks with default pagination

**Request**:
```http
GET /api/tasks
Authorization: Bearer {your_token}
```

**Expected Response**:
```json
{
  "success": true,
  "data": {
    "items": [
      { "id": "...", "title": "Task 1", ... },
      { "id": "...", "title": "Task 2", ... },
      ...
    ],
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

### Test 3.2: Get Tasks - Custom Page Size
**Request**:
```http
GET /api/tasks?pageNumber=1&pageSize=5
Authorization: Bearer {your_token}
```

**Expected Response**:
- Returns exactly 5 items
- `totalPages` = ceil(50 / 5) = 10

---

### Test 3.3: Get Tasks - Second Page
**Request**:
```http
GET /api/tasks?pageNumber=2&pageSize=10
Authorization: Bearer {your_token}
```

**Expected Response**:
- `hasNextPage` = true (if totalCount > 20)
- `hasPreviousPage` = true
- Items 11-20 from total list

---

### Test 3.4: Get Tasks - Last Page
**Request** (assuming 5 total pages):
```http
GET /api/tasks?pageNumber=5&pageSize=10
Authorization: Bearer {your_token}
```

**Expected Response**:
- `hasNextPage` = false
- `hasPreviousPage` = true
- Fewer than 10 items (remaining items)

---

### Test 3.5: Get Tasks - Filter by ProjectId
**Request**:
```http
GET /api/tasks?pageNumber=1&pageSize=10&projectId={projectId}
Authorization: Bearer {your_token}
```

**Expected Response**:
- Only returns tasks where `ProjectId == {projectId}`
- `totalCount` reflects filtered count

---

### Test 3.6: Get Projects with Pagination
**Request**:
```http
GET /api/projects?pageNumber=1&pageSize=10
Authorization: Bearer {your_token}
```

**Expected Response**:
```json
{
  "success": true,
  "data": {
    "items": [
      { "id": "...", "name": "Project 1", ... },
      ...
    ],
    "totalCount": 25,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 3,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

---

### Test 3.7: Invalid Page Number
**Request**:
```http
GET /api/tasks?pageNumber=999&pageSize=10
Authorization: Bearer {your_token}
```

**Expected Response**: 
- Returns empty items array
- Correct pagination metadata

---

### Test 3.8: Invalid Page Size
**Request**:
```http
GET /api/tasks?pageNumber=1&pageSize=0
Authorization: Bearer {your_token}
```

**Expected Response**:
- Should handle gracefully (either error or use default)
- Current implementation: May cause division by zero in calculations

**Recommendation**: Add validation:
```csharp
if (pageSize <= 0) pageSize = 10;
if (pageNumber <= 0) pageNumber = 1;
```

---

## Test Data Setup Script

### Using cURL to create test data:

```bash
#!/bin/bash

TOKEN="your_jwt_token_here"
BASE_URL="https://localhost:5001/api"

# Create a project
PROJECT_RESPONSE=$(curl -X POST "$BASE_URL/projects" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Project",
    "description": "For testing pagination"
  }')

PROJECT_ID=$(echo $PROJECT_RESPONSE | jq -r '.data.id')
echo "Created project: $PROJECT_ID"

# Create multiple tasks
for i in {1..25}; do
  curl -X POST "$BASE_URL/tasks" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d "{
      \"projectId\": \"$PROJECT_ID\",
      \"title\": \"Task $i\",
      \"description\": \"Description for task $i\",
      \"assignedToId\": \"your_user_id\",
      \"priority\": 1,
      \"dueDate\": \"2024-12-31T23:59:59Z\"
    }"
done

echo "Test data created successfully!"
```

---

## Regression Tests Checklist

- [ ] Create new SubTask with valid data
- [ ] Create SubTask with invalid TaskId
- [ ] Update SubTask title only
- [ ] Update SubTask isDone only
- [ ] Delete existing SubTask
- [ ] Try to delete non-existent SubTask
- [ ] Authorize user before updating project
- [ ] Forbid non-owner from deleting project
- [ ] Forbid member from removing another member
- [ ] Paginate tasks with various page sizes
- [ ] Filter tasks by projectId
- [ ] Verify navigation flags on first page
- [ ] Verify navigation flags on last page
- [ ] Handle invalid pagination parameters

---

## Performance Tests

### Load Test - Pagination Performance
**Scenario**: Test pagination response time with large dataset

```bash
# Using Apache Bench
ab -n 1000 -c 10 -H "Authorization: Bearer $TOKEN" \
  "https://localhost:5001/api/tasks?pageNumber=1&pageSize=50"
```

**Expected**: 
- Response time < 200ms per request
- No memory leaks
- Connection pool properly managed

---

## Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| 401 Unauthorized | Invalid/expired token | Generate new token |
| 403 Forbidden | Insufficient permissions | Check user role in project |
| 404 Not Found | Resource doesn't exist | Verify resource ID exists |
| 500 Server Error | Exception in service | Check application logs |
| Empty items array | Page exceeds totalPages | Verify pageNumber is valid |

