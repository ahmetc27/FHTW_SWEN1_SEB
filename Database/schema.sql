-- Drop tables if they exist (for clean setup)
DROP TABLE IF EXISTS history CASCADE;
DROP TABLE IF EXISTS tournaments CASCADE;
DROP TABLE IF EXISTS users CASCADE;

-- Create users table
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(100) NOT NULL,
    elo INT NOT NULL DEFAULT 100,
    token VARCHAR(100),
    bio TEXT,
    image TEXT
);

-- Create tournaments table
CREATE TABLE tournaments (
    id SERIAL PRIMARY KEY,
    start_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    end_time TIMESTAMP,
    status VARCHAR(20) NOT NULL DEFAULT 'in_progress',
    winner_id INT,
    is_draw BOOLEAN DEFAULT FALSE,
    CONSTRAINT fk_winner FOREIGN KEY (winner_id) REFERENCES users(id)
);

-- Create history table (for exercise records)
CREATE TABLE history (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    pushup_count INT NOT NULL,
    duration INT NOT NULL,
    timestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    tournament_id INT,
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT fk_tournament FOREIGN KEY (tournament_id) REFERENCES tournaments(id)
);