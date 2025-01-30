CREATE OR REPLACE FUNCTION create_user(
    p_login TEXT,
    p_password TEXT,
    p_name TEXT,
    p_surname TEXT,
    p_age INT
) 
RETURNS INT AS $$
DECLARE 
    new_user_id INT;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM users WHERE user_login = p_login) THEN
        INSERT INTO users (user_login, user_password, user_name, user_surname, user_age)
        VALUES (p_login, p_password, p_name, p_surname, p_age)
        RETURNING user_id INTO new_user_id;

        RETURN new_user_id;
    ELSE
        RETURN NULL;
    END IF;
END;
$$ LANGUAGE plpgsql;