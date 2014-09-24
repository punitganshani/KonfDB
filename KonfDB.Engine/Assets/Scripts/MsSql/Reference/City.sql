CREATE TABLE [Reference].[City] (
    [CityId]    INT          NOT NULL,
    [CountryId] INT          NOT NULL,
    [City]      VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Reference.City] PRIMARY KEY CLUSTERED ([CityId] ASC),
    CONSTRAINT [FK_Reference.City_Reference.Country] FOREIGN KEY ([CountryId]) REFERENCES [Reference].[Country] ([CountryId])
);

