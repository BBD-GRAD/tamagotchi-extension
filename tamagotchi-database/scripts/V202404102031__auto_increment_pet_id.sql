DROP TABLE dbo.Pets;

CREATE TABLE dbo.Pets (
    pet_id INT IDENTITY(1,1) NOT NULL,
    user_id VARCHAR(50) NOT NULL,
    pet_name VARCHAR(50) NOT NULL,
    xp BIGINT NOT NULL,
    health FLOAT NOT NULL,
    food FLOAT NOT NULL,
    water FLOAT NOT NULL,
    stamina FLOAT NOT NULL,
    CONSTRAINT pets_pet_id_primary PRIMARY KEY (pet_id),
    CONSTRAINT pets_user_id_foreign FOREIGN KEY (user_id) REFERENCES dbo.Users (user_id)
);
