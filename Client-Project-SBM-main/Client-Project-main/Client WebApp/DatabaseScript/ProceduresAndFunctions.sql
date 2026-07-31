USE [SBSApplication]
GO
/****** Object:  User [SBSAppDBUser]    Script Date: 2026-05-25 3:11:28 PM ******/
CREATE USER [SBSAppDBUser] FOR LOGIN [SBSAppDBUser] WITH DEFAULT_SCHEMA=[SBSAppDBUser]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [SBSAppDBUser]
GO
ALTER ROLE [db_datareader] ADD MEMBER [SBSAppDBUser]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [SBSAppDBUser]
GO
/****** Object:  Schema [SBSAppDBUser]    Script Date: 2026-05-25 3:11:29 PM ******/
CREATE SCHEMA [SBSAppDBUser]
GO
/****** Object:  UserDefinedFunction [SBSAppDBUser].[fn_sbs_credentials_check]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===========================
-- function to check password
-- ===========================
CREATE FUNCTION [SBSAppDBUser].[fn_sbs_credentials_check]
(
    @p_username VARCHAR(100),
    @p_password VARCHAR(100)
)
RETURNS BIT
AS
BEGIN
    DECLARE @isValid BIT = 0;
    DECLARE @hashed_input VARBINARY(64);

    SET @hashed_input = HASHBYTES('SHA2_512', CONVERT(VARBINARY(MAX), @p_password));

    IF EXISTS (
        SELECT 1
        FROM sbs_userMaster
        WHERE username = @p_username
          AND password = @hashed_input
    )
    BEGIN
        SET @isValid = 1;
    END
	SET @isValid = 1;
    RETURN @isValid;
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_CombinedSubcontractorEntityReport]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ======================================
-- SP: CombinedSubcontractorEntityReport
-- ======================================

CREATE PROCEDURE [SBSAppDBUser].[usp_CombinedSubcontractorEntityReport]
@p_subcontractorName VARCHAR(255) = NULL,
@p_fromDate DATE,
@p_toDate DATE,
@p_isTrollyApplicable BIT = 0
AS
BEGIN
    SELECT Date, DocketNumber, InvoiceNo, Customer, Quantity, TotalAmount, LevhiAmount, FinalAmount
    
    FROM(
    
        SELECT 
            FORMAT(inv.invoiceDate, 'dd-MM-yyyy') AS DateOrder,
            FORMAT(inv.invoiceDate, 'dd-MM-yyyy') AS Date,
            inv.DocketNumber,
            ISNULL(STRING_AGG(inv.invoiceNo, ', '),'') AS InvoiceNo,        
            sc.name AS Customer,
            ISNULL(CAST(FORMAT(SUM(inv.quantity), 'N0') AS VARCHAR(20)),'') AS Quantity,
            '' TotalAmount, '' LevhiAmount, '' FinalAmount,
            --SUM(inv.totalAmount) totalAmount,
            --SUM((inv.totalAmount * 0.4)) levhiAmount,
            --SUM((inv.totalAmount + (inv.totalAmount * 0.4))) finalAmount,
            1 AS OrderNumber

        
        FROM sbs_invoiceDetails inv
        JOIN sbs_subContractor sc ON inv.subcontractorId = sc.id
        WHERE inv.isDeleted = 0 AND sc.isDeleted = 0 AND inv.IsLeviApplicable = 1
            AND ( (@p_isTrollyApplicable = 0 AND ISNULL(inv.TrollyQuantity, 0) = 0)
                    OR (@p_isTrollyApplicable = 1 AND ISNULL(inv.TrollyQuantity, 0) >= 1))
            AND inv.invoiceDate BETWEEN ISNULL(@p_fromDate,'01-01-1900') AND ISNULL(@p_toDate,GETDATE()) 
            AND (sc.name LIKE '%' + ISNULL(@p_subcontractorName,'') + '%')
        GROUP BY 
            FORMAT(inv.invoiceDate, 'dd-MM-yyyy'),
            inv.DocketNumber,       
            sc.name
    
        UNION ALL
    
        SELECT 
            FORMAT(inv.invoiceDate, 'dd-MM-yyyy') AS DateOrder,
            '', '','', 'TOTAL' AS Customer,
            ISNULL(CAST( FORMAT(SUM(inv.quantity), 'N0') AS VARCHAR(20)),'')  AS Quantity,
            ISNULL(CAST( FORMAT(SUM(inv.totalAmount), 'N0') AS VARCHAR(20)),'') AS Quantity, 
            ISNULL(CAST( FORMAT(SUM((inv.totalAmount * 0.4)), 'N0')  AS VARCHAR(20)),'') levhiAmount,
            '', --CAST(SUM((inv.totalAmount + (inv.totalAmount * 0.4))) AS VARCHAR(20))  finalAmount,
            2 AS OrderNumber        
        FROM sbs_invoiceDetails inv
        JOIN sbs_subContractor sc ON inv.subcontractorId = sc.id
        WHERE inv.isDeleted = 0 AND sc.isDeleted = 0 AND inv.IsLeviApplicable = 1
            AND  ( (@p_isTrollyApplicable = 0 AND ISNULL(inv.TrollyQuantity, 0) = 0)
                    OR (@p_isTrollyApplicable = 1 AND ISNULL(inv.TrollyQuantity, 0) >= 1))
            AND inv.invoiceDate BETWEEN ISNULL(@p_fromDate,'01-01-1900') AND ISNULL(@p_toDate,GETDATE()) 
            AND (sc.name LIKE '%' + ISNULL(@p_subcontractorName,'') + '%')
        GROUP BY 
            FORMAT(inv.invoiceDate, 'dd-MM-yyyy')
    
        UNION ALL
   
        SELECT 
            FORMAT(inv.invoiceDate, 'dd-MM-yyyy') AS DateOrder,
            '', '','', (CASE WHEN @p_isTrollyApplicable = 0 THEN 'GRAND TOTAL' ELSE 'HAND TROLLY' END) AS Customer,
            ISNULL(CAST(FORMAT(SUM(inv.TrollyQuantity), 'N0') AS VARCHAR(20)),'')  AS Quantity, 
            ISNULL(CAST(FORMAT(SUM(inv.TrollyAmount), 'N0') AS VARCHAR(20)),'')  totalAmount,
            '', ISNULL(CAST(FORMAT(SUM((ISNULL(inv.TrollyAmount,0) + inv.totalAmount + (inv.totalAmount * 0.4))), 'N0') AS VARCHAR(20)),'') finalAmount,
            3 AS OrderNumber
        FROM sbs_invoiceDetails inv
        JOIN sbs_subContractor sc ON inv.subcontractorId = sc.id
        WHERE inv.isDeleted = 0 AND sc.isDeleted = 0 AND inv.IsLeviApplicable = 1
            AND  ( (@p_isTrollyApplicable = 0 AND ISNULL(inv.TrollyQuantity, 0) = 0)
                    OR (@p_isTrollyApplicable = 1 AND ISNULL(inv.TrollyQuantity, 0) >= 1))
            AND inv.invoiceDate BETWEEN ISNULL(@p_fromDate,'01-01-1900') AND ISNULL(@p_toDate,GETDATE()) 
            AND (sc.name LIKE '%' + ISNULL(@p_subcontractorName,'') + '%')

        GROUP BY 
            FORMAT(inv.invoiceDate, 'dd-MM-yyyy')

        UNION ALL
   
        SELECT 
            '31-12-9999' AS DateOrder,
            '', '','', '',
            'TOTAL' AS Quantity, 
            'AMOUNT'  totalAmount,
            'RS', ISNULL(CAST(FORMAT(SUM((ISNULL(inv.TrollyAmount,0) + inv.totalAmount + (inv.totalAmount * 0.4))), 'N0') AS VARCHAR(20)),'') finalAmount,
            4 AS OrderNumber
        FROM sbs_invoiceDetails inv
        JOIN sbs_subContractor sc ON inv.subcontractorId = sc.id
        WHERE inv.isDeleted = 0 AND sc.isDeleted = 0 AND inv.IsLeviApplicable = 1
            AND  ( (@p_isTrollyApplicable = 0 AND ISNULL(inv.TrollyQuantity, 0) = 0)
                    OR (@p_isTrollyApplicable = 1 AND ISNULL(inv.TrollyQuantity, 0) >= 1))
            AND inv.invoiceDate BETWEEN ISNULL(@p_fromDate,'01-01-1900') AND ISNULL(@p_toDate,GETDATE()) 
            AND (sc.name LIKE '%' + ISNULL(@p_subcontractorName,'') + '%')
    
    
    ) CombinedSubcontractorEntity
    Order by DateOrder,OrderNumber
    

--    sbs_invoiceDetails inv WHERE inv.IsLeviApplicable = 1
    

END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_MonthlyPaymentSubcontractorWiseTotalPayment]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ======================================
-- SP: MonthlyPaymentSubcontractorWiseTotalPayment (Updated)
-- ======================================
CREATE   PROCEDURE [SBSAppDBUser].[usp_MonthlyPaymentSubcontractorWiseTotalPayment]
    @p_subcontractorName VARCHAR(255) = NULL,
    @p_fromDate DATE,
    @p_toDate DATE
AS
BEGIN
    BEGIN TRY
        SELECT 
            sc.name AS SubContractorName,
            FORMAT(inv.invoiceDate, 'MM-yyyy') AS MonthAndYear,
            SUM(CASE WHEN inv.paymentMode = 'Cash' THEN inv.totalAmount ELSE 0 END) AS CashAmount,
            SUM(CASE WHEN inv.paymentMode = 'Balance' THEN inv.totalAmount ELSE 0 END) AS BalanceAmount,
            SUM(ISNULL(pd.amountPaid, 0)) AS PaidAmount
        FROM sbs_invoiceDetails inv
        JOIN sbs_subContractor sc ON inv.subcontractorId = sc.id
        JOIN (
            SELECT invoiceId, SUM(amountPaid) AS amountPaid
            FROM sbs_paymentDetails
            WHERE isDeleted = 0
            GROUP BY invoiceId
        ) pd ON inv.id = pd.invoiceId
        WHERE 
            inv.isDeleted = 0 AND
            sc.isDeleted = 0 AND
            inv.invoiceDate BETWEEN @p_fromDate AND @p_toDate AND
            (sc.name LIKE '%' + ISNULL(@p_subcontractorName,'') + '%')
        GROUP BY sc.name, FORMAT(inv.invoiceDate, 'MM-yyyy')
    END TRY
    BEGIN CATCH
        SELECT 'Fail' AS status;
    END CATCH
END
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_PaidBalancePaymentReport]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ===========================================
-- Reports
-- ===========================================
-- ======================================
-- SP: PaidBalancePaymentReport (Updated)
-- Contractor Report
-- SELECT CreatedAt AT TIME ZONE 'UTC' AT TIME ZONE 'India Standard Time' AS LocalTime, *  FROM SBS_InvoiceDetails
-- ======================================
-- usp_PaidBalancePaymentReport NULL,NULL,NULL,NULL, 0
-- usp_PaidBalancePaymentReport NULL,NULL,NULL,NULL, 1
CREATE   PROCEDURE usp_PaidBalancePaymentReport
    @p_subcontractorName VARCHAR(255) = NULL,
    @p_bankName VARCHAR(255) = NULL,
    @p_fromDate DATE,
    @p_toDate DATE,
    @p_isLevhiApplicable BIT = 0
AS
BEGIN
    BEGIN TRY
        SELECT 
            -- Date	Receipt No.	LR No.	Description	Box	Peti	Motor	Total Type Rate	Total Amount
            inv.invoiceDate AS InvoiceDate,
            inv.invoiceNo ReceiptNumber,
            inv.LRNumber,
            sc.name AS SubContractor,
            inv.VehicleNumber,
            -- bm.bankName AS BankName,
            inv.unitAmount as 'TotalTypeRate',
            
            --inv.totalAmount,
            --inv.quantity

            SUM(inv.totalAmount - ISNULL(inv.commissionPercentage,0)) AS InvoiceAmount,
            (CASE WHEN inv.ProductId = 1 THEN CAST(inv.quantity AS VARCHAR(10)) ELSE '-' END) AS Box,
            (CASE WHEN inv.ProductId = 2 THEN CAST(inv.quantity AS VARCHAR(10)) ELSE '-' END) AS Peti,
            (CASE WHEN inv.ProductId = 3 THEN CAST(inv.quantity AS VARCHAR(10)) ELSE '-' END) AS Motors
            
        FROM sbs_invoiceDetails inv
        JOIN sbs_subContractor sc ON inv.subcontractorId = sc.id
        WHERE 
            inv.IsLeviApplicable = @p_isLevhiApplicable 
            -- AND inv.invoiceNo = '26166'
            AND inv.isDeleted = 0 
            -- AND pd.isDeleted = 0 
            AND inv.invoiceDate BETWEEN ISNULL(@p_fromDate,'01-01-1900') AND ISNULL(@p_toDate,GETDATE()) 
            AND (sc.name LIKE '%' + ISNULL(@p_subcontractorName,'') + '%') 
            AND (
                (inv.VehicleNumber LIKE '%' + ISNULL(@p_bankName,'') + '%')
                OR (inv.LRNumber LIKE '%' + ISNULL(@p_bankName,'') + '%')
                OR (inv.invoiceNo LIKE '%' + ISNULL(@p_bankName,'') + '%')
                )

        GROUP BY 
        inv.invoiceDate,
            inv.invoiceNo,
            inv.LRNumber,
            sc.name,
            inv.VehicleNumber,
            inv.unitAmount,
            inv.ProductId,
            inv.quantity


    END TRY
    BEGIN CATCH
        SELECT 'Fail' AS status;
    END CATCH
END
GO



/****** Object:  StoredProcedure [SBSAppDBUser].[usp_ProductWisePayment]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ======================================
-- SP: ProductWisePayment (Updated)
-- ======================================
CREATE   PROCEDURE [SBSAppDBUser].[usp_ProductWisePayment]
    @p_productName VARCHAR(255) = NULL,
    @p_subcontractorName VARCHAR(255) = NULL,
    @p_fromDate DATE,
    @p_toDate DATE
AS
BEGIN
    BEGIN TRY
        SELECT 
            pm.description AS ProdName,
            sc.name AS SubContractorName,
            SUM(CASE WHEN inv.paymentMode = 'Cash' THEN inv.totalAmount ELSE 0 END) AS CashAmount,
            SUM(CASE WHEN inv.paymentMode = 'Balance' THEN inv.totalAmount ELSE 0 END) AS BalanceAmount,
            SUM(ISNULL(pd.amountPaid, 0)) AS PaidAmount
        FROM sbs_invoiceDetails inv
        JOIN sbs_productMaster pm ON inv.productId = pm.id
        JOIN sbs_subContractor sc ON inv.subcontractorId = sc.id
        JOIN (
            SELECT invoiceId, SUM(amountPaid) AS amountPaid
            FROM sbs_paymentDetails
            WHERE isDeleted = 0
            GROUP BY invoiceId
        ) pd ON inv.id = pd.invoiceId
        WHERE 
            inv.isDeleted = 0 AND
            pm.isDeleted = 0 AND
            sc.isDeleted = 0 AND
            inv.invoiceDate BETWEEN @p_fromDate AND @p_toDate AND
            (pm.description LIKE '%' + ISNULL(@p_productName,'') + '%') AND
            (@p_subcontractorName IS NULL OR sc.name LIKE '%' + @p_subcontractorName + '%')
        GROUP BY pm.description, sc.name
    END TRY
    BEGIN CATCH
        SELECT 'Fail' AS status;
    END CATCH
END
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_additional_entities_delete]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Delete

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_additional_entities_delete]
    @p_id INT,
    @p_updatedBy INT
AS
BEGIN
    BEGIN TRY

        UPDATE sbs_additionalEntity
        SET isDeleted = 1,
            updatedBy = @p_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_DeletedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_additional_entities_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Get

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_additional_entities_get]
    @p_id INT = NULL,
    @p_companyId INT,
    @p_subContractorId INT = NULL
AS
BEGIN

    SELECT 
        ae.id AS R_id,
        ae.type AS R_type,
        ae.amount AS R_amount,
        ae.quantity AS R_quantity,
        ae.date AS R_date,
        ae.companyId AS R_companyId,
        ae.subContractorId AS R_subContractorId,
        sc.name AS R_subContractorName
    FROM sbs_additionalEntity ae
    JOIN sbs_subContractor sc ON ae.subContractorId = sc.id
    WHERE ae.isDeleted = 0
	AND sc.isDeleted = 0
      AND ae.companyId = @p_companyId
      AND (ISNULL(@p_id, 0)=0 OR ae.id = @p_id)
      AND (ISNULL(@p_subContractorId, 0)=0 OR ae.subContractorId = @p_subContractorId);
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_additional_entities_insert]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =====================================
-- ADDITIONAL ENTITY CRUD STORED PROCEDURES
-- =====================================

--Insert

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_additional_entities_insert]
    @p_type VARCHAR(50),
    @p_amount DECIMAL(10,2),
    @p_quantity INT,
    @p_date DATETIME2,
    @p_companyId INT,
    @p_subContractorId INT,
    @p_createdBy INT
AS
BEGIN
    BEGIN TRY
        DECLARE @V_InsertedID INT = NULL;

        INSERT INTO sbs_additionalEntity (type, amount, quantity, date, companyId, subContractorId, createdBy, createdAt, isActive, isDeleted)
        VALUES (@p_type, @p_amount, @p_quantity, @p_date, @p_companyId, @p_subContractorId, @p_createdBy, GETDATE(), 1, 0);

        SET @V_InsertedID = SCOPE_IDENTITY();
        SELECT 'SUCCESS' AS R_Status, @V_InsertedID AS R_InsertedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, NULL AS R_InsertedID, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @V_InsertedID,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_additional_entities_update]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Update

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_additional_entities_update]
    @p_id INT,
    @p_type VARCHAR(50),
    @p_amount DECIMAL(10,2),
    @p_quantity INT,
    @p_date DATETIME2,
    @p_companyId INT,
    @p_subContractorId INT,
    @p_updatedBy INT
AS
BEGIN
    BEGIN TRY

        UPDATE sbs_additionalEntity
        SET type = @p_type,
            amount = @p_amount,
            quantity = @p_quantity,
            date = @p_date,
            companyId = @p_companyId,
            subContractorId = @p_subContractorId,
            updatedBy = @p_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_UpdatedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_bankMaster_delete]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- DELETE 

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_bankMaster_delete]
    @P_id INT,
    @P_updatedBy INT
AS
BEGIN
    
    BEGIN TRY
       
        UPDATE sbs_bankMaster
        SET isDeleted = 1,
            updatedBy = @P_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @P_id;

        SELECT 'SUCCESS' AS R_Status, @P_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @P_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @P_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_bankMaster_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- GET 
CREATE PROCEDURE usp_sbs_bankMaster_get
    @P_id INT = NULL
AS
BEGIN    

    SELECT
        id AS R_id,
        bankName AS R_bankName,
        branch AS R_branch,
        bankName + ' (' + branch + ')' AS R_bankNameSelect
    FROM sbs_bankMaster
    WHERE isDeleted = 0
      AND (@P_id IS NULL OR id = @P_id);

END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_bankMaster_insert]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =====================================
-- BANK MASTER CRUD STORED PROCEDURES
-- =====================================

-- Insert

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_bankMaster_insert]
    @P_bankName VARCHAR(100),
    @P_branch VARCHAR(50),
    @P_createdBy INT
AS
BEGIN
    BEGIN TRY
        DECLARE @V_InsertedID INT = NULL;
        
        INSERT INTO sbs_bankMaster (
            bankName, branch, createdBy, createdAt, isActive, isDeleted
        )
        VALUES (
            @P_bankName, @P_branch, @P_createdBy, GETDATE(), 1, 0
        );

        SET @V_InsertedID = SCOPE_IDENTITY();

        SELECT 'SUCCESS' AS R_Status, @V_InsertedID AS R_InsertedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, NULL AS R_InsertedID, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @V_InsertedID,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_bankMaster_update]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- UPDATE

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_bankMaster_update]
    @P_id INT,
    @P_bankName VARCHAR(100),
    @P_branch VARCHAR(50),
    @P_updatedBy INT
AS
BEGIN
    
    BEGIN TRY
       
        UPDATE sbs_bankMaster
        SET bankName = @P_bankName,
            branch = @P_branch,
            updatedBy = @P_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @P_id AND isDeleted = 0;

        SELECT 'SUCCESS' AS R_Status, @P_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @P_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @P_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_companyMaster_delete]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- DELETE

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_companyMaster_delete]
    @p_id INT,
    @p_updatedBy INT
AS
BEGIN
    BEGIN TRY
        UPDATE sbs_companyMaster
        SET isDeleted = 1, updatedBy = @p_updatedBy, updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_companyMaster_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- GET

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_companyMaster_get]
    @p_companyID INT = NULL,
    @p_search VARCHAR(255) = NULL    
AS
BEGIN

    SELECT id, name, address, phone, email
    FROM sbs_companyMaster
    WHERE isDeleted = 0
      AND (
         (ISNULL(@p_companyID, 0)=0 OR id = @p_companyID)
     AND (name LIKE '%' + ISNULL(@p_search,'') + '%'
    OR email LIKE '%' + ISNULL(@p_search,'') + '%' )
    )
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_companyMaster_insert]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- INSERT

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_companyMaster_insert]
    @p_name VARCHAR(255),
    @p_address VARCHAR(4000),
    @p_phone VARCHAR(20),
    @p_email VARCHAR(100),
    @p_createdBy INT
AS
BEGIN
    DECLARE @V_InsertedID INT = NULL;

    BEGIN TRY
        INSERT INTO sbs_companyMaster (name, address, phone, email, createdBy, createdAt, isActive, isDeleted)
        VALUES (@p_name, @p_address, @p_phone, @p_email, @p_createdBy, GETDATE(), 1, 0);

        SET @V_InsertedID = SCOPE_IDENTITY();

        SELECT 'SUCCESS' AS R_Status, @V_InsertedID AS R_InsertedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @V_InsertedID AS R_InsertedID, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @V_InsertedID,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_companyMaster_update]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- UPDATE

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_companyMaster_update]
    @p_id INT,
    @p_name VARCHAR(255),
    @p_address VARCHAR(4000),
    @p_phone VARCHAR(20),
    @p_email VARCHAR(100),
    @p_updatedBy INT
AS
BEGIN
    BEGIN TRY
        UPDATE sbs_companyMaster
        SET name = @p_name,
            address = @p_address,
            phone = @p_phone,
            email = @p_email,
            updatedBy = @p_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_invoiceDetails_delete]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_invoiceDetails_delete]
    @p_id INT,
    @p_updatedBy INT
AS
BEGIN
    
    BEGIN TRY

        UPDATE sbs_invoiceDetails
        SET isDeleted = 1, updatedBy = @p_updatedBy, updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;
        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_invoiceDetails_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--use SBSApplication
-- EXEC usp_sbs_invoiceDetails_get NULL, 1, 0
CREATE PROCEDURE usp_sbs_invoiceDetails_get
    @p_id INT = NULL,
    @P_companyID INT,
	@P_IsLeviApplicable BIT = 0
AS
BEGIN
    SELECT
		inv.id AS R_id,
        inv.invoiceNo AS R_invoiceNo,
        inv.companyId AS R_companyId,
        inv.subcontractorId AS R_subcontractorId,
        sc.name AS R_subcontractorName,
        inv.productId AS R_productId,
        pm.description AS R_productName,
        pm.unitPrice AS unitPrice,
        inv.invoiceDate AS R_invoiceDate,
        inv.status AS R_status,
        inv.paymentMode AS R_invoiceType,
        inv.quantity AS R_quantity,
        inv.unitAmount AS R_unitAmount,
        inv.totalAmount AS R_totalAmount,
        inv.commissionPercentage AS R_commissionPercentage,
        inv.commissionAmount AS R_commissionAmount,
        inv.paymentMode AS R_paymentMode,
		inv.GroupNumber AS R_GroupNumber,
		inv.LRNumber AS R_LRNumber,
		inv.VehicleNumber AS R_VehicleNumber,
		inv.IsLeviApplicable AS R_IsLeviApplicable,
		inv.Levi AS R_Levi,
		inv.DocketNumber AS R_DocketNumber,
		inv.TrollyQuantity AS R_TrollyQuantity,
		inv.TrollyAmount AS R_TrollyAmount,
        inv.invoiceNo + ' (' + sc.name + ')'  AS R_invoiceNoSelect,
        inv.invoiceNo + ' (' + sc.name + ') ' + FORMAT(inv.invoiceDate,'yyyy-MMM-dd')   AS R_invoiceNoDateSelect,
        inv.createdBy,
        um.username,
        FORMAT((inv.createdAt AT TIME ZONE 'UTC' AT TIME ZONE 'India Standard Time' ),'yyyy-MMM-dd hh:mm:ss tt') createdAt

    FROM sbs_invoiceDetails inv
    INNER JOIN sbs_productMaster pm ON inv.productId = pm.id
    INNER JOIN sbs_subContractor sc ON inv.subcontractorId = sc.id
    INNER JOIN SBS_UserMaster um ON inv.createdBy = um.id
    WHERE inv.isDeleted = 0
      AND inv.companyID = @P_companyID
      AND (@p_id IS NULL OR inv.id = @p_id)
	  AND IsLeviApplicable = @P_IsLeviApplicable;
END
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_invoiceDetails_insert]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- UPDATE sbs_invoiceDetails SET IsLeviApplicable = 1 WHERE id = 22


-- =====================================
-- INVOICE DETAILS CRUD STORED PROCEDURES
-- =====================================
-- INSERT
CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_invoiceDetails_insert]
    @P_invoiceNo VARCHAR(100),
    @P_companyId INT,
    @P_subcontractorId INT,
    @P_productId INT,
    @P_invoiceDate DATETIME2,
    @P_quantity INT,
    @P_unitAmount DECIMAL(10,2),
    @P_totalAmount DECIMAL(10,2),
    @P_commissionPercentage DECIMAL(5,2) = NULL,
    @P_commissionAmount DECIMAL(18,2) = NULL,
    @P_paymentMode VARCHAR(100),
    @P_createdBy INT,
	@P_GroupNumber VARCHAR(50),
	@P_LRNumber VARCHAR(50),
	@P_VehicleNumber VARCHAR(50),
	@P_IsLeviApplicable BIT,
	@P_Levi VARCHAR(20),
	@P_DocketNumber VARCHAR(20),
	@P_TrollyQuantity INT,
	@P_TrollyAmount Decimal(18,2)

AS
BEGIN
    BEGIN TRY
        DECLARE @V_InsertedID INT = NULL;

        INSERT INTO sbs_invoiceDetails (
            invoiceNo, companyId, subcontractorId, productId, invoiceDate, quantity,
            unitAmount, totalAmount, commissionPercentage, commissionAmount,
            paymentMode, createdBy, createdAt, isActive, isDeleted,GroupNumber,LRNumber,VehicleNumber,
			IsLeviApplicable,Levi,DocketNumber,TrollyQuantity,TrollyAmount
        )
        VALUES (
            @P_invoiceNo, @P_companyId, @P_subcontractorId, @P_productId, @P_invoiceDate, @P_quantity,
            @P_unitAmount, @P_totalAmount, @P_commissionPercentage, @P_commissionAmount,
            @P_paymentMode, @P_createdBy, GETDATE(), 1, 0,@P_GroupNumber,@P_LRNumber,@P_VehicleNumber,
			@P_IsLeviApplicable,@P_Levi,@P_DocketNumber,@P_TrollyQuantity,@P_TrollyAmount
        );

        SET @V_InsertedID = SCOPE_IDENTITY();

        SELECT 'SUCCESS' AS R_Status, @V_InsertedID AS R_InsertedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @V_InsertedID AS R_InsertedID, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;
        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @V_InsertedID,
            @p_procName = @V_ProcName;
    END CATCH
END
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_invoiceDetails_update]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- sp_rename 'sbs_invoiceDetails.TrollyAmonut', 'TrollyAmount' 

-- UPDATE 
CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_invoiceDetails_update]
    @p_id INT,
    @P_invoiceNo VARCHAR(100),
    @P_productId INT,
    @P_companyID INT,
    @P_invoiceDate DATETIME2,
    @P_quantity INT,
    @P_unitAmount DECIMAL(10,2),
    @P_totalAmount DECIMAL(10,2),
    @P_commissionPercentage DECIMAL(5,2) = NULL,
    @P_commissionAmount DECIMAL(18,2) = NULL,
    @P_paymentMode VARCHAR(100),
    @P_status VARCHAR(20),
    @P_updatedBy INT,
	@P_GroupNumber VARCHAR(50),
	@P_LRNumber VARCHAR(50),
	@P_VehicleNumber VARCHAR(50),
	@P_Levi VARCHAR(20),
	@P_DocketNumber VARCHAR(20),
	@P_TrollyQuantity INT,
	@P_TrollyAmount Decimal(18,2)
AS
BEGIN
    BEGIN TRY
        UPDATE sbs_invoiceDetails
        SET invoiceNo = @P_invoiceNo,
            productId = @P_productId,
            companyId = @P_companyID,
            invoiceDate = @P_invoiceDate,
            quantity = @P_quantity,
            unitAmount = @P_unitAmount,
            totalAmount = @P_totalAmount,
            commissionPercentage = @P_commissionPercentage,
            commissionAmount = @P_commissionAmount,
            paymentMode = @P_paymentMode,
            status = @P_status,
            updatedBy = @P_updatedBy,
            updatedAt = GETDATE(),
			GroupNumber = @P_GroupNumber,
			LRNumber = @P_LRNumber,
			VehicleNumber = @P_VehicleNumber,
			Levi = @P_Levi,
			DocketNumber = @P_DocketNumber,
			TrollyQuantity = @P_TrollyQuantity,
			TrollyAmount = @P_TrollyAmount

			WHERE id = @p_id AND isDeleted = 0;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;
        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_logError]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =====================================
-- COMPANY MASTER CRUD STORED PROCEDURES
-- =====================================
-- error logging sp
CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_logError]
    @p_errorCode INT,
    @p_errorMsg NVARCHAR(MAX),
    @p_errorLine INT,
    @p_recordId INT = NULL,
    @p_procName SYSNAME
AS
BEGIN
    INSERT INTO sbs_errorLog (
        error_code,
        error_msg,
        error_line,
        record_id,
        procedure_name,
        date_time
    )
    VALUES (
        @p_errorCode,
        @p_errorMsg,
        @p_errorLine,
        @p_recordId,
        @p_procName,
        SYSDATETIME()
    );
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_paymentDetails_delete]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- DELETE
CREATE     PROCEDURE usp_sbs_paymentDetails_delete
    @p_id INT,
    @p_updatedBy INT
AS
BEGIN
    
    BEGIN TRY
       
        UPDATE sbs_paymentDetails
        SET isDeleted = 1,
            updatedBy = @p_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id;

        UPDATE sbs_PaymentDetailsInvoices
        SET isDeleted = 1,
            updatedBy = @p_updatedBy,
            updatedAt = GETDATE()
        WHERE PaymentID = @p_id;

        UPDATE sbs_invoiceDetails
        SET STATUS = 'Pending'
        WHERE ID IN (SELECT InvoiceId FROM sbs_PaymentDetailsInvoices  WHERE PaymentID = @p_id)

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_paymentDetails_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- GET
CREATE PROCEDURE [SBSAppDBUser].[usp_sbs_paymentDetails_get]
    @p_id INT = NULL,
    @P_companyID INT
AS
BEGIN
    SELECT 
        pd.id AS R_id,
        pd.invoiceId AS R_invoiceId,
        inv.invoiceNo AS R_invoiceNo,
        pd.paymentDate AS R_paymentDate,
        pd.fromDate AS R_fromDate,
        pd.toDate AS R_toDate,
        pd.amountPaid AS R_amountPaid,
        pd.paymentMode AS R_paymentMode,
        pd.bankId AS R_bankId,
        bm.bankName AS R_bankName,
        pd.paymentStatus AS R_paymentStatus,
		pd.subcontractorId AS R_subcontractorId,
		(CASE WHEN ISNULL(pd.invoiceId,0) = 0 THEN sc.Name ELSE scInv.Name END) AS R_subcontractorName
    FROM sbs_paymentDetails pd
    LEFT JOIN sbs_invoiceDetails inv ON pd.invoiceId = inv.id -- AND inv.companyID = @P_companyID
    LEFT JOIN sbs_bankMaster bm ON pd.bankId = bm.id
	LEFT JOIN SBS_SubContractor scInv ON inv.subcontractorId = scInv.id
	LEFT JOIN SBS_SubContractor sc ON pd.subcontractorId = sc.id
    WHERE pd.isDeleted = 0
      AND (ISNULL(inv.isDeleted,0) = 0)
      AND (@p_id IS NULL OR pd.id = @p_id);
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_paymentDetails_insert]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =====================================
-- PAYMENT DETAILS CRUD STORED PROCEDURES
-- =====================================
-- INSERT

CREATE   PROCEDURE usp_sbs_paymentDetails_insert
    @P_invoiceNo VARCHAR(50),
    @P_paymentDate DATETIME2,
    @P_fromDate DATETIME2 = NULL,
    @P_toDate DATETIME2 = NULL,
    @P_amountPaid DECIMAL(10,2),
    @P_paymentMode VARCHAR(20) = NULL,
    @P_paymentStatus VARCHAR(20)= NULL,
    @P_bankId INT = NULL,
	@P_subcontractorId INT = NULL,
    @P_createdBy INT
AS
BEGIN
    BEGIN TRY
        DECLARE @V_InsertedID INT = NULL;
        DECLARE @V_invoiceId INT = NULL;
 
        -- Fetch the invoiceId using invoiceNo
        SELECT @V_invoiceId = id
        FROM sbs_invoiceDetails
        WHERE invoiceNo = @P_invoiceNo AND isDeleted = 0;
 


        -- Insert payment record
        INSERT INTO sbs_paymentDetails (
            invoiceId, subcontractorId, paymentDate, fromDate, toDate, amountPaid, paymentMode,
            paymentStatus, bankId, createdBy, createdAt, isActive, isDeleted
        )
        VALUES (
            @V_invoiceId, @P_subcontractorId, @P_paymentDate, @P_fromDate, @P_toDate, @P_amountPaid, @P_paymentMode,
            @P_paymentStatus, @P_bankId, @P_createdBy, GETDATE(), 1, 0
        );
        SET @V_InsertedID = SCOPE_IDENTITY();
 
         IF (ISNULL(@V_invoiceId,0) = 0 AND @V_InsertedID > 0)
         BEGIN
                -- WITH InvoiceRunning AS (
                    SELECT  ID, SUM(totalAmount) OVER (ORDER BY invoiceDate ASC, ID ASC) AS RunningTotal , invoiceDate, totalAmount
                    INTO #InvoiceRunning
                    FROM sbs_invoiceDetails
                    WHERE 
                    ISdeleted = 0 AND STATUS != 'Paid'
                    AND subcontractorId = @P_subcontractorId
                    AND (invoiceDate BETWEEN @P_fromDate AND @P_toDate
                        OR (invoiceDate = @P_paymentDate)
                    )
                 
                INSERT INTO sbs_PaymentDetailsInvoices (PaymentId,InvoiceId,AmountPaid,IsActive,IsDeleted,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt)
                SELECT  @V_InsertedID,ID,TotalAmount,1,0,@P_createdBy, GETDATE(),NULL,NULL
                    FROM #InvoiceRunning
                    WHERE RunningTotal <= @P_amountPaid
                    ORDER BY invoiceDate ASC, ID ASC

                UPDATE sbs_invoiceDetails
                SET STATUS = 'Paid'
                WHERE ID IN (SELECT ID FROM #InvoiceRunning WHERE RunningTotal <= @P_amountPaid)
         END

        SELECT 
            'SUCCESS' AS R_Status, 
            @V_InsertedID AS R_InsertedID, 
            NULL AS R_ErrorNumber, 
            NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);
 
        SELECT 
            'FAIL' AS R_Status, 
            NULL AS R_InsertedID, 
            @V_ErrorNumber AS R_ErrorNumber, 
            @V_ErrorMessage AS R_ErrorMessage;
 
        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @V_InsertedID,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_paymentDetails_update]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- UPDATE
CREATE PROCEDURE usp_sbs_paymentDetails_update
    @p_id INT,
	@p_invoiceNo  VARCHAR(20) = NULL,
    @P_paymentDate DATETIME2,
    @P_fromDate DATETIME2 = NULL,
    @P_toDate DATETIME2 = NULL,
    @P_amountPaid DECIMAL(10,2),
    @P_paymentMode VARCHAR(20),
    @P_bankId INT = NULL,
    @P_paymentStatus VARCHAR(20),
	@P_subcontractorId INT = NULL,
    @P_updatedBy INT
AS
BEGIN
    BEGIN TRY
		 DECLARE @V_invoiceId INT = NULL;
 
        -- Fetch the invoiceId using invoiceNo
        SELECT @V_invoiceId = id
        FROM sbs_invoiceDetails
        WHERE invoiceNo = @P_invoiceNo AND isDeleted = 0;

        UPDATE sbs_paymentDetails
        SET invoiceId = @V_invoiceId,
		    subcontractorId = @P_subcontractorId,
			paymentDate = @P_paymentDate,
            fromDate = @P_fromDate,
            toDate = @P_toDate,
            amountPaid = @P_amountPaid,
            paymentMode = @P_paymentMode,
            bankId = @P_bankId,
            paymentStatus = @P_paymentStatus,
            updatedBy = @P_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id AND isDeleted = 0;

        IF (ISNULL(@p_id,0) > 0)
         BEGIN
                UPDATE  sbs_PaymentDetailsInvoices  SET IsDeleted = 1 WHERE PaymentId = @p_id;
                UPDATE sbs_invoiceDetails
                SET STATUS = 'Pending'
                WHERE ID IN (SELECT InvoiceId FROM sbs_PaymentDetailsInvoices  WHERE PaymentID = @p_id)

                -- WITH InvoiceRunning AS (
                    SELECT  ID, SUM(totalAmount) OVER (ORDER BY invoiceDate ASC, ID ASC) AS RunningTotal , invoiceDate, totalAmount
                    INTO #InvoiceRunning
                    FROM sbs_invoiceDetails
                    WHERE 
                    ISdeleted = 0 -- AND STATUS != 'Paid'
                    AND subcontractorId = @P_subcontractorId
                    AND (invoiceDate BETWEEN @P_fromDate AND @P_toDate
                        OR (invoiceDate = @P_paymentDate)
                    )
                 
                INSERT INTO sbs_PaymentDetailsInvoices (PaymentId,InvoiceId,AmountPaid,IsActive,IsDeleted,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt)
                SELECT  @p_id,ID,TotalAmount,1,0,@P_updatedBy, GETDATE(),NULL,NULL
                    FROM #InvoiceRunning
                    WHERE RunningTotal <= @P_amountPaid
                    ORDER BY invoiceDate ASC, ID ASC;

                UPDATE sbs_invoiceDetails
                SET STATUS = 'Paid'
                WHERE ID IN (SELECT ID FROM #InvoiceRunning WHERE RunningTotal <= @P_amountPaid)
         END

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_productMaster_delete]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- DELETE

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_productMaster_delete]
    @p_id INT,
    @p_updatedBy INT
AS
BEGIN
    

    BEGIN TRY

        UPDATE sbs_productMaster
        SET isDeleted = 1, updatedBy = @p_updatedBy, updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_productMaster_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- GET

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_productMaster_get]
    @P_id INT = NULL,
    @P_companyID INT,
    @P_search VARCHAR(255) = NULL
AS
BEGIN
    SELECT 
        
        id AS R_id,
        companyID AS R_companyID,
        description AS R_description,
        unitPrice AS R_unitPrice

    FROM sbs_productMaster
    WHERE isDeleted = 0
      AND companyID = @P_companyID
      AND (ISNULL(@P_id, 0) = 0 OR id = @P_id)
      AND (description LIKE '%' + ISNULL(@P_search, '') + '%')
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_productMaster_insert]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =====================================
-- PRODUCT MASTER CRUD STORED PROCEDURES
-- =====================================

-- INSERT

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_productMaster_insert]
    @P_description VARCHAR(255),
    @P_unitPrice DECIMAL(10,2),
    @P_companyID INT,
    @P_createdBy INT
AS
BEGIN
    
    DECLARE @V_InsertedID INT = NULL;

    BEGIN TRY

        INSERT INTO sbs_productMaster (companyID, description, unitPrice, createdBy, createdAt, isActive, isDeleted)
        VALUES (@P_companyID, @P_description, @P_unitPrice, @P_createdBy, GETDATE(), 1, 0);

        SET @V_InsertedID = SCOPE_IDENTITY();
        SELECT 'SUCCESS' AS R_Status, @V_InsertedID AS R_InsertedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @V_InsertedID AS R_InsertedID, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @V_InsertedID,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_productMaster_update]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- UPDATE

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_productMaster_update]
    @p_id INT,
    @P_companyID INT,
    @P_description VARCHAR(255),
    @P_unitPrice DECIMAL(10,2),
    @P_updatedBy INT
AS
BEGIN
    

    BEGIN TRY

        UPDATE sbs_productMaster
        SET companyID = @P_companyID,
            description = @P_description,
            unitPrice = @P_unitPrice,
            updatedBy = @P_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id AND isDeleted = 0;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_roleAccess_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_roleAccess_get]
    @p_roleId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        rac.id AS A_id,                         -- Primary key of roleAccessControl
        rac.roleId AS A_roleId,
        rm.roleName AS A_roleName,
        rac.screenName AS A_screenName,
        rac.screenCode AS A_screenCode,
        rac.viewAccess AS A_viewAccess,
        rac.createAccess AS A_createAccess,
        rac.editAccess AS A_editAccess,
        rac.deleteAccess AS A_deleteAccess,
        rac.createdBy AS A_createdBy,
        rac.createdAt AS A_createdAt,
        rac.updatedBy AS A_updatedBy,
        rac.updatedAt AS A_updatedAt
    FROM sbs_roleAccessControl rac
    JOIN sbs_roleMaster rm ON rac.roleId = rm.id
    WHERE 
        rac.roleId = ISNULL(@p_roleId, rac.roleId)
        
    ORDER BY rac.screenCode;
END;

GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_roleAccessControl_insert]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =====================================
-- roleAccessControl CRUD Procedure
-- =====================================
-- insert
CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_roleAccessControl_insert]
    @p_roleId INT,
    @p_screenName VARCHAR(100),
    @p_screenCode VARCHAR(50),
    @p_viewAccess BIT = 0,
    @p_createAccess BIT = 0,
    @p_editAccess BIT = 0,
    @p_deleteAccess BIT = 0,
    @p_createdBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE 
            @V_InsertedID INT

        INSERT INTO sbs_roleAccessControl (
            roleId, screenName, screenCode,
            viewAccess, createAccess, editAccess, deleteAccess,
            createdBy, createdAt
        )
        VALUES (
            @p_roleId, @p_screenName, @p_screenCode,
            @p_viewAccess, @p_createAccess, @p_editAccess, @p_deleteAccess,
            @p_createdBy, GETDATE()
        );

        SET @V_InsertedID = SCOPE_IDENTITY();

        SELECT 'SUCCESS' AS R_Status, @V_InsertedID AS R_InsertedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE 
            @V_ErrorNumber INT = ERROR_NUMBER(),
            @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE(),
            @V_ErrorLine INT = ERROR_LINE(),
            @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, NULL AS R_InsertedID, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = NULL,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_roleAccessControl_update]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- update
CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_roleAccessControl_update]
    @p_id INT,
    @p_viewAccess BIT,
    @p_createAccess BIT,
    @p_editAccess BIT,
    @p_deleteAccess BIT,
    @p_updatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY    
        UPDATE sbs_roleAccessControl
        SET 
            viewAccess = @p_viewAccess,
            createAccess = @p_createAccess,
            editAccess = @p_editAccess,
            deleteAccess = @p_deleteAccess,
            updatedBy = @p_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE 
            @V_ErrorNumber INT = ERROR_NUMBER(),
            @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE(),
            @V_ErrorLine INT = ERROR_LINE(),
            @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_roleMaster_delete]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- DELETE

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_roleMaster_delete]
    @p_id INT,
    @p_updatedBy INT
AS
BEGIN
    BEGIN TRY
        UPDATE sbs_roleMaster
        SET isDeleted = 1, updatedBy = @p_updatedBy, updatedAt = GETDATE()
        WHERE id = @p_id;
        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_roleMaster_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- GET

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_roleMaster_get]
    @p_id INT = NULL
AS
BEGIN
    
    SELECT id, roleName, description
    FROM sbs_roleMaster
    WHERE isDeleted = 0 AND (@p_id IS NULL OR id = @p_id);
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_roleMaster_insert]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =====================================
-- ROLE MASTER CRUD STORED PROCEDURES
-- =====================================

-- INSERT

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_roleMaster_insert]
    @p_roleName VARCHAR(50),
    @p_description VARCHAR(4000),
    @p_createdBy INT
AS
BEGIN
    DECLARE @V_InsertedID INT = NULL;

    BEGIN TRY
        INSERT INTO sbs_roleMaster (roleName, description, createdBy, createdAt, isActive, isDeleted)
        VALUES (@p_roleName, @p_description, @p_createdBy, GETDATE(), 1, 0);
        SET @V_InsertedID = SCOPE_IDENTITY();
        SELECT 'SUCCESS' AS R_Status, @V_InsertedID AS R_InsertedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @V_InsertedID AS R_InsertedID, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @V_InsertedID,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_roleMaster_update]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- UPDATE

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_roleMaster_update]
    @p_id INT,
    @p_roleName VARCHAR(50),
    @p_description VARCHAR(4000),
    @p_updatedBy INT
AS
BEGIN
    BEGIN TRY
        UPDATE sbs_roleMaster
        SET roleName = @p_roleName,
            description = @p_description,
            updatedBy = @p_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id;
        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[USP_SBS_SalaryDetails_delete]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Delete
CREATE   PROCEDURE [SBSAppDBUser].[USP_SBS_SalaryDetails_delete]
    @p_id INT,
    @p_updatedBy INT
AS
BEGIN
    BEGIN TRY

        UPDATE SBS_SalaryDetails
        SET isDeleted = 1,
            updatedBy = @p_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_DeletedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC USP_SBS_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[USP_SBS_SalaryDetails_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Get
CREATE   PROCEDURE [SBSAppDBUser].[USP_SBS_SalaryDetails_get]
    @p_id INT = NULL,
    @p_companyId INT,
    @p_bankid INT = NULL
AS
BEGIN

    SELECT 
        sd.id AS R_id,
        sd.ToliNo AS R_ToliNo,
        sd.amount AS R_amount,
        sd.BankId AS R_BankId,
        sd.date AS R_date,
        sd.companyId AS R_companyId,
        bm.bankname AS R_bankName
    FROM SBS_SalaryDetails sd
    JOIN SBS_BankMaster bm ON sd.bankid = bm.id
    WHERE sd.isDeleted = 0
	AND bm.isDeleted = 0
      AND sd.companyId = @p_companyId
      AND (ISNULL(@p_id, 0)=0 OR sd.id = @p_id)
      AND (ISNULL(@p_bankid, 0)=0 OR sd.bankid = @p_bankid);
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[USP_SBS_SalaryDetails_insert]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =====================================
-- Salary Details CRUD STORED PROCEDURES
-- =====================================

--Insert
CREATE   PROCEDURE [SBSAppDBUser].[USP_SBS_SalaryDetails_insert]
    @p_companyId INT,
	@p_toliNo VARCHAR(50),
    @p_amount DECIMAL(10,2),
    @p_BankId INT,
    @p_date DATETIME2,
    @p_createdBy INT
AS
BEGIN
    BEGIN TRY
        DECLARE @V_InsertedID INT = NULL;

        INSERT INTO SBS_SalaryDetails (companyId, ToliNo, amount, BankId, date, createdBy, createdAt, isActive, isDeleted)
        VALUES (@p_companyId, @p_toliNo, @p_amount, @p_BankId, @p_date, @p_createdBy, GETDATE(), 1, 0);

        SET @V_InsertedID = SCOPE_IDENTITY();
        SELECT 'SUCCESS' AS R_Status, @V_InsertedID AS R_InsertedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, NULL AS R_InsertedID, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC USP_SBS_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @V_InsertedID,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[USP_SBS_SalaryDetails_update]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Update
CREATE   PROCEDURE [SBSAppDBUser].[USP_SBS_SalaryDetails_update]
    @p_id INT,
    @p_toliNo VARCHAR(50),
    @p_amount DECIMAL(10,2),
    @p_BankId INT,
    @p_date DATETIME2,
    @p_companyId INT,
    @p_updatedBy INT
AS
BEGIN
    BEGIN TRY

        UPDATE SBS_SalaryDetails
        SET ToliNo = @p_toliNo,
            amount = @p_amount,
            BankId = @p_BankId,
            date = @p_date,
            companyId = @p_companyId,
            updatedBy = @p_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_UpdatedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC USP_SBS_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_subContractor_delete]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- DELETE

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_subContractor_delete]
    @p_id INT,
    @p_updatedBy INT
AS
BEGIN
    

    BEGIN TRY
        UPDATE sbs_subContractor
        SET isDeleted = 1, updatedBy = @p_updatedBy, updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_subContractor_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- GET
CREATE    PROCEDURE usp_sbs_subContractor_get
    @p_id INT = NULL,
    @p_search VARCHAR(100) = NULL,
    @p_companyID INT
AS
BEGIN
    
    SELECT id, companyId, name
    FROM sbs_subContractor
    WHERE isDeleted = 0
    AND companyId = @p_companyID
    AND (ISNULL(@p_id, 0)=0 OR id = @p_id)
    AND (name LIKE '%' +ISNULL(@p_search,'') + '%')
    ORDER BY Name 
END
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_subContractor_insert]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ==============================================

-- =====================================
-- SUBCONTRACTOR CRUD STORED PROCEDURES
-- =====================================

-- INSERT

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_subContractor_insert]
    @p_companyId INT,
    @p_name VARCHAR(255),
    @p_createdBy INT
AS
BEGIN
    
    DECLARE @V_InsertedID INT = NULL;

    BEGIN TRY
        INSERT INTO sbs_subContractor (companyId, name, createdBy, createdAt, isActive, isDeleted)
        VALUES (@p_companyId, @p_name, @p_createdBy, GETDATE(), 1, 0);

        SET @V_InsertedID = SCOPE_IDENTITY();
        SELECT 'SUCCESS' AS R_Status, @V_InsertedID AS R_InsertedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @V_InsertedID AS R_InsertedID, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @V_InsertedID,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_subContractor_update]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- UPDATE

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_subContractor_update]
    @p_id INT,
    @p_companyId INT,
    @p_name VARCHAR(255),
    @p_updatedBy INT
AS
BEGIN
    

    BEGIN TRY
        UPDATE sbs_subContractor
        SET companyId = @p_companyId,
            name = @p_name,
            updatedBy = @p_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_userAccess_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_userAccess_get]
    @p_userId INT = NULL,
    @p_username VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.id AS U_userId,
        u.username AS U_username,
        u.email AS U_email,
        u.roleMasterId AS U_roleId,
        rm.roleName AS U_roleName,
        rac.screenName AS A_screenName,
        rac.screenCode AS A_screenCode,
        rac.viewAccess AS A_viewAccess,
        rac.createAccess AS A_createAccess,
        rac.editAccess AS A_editAccess,
        rac.deleteAccess AS A_deleteAccess
    FROM sbs_userMaster u
    JOIN sbs_roleMaster rm ON u.roleMasterId = rm.id
    JOIN sbs_roleAccessControl rac ON rac.roleId = rm.id
    WHERE 
        (@p_userId IS NULL OR u.id = @p_userId)
        AND (@p_username IS NULL OR u.username = @p_username)
        AND u.isDeleted = 0
        AND rm.isDeleted = 0
        AND rac.screenCode IS NOT NULL
    ORDER BY rac.screenCode;
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_userMaster_change_password]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--change password

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_userMaster_change_password]
    @p_username VARCHAR(100),
    @p_current_password VARCHAR(100),
    @p_new_password VARCHAR(100)
AS
BEGIN

    DECLARE @new_hashed VARBINARY(64);

    IF dbo.fn_sbs_credentials_check(@p_username, @p_current_password) = 1
    BEGIN
        SET @new_hashed = HASHBYTES('SHA2_512', CONVERT(VARBINARY(MAX), @p_new_password));

        UPDATE sbs_userMaster
        SET password = @new_hashed
        WHERE username = @p_username;

        SELECT 'SUCCESS' AS R_Status, @p_username AS R_targetUser, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END
    ELSE
    BEGIN
        SELECT 'FAIL' AS R_Status, @p_username AS R_targetUser, ERROR_NUMBER() AS R_ErrorNumber, ERROR_MESSAGE() AS R_ErrorMessage;
    END
END
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_userMaster_delete]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- DELETE

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_userMaster_delete]
    @p_id INT,
    @p_updatedBy INT
AS
BEGIN
    

    BEGIN TRY
        UPDATE sbs_userMaster
        SET isDeleted = 1, updatedBy = @p_updatedBy, updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;
        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_userMaster_get]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- GET

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_userMaster_get]
    @p_id INT = NULL,
    @p_search VARCHAR(100) = NULL,
    @p_companyID INT = null
AS
BEGIN

    SELECT 
        um.id, 
        um.companyID, 
        um.roleMasterId, 
        rm.roleName,
        um.isActive,
        um.username,
        um.email
    FROM sbs_userMaster um
    JOIN sbs_roleMaster rm
        ON um.roleMasterId = rm.id
    WHERE 
        um.isDeleted = 0
    AND
    rm.isDeleted = 0
    AND
    um.companyId = @p_companyID
    AND(
    (ISNULL(@p_id, 0)=0 OR um.id = @p_id)
        AND 
    (um.username LIKE '%' + ISNULL(@p_search,'') + '%'
    OR um.email LIKE '%' + ISNULL(@p_search,'') + '%')
    )
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_userMaster_insert]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =====================================
-- USER MASTER CRUD STORED PROCEDURES
-- =====================================

-- insert

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_userMaster_insert]
    @p_roleMasterId INT,
    @p_companyID INT,
    @p_username VARCHAR(50),
    @p_email VARCHAR(100),
    @p_Password VARCHAR(255), 
    @p_createdBy INT
AS
BEGIN
    
    DECLARE @V_InsertedID INT = NULL;

    BEGIN TRY
        INSERT INTO sbs_userMaster (
            roleMasterId, companyId, username, email, password, createdBy, createdAt, isActive, isDeleted
        )
        VALUES (
            @p_roleMasterId, @p_companyID, @p_username, @p_email, HASHBYTES('SHA2_512', CONVERT(VARBINARY(MAX), @p_Password)), @p_createdBy, GETDATE(), 1, 0
        );

        SET @V_InsertedID = SCOPE_IDENTITY();
        SELECT 'SUCCESS' AS R_Status, @V_InsertedID AS R_InsertedID, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @V_InsertedID AS R_InsertedID, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;
        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @V_InsertedID,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_userMaster_isActive]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- isActive

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_userMaster_isActive]
    @p_id INT,
    @p_isActive INT
AS
BEGIN
    BEGIN TRY
        UPDATE sbs_userMaster
        SET isActive = @p_isActive
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_userMaster_login]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- usp_sbs_userMaster_login 'superadmin','superadmin'

CREATE PROCEDURE [SBSAppDBUser].[usp_sbs_userMaster_login]
    @p_username VARCHAR(100),
    @p_password VARCHAR(100)
AS
BEGIN
	DECLARE @PassCheckValue BIT 
	SET @PassCheckValue = (SELECT SBSAppDBUser.fn_sbs_credentials_check(@p_username, @p_password))
    IF (@PassCheckValue = 1)
    BEGIN
        SELECT  
            'True'    AS    isValid,
            id AS user_Id,
            companyID AS company_ID
        FROM sbs_userMaster
        WHERE username = @p_username
        AND isDeleted = 0
        AND isActive = 1;
    END
    ELSE
    BEGIN
        SELECT 
            'False' AS isValid,
             NULL AS user_ID,
             NULL AS companyID;
    END
END

GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_sbs_userMaster_update]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- UPDATE

CREATE   PROCEDURE [SBSAppDBUser].[usp_sbs_userMaster_update]
    @p_id INT,
    @p_roleMasterId INT,
    @p_companyID INT,
    @p_username VARCHAR(50),
    @p_email VARCHAR(100),
    @p_updatedBy INT
AS
BEGIN
    

    BEGIN TRY
        UPDATE sbs_userMaster
        SET roleMasterId = @p_roleMasterId,
            companyId = @p_companyID,
            username = @p_username,
            email = @p_email,
            updatedBy = @p_updatedBy,
            updatedAt = GETDATE()
        WHERE id = @p_id;

        SELECT 'SUCCESS' AS R_Status, @p_id AS R_targetId, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY
    BEGIN CATCH
        DECLARE @V_ErrorNumber INT = ERROR_NUMBER();
        DECLARE @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @V_ErrorLine INT = ERROR_LINE();
        DECLARE @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, @p_id AS R_targetId, @V_ErrorNumber AS R_ErrorNumber, @V_ErrorMessage AS R_ErrorMessage;
        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @p_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_SendCompanyEmail]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- email 


CREATE   PROCEDURE [SBSAppDBUser].[usp_SendCompanyEmail]
    @company_id INT,
    @to_email NVARCHAR(100),
    @cc_email NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @subject NVARCHAR(200),
        @body NVARCHAR(MAX),
        @profile_name NVARCHAR(50) = 'SmartBillingMail',     -- check your profile name
        @invoiceQuery NVARCHAR(MAX),
        @paymentQuery NVARCHAR(MAX),
        @invoiceFile NVARCHAR(200),
        @paymentFile NVARCHAR(200),
        @companyIdStr NVARCHAR(20),
        @companyName NVARCHAR(100)

    BEGIN TRY
        SELECT @companyName = name 
        FROM sbs_companyMaster 
        WHERE id = @company_id;

        SET @companyIdStr = CAST(@company_id AS NVARCHAR(20))

        SET @invoiceFile = 'InvoiceData_' + @companyName + '_' + @companyIdStr + '.csv'
        SET @paymentFile = 'PaymentData_' + @companyName + '_' + @companyIdStr + '.csv'

        SET @invoiceQuery = ' SET NOCOUNT ON

SELECT 
    c.id AS [Company ID],
    c.name AS [Company Name],
    i.id AS [Invoice ID],
    CONVERT(VARCHAR(10), i.invoiceDate, 120) AS [Invoice Date],
    i.status AS [Status],
    i.quantity AS [Quantity],
    i.totalAmount AS [Total Amount]
FROM 
    dbo.sbs_companyMaster c
JOIN 
    dbo.sbs_invoiceDetails i ON c.id = i.companyId
WHERE 
    c.id = ' + @companyIdStr + '
ORDER BY 
    i.invoiceDate DESC';

        SET @paymentQuery = 'SET NOCOUNT ON

SELECT 
    c.id AS [Company ID],
    c.name AS [Company Name],
    b.bankName AS [Bank Name],
    CONVERT(VARCHAR(10), p.paymentDate, 120) AS [Payment Date],
    p.paymentMode AS [Payment Mode],
    p.amountPaid AS [Amount Paid],
    i.id AS [Invoice ID]
FROM 
    dbo.sbs_companyMaster c
JOIN 
    dbo.sbs_invoiceDetails i ON c.id = i.companyId
JOIN 
    dbo.sbs_paymentDetails p ON i.id = p.invoiceId
LEFT JOIN 
    dbo.sbs_bankMaster b ON p.bankId = b.id
WHERE 
    c.id = ' + @companyIdStr + '
ORDER BY 
    p.paymentDate DESC';

        SET @subject = 'Invoice & Payment Report for ' + @companyName + ' (ID: ' + @companyIdStr + ')'

        SET @body = 
            'Dear Team,<br><br>' +
            'Please find attached the Invoice and Payment reports for:<br>' +
            '<b>Company Name:</b> ' + @companyName + '<br>' +
            '<b>Company ID:</b> ' + @companyIdStr + '<br><br>' +
            'Regards,<br>Smart Billing System';

        -- Send invoice report
        EXEC msdb.dbo.usp_send_dbmail
            @profile_name = @profile_name,
            @recipients = @to_email,
            @copy_recipients = @cc_email,
            @subject = @subject,
            @body = @body,
            @body_format = 'HTML',
            @execute_query_database = 'ClientDb',         --chech your  database name
            @query = @invoiceQuery,
            @query_attachment_filename = @invoiceFile,
            @attach_query_result_as_file = 1,
            @query_result_separator = ':',
            @query_result_no_padding = 1,
            @query_result_header = 1,
            @query_result_width = 32767,
            @exclude_query_output = 1;

        -- Send payment report
        EXEC msdb.dbo.usp_send_dbmail
            @profile_name = @profile_name,
            @recipients = @to_email,
            @copy_recipients = @cc_email,
            @subject = @subject,
            @body = @body,
            @body_format = 'HTML',
            @execute_query_database = 'ClientDb',         --check your database name
            @query = @paymentQuery,
            @query_attachment_filename = @paymentFile,
            @attach_query_result_as_file = 1,
            @query_result_separator = ':',
            @query_result_no_padding = 1,
            @query_result_header = 1,
            @query_result_width = 32767,
            @exclude_query_output = 1;

        SELECT 'SUCCESS' AS R_Status, NULL AS R_ErrorNumber, NULL AS R_ErrorMessage;
    END TRY

    BEGIN CATCH
        DECLARE 
            @V_ErrorNumber INT = ERROR_NUMBER(),
            @V_ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE(),
            @V_ErrorLine INT = ERROR_LINE(),
            @V_ProcName SYSNAME = OBJECT_NAME(@@PROCID);

        SELECT 'FAIL' AS R_Status, ERROR_NUMBER() AS R_ErrorNumber, ERROR_MESSAGE() AS R_ErrorMessage;

        EXEC usp_sbs_logError 
            @p_errorCode = @V_ErrorNumber,
            @p_errorMsg = @V_ErrorMessage,
            @p_errorLine = @V_ErrorLine,
            @p_recordId = @company_id,
            @p_procName = @V_ProcName;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [SBSAppDBUser].[usp_UnPaidBalancePaymentReport]    Script Date: 2026-05-25 3:11:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ======================================
-- SP: UnPaidBalancePaymentReport (Updated)
-- ======================================
-- EXEC usp_UnPaidBalancePaymentReport NULL, NULL, NULL, 0

CREATE PROCEDURE usp_UnPaidBalancePaymentReport
-- DECLARE    
    @p_subcontractorName VARCHAR(255) = NULL, -- 'jain', --NULL,
    @p_fromDate DATE,
    @p_toDate DATE,
    @p_isLevhiApplicable BIT = 0
 AS
BEGIN
    BEGIN TRY
    
    DECLARE @CashBankMasterID INT = 0
    SET @CashBankMasterID = (SELECT TOP 1 ID 
                            FROM SBS_BankMaster WHERE isDeleted = 0 AND TRIM(BankName) = 'CASH')


        SELECT      
            inv.invoiceDate AS InvoiceDate,
            inv.invoiceNo + ISNULL((' (' + sc.name + ')'),'') ReceiptNumber,
            p.description ProductName,
            sc.name AS SubContractor,
            AVG(inv.quantity) quantity,
            inv.unitAmount,
            FORMAT(AVG(inv.commissionAmount),'N2') commissionAmount,
            FORMAT(AVG(inv.totalAmount - ISNULL(inv.commissionPercentage,0)),'N2') AS InvoiceAmount,
            pd.invoiceId,
            ISNULL((CASE 
                    WHEN (pd.invoiceId IS NOT NULL AND pd.bankId != @CashBankMasterID)  THEN CAST(FORMAT(SUM(pd.amountPaid),'N2') AS VARCHAR(10)) 
                    ELSE '0' END),0) AS Bank,
            ISNULL((CASE WHEN inv.paymentMode = 'CASH' THEN CAST(FORMAT(AVG(inv.totalAmount),'N2') AS VARCHAR(20))
                         WHEN inv.paymentMode = 'BALANCE' AND pd.bankId = @CashBankMasterID 
                                THEN CAST(FORMAT(AVG(pd.amountPaid),'N2') AS VARCHAR(20))
                         ELSE '0' END),0) AS Cash,
            
            ISNULL((CASE WHEN inv.paymentMode = 'CASH' THEN '0' 
                WHEN inv.paymentMode = 'Balance' AND pd.invoiceId IS NOT NULL AND (CAST((AVG(inv.totalAmount) - ISNULL(SUM(pd.amountPaid),0)) AS DECIMAL(18,2)) <= 0) 
                THEN '0' ELSE CAST((AVG(inv.totalAmount) - ISNULL(SUM(pd.amountPaid),0)) AS VARCHAR(10)) END),0) AS Balance
            
        FROM sbs_invoiceDetails inv
        JOIN sbs_subContractor sc ON inv.subcontractorId = sc.id
        JOIN sbs_productMaster p ON inv.productId = p.id
        LEFT JOIN 
        ( SELECT pdd.id, ISNULL(pdd.invoiceId, pdi.invoiceId) invoiceId, 
            (CASE WHEN (ISNULL(pdd.invoiceId,0) = 0) THEN pdi.amountPaid 
              --   WHEN pdd.bankId = @CashBankMasterID THEN 0 
            ELSE pdd.amountPaid END) amountPaid, pdd.bankId
        FROM sbs_paymentDetails pdd
        LEFT JOIN sbs_PaymentDetailsInvoices pdi ON pdd.ID = pdi.PaymentId 
        WHERE pdd.IsDeleted = 0 
        AND ISNULL(pdi.IsDeleted,0) = 0

        ) pd  ON inv.id = pd.invoiceId

         WHERE 
            inv.IsLeviApplicable = @p_isLevhiApplicable
            AND inv.isDeleted = 0 
            AND inv.invoiceDate BETWEEN ISNULL(@p_fromDate,'01-01-1900') AND ISNULL(@p_toDate,GETDATE()) 
            AND (sc.name LIKE '%' + ISNULL(@p_subcontractorName,'') + '%')

        GROUP BY 
        inv.invoiceDate,
            inv.invoiceNo,
            pd.id,
            pd.bankId,
            p.description,
            sc.name,
            inv.unitAmount,
            inv.paymentMode,
            pd.invoiceId

    END TRY
    BEGIN CATCH
        SELECT 'Fail' AS status;
        SELECt ERROR_MESSAGE()
    END CATCH
END
GO
