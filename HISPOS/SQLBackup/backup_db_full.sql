USE LocalNewDB;
GO

DECLARE @MyFileName VARCHAR(1000)
SELECT @MyFileName = (SELECT '/var/opt/mssql/data/backup/Full_' + convert(varchar(500),GetDate(),112) + '.bak')

BACKUP DATABASE LocalNewDB
TO DISK = @MyFileName
	WITH FORMAT,
		MEDIANAME = 'SQLServerBackups',
			NAME = 'Full Backup of LocalNewDB';
GO
