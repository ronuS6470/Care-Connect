/*
    Adds local password sign-in to the Users table.

    Safe to re-run: the column is only added when missing, and the backfill only touches rows that
    still have no password.

    Run with:
        sqlcmd -S "(localdb)\mssqllocaldb" -d CareConnectDb -i 001-add-password-auth.sql

    ------------------------------------------------------------------------------------------
    DEVELOPMENT SEED DATA — NOT FOR PRODUCTION.

    The backfill below gives every existing account the same well-known password so the seeded
    data is usable from the sign-in screen. In any real environment, drop the backfill statement
    and provision passwords per-user instead.

        Password: Passw0rd!

    The literal is a PBKDF2-HMAC-SHA256 digest in the format produced and understood by
    CareConnect.Infrastructure.Auth.PasswordHasher — v1.{iterations}.{base64Salt}.{base64Hash}.
    It is not reversible, but it is public knowledge, which is exactly why it must never reach a
    deployed environment.
    ------------------------------------------------------------------------------------------
*/

IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Users')
      AND name = N'PasswordHash'
)
BEGIN
    ALTER TABLE dbo.Users ADD PasswordHash NVARCHAR(500) NULL;
END
GO

UPDATE dbo.Users
SET PasswordHash = 'v1.210000.23orTF9nzI8lY15zIrab3g==.x5rILGCRCmmc1ybfgwTFpcvSvGK5TFeQJctqa1PEnpg='
WHERE PasswordHash IS NULL;
GO

SELECT Id, Email, Role, IsActive, CASE WHEN PasswordHash IS NULL THEN 'no' ELSE 'yes' END AS HasPassword
FROM dbo.Users
ORDER BY Id;
GO
