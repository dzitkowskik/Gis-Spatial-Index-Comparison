SELECT * FROM pg_index where 
--indexrelid = 'random_points_10000_btree_idx'::regclass 
--OR indexrelid = 'random_points_10000_btree_idx'::regclass
--OR indexrelid = 'random_points_10000_gist_idx'::regclass
--OR indexrelid = 'random_points_100000_btree_idx'::regclass
--OR indexrelid = 'random_points_100000_gist_idx'::regclass
--OR indexrelid = 'random_points_1000_gist_idx'::regclass
--OR indexrelid = 'random_points_1000_btree_idx'::regclass
indexrelid = 'countries_gist_idx'::regclass
OR
indexrelid = 'random_points_10000_gist_idx'::regclass
OR 
indexrelid = 'random_points_10000_btree_idx'::regclass;

UPDATE pg_index SET indislive = true, indisvalid = true where indexrelid = 'countries'::regclass;
UPDATE pg_index SET indislive = true, indisvalid = true where indexrelid = 'random_points_10000_gist_idx'::regclass;
UPDATE pg_index SET indislive = false, indisvalid = false where indexrelid = 'random_points_10000_gist_idx'::regclass;
UPDATE pg_index SET indislive = false, indisvalid = false where indexrelid = 'random_points_10000_btree_idx'::regclass;

SELECT * FROM random_points_10000 A WHERE A.id IN
(
	SELECT id 
	FROM random_points_10000 B 
	ORDER BY ST_DISTANCE(A.geom, B.geom)
	LIMIT 5
)


SELECT * FROM random_points_1000000 A WHERE A.id IN 
(
	SELECT id
	FROM random_points_100000 B
	ORDER BY A.geom <-> B.geom
	LIMIT 5
)

SELECT * FROM countries A WHERE A.gid IN
(
	SELECT gid
	FROM countries B
	WHERE ST_TOUCHES(A.geom, B.geom)
)



DO 
$BODY$
DECLARE
	i INT := 0;
BEGIN
	IF NOT EXISTS 
	(
		SELECT 1
		FROM   pg_class c
		JOIN   pg_namespace n ON n.oid = c.relnamespace
		WHERE  c.relname = 'random_points_100'
		AND    n.nspname = 'public'
	) 
	THEN
		CREATE TABLE random_points_100
		(
			id serial NOT NULL,
			geom geometry,
			CONSTRAINT random_points_100_pk PRIMARY KEY (id),
			CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
			CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
		) WITH (OIDS=FALSE);
		ALTER TABLE random_points_100 OWNER TO postgres;


		FOR i IN 1..100 
		LOOP
			INSERT INTO public.random_points_100(geom)
				VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
		END LOOP;
	END IF;
END
$BODY$;


