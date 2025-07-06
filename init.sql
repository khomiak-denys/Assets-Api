CREATE TABLE IF NOT EXISTS public."priceInfo_data"
(
    id BIGSERIAL NOT NULL,
    symbol character varying(20) COLLATE pg_catalog."default" NOT NULL,
    price numeric(10,2) NOT NULL,
    lastupdate timestamp with time zone NOT NULL,
    CONSTRAINT "priceInfo_data_pkey" PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."priceInfo_data"
    OWNER to postgres;

CREATE INDEX IF NOT EXISTS idx_priceinfo_symbol_lastUpdate
    ON public."priceInfo_data" USING btree
    (symbol COLLATE pg_catalog."default" ASC NULLS LAST, lastupdate ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public.assets_data
(
    id character varying(50) COLLATE pg_catalog."default" NOT NULL,
    symbol character varying(50) COLLATE pg_catalog."default" NOT NULL,
    name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    provider character varying(255) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT assets_data_pkey PRIMARY KEY (symbol)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.assets_data
    OWNER to postgres;