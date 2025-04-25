using System.Data;
using Npgsql;
using SEB.DTOs;
using SEB.Interfaces;
using SEB.Models;

namespace SEB.Repositories;

public class TournamentRepository : BaseRepository, ITournamentRepository
{
    public Tournament? GetCurrentTournament()
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id, start_time, end_time, status FROM tournaments WHERE status=@status";
        AddParameterWithValue(command, "@status", DbType.String, "active");

        using IDataReader reader = command.ExecuteReader();
        
        if(reader.Read())
        {
            return new Tournament
            {
                Id = reader.GetInt32(0),
                StartTime = reader.GetDateTime(1),
                EndTime = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                Status = reader.GetString(3)
            };
        }
        return null;
    }

    public Tournament StartNewTournament()
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO tournaments (start_time, status) VALUES (@startTime, @status) RETURNING id, start_time, end_time, status";
        AddParameterWithValue(command, "@startTime", DbType.DateTime, DateTime.Now);
        AddParameterWithValue(command, "@status", DbType.String, "active");

        using IDataReader reader = command.ExecuteReader();
        if(reader.Read())
        {
            return new Tournament
            {
                Id = reader.GetInt32(0),
                StartTime = reader.GetDateTime(1),
                EndTime = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                Status = reader.GetString(3)
            };
        }
        throw new Exception("Tournament creation failed.");
    }

    public TournamentParticipant? GetParticipant(int tId, int uId)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT tournament_id, user_id, total_count, total_duration FROM tournament_participants WHERE tournament_id=@tId AND user_id=@uId";
        AddParameterWithValue(command, "@tId", DbType.Int32, tId);
        AddParameterWithValue(command, "@uId", DbType.Int32, uId);

        using IDataReader reader = command.ExecuteReader();
        if(reader.Read())
        {
            return new TournamentParticipant
            {
                TournamentId = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                TotalCount = reader.GetInt32(2),
                TotalDuration = reader.GetInt32(3)
            };
        }
        return null;
    }

    public void AddParticipant(int tournamentId, int userId, int count, int duration)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = 
                    "INSERT INTO tournament_participants (tournament_id, user_id, total_count, total_duration) " + 
                    "VALUES (@tournamentId, @userId, @count, @duration)";

        AddParameterWithValue(command, "@tournamentId", DbType.Int32, tournamentId);
        AddParameterWithValue(command, "@userId", DbType.Int32, userId);
        AddParameterWithValue(command, "@count", DbType.Int32, count);
        AddParameterWithValue(command, "@duration", DbType.Int32, duration);

        int affectedRows = command.ExecuteNonQuery();
        if(affectedRows == 0)
            throw new Exception("Update failed.");
    }

    public void UpdateParticipant(int tournamentId, int userId, TournamentParticipant participant)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = 
                    "UPDATE tournament_participants SET total_count = @count, total_duration = @duration " +
                    "WHERE tournament_id = @tId AND user_id = @uId";

        AddParameterWithValue(command, "@tId", DbType.Int32, tournamentId);
        AddParameterWithValue(command, "@uId", DbType.Int32, userId);
        AddParameterWithValue(command, "@count", DbType.Int32, participant.TotalCount);
        AddParameterWithValue(command, "@duration", DbType.Int32, participant.TotalDuration);
        
        int affectedRows = command.ExecuteNonQuery();
        if(affectedRows == 0)
            throw new Exception("Update failed.");
    }

    public void EndTournament(int tournamentId)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE tournaments SET status=@status, end_time = @endTime WHERE id=@tId";
        AddParameterWithValue(command, "@tId", DbType.Int32, tournamentId);
        AddParameterWithValue(command, "@endTime", DbType.DateTime, DateTime.Now);
        AddParameterWithValue(command, "@status", DbType.String, "ended");
        
        int affectedRows = command.ExecuteNonQuery();
        if(affectedRows == 0)
            throw new Exception("Failed to end tournament");
    }
}