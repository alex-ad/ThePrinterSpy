> About
- Application monitors the printing of documents. Collected data stores in MSSQL database.

> Components
1. Windows Service - the printing monitor
2. Control Panel - viewing collected data

> Language
- C# (.NET Framework 4/4.7)

> System Requirements
- Control Panel: .NET Framework 4.7 (Windows 7/10)
- Monitoring Service: .NET Framework 4 with KB2468871 (Windows XP/7/10)

> Instruction
- Identity data for MSSQL DB must stored in the Windows Registry.
1. Key "HKEY_LOCAL_MACHINE\SOFTWARE\alex-ad\ThePrinterSpy"
2. String value "DbServer" - database server name
3. String value "DbName" - database name
4. String value "DbPassword" - database user password
5. String value "DbUser" - database user name

> Installers
- Windows XP (service only) - https://drive.google.com/file/d/1IjWftPIgw8wg63JWkhFnJbUhPclR57me/view?usp=sharing
- Windows 7/10 (service with control panel) - https://drive.google.com/file/d/1BLd4EODD-S5oulCG0Xtyr1dwbOPL-oeO/view?usp=sharing
