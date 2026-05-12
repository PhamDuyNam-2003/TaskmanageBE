# Database Migration Guide

## Overview
The implementation uses existing database entities:
- **SubTask** model already exists in your database
- **ProjectMember** model already includes Role field

No new migrations are required to deploy this implementation.

---

## Verification Checklist

### Verify SubTask Table Exists
```sql
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'SubTasks';
```

Expected columns:
- Id (GUID, PK)
- TaskItemId (GUID, FK to TaskItems)
- Title (VARCHAR)
- IsDone (BIT)
- CreatedAt (DATETIME)

---

### Verify ProjectMember Table Has Role
```sql
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ProjectMembers' 
ORDER BY ORDINAL_POSITION;
```

Expected columns:
- ProjectId (GUID, PK)
- UserId (GUID, PK)
- Role (INT) - Values: 0=Member, 1=Manager, 2=Owner
- JoinedAt (DATETIME)

---

### Verify Foreign Keys
```sql
-- Check SubTask → TaskItem relationship
SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
WHERE CONSTRAINT_NAME LIKE '%SubTask%' AND CONSTRAINT_NAME LIKE '%TaskItem%';

-- Check ProjectMember → Project relationship
SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
WHERE CONSTRAINT_NAME LIKE '%ProjectMember%' AND CONSTRAINT_NAME LIKE '%Project%';
```

---

## If Migration Is Needed

### Create Migration (if required)
```bash
cd BE
dotnet ef migrations add EnhanceProjectRoleAndSubTasks -o Migrations
dotnet ef database update
```

### Rollback (if issues occur)
```bash
cd BE
# Remove last migration (data loss possible)
dotnet ef migrations remove

# Or rollback to specific migration
dotnet ef database update {MigrationName}
```

---

## Data Integrity Checks

After deployment, run these checks:

### Check for orphaned SubTasks
```sql
SELECT s.* FROM SubTasks s
LEFT JOIN TaskItems t ON s.TaskItemId = t.Id
WHERE t.Id IS NULL;
```
Expected: 0 rows

### Check for invalid ProjectMember Roles
```sql
SELECT * FROM ProjectMembers 
WHERE Role NOT IN (0, 1, 2);
```
Expected: 0 rows

### Check for members without projects
```sql
SELECT pm.* FROM ProjectMembers pm
LEFT JOIN Projects p ON pm.ProjectId = p.Id
WHERE p.Id IS NULL;
```
Expected: 0 rows

---

## Index Creation (Optional but Recommended)

For better performance with pagination:

```sql
-- Index for SubTask queries
CREATE INDEX IX_SubTasks_TaskItemId 
ON SubTasks(TaskItemId) 
INCLUDE (Title, IsDone, CreatedAt);

-- Index for authorization checks
CREATE INDEX IX_ProjectMembers_UserProject 
ON ProjectMembers(UserId, ProjectId, Role);

-- Index for pagination queries
CREATE INDEX IX_TaskItems_CreatedAt_Desc 
ON TaskItems(CreatedAt DESC) 
INCLUDE (ProjectId, Title, Status);

-- Index for project listing
CREATE INDEX IX_Projects_CreatedAt_Desc 
ON Projects(CreatedAt DESC);
```

---

## Backup Before Deployment

**CRITICAL**: Always backup production database before deploying.

```sql
-- Full database backup
BACKUP DATABASE [TaskManageDB] 
TO DISK = 'C:\Backups\TaskManageDB_backup_2024-12-09.bak'
WITH INIT, COMPRESSION;

-- Verify backup
RESTORE VERIFYONLY 
FROM DISK = 'C:\Backups\TaskManageDB_backup_2024-12-09.bak';
```

Or using SQL Server Management Studio:
1. Right-click Database → Tasks → Back Up...
2. Set backup type: Full
3. Backup to: New Device → Add file path
4. Click OK

---

## Deployment Steps

### Step 1: Backup Production Database
```bash
# Using Azure SQL (if applicable)
az sql db export --resource-group {rg} \
  --server {server} --name TaskManageDB \
  --admin-user {username} --admin-password {password} \
  --storage-key {key} --storage-key-type SharedAccessKey \
  --storage-uri https://{account}.blob.core.windows.net/{container}/backup.bacpac
```

### Step 2: Verify Entity Framework Migrations
```bash
cd BE

# Check migration status
dotnet ef migrations list

# Generate SQL script (preview changes)
dotnet ef migrations script --output migration.sql
```

### Step 3: Update Database (if needed)
```bash
# Apply pending migrations
dotnet ef database update
```

### Step 4: Deploy Application
```bash
# Build release version
dotnet build -c Release

# Publish
dotnet publish -c Release -o ./publish
```

### Step 5: Verify Deployment
- Test all SubTask endpoints
- Verify authorization on project operations
- Confirm pagination works with various page sizes
- Check application logs for errors

---

## Rollback Plan (if issues occur)

### Quick Rollback
```bash
cd BE

# Revert to previous migration
dotnet ef database update {PreviousMigrationName}

# Or rollback application version
# 1. Stop IIS/Kestrel
# 2. Deploy previous version
# 3. Restart service
```

### Database Restore
```bash
# Restore from backup
RESTORE DATABASE [TaskManageDB] 
FROM DISK = 'C:\Backups\TaskManageDB_backup_2024-12-09.bak' 
WITH REPLACE;
```

---

## Deployment Checklist

- [ ] Create database backup
- [ ] Review migration script (if applicable)
- [ ] Test on staging environment
- [ ] Verify all indices are created
- [ ] Check data integrity
- [ ] Test SubTask endpoints
- [ ] Test authorization on projects
- [ ] Verify pagination with real data
- [ ] Monitor application logs
- [ ] Verify no connection timeouts
- [ ] Check performance metrics

---

## Monitoring After Deployment

### Application Logs
```csharp
// Logs to check
"Authorization failed for user"
"SubTask creation failed"
"Database query timeout"
"Invalid pagination parameters"
```

### Performance Queries
```sql
-- Check slow queries
SELECT * FROM sys.dm_exec_requests 
WHERE status != 'sleeping';

-- Index usage
SELECT object_name(s.object_id) as Table_Name,
       i.name as Index_Name,
       s.seeks, s.scans, s.lookups
FROM sys.dm_db_index_usage_stats s
JOIN sys.indexes i ON s.object_id = i.object_id 
  AND s.index_id = i.index_id
ORDER BY (s.seeks + s.scans + s.lookups) DESC;
```

---

## FAQ

### Q: Do I need to run migrations?
**A**: No, the implementation uses existing tables (SubTask, ProjectMember with Role).

### Q: Will existing data be affected?
**A**: No, this is purely additive. Existing projects, tasks, and subtasks are unaffected.

### Q: What about existing SubTasks without roles?
**A**: ProjectMember.Role defaults to Member (0) for existing records.

### Q: How do I handle the transition for existing project members?
**A**: All existing ProjectMembers automatically get Role = Member (0). Manually update roles in database if needed:
```sql
UPDATE ProjectMembers 
SET Role = 2 -- Owner
WHERE ProjectId = {projectId} AND UserId = {ownerId};
```

### Q: Can I revert these changes?
**A**: Yes, the code changes are additive and don't modify existing functionality. Simply rollback the application code.

---

## Support

For database-related issues:
1. Check application logs for specific errors
2. Run data integrity checks above
3. Review migration history: `dotnet ef migrations list`
4. Verify connection string in appsettings.json
5. Check SQL Server logs for additional information

---

## Connection String Reference

**appsettings.json** should contain:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TaskManageDB;Integrated Security=true;"
  }
}
```

Or for Azure SQL:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:server.database.windows.net,1433;Initial Catalog=TaskManageDB;Persist Security Info=False;User ID=username;Password=password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

---

## Conclusion

The implementation is designed to work with your existing database schema. No data migration is required. Simply deploy the updated application code and test the new features.
