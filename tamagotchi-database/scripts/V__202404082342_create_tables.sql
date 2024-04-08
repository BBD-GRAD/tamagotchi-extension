-- Create the table
CREATE TABLE dbo.Statuses (
    status_id INT NOT NULL,
    status_name VARCHAR(50) NOT NULL,
    CONSTRAINT statuses_status_id_primary PRIMARY KEY (status_id)
);
GO

CREATE TABLE dbo.Settings (
    setting_id BIGINT NOT NULL,
    theme VARCHAR(50) NOT NULL,
    CONSTRAINT settings_setting_id_primary PRIMARY KEY (setting_id)
);
GO

CREATE TABLE dbo.Owners (
    owner_id INT NOT NULL,
    setting_id INT NOT NULL,
    user_id VARCHAR(50) NOT NULL,
    email VARCHAR(50) NOT NULL,
    highscore INT NOT NULL,
    CONSTRAINT owners_owner_id_primary PRIMARY KEY (owner_id),
    CONSTRAINT owners_setting_id_foreign FOREIGN KEY (setting_id) REFERENCES dbo.Settings (setting_id)
);
GO

CREATE TABLE dbo.Pets (
    pet_id INT NOT NULL,
    owner_id INT NOT NULL,
    status_id INT NOT NULL,
    pet_name VARCHAR(50) NOT NULL,
    xp INT NOT NULL,
    health INT NOT NULL,
    hunger INT NOT NULL,
    thirst INT NOT NULL,
    sleep INT NOT NULL,
    happiness INT NOT NULL,
    CONSTRAINT pets_pet_id_primary PRIMARY KEY (pet_id),
    CONSTRAINT pets_owner_id_foreign FOREIGN KEY (owner_id) REFERENCES dbo.Owners (owner_id),
    CONSTRAINT pets_status_id_foreign FOREIGN KEY (status_id) REFERENCES dbo.Statuses (status_id)
);
GO
