using Dapper;
using ERP.DAL.Interface.ICompany;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.RWSplitting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;

namespace ERP.DAL.Implement.Company
{
    public class QualificationManager : IQualificationManager
    {
        public QualificationManager(Environment.GlobalConfig.DB.FromType fromType) { }

        public IList<SupplierInformationInfo> GetSupplierQualificationBySupplierId(Guid supplierId)
        {
            IList<SupplierInformationInfo> list = new List<SupplierInformationInfo>();
            const string SQL = "SELECT [ID],[IdentifyID],[QualificationType],[Number],[Path],[UploadDate],[OverdueDate],[Type],[ExtensionName] ,[FilialeID] FROM [Information] WHERE [IdentifyID]=@supplierId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<SupplierInformationInfo>(SQL, new { supplierId = supplierId }).AsList();
            }
        }

        public bool Insert(SupplierInformationInfo supplierInformationInfo)
        {
            const string SQL = "INSERT INTO Information ([ID],[IdentifyID],[QualificationType],[Number],[Path],[UploadDate],[OverdueDate],[Type],[ExtensionName],[FilialeID]) VALUES (@ID,@IdentifyID,@QualificationType,@Number,@Path,@UploadDate,@OverdueDate,@Type,@ExtensionName,@FilialeID);";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new {
                    ID = supplierInformationInfo.ID,
                    IdentifyID = supplierInformationInfo.IdentifyId,
                    QualificationType = supplierInformationInfo.QualificationType,
                    Number = supplierInformationInfo.Number,
                    Path = supplierInformationInfo.Path,
                    UploadDate = supplierInformationInfo.UploadDate,
                    OverdueDate = supplierInformationInfo.OverdueDate,
                    Type = supplierInformationInfo.Type,
                    ExtensionName = supplierInformationInfo.ExtensionName,
                    FilialeID = supplierInformationInfo.FilialeID,
                }) > 0;
            }
        }

        public bool Delete(Guid supplierInformationId)
        {
            const string SQL = "DELETE Information WHERE ID=@ID";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    ID = supplierInformationId
                }) > 0;
            }
        }

        public bool Update(SupplierInformationInfo supplierInformationInfo)
        {
            const string SQL = "UPDATE Information SET [Number]=@Number,[Path]=@Path,[UploadDate]=@UploadDate,[OverdueDate]=@OverdueDate,[Type]=@Type,[ExtensionName]=@ExtensionName,[FilialeID]=@FilialeID WHERE ID=@ID ;";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    ID = supplierInformationInfo.ID,
                    Number = supplierInformationInfo.Number,
                    Path = supplierInformationInfo.Path,
                    UploadDate = supplierInformationInfo.UploadDate,
                    OverdueDate = supplierInformationInfo.OverdueDate,
                    Type = supplierInformationInfo.Type,
                    ExtensionName = supplierInformationInfo.ExtensionName,
                    FilialeID = supplierInformationInfo.FilialeID,
                }) > 0;
            }
        }
    }
}
