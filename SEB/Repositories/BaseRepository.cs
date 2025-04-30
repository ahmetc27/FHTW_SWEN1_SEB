using System.Data;

namespace SEB.Repositories;
public class BaseRepository
{
    protected readonly string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";

    protected void AddParameterWithValue(IDbCommand command, string parameterName, DbType type,object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.DbType = type;
        parameter.ParameterName = parameterName;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}