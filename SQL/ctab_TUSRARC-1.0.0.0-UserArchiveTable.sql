CREATE TABLE tusrarc
(
    idt integer NOT NULL,
    mel text NOT NULL,
    pwd text NOT NULL,
	usrslt text NOT NULL,
    age integer NOT NULL,
    psd varchar(50),
    phnnbr varchar(20),
    typ varchar(20),
    dsc text,
    rle varchar(20),
    credat date,
    usrnot double precision,
    dateft timestamp without time zone,
    datsup timestamp without time zone not null,
    CONSTRAINT tusrarc_pkey PRIMARY KEY (idt, dateft)
)
TABLESPACE pg_default;

COMMENT ON TABLE tusrarc IS 'User Archive Table';
COMMENT ON COLUMN tusrarc.idt IS 'User identifier';
COMMENT ON COLUMN tusrarc.mel IS 'User mail adress';
COMMENT ON COLUMN tusrarc.pwd IS 'User password';
COMMENT ON COLUMN tusrarc.usrslt IS 'User salt';
COMMENT ON COLUMN tusrarc.age IS 'User age';
COMMENT ON COLUMN tusrarc.psd IS 'User pseudo';
COMMENT ON COLUMN tusrarc.phnnbr IS 'User Phone Number';
COMMENT ON COLUMN tusrarc.typ IS 'User Type (Customer/Business)';
COMMENT ON COLUMN tusrarc.dsc IS 'User Description';
COMMENT ON COLUMN tusrarc.rle IS 'User Role (Common/Admin)';
COMMENT ON COLUMN tusrarc.credat IS 'User creation date';
COMMENT ON COLUMN tusrarc.usrnot IS 'User average note';
COMMENT ON COLUMN tusrarc.dateft IS 'Modification date';
COMMENT ON COLUMN tusrarc.datsup IS 'Suppression date';
