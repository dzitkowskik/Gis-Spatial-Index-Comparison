PostGis procedures:

// SELECT NEAREST POINTS FROM POINT 60, 40
SELECT id, geom, ST_ASTEXT(geom)
FROM public.random_points_gistindex
WHERE ST_DWithin(geom, ST_SetSRID(ST_MakePoint(60, 40), 4326), 1);

// FILL WITH RANDOM POINTS
DO
$do$
DECLARE i INT := 0;
BEGIN
FOR i IN 1..1000000 LOOP
   INSERT INTO public.random_points_gistindex(geom)
	VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
END LOOP;
END
$do$

// GISTINDEX RANDOM TABLE CREATE
CREATE TABLE random_points_gistindex
(
  id serial NOT NULL,
  geom geometry,
  CONSTRAINT random_points_gistindex_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE random_points_gistindex OWNER TO postgres;

-- Index: terrain_points_geom_idx

-- DROP INDEX terrain_points_geom_idx;

 CREATE INDEX random_points_gistindex_idx
   ON random_points_gistindex
   USING gist (geom);

-- NOINDEX RANDOM TABLE CREATE
CREATE TABLE public.random_points_noindex
(
  id integer NOT NULL DEFAULT nextval('random_points_noindex_id_seq'::regclass),
  geom geometry,
  CONSTRAINT random_points_noindex_pk PRIMARY KEY (id),
  CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
  CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.random_points_noindex
  OWNER TO postgres;