using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Client.Application.Interfaces;

namespace Client.Persistence.Repositories
{
    public class CommonRepository : ICommonRepository
    {
        private readonly IDbConnection _db;

        public CommonRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<DataSet> GetBackupDataAsync(int companyId)
        {
            var dataSet = new DataSet();

            if (_db.State != ConnectionState.Open)
            {
                _db.Open();
            }

            using var cmd = _db.CreateCommand();
            cmd.CommandText = "usp_SBS_getBackupData";
            cmd.CommandType = CommandType.StoredProcedure;

            var param = cmd.CreateParameter();
            param.ParameterName = "@company_id";
            param.Value = companyId;
            cmd.Parameters.Add(param);

            if (cmd is DbCommand dbCmd)
            {
                using var reader = await dbCmd.ExecuteReaderAsync();
                int tableIndex = 1;
                do
                {
                    var table = new DataTable($"Table_{tableIndex++}");

                    int fieldCount = reader.FieldCount;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        string colName = reader.GetName(i);
                        if (string.IsNullOrWhiteSpace(colName))
                        {
                            colName = $"Column_{i + 1}";
                        }

                        string uniqueColName = colName;
                        int counter = 1;
                        while (table.Columns.Contains(uniqueColName))
                        {
                            uniqueColName = $"{colName}_{counter++}";
                        }

                        Type colType = reader.GetFieldType(i) ?? typeof(string);
                        table.Columns.Add(uniqueColName, colType);
                    }

                    while (await reader.ReadAsync())
                    {
                        var row = table.NewRow();
                        for (int i = 0; i < fieldCount; i++)
                        {
                            row[i] = reader.GetValue(i);
                        }
                        table.Rows.Add(row);
                    }

                    dataSet.Tables.Add(table);

                } while (await reader.NextResultAsync());
            }
            else
            {
                using var reader = cmd.ExecuteReader();
                int tableIndex = 1;
                do
                {
                    var table = new DataTable($"Table_{tableIndex++}");

                    int fieldCount = reader.FieldCount;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        string colName = reader.GetName(i);
                        if (string.IsNullOrWhiteSpace(colName))
                        {
                            colName = $"Column_{i + 1}";
                        }

                        string uniqueColName = colName;
                        int counter = 1;
                        while (table.Columns.Contains(uniqueColName))
                        {
                            uniqueColName = $"{colName}_{counter++}";
                        }

                        Type colType = reader.GetFieldType(i) ?? typeof(string);
                        table.Columns.Add(uniqueColName, colType);
                    }

                    while (reader.Read())
                    {
                        var row = table.NewRow();
                        for (int i = 0; i < fieldCount; i++)
                        {
                            row[i] = reader.GetValue(i);
                        }
                        table.Rows.Add(row);
                    }

                    dataSet.Tables.Add(table);

                } while (reader.NextResult());
            }

            return dataSet;
        }
    }
}
