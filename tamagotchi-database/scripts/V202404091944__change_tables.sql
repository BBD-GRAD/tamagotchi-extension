ALTER TABLE dbo.Pets DROP CONSTRAINT pets_status_id_foreign;
ALTER TABLE dbo.Owners DROP CONSTRAINT owners_setting_id_foreign;
ALTER TABLE dbo.Pets DROP CONSTRAINT pets_owner_id_foreign;

DROP TABLE dbo.Statuses;
DROP TABLE dbo.Settings;
DROP TABLE dbo.Owners;
DROP TABLE dbo.Pets;

CREATE TABLE dbo.Themes (
    theme_id INT NOT NULL,
    theme_name VARCHAR(50) NOT NULL,
    CONSTRAINT themes_theme_id_primary PRIMARY KEY (theme_id)
);

CREATE TABLE dbo.Users (
    user_id VARCHAR(50) NOT NULL,
    theme_id INT NOT NULL,
    email VARCHAR(50) NOT NULL,
    highscore INT NOT NULL,
    CONSTRAINT users_user_id_primary PRIMARY KEY (user_id),
    CONSTRAINT users_theme_id_foreign FOREIGN KEY (theme_id) REFERENCES dbo.Themes (theme_id)
);

CREATE TABLE dbo.Pets (
    pet_id INT NOT NULL,
    user_id VARCHAR(50) NOT NULL,
    pet_name VARCHAR(50) NOT NULL,
    xp INT NOT NULL,
    health INT NOT NULL,
    food INT NOT NULL,
    water INT NOT NULL,
    stamina INT NOT NULL,
    CONSTRAINT pets_pet_id_primary PRIMARY KEY (pet_id),
    CONSTRAINT pets_user_id_foreign FOREIGN KEY (user_id) REFERENCES dbo.Users (user_id)
);
