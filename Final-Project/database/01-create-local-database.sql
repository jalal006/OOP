IF DB_ID(N'EquipmentMaintenanceLog') IS NULL
BEGIN
    BEGIN TRY
        CREATE DATABASE [EquipmentMaintenanceLog];
    END TRY
    BEGIN CATCH
        -- The database may have been created by another startup.
        IF ERROR_NUMBER() <> 1801
            THROW;
    END CATCH;
END;
