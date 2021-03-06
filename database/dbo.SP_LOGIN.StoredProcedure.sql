USE [QLVT]
GO
/****** Object:  StoredProcedure [dbo].[SP_LOGIN]    Script Date: 12/23/2018 5:05:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[SP_LOGIN]
@LOGINNAME NVARCHAR(100)
AS
BEGIN
DECLARE @MANV INT
SELECT @MANV = (SELECT name FROM sys.sysusers WHERE sid = (SELECT SUSER_SID(@LOGINNAME)))
SELECT N'MANV'		= @MANV, 
	   N'HOTEN'		= (SELECT HO+' '+TEN FROM dbo.NhanVien WHERE MANV = @MANV),
	   N'NHOM' = name
FROM sys.sysusers
WHERE uid = (SELECT groupuid FROM sys.sysmembers
			 WHERE memberuid = (SELECT uid FROM sys.sysusers
								WHERE sid = SUSER_SID(@LOGINNAME)))
END
GO
