CREATE TABLE TFRDSHP
(
    USRIDT int NOT NULL,
    FRDPSD varchar(50) NOT NULL,
    STRDAT date,
    INDRSQUSR varchar(1),
    INDWTG varchar(1),
    CONSTRAINT TFRDSHP_PK PRIMARY KEY (USRIDT, FRDPSD)
)
TABLESPACE pg_default;
COMMENT ON TABLE TFRDSHP IS 'Friendship table';
COMMENT ON COLUMN TFRDSHP.USRIDT IS 'User identifier';
COMMENT ON COLUMN TFRDSHP.FRDPSD IS 'Friend pseudo';
COMMENT ON COLUMN TFRDSHP.STRDAT IS 'Friendship creation date';
COMMENT ON COLUMN TFRDSHP.INDRSQUSR IS 'Indicator Friendship is requested by User?';
COMMENT ON COLUMN TFRDSHP.INDWTG IS 'Indicator Is Friendship waiting for validation';