--in master
CREATE LOGIN datamigrationlogin WITH password=<password-in-single-quotes>;

--in DataMigration
CREATE USER datamigrationuser FROM LOGIN datamigrationlogin;
EXEC sp_addrolemember 'db_datareader', 'datamigrationuser';
EXEC sp_addrolemember 'db_datawriter', 'datamigrationuser';