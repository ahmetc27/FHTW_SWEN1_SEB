using System.Data;

namespace SEB.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly string connectionString;

        protected BaseRepository()
        {
            connectionString = AppConfig.ConnectionString;
        }

        protected static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}