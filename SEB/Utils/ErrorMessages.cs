namespace SEB.Utils;

public static class ErrorMessages
{
    // User registration and authentication
    public const string UsernameTaken = "Username is already taken";
    public const string InvalidCredentials = "Invalid username or password";
    public const string InvalidUsernameOrToken = "Access denied: invalid username or token";
    
    // Request validation
    public const string InvalidJson = "Invalid JSON body";
    public const string InvalidRequestBody = "Invalid request body";
    public const string InvalidUsername = "Username invalid in request line";
    public const string UsernameRequired = "Username must not be empty";
    public const string PasswordRequired = "Password must not be empty";
    public const string UserIdNotFound = "Could not find user ID for the provided token";
    public const string InvalidCount = "Count must be positive number";
    public const string InvalidDuration = "Duration must be between 1 and 120 seconds";
    
    // Authentication
    public const string TokenRequired = "Header token required";
    public const string InvalidToken = "Invalid token";
    public const string TokenAlreadyExists = "Token already exists";
    public const string TokenNotFound = "Token does not exist";
    
    // Endpoints
    public const string InvalidPath = "Invalid path";
    public const string EndpointNotFound = "Endpoint not found";
    
    // Stats, History and Tournament
    public const string StatsNotFound = "Could not retrieve user stats";
    public const string TournamentNotFound = "Tournament not found";
    public const string NoParticipants = "No participants in tournament";
}