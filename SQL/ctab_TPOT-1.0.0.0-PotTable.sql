CREATE TABLE TPOT
(
	IDT int NOT NULL,
    ORG text,
    POTMOD varchar(10),
    CURMNT float8,
    TGTMNT float8,
    TRPIDT int NOT NULL,
    NAM text,
    STRDAT DATE,
    ENDDAT DATE,
    VALDAT DATE,
    DSC text,
    INDCANCEL varchar(1),
    CANCELRSN text,
    CANCELDAT date,
    DATEFT timestamp without time zone,
    CONSTRAINT PK_TPOT PRIMARY KEY (IDT)
)
TABLESPACE pg_default;

COMMENT ON TABLE TPOT IS 'Pot table';
COMMENT ON COLUMN TPOT.IDT IS 'Pot identifier';
COMMENT ON COLUMN TPOT.ORG IS 'Pot Organizer';
COMMENT ON COLUMN TPOT.POTMOD IS 'Pot mode (Lead/Shared)';
COMMENT ON COLUMN TPOT.CURMNT IS 'Pot current amount';
COMMENT ON COLUMN TPOT.TGTMNT IS 'Pot target amount';
COMMENT ON COLUMN TPOT.TRPIDT IS 'Trip identifier';
COMMENT ON COLUMN TPOT.NAM IS 'Pot name';
COMMENT ON COLUMN TPOT.STRDAT IS 'Pot Start Date';
COMMENT ON COLUMN TPOT.ENDDAT IS 'Pot end date';
COMMENT ON COLUMN TPOT.VALDAT IS 'Pot validation date';
COMMENT ON COLUMN TPOT.DSC IS 'Pot description';
COMMENT ON COLUMN TPOT.INDCANCEL IS 'Indicator => Pot is cancelled?';
COMMENT ON COLUMN TPOT.CANCELRSN IS 'Cancellation reason';
COMMENT ON COLUMN TPOT.CANCELDAT IS 'Cancellation date (if pot is cancelled)';
COMMENT ON COLUMN TPOT.DATEFT IS 'Modification Date';
