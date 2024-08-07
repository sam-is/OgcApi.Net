SET NOCOUNT ON;
SET XACT_ABORT ON;

USE [{0}];

CREATE TABLE [dbo].[Polygons] (
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Geom] [geometry] NULL,
	[Name] [nvarchar](255) NULL,
	[Number] [int] NULL,
	[S] [float] NULL,
	[Date] [datetime] NULL
	CONSTRAINT [PK_Polygons] PRIMARY KEY CLUSTERED ([Id] ASC)
);

INSERT INTO [dbo].[Polygons] ([Geom], [Name], [Number], [S], [Date])
VALUES (geometry::STGeomFromText('POLYGON((0 0, 0 1000000, 1000000 1000000, 1000000 0, 0 0))', 3857), 'Simple polygon', 1, 0.25, '2020-01-01'),
       (geometry::STGeomFromText('POLYGON((2000000 0, 2000000 1000000, 3000000 1000000, 3000000 0, 2000000 0), (2250000 250000, 2250000 750000, 2750000 750000, 2750000 250000, 2250000 250000))', 3857), 'Polygon with hole', 2, 1.25, '2021-01-01'),
	   (geometry::STGeomFromText('MULTIPOLYGON(((0 2000000, 0 3000000, 1000000 3000000, 1000000 2000000, 0 2000000)), ((1250000 2250000, 1250000 2750000, 1750000 2750000, 1750000 2250000, 1250000 2250000)))', 3857), 'MultiPolygon with two parts', 3, 12.25, '2022-01-01'),
	   (geometry::STGeomFromText('MULTIPOLYGON(((2000000 2000000, 2000000 3000000, 3000000 3000000, 3000000 2000000, 2000000 2000000), (2250000 2250000, 2250000 2750000, 2750000 2750000, 2750000 2250000, 2250000 2250000)), ((3250000 2250000, 3250000 2750000, 3750000 2750000, 3750000 2250000, 3250000 2250000)))', 3857), 'MultiPolygon with two parts, one with hole', 4, 113, '2023-01-01'),
	   (geometry::STGeomFromText('POLYGON((0 0, 0 500000, 500000 500000, 500000 0, 0 0))', 3857), 'Simple polygon', 1, 0.25, '2024-01-01')

CREATE TABLE [dbo].[LineStrings] (
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Geom] [geometry] NULL,
	[Name] [nvarchar](255) NULL,
	CONSTRAINT [PK_LineStrings] PRIMARY KEY CLUSTERED ([Id] ASC)
);

INSERT INTO [dbo].[LineStrings] ([Geom], [Name])
VALUES (geometry::STGeomFromText('LINESTRING(4000000 2000000, 5000000 1000000)', 3857), 'Simple LineString'),
       (geometry::STGeomFromText('MULTILINESTRING((4000000 2000000, 4000000 3000000), (5000000 2000000, 6000000 3000000))', 3857), 'MiltiLineString');

CREATE TABLE [dbo].[Points] (
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Geom] [geometry] NULL,
	[Name] [nvarchar](255) NULL,
	CONSTRAINT [PK_Points] PRIMARY KEY CLUSTERED ([Id] ASC)
);

INSERT INTO [dbo].[Points] ([Geom], [Name])
VALUES (geometry::STGeomFromText('POINT(500000 500000)', 3857), 'Simple point'),
       (geometry::STGeomFromText('MULTIPOINT((500000 2500000), (2500000 500000), (2500000 2500000))', 3857), 'MiltiPoint');

CREATE TABLE [dbo].[PointsWithApiKey] (
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Geom] [geometry] NULL,
	[Name] [nvarchar](255) NULL,
	[Key] [nvarchar](255) NULL,
	CONSTRAINT [PK_PointsWithKey] PRIMARY KEY CLUSTERED ([Id] ASC)
);

INSERT INTO [dbo].[PointsWithApiKey] ([Geom], [Name], [Key])
VALUES (geometry::STGeomFromText('POINT(500000 500000)', 3857), 'Point 1', '1'),
       (geometry::STGeomFromText('POINT(500000 2500000)', 3857), 'Point 2', '2'),
	   (geometry::STGeomFromText('POINT(2500000 500000)', 3857), 'Point 3', '2'),
	   (geometry::STGeomFromText('POINT(2500000 2500000)', 3857), 'Point 4', '2');	   