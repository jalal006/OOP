-- Run this in the database assigned to the project.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @lockResult int;
EXEC @lockResult = sys.sp_getapplock
    @Resource = N'EquipmentMaintenanceLog.Schema',
    @LockMode = N'Exclusive',
    @LockOwner = N'Transaction',
    @LockTimeout = 10000;

IF @lockResult < 0
    THROW 50001, 'Could not acquire the maintenance schema lock.', 1;

IF OBJECT_ID(N'dbo.MaintenanceRecords', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MaintenanceRecords
    (
        Id int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_MaintenanceRecords PRIMARY KEY,
        EquipmentName nvarchar(100) NOT NULL,
        IssueDescription nvarchar(500) NOT NULL,
        ReportedDate date NOT NULL,
        Status tinyint NOT NULL,
        CONSTRAINT CK_MaintenanceRecords_Status CHECK (Status IN (1, 2, 3)),
        CONSTRAINT CK_MaintenanceRecords_EquipmentName CHECK (LEN(TRIM(EquipmentName)) > 0),
        CONSTRAINT CK_MaintenanceRecords_IssueDescription CHECK (LEN(TRIM(IssueDescription)) > 0)
    );

    -- Demo records are inserted only when the table is first created.
    INSERT dbo.MaintenanceRecords (EquipmentName, IssueDescription, ReportedDate, Status)
    VALUES
        (N'Conveyor 2', N'Belt stops intermittently during operation.', '2026-11-24', 1),
        (N'Robot Cell A', N'Gripper fails to close on the first attempt.', '2026-11-23', 2),
        (N'Pneumatic Press 1', N'Replaced the worn air hose and verified pressure.', '2026-11-22', 3);
END;

COMMIT TRANSACTION;
