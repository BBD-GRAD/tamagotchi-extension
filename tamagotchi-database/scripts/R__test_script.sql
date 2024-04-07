-- Drop the table if it exists
IF OBJECT_ID('example_table', 'U') IS NOT NULL
BEGIN
    DROP TABLE example_table;
END
GO

-- Create the table
CREATE TABLE example_table (
    id INT PRIMARY KEY,
    name VARCHAR(100)
);
GO

-- Insert data into the table
INSERT INTO example_table (id, name) VALUES (1, 'Example 1');
INSERT INTO example_table (id, name) VALUES (2, 'Example 2_v5');
GO
