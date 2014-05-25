DO $$
BEGIN
IF NOT EXISTS 
(
    SELECT 1
    FROM   pg_class c
    JOIN   pg_namespace n ON n.oid = c.relnamespace
    WHERE  c.relname = 'random_points_10000_btree_idx'
    AND    n.nspname = 'public'
) THEN 
  CREATE INDEX random_points_10000_btree_idx ON random_points_10000 USING btree (geom);
END IF;
END$$;

DO $$
BEGIN
IF NOT EXISTS 
(
    SELECT 1
    FROM   pg_class c
    JOIN   pg_namespace n ON n.oid = c.relnamespace
    WHERE  c.relname = 'random_points_10000_gist_idx'
    AND    n.nspname = 'public'
) THEN 
  CREATE INDEX random_points_10000_gist_idx ON random_points_10000 USING gist (geom);
END IF;
END$$;

DO $$
BEGIN
IF NOT EXISTS 
(
    SELECT 1
    FROM   pg_class c
    JOIN   pg_namespace n ON n.oid = c.relnamespace
    WHERE  c.relname = 'random_points_10000_gin_idx'
    AND    n.nspname = 'public'
) THEN 
  CREATE INDEX random_points_10000_gin_idx ON random_points_10000 USING gin (geom);
END IF;
END$$;

DO $$
BEGIN
IF NOT EXISTS 
(
    SELECT 1
    FROM   pg_class c
    JOIN   pg_namespace n ON n.oid = c.relnamespace
    WHERE  c.relname = 'random_points_10000_sp-gist_idx'
    AND    n.nspname = 'public'
) THEN 
  CREATE INDEX random_points_10000_sp-gist_idx ON random_points_10000 USING sp-gist (geom);
END IF;
END$$;

DO $$
BEGIN
IF NOT EXISTS 
(
    SELECT 1
    FROM   pg_class c
    JOIN   pg_namespace n ON n.oid = c.relnamespace
    WHERE  c.relname = 'random_points_10000_hash_idx'
    AND    n.nspname = 'public'
) THEN 
  CREATE INDEX random_points_10000_hash_idx ON random_points_10000 USING hash (geom);
END IF;
END$$;