CREATE OR REPLACE FUNCTION update_user(
    p_id INT,
    p_password TEXT,
    p_name TEXT,
    p_surname TEXT,
    p_age INT
)
    RETURNS VOID AS $$
BEGIN
    IF EXISTS (SELECT 1 FROM users WHERE user_id = p_id) THEN
        UPDATE users
        SET
            user_password = p_password,
            user_name = p_name,
            user_surname = p_surname,
            user_age = p_age
        WHERE user_id = p_id;
    END IF;
END;
$$ LANGUAGE plpgsql;