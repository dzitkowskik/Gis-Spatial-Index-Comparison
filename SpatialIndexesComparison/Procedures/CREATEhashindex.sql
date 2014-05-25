CREATE TABLE public.random_points_btreeindex_10000
(
  id serial NOT NULL,
  geom geometry,
  CONSTRAINT random_points_btreeindex_10000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
) WITH (OIDS=FALSE);
ALTER TABLE random_points_btreeindex_10000 OWNER TO postgres;

CREATE INDEX random_points_btreeindex_10000_idx
ON random_points_btreeindex_10000
USING btree (geom);

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..10000 LOOP
   INSERT INTO random_points_btreeindex_10000 (geom)
  VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;


CREATE TABLE public.random_points_btreeindex_100000
(
  id serial NOT NULL,
  geom geometry,
  CONSTRAINT random_points_btreeindex_100000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
) WITH (OIDS=FALSE);
ALTER TABLE random_points_btreeindex_100000 OWNER TO postgres;

CREATE INDEX random_points_btreeindex_100000_idx
ON random_points_btreeindex_100000
USING btree (geom);

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..100000 LOOP
   INSERT INTO random_points_btreeindex_100000 (geom)
  VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;


CREATE TABLE public.random_points_btreeindex_1000000
(
  id serial NOT NULL,
  geom geometry,
  CONSTRAINT random_points_btreeindex_1000000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
) WITH (OIDS=FALSE);
ALTER TABLE random_points_btreeindex_1000000 OWNER TO postgres;

CREATE INDEX random_points_btreeindex_1000000_idx
ON random_points_btreeindex_1000000
USING btree (geom);

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..1000000 LOOP
   INSERT INTO random_points_btreeindex_1000000 (geom)
  VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;


CREATE TABLE public.random_points_btreeindex_2000000
(
  id serial NOT NULL,
  geom geometry,
  CONSTRAINT random_points_btreeindex_2000000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
) WITH (OIDS=FALSE);
ALTER TABLE random_points_btreeindex_2000000 OWNER TO postgres;

CREATE INDEX random_points_btreeindex_2000000_idx
ON random_points_btreeindex_2000000
USING btree (geom);

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..2000000 LOOP
   INSERT INTO random_points_btreeindex_2000000 (geom)
  VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;


CREATE TABLE public.random_points_btreeindex_3000000
(
  id serial NOT NULL,
  geom geometry,
  CONSTRAINT random_points_btreeindex_3000000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
) WITH (OIDS=FALSE);
ALTER TABLE random_points_btreeindex_3000000 OWNER TO postgres;

CREATE INDEX random_points_btreeindex_3000000_idx
ON random_points_btreeindex_3000000
USING btree (geom);

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..3000000 LOOP
   INSERT INTO random_points_btreeindex_3000000 (geom)
  VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;


CREATE TABLE public.random_points_btreeindex_4000000
(
  id serial NOT NULL,
  geom geometry,
  CONSTRAINT random_points_btreeindex_4000000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
) WITH (OIDS=FALSE);
ALTER TABLE random_points_btreeindex_4000000 OWNER TO postgres;

CREATE INDEX random_points_btreeindex_4000000_idx
ON random_points_btreeindex_4000000
USING btree (geom);

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..4000000 LOOP
   INSERT INTO random_points_btreeindex_4000000 (geom)
  VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;


CREATE TABLE public.random_points_btreeindex_5000000
(
  id serial NOT NULL,
  geom geometry,
  CONSTRAINT random_points_btreeindex_5000000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
) WITH (OIDS=FALSE);
ALTER TABLE random_points_btreeindex_5000000 OWNER TO postgres;

CREATE INDEX random_points_btreeindex_5000000_idx
ON random_points_btreeindex_5000000
USING btree (geom);

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..5000000 LOOP
   INSERT INTO random_points_btreeindex_5000000 (geom)
  VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;