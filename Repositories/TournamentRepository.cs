using System.Data;
using Npgsql;
using SEB.Models;

namespace SEB.Repositories
{
    public class TournamentRepository(string connectionString)
    {
        public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        public Tournament? GetCurrentTournament()
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            Tournament? tournament = null;

            using(IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT id, start_time, end_time, status, winner_id, is_draw
                    FROM tournaments 
                    WHERE status = 'in_progress'
                    ORDER BY start_time DESC
                    LIMIT 1";

                using IDataReader reader = command.ExecuteReader();
                if(reader.Read())
                {
                    tournament = new Tournament
                    {
                        Id = reader.GetInt32(0),
                        StartTime = reader.GetDateTime(1),
                        EndTime = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                        Status = reader.GetString(3),
                        WinnerId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                        IsDraw = reader.GetBoolean(5)
                    };
                }
                else return null;
            }
            if(tournament != null)
            {
                using(IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT u.id, u.username, COALESCE(SUM(h.pushup_count), 0) as total_pushups
                        FROM users u
                        JOIN history h ON u.id = h.user_id
                        WHERE h.tournament_id = @tournamentId
                        GROUP BY u.id, u.username
                        ORDER BY total_pushups DESC";
                        
                    AddParameterWithValue(command, "tournamentId", DbType.Int32, tournament.Id);
                    
                    using IDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        tournament.Participants.Add(new Participant
                        {
                            UserId = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            TotalPushups = reader.GetInt32(2)
                        });
                    }
                }
            }
            
            return tournament;
        }
    }
}