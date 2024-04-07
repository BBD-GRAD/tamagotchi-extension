-- Drop the table if it exists
IF OBJECT_ID('dbo.example_table', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.example_table;
END
GO

-- Create the table
CREATE TABLE dbo.example_table (
    id INT PRIMARY KEY,
    name VARCHAR(100)
);
GO

-- Insert data into the table
INSERT INTO dbo.example_table (id, name) VALUES (1, 'Example 1');
INSERT INTO dbo.example_table (id, name) VALUES (2, 'Example 2');
GO
