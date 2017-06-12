CREATE TABLE TFRDSHPARC
(
    USRIDT int NOT NULL,
    FRDPSD varchar(50) NOT NULL,
    STRDAT date,
    INDRSQUSR varchar(1),
    INDWTG varchar(1),
    DATEFT timestamp without time zone,
    DATSUP timestamp without time zone NOT NULL,
    CONSTRAINT TFRDSHPARC_PK PRIMARY KEY (USRIDT, FRDPSD, DATEFT)
)
TABLESPACE pg_default;
COMMENT ON TABLE TFRDSHPARC IS 'Friendship archive table';
COMMENT ON COLUMN TFRDSHPARC.USRIDT IS 'User identifier';
COMMENT ON COLUMN TFRDSHPARC.FRDPSD IS 'Friend pseudo';
COMMENT ON COLUMN TFRDSHPARC.STRDAT IS 'Friendship creation date';
COMMENT ON COLUMN TFRDSHPARC.INDRSQUSR IS 'Indicator Friendship is requested by User?';
COMMENT ON COLUMN TFRDSHPARC.INDWTG IS 'Indicator Is Friendship waiting for validation';
COMMENT ON COLUMN TFRDSHPARC.DATEFT IS 'Modification Date';
COMMENT ON COLUMN TFRDSHPARC.DATSUP IS 'Suppression Date';

