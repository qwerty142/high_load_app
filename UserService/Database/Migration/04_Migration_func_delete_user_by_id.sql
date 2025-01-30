CREATE OR REPLACE FUNCTION delete_user_by_id(
    p_id INT
)
    RETURNS VOID AS $$
BEGIN
    IF EXISTS (SELECT 1 FROM users WHERE user_id = p_id) THEN
        DELETE FROM users
        WHERE user_id = p_id;
    END IF;
END;
$$ LANGUAGE plpgsql;