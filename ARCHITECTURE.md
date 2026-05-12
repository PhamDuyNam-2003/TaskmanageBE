# Architectural Decisions & Implementation Details

## 1. SubTask Management Architecture

### Design Pattern
- **Service-Repository Pattern**: TaskService handles all business logic
- **DTO Pattern**: SubTaskDto and UpdateSubTaskDto for API contracts
- **AutoMapper**: Automatic mapping between entities and DTOs

### Database Relationships
```
TaskItem (1) ──────────── (Many) SubTask
   |
   └── Has ICollection<SubTask> SubTasks property
```

### Error Handling
- Validates that parent TaskItem exists before creating SubTask
- Returns null for update/get operations if resource not found
- Returns false for delete if resource not found
- Throws exceptions for critical business logic violations

### Cascade Behavior
- When TaskItem is deleted, its SubTasks are cascade deleted (EF Core configuration)
- SubTasks are soft-deleted along with parent Task if applicable

---

## 2. Authorization & Role-Based Access Control

### Architecture Pattern
- **Claims-Based Authorization**: Uses `User.GetUserId()` to extract authenticated user
- **Project-Scoped Roles**: Each user has a role PER project (via ProjectMember)
- **Owner Override**: Project owner always has full permissions

### Authorization Flow

```
Request → Extract UserId from Claims
    ↓
Check ProjectMember.Role for user in project
    ↓
    ├─ Owner (2): Full permissions
    ├─ Manager (1): Can update, add/remove members
    └─ Member (0): Read-only for most operations
```

### Permission Matrix

| Action | Owner | Manager | Member | Anonymous |
|--------|-------|---------|--------|-----------|
| View Project | ✅ | ✅ | ✅ | ❌ |
| Update Project | ✅ | ✅ | ❌ | ❌ |
| Delete Project | ✅ | ❌ | ❌ | ❌ |
| Add Member | ✅ | ✅ | ❌ | ❌ |
| Remove Member | ✅ | ✅ | ❌ | ❌ |
| Create Task | ✅ | ✅ | ✅ | ❌ |

### Service Layer Authorization
Authorization is implemented at the SERVICE level (not just controller):
- If authorization fails, `UnauthorizedAccessException` is thrown
- Controller catches and returns 403 Forbidden
- Prevents unauthorized operations even if called from other services

**Why Service Level Authorization?**
- Protects against direct database access
- Maintains security if API contracts change
- Ensures consistency across multiple endpoints

---

## 3. Pagination & Search Implementation

### Pagination Strategy
- **Server-Side Pagination**: More efficient for large datasets
- **Skip-Take Pattern**: Standard LINQ pagination approach
- **Total Count Metadata**: Enables client-side page calculations

### Pagination Formula
```csharp
Skip = (PageNumber - 1) * PageSize
Take = PageSize
TotalPages = (TotalCount + PageSize - 1) / PageSize
```

### Performance Considerations

**Current Implementation:**
- Single database query with `.Count()` for total
- Separate query for paginated items
- Ordered by `CreatedAt DESC` (newest first)

**Future Optimization (if needed):**
```csharp
// Could use window functions in SQL Server
var query = _context.TaskItems
    .AsNoTracking()
    .Select(x => new { Item = x, RowNum = EF.Functions.RowNumber() })
    .Where(x => x.RowNum > (pageNumber - 1) * pageSize && 
              x.RowNum <= pageNumber * pageSize)
    .Select(x => x.Item);
```

### Filtering Strategy
- **Optional Filters**: `projectId` filter for tasks is optional
- **Composable Queries**: Filters are built conditionally
- **Extensible Design**: Easy to add more filters later

Example of composable filtering:
```csharp
var query = _context.TaskItems.AsQueryable();

if (projectId.HasValue)
    query = query.Where(x => x.ProjectId == projectId);

if (!string.IsNullOrEmpty(status))
    query = query.Where(x => x.Status == status);

// Additional filters can be stacked here
```

---

## 4. API Response Structure

### Consistent Response Format
All endpoints return:
```json
{
  "success": boolean,
  "message": "optional message",
  "data": "response data or null"
}
```

Using `ApiResponse<T>` generic class for type safety.

### HTTP Status Codes Strategy
- **200 OK**: Successful GET, PUT, DELETE
- **201 Created**: Would be better for POST, but current implementation uses 200
- **400 Bad Request**: Validation errors, business rule violations
- **403 Forbidden**: Authorization failures
- **404 Not Found**: Resource doesn't exist
- **500 Internal Server Error**: Unhandled exceptions

---

## 5. Database Schema Implications

### New/Modified Entities

**SubTask** (Existing, used in this implementation)
```csharp
Id: PrimaryKey
TaskItemId: ForeignKey → TaskItem
Title: string
IsDone: bool
CreatedAt: DateTime
```

**ProjectMember** (Existing, enhanced usage)
```csharp
ProjectId: PrimaryKey, ForeignKey → Project
UserId: PrimaryKey, ForeignKey → User
Role: ProjectRole enum (0, 1, 2)
JoinedAt: DateTime
```

### Indexes for Performance
**Should consider adding:**
- Index on `SubTask.TaskItemId` (frequent WHERE clauses)
- Index on `ProjectMember.UserId` (authorization checks)
- Index on `TaskItem.CreatedAt DESC` (pagination queries)

**SQL Example:**
```sql
CREATE INDEX IX_SubTask_TaskItemId ON SubTask(TaskItemId);
CREATE INDEX IX_ProjectMember_UserId ON ProjectMember(UserId);
CREATE INDEX IX_TaskItem_CreatedAtDesc ON TaskItem(CreatedAt DESC);
```

---

## 6. Security Considerations

### Current Implementation
✅ **JWT Token Authentication**: Via `[Authorize]` attribute
✅ **Authorization Checks**: Role-based at service level
✅ **Input Validation**: DTOs for type safety
✅ **Soft Deletes**: Preserves data integrity

### Recommendations for Enhancement
- [ ] Add SQL injection prevention (already using EF Core parameterized queries ✅)
- [ ] Implement rate limiting on sensitive endpoints
- [ ] Add audit logging for permission changes
- [ ] Implement IP whitelisting for admin operations
- [ ] Add request logging and monitoring
- [ ] Implement CORS policies properly
- [ ] Add HTTPS enforcement
- [ ] Implement API versioning for backward compatibility

---

## 7. Extensibility & Future Enhancements

### Easy to Add
1. **Additional Project Roles**: Just add new enum values to `ProjectRole`
2. **More Filters**: Extend the `GetAllAsync` method with additional parameters
3. **Advanced Search**: Add full-text search using EF Core query filters
4. **Export Functionality**: Use existing DTOs to generate Excel/PDF

### Scalability Considerations
- Current implementation suitable for ~100K tasks per project
- For larger scale, consider:
  - Database sharding by project
  - Caching layer (Redis) for frequently accessed projects
  - Elasticsearch for advanced search
  - Background job processing (Hangfire) for bulk operations

### Testing Strategy
- **Unit Tests**: Mock ITaskService and IProjectService
- **Integration Tests**: Use test database for full flow testing
- **Authorization Tests**: Verify role-based access control
- **Pagination Tests**: Test boundary conditions (page 1, last page, invalid page)

---

## 8. Code Quality

### Naming Conventions
- ✅ PascalCase for class names and properties
- ✅ camelCase for method parameters
- ✅ Clear, descriptive names
- ✅ Vietnamese error messages for user-facing errors

### SOLID Principles Applied
- **S** (Single Responsibility): Each service handles one domain
- **O** (Open/Closed): Easy to extend, services don't need modification for new features
- **L** (Liskov Substitution): Interfaces properly defined, implementations are substitutable
- **I** (Interface Segregation): ITaskService focused on task operations
- **D** (Dependency Injection): Services injected via constructor

### DRY (Don't Repeat Yourself)
- ✅ Authorization logic centralized in services
- ✅ Error response structure in ApiResponse<T>
- ✅ AutoMapper for consistent DTO mapping

---

## 9. Migration Path (if needed)

For future database changes:
```bash
# Add new migration
dotnet ef migrations add {MigrationName} --project BE

# Update database
dotnet ef database update --project BE

# Script migration (for production deployment)
dotnet ef migrations script {PreviousMigration} {NewMigration} --output migration.sql
```

---

## Summary

This implementation provides:
- ✅ **Complete SubTask Management** with full CRUD operations
- ✅ **Robust Authorization** using role-based access control
- ✅ **Scalable Pagination** for handling large datasets
- ✅ **Clean Architecture** following SOLID principles
- ✅ **Production-Ready Code** with error handling and validation
- ✅ **Extensible Design** for future enhancements
