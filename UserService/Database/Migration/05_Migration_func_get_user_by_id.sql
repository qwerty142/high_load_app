CREATE OR REPLACE FUNCTION get_user_by_id(
    p_id INT
) 
RETURNS TABLE("id" INT, "login" TEXT, "password" TEXT, "name" TEXT, "surname" TEXT, "age" INT) AS $$
BEGIN
    RETURN QUERY
        (
            SELECT user_id, user_login, user_password, user_name, user_surname, user_age
            FROM users
            WHERE user_id = p_id
        );
END;
$$ LANGUAGE plpgsql;