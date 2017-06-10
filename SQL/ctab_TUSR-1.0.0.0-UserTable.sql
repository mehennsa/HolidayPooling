CREATE TABLE tusr
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
    CONSTRAINT tusr_pkey PRIMARY KEY (idt)
)
TABLESPACE pg_default;

COMMENT ON TABLE tusr IS 'User Table';
COMMENT ON COLUMN tusr.idt IS 'User identifier';
COMMENT ON COLUMN tusr.mel IS 'User mail adress';
COMMENT ON COLUMN tusr.pwd IS 'User password';
COMMENT ON COLUMN tusr.usrslt IS 'User salt';
COMMENT ON COLUMN tusr.age IS 'User age';
COMMENT ON COLUMN tusr.psd IS 'User pseudo';
COMMENT ON COLUMN tusr.phnnbr IS 'User Phone Number';
COMMENT ON COLUMN tusr.typ IS 'User Type (Customer/Business)';
COMMENT ON COLUMN tusr.dsc IS 'User Description';
COMMENT ON COLUMN tusr.rle IS 'User Role (Common/Admin)';
COMMENT ON COLUMN tusr.credat IS 'User creation date';
COMMENT ON COLUMN tusr.usrnot IS 'User average note';
COMMENT ON COLUMN tusr.dateft IS 'Modification date';
