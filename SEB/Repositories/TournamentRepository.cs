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
        command.CommandText = "SELECT id, start_time, end_time, status, winner FROM tournaments WHERE status = @status";
        AddParameterWithValue(command, "@status", DbType.String, "active");

        using IDataReader reader = command.ExecuteReader();
        
        if(reader.Read())
        {
            return new Tournament
            {
                Id = reader.GetInt32(0),
                StartTime = reader.GetDateTime(1),
                EndTime = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                Status = reader.GetString(3),
                Winner = reader.IsDBNull(4) ? null : reader.GetString(4)
            };
        }
        return null;
    }

    public Tournament StartNewTournament()
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO tournaments (start_time, status) VALUES (@startTime, @status) RETURNING id, start_time, end_time, status, winner";
        AddParameterWithValue(command, "@startTime", DbType.DateTime, DateTime.UtcNow);
        AddParameterWithValue(command, "@status", DbType.String, "active");

        using IDataReader reader = command.ExecuteReader();
        if(reader.Read())
        {
            return new Tournament
            {
                Id = reader.GetInt32(0),
                StartTime = reader.GetDateTime(1),
                EndTime = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                Status = reader.GetString(3),
                Winner = reader.IsDBNull(4) ? null : reader.GetString(4)
            };
        }
        throw new Exception("Tournament creation failed.");
    }

    public TournamentParticipant? GetParticipant(int tournamentId, int userId)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT tournament_id, user_id, total_count, total_duration FROM tournament_participants WHERE tournament_id = @tournamentId AND user_id = @userId";
        AddParameterWithValue(command, "@tournamentId", DbType.Int32, tournamentId);
        AddParameterWithValue(command, "@userId", DbType.Int32, userId);

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
                    "WHERE tournament_id = @tournamentId AND user_id = @userId";

        AddParameterWithValue(command, "@tournamentId", DbType.Int32, tournamentId);
        AddParameterWithValue(command, "@userId", DbType.Int32, userId);
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
        command.CommandText = "UPDATE tournaments SET status=@status, end_time = @endTime WHERE id=@tournamentId";
        AddParameterWithValue(command, "@tournamentId", DbType.Int32, tournamentId);
        AddParameterWithValue(command, "@endTime", DbType.DateTime, DateTime.UtcNow);
        AddParameterWithValue(command, "@status", DbType.String, "ended");
        
        int affectedRows = command.ExecuteNonQuery();
        if(affectedRows == 0)
            throw new Exception("Failed to end tournament");
    }

    public List<TournamentParticipant> GetParticipants(int tournamentId)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT tournament_id, user_id, total_count, total_duration FROM tournament_participants WHERE tournament_id = @tournamentId";
        AddParameterWithValue(command, "@tournamentId", DbType.Int32, tournamentId);

        using IDataReader reader = command.ExecuteReader();
        var participants = new List<TournamentParticipant>();
        
        while(reader.Read())
        {
            participants.Add(new TournamentParticipant
            {
                TournamentId = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                TotalCount = reader.GetInt32(2),
                TotalDuration = reader.GetInt32(3)
            });
        }
        
        return participants;
    }

    public List<Tournament>? GetAllTournaments()
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id, start_time, end_time, status, winner FROM tournaments WHERE status = @status ORDER BY start_time";
        AddParameterWithValue(command, "@status", DbType.String, "ended");

        using IDataReader reader = command.ExecuteReader();

        List<Tournament> tournaments = new();
        
        while(reader.Read())
        {
            Tournament tournament =  new Tournament
            {
                Id = reader.GetInt32(0),
                StartTime = reader.GetDateTime(1),
                EndTime = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                Status = reader.GetString(3),
                Winner = reader.GetString(4)
            };
            tournaments.Add(tournament);
        }
        return tournaments;
    }

    public void SetWinner(int tournamentId, List<TournamentParticipant> winners)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();

        string winnerNames = string.Join(",", winners.Select(w => w.UserId));

        command.CommandText = "UPDATE tournaments SET winner = @winner WHERE id = @tournamentId";
        AddParameterWithValue(command, "@tournamentId", DbType.Int32, tournamentId);
        AddParameterWithValue(command, "@winner", DbType.String, winnerNames);

        int affectedRows = command.ExecuteNonQuery();
        if(affectedRows == 0)
            throw new Exception("Failed to set tournament winner");
    }
}