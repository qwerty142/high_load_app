CREATE OR REPLACE FUNCTION get_user_by_name(
    p_name TEXT,
    p_surname TEXT
)
RETURNS TABLE("id" INT, "login" TEXT, "password" TEXT, "name" TEXT, "surname" TEXT, "age" INT) AS $$
BEGIN
    RETURN QUERY
        (
            SELECT user_id, user_login, user_password, user_name, user_surname, user_age
            FROM users
            WHERE user_name = p_name AND user_surname = p_surname
        );
END;
$$ LANGUAGE plpgsql;