UPDATE pg_index SET indislive = true, indisvalid = true where indexrelid = 'countries_geom_gist'::regclass;


UPDATE pg_index SET indislive = true, indisvalid = true where indexrelid = 'random_points_50000'::regclass;
UPDATE pg_index SET indislive = false, indisvalid = false where indexrelid = 'random_points_50000'::regclass;

SELECT 1 FROM random_points_5000 WHERE ST_DWithin(geom, ST_SETSRID(ST_MAKEPOINT(60, 50), 4326), 0.1)

SELECT 1 
FROM random_points_50000 AS A 
WHERE A.geom IN 
(
	SELECT B.geom
	FROM random_points_50000 AS B
	ORDER BY A.geom <-> B.geom
	LIMIT 5
)



SELECT 1 
FROM countries AS A 
WHERE A.geom IN 
(
	SELECT B.geom
	FROM (SELECT * FROM countries WHERE ST_INTERSECTS(ST_ENVELOPE(geom), A.geom)) AS B
	WHERE ST_TOUCHES(A.geom,B.geom)
)

DO
$$
DECLARE
BB GEOMETRY
BEGIN

SET GEOMETRY = 

SELECT 1 
FROM countries AS A 
WHERE A.geom IN 
(
	SELECT B.geom
	FROM (SELECT * FROM countries WHERE ST_INTERSECTS(ST_ENVELOPE(geom), A.geom)) AS B
	WHERE ST_TOUCHES(A.geom,B.geom)
)


END
&&

SELECT * FROM (SELECT * FROM countries LIMIT 5) AS A WHERE A.geom IN 
(
	SELECT B.geom
	FROM (SELECT * FROM countries WHERE A.geom && geom ) AS B
	WHERE ST_TOUCHES(A.geom, B.geom)
)

SELECT 1 FROM
(
SELECT A.geom AS x, B.geom AS y FROM
countries A LEFT JOIN
countries B ON (NOT ST_EQUALS(A.geom, B.geom)) AND A.geom && B.geom
) AS W
WHERE ST_TOUCHES(W.x, W.y) 

SELECT * FROM countries

DO $proc$
DECLARE
  StartTime timestamptz;
  EndTime timestamptz;
  Delta interval;
BEGIN
  StartTime := clock_timestamp();

CREATE INDEX random_points_5000000_gist_idx
   ON public.random_points_5000000 USING gist (geom);
 
  EndTime := clock_timestamp();
  Delta := 1000 * ( extract(epoch from EndTime) - extract(epoch from StartTime) );
  RAISE NOTICE 'Duration in millisecs=%', Delta;
END;
$proc$;