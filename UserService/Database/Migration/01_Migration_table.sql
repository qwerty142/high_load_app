CREATE TABLE IF NOT EXISTS users(
       user_id SERIAL,
       user_login TEXT,
       user_password TEXT,
       user_name TEXT,
       user_surname TEXT,
       user_age INT,
       PRIMARY KEY(user_id, user_login)
);