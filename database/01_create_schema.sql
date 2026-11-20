-- ============================================================
-- ThaiBev Assignment: Person Management
-- สร้างฐานข้อมูลและตาราง Person
--
-- หมายเหตุ: ปกติไม่ต้องรัน script นี้ เพราะ API จะสร้าง schema ผ่าน EF migration
-- เองตอน startup (ดู README) script นี้มีไว้สำหรับผู้ที่ต้องการสร้าง schema เอง
-- ใช้กับ SQL Server 2019+ (รวมถึง container mssql/server:2022)
-- ============================================================

IF DB_ID(N'ThaiBevAssignment') IS NULL
BEGIN
    CREATE DATABASE ThaiBevAssignment;
END
GO

USE ThaiBevAssignment;
GO

IF OBJECT_ID(N'dbo.Person', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Person
    (
        Id         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Person PRIMARY KEY,
        FirstName  NVARCHAR(100)    NOT NULL,
        LastName   NVARCHAR(100)    NOT NULL,
        BirthDate  DATE             NOT NULL,            -- เก็บเป็น ค.ศ. (ISO yyyy-MM-dd) — UI แปลงเป็น พ.ศ. เอง
        Address    NVARCHAR(500)    NOT NULL,
        CreatedAt  DATETIME2(0)     NOT NULL CONSTRAINT DF_Person_CreatedAt DEFAULT SYSUTCDATETIME()
    );
END
GO
