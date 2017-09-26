INSERT INTO [dbo].[AspNetRoles]
           ([Name])
     VALUES
           ('Administrator')
GO

INSERT INTO [dbo].[AspNetRoles]
           ([Name])
     VALUES
           ('Member')
GO

INSERT INTO [dbo].[AspNetRoles]
           ([Name])
     VALUES
           ('Banned')
GO

INSERT INTO [dbo].[GeneralSettings]
           ([Key]
           ,[Value])
     VALUES
           ('AzureCategoriesImagesUrlFormat'
           ,'http://127.0.0.1:10000/devstoreaccount1/{0}/{1}/{2}')
GO


INSERT INTO [dbo].[GeneralSettings]
           ([Key]
           ,[Value])
     VALUES
           ('AzureCategoriesContainer'
           ,'categories')
GO


INSERT INTO [dbo].[GeneralSettings]
           ([Key]
           ,[Value])
     VALUES
           ('AzureCategoriesImagesDirectory'
           ,'images')
GO

