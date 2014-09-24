CREATE TABLE [Reference].[Country] (
    [CountryId]   INT          IDENTITY (100, 1) NOT NULL,
    [CountryName] VARCHAR (70) NOT NULL,
    [ISO2]        VARCHAR (2)  NULL,
    [ISO3]        VARCHAR (3)  NULL,
    CONSTRAINT [PK_Reference.Country] PRIMARY KEY CLUSTERED ([CountryId] ASC)
);





