CREATE TABLE public.random_points_noindex_10000
(
  id integer NOT NULL DEFAULT nextval('random_points_noindex_id_seq'::regclass),
  geom geometry,
  CONSTRAINT random_points_noindex_10000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.random_points_noindex
  OWNER TO postgres;

CREATE TABLE public.random_points_noindex_100000
(
  id integer NOT NULL DEFAULT nextval('random_points_noindex_id_seq'::regclass),
  geom geometry,
  CONSTRAINT random_points_noindex_100000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.random_points_noindex
  OWNER TO postgres;

CREATE TABLE public.random_points_noindex_1000000
(
  id integer NOT NULL DEFAULT nextval('random_points_noindex_id_seq'::regclass),
  geom geometry,
  CONSTRAINT random_points_noindex_1000000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.random_points_noindex
  OWNER TO postgres;

CREATE TABLE public.random_points_noindex_2000000
(
  id integer NOT NULL DEFAULT nextval('random_points_noindex_id_seq'::regclass),
  geom geometry,
  CONSTRAINT random_points_noindex_2000000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.random_points_noindex
  OWNER TO postgres;

CREATE TABLE public.random_points_noindex_3000000
(
  id integer NOT NULL DEFAULT nextval('random_points_noindex_id_seq'::regclass),
  geom geometry,
  CONSTRAINT random_points_noindex_3000000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.random_points_noindex
  OWNER TO postgres;

CREATE TABLE public.random_points_noindex_4000000
(
  id integer NOT NULL DEFAULT nextval('random_points_noindex_id_seq'::regclass),
  geom geometry,
  CONSTRAINT random_points_noindex_4000000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.random_points_noindex
  OWNER TO postgres;

CREATE TABLE public.random_points_noindex_5000000
(
  id integer NOT NULL DEFAULT nextval('random_points_noindex_id_seq'::regclass),
  geom geometry,
  CONSTRAINT random_points_noindex_5000000_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.random_points_noindex
  OWNER TO postgres;

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..10000 LOOP
   INSERT INTO public.random_points_noindex_10000(geom)
	VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..100000 LOOP
   INSERT INTO public.random_points_noindex_100000(geom)
	VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..1000000 LOOP
   INSERT INTO public.random_points_noindex_1000000(geom)
	VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..2000000 LOOP
   INSERT INTO public.random_points_noindex_2000000(geom)
	VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..3000000 LOOP
   INSERT INTO public.random_points_noindex_3000000(geom)
	VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..4000000 LOOP
   INSERT INTO public.random_points_noindex_4000000(geom)
	VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;

DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..5000000 LOOP
   INSERT INTO public.random_points_noindex_5000000(geom)
	VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$;