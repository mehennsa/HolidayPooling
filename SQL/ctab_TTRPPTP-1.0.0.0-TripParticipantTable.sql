CREATE TABLE TTRPPTP
(
	TRPIDT int NOT NULL,
    USRPSD VARCHAR(50) NOT NULL,
    INDUSRPTP VARCHAR(1),
    TRPNOT float8,
    VALDAT date,
    DATEFT timestamp without time zone,
    CONSTRAINT PK_TTRPPTP PRIMARY KEY (TRPIDT, USRPSD)
)
TABLESPACE pg_default;
COMMENT ON TABLE TTRPPTP IS 'Trip participant table';
COMMENT ON COLUMN TTRPPTP.TRPIDT IS 'Trip identifier';
COMMENT ON COLUMN TTRPPTP.USRPSD IS 'User pseudo';
COMMENT ON COLUMN TTRPPTP.INDUSRPTP IS 'User participated indicator';
COMMENT ON COLUMN TTRPPTP.TRPNOT IS 'User note on the trip';
COMMENT ON COLUMN TTRPPTP.VALDAT IS 'Validation date';
COMMENT ON COLUMN TTRPPTP.DATEFT IS 'Modification date';
