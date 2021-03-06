CREATE TABLE TTRP
(
	IDT int NOT NULL,
    NAM varchar(50) NOT NULL,
    DSC text,
    PRC float8,
    NBRMAXPRS int,
    LOC text,
    ORG varchar(50),
    STRDAT date,
    ENDDAT date,
    VALDAT date,
    TRPNOT float8,
    DATEFT timestamp without time zone,
    CONSTRAINT PK_TTRP PRIMARY KEY (IDT)
)
TABLESPACE pg_default;

COMMENT ON TABLE TTRP IS 'Trip table';
COMMENT ON COLUMN TTRP.IDT IS 'Trip identifier';
COMMENT ON COLUMN TTRP.NAM IS 'Trip Name';
COMMENT ON COLUMN TTRP.DSC IS 'Trip description';
COMMENT ON COLUMN TTRP.PRC IS 'Trip Price';
COMMENT ON COLUMN TTRP.NBRMAXPRS IS 'Number max of person';
COMMENT ON COLUMN TTRP.LOC IS 'Trip Location';
COMMENT ON COLUMN TTRP.ORG IS 'Trip Organizer';
COMMENT ON COLUMN TTRP.STRDAT IS 'Trip start date';
COMMENT ON COLUMN TTRP.ENDDAT IS 'Trip end date';
COMMENT ON COLUMN TTRP.VALDAT IS 'Trip validity date';
COMMENT ON COLUMN TTRP.TRPNOT IS 'Trip average note';
COMMENT ON COLUMN TTRP.DATEFT IS 'Modification Date';
