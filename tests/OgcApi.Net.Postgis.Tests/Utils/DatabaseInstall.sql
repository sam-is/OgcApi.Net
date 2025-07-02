CREATE EXTENSION IF NOT EXISTS postgis;

CREATE TABLE IF NOT EXISTS polygons (
    id serial NOT NULL,
    geom geometry,
    name character varying(255) COLLATE pg_catalog."default",
    num integer,
    s double precision,
    date timestamp without time zone,
    CONSTRAINT polygons_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

INSERT INTO polygons (geom, name, num, s, date)
VALUES (ST_GeomFromText('POLYGON((0 0, 0 1000000, 1000000 1000000, 1000000 0, 0 0))', 3857), 'Simple polygon', 1, 0.25, '2020-01-01'),
       (ST_GeomFromText('POLYGON((2000000 0, 2000000 1000000, 3000000 1000000, 3000000 0, 2000000 0), (2250000 250000, 2250000 750000, 2750000 750000, 2750000 250000, 2250000 250000))', 3857), 'Polygon with hole', 2, 1.25, '2021-01-01'),
	   (ST_GeomFromText('MULTIPOLYGON(((0 2000000, 0 3000000, 1000000 3000000, 1000000 2000000, 0 2000000)), ((1250000 2250000, 1250000 2750000, 1750000 2750000, 1750000 2250000, 1250000 2250000)))', 3857), 'MultiPolygon with two parts', 3, 12.25, '2022-01-01'),
	   (ST_GeomFromText('MULTIPOLYGON(((2000000 2000000, 2000000 3000000, 3000000 3000000, 3000000 2000000, 2000000 2000000), (2250000 2250000, 2250000 2750000, 2750000 2750000, 2750000 2250000, 2250000 2250000)), ((3250000 2250000, 3250000 2750000, 3750000 2750000, 3750000 2250000, 3250000 2250000)))', 3857), 'MultiPolygon with two parts, one with hole', 4, 113, '2023-01-01');

CREATE TABLE IF NOT EXISTS linestrings (
	id serial NOT NULL,
    geom geometry,
    name character varying(255) COLLATE pg_catalog."default",
	CONSTRAINT linestrings_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

INSERT INTO linestrings (geom, name)
VALUES (ST_GeomFromText('LINESTRING(4000000 0, 4000000 1000000)', 3857), 'Simple LineString'),
       (ST_GeomFromText('MULTILINESTRING((4000000 2000000, 4000000 3000000), (5000000 2000000, 5000000 3000000))', 3857), 'MiltiLineString');

CREATE TABLE IF NOT EXISTS points (
	id serial NOT NULL,
    geom geometry,
    name character varying(255) COLLATE pg_catalog."default",
	CONSTRAINT points_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

INSERT INTO points (geom, name)
VALUES (ST_GeomFromText('POINT(500000 500000)', 3857), 'Simple point'),
       (ST_GeomFromText('MULTIPOINT((500000 2500000), (2500000 500000), (2500000 2500000))', 3857), 'MiltiPoint');

CREATE TABLE IF NOT EXISTS empty_table (
	id serial NOT NULL,
    geom geometry,
    name character varying(255) COLLATE pg_catalog."default",
	CONSTRAINT empty_table_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS points_with_api_key (
	id serial NOT NULL,
    geom geometry,
    name character varying(255) COLLATE pg_catalog."default",
	key character varying(255),
	CONSTRAINT points_with_api_key_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

INSERT INTO points_with_api_key (geom, name, key)
VALUES (ST_GeomFromText('POINT(500000 500000)', 3857), 'Point 1', '1'),
       (ST_GeomFromText('POINT(500000 2500000)', 3857), 'Point 2', '2'),
	   (ST_GeomFromText('POINT(2500000 500000)', 3857), 'Point 3', '2'),
	   (ST_GeomFromText('POINT(2500000 2500000)', 3857), 'Point 4', '2');
       
CREATE TABLE IF NOT EXISTS polygons_for_insert (
    id serial NOT NULL,
    geom geometry,
    name character varying(255) COLLATE pg_catalog."default",
    num integer,
    s double precision,
    date timestamp without time zone,
    CONSTRAINT polygons_for_insert_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;