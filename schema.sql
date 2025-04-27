CREATE TABLE users (
	id SERIAL PRIMARY KEY,
	username VARCHAR(50) NOT NULL UNIQUE,
	password VARCHAR(50) NOT NULL,
	elo INT NOT NULL DEFAULT 100,
	token VARCHAR(50) DEFAULT NULL,
	name VARCHAR(50),
	bio TEXT NULL,
	image VARCHAR(50)
);

CREATE TABLE tournaments (
    id SERIAL PRIMARY KEY,
    start_time TIMESTAMP NOT NULL,
    end_time TIMESTAMP,
    status VARCHAR(20) NOT NULL DEFAULT 'active', -- 'active', 'ended'
    winner VARCHAR(50)
);

CREATE TABLE history (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
	name VARCHAR(50),
    count INT NOT NULL,
    duration INT NOT NULL,
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	tournament_id INT REFERENCES tournaments(id)
);

CREATE TABLE tournament_participants (
    tournament_id INT NOT NULL REFERENCES tournaments(id),
    user_id INT NOT NULL REFERENCES users(id),
    total_count INT NOT NULL DEFAULT 0,
    total_duration INT NOT NULL DEFAULT 0,
    PRIMARY KEY (tournament_id, user_id)
);