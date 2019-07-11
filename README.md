# <h1>Audit with Entity Framework</h1>
<h3>(Example with C# MVC)</h3>
<b>PS: For This project is work, you need the tables in a database.</b>
<br />

In this Example, I'm using the Database PostgresSQL

<h3>Scripts:</h3>

<h4>-->Schema</h4>

CREATE SCHEMA "Audit" AUTHORIZATION postgres;

COMMENT ON SCHEMA "Audit" IS 'standard public schema'

<h4>--> Tables</h4>

<b>Audit:</b>

-- Drop table

-- DROP TABLE "Audit"."Audit"

CREATE TABLE "Audit"."Audit" (
	"IdAudit" serial NOT NULL,
	"Entity" varchar NOT NULL,
	"IdRegister" int4 NOT NULL,
	"Column" varchar NOT NULL,
	"Value_Current" text NULL DEFAULT ''::character varying,
	"Value_Original" text NULL DEFAULT ''::character varying,
	"DateOccurrence" timestamp NOT NULL,
	"IdUserModified" int4 NOT NULL,
	"Action" varchar NOT NULL,
	CONSTRAINT auditoria_pk PRIMARY KEY ("IdAudit")
);

-- Permissions

ALTER TABLE "Audit"."Audit" OWNER TO postgres;
GRANT ALL ON TABLE "Audit"."Audit" TO postgres;

<b>Person:</b>

-- Drop table

-- DROP TABLE "Audit"."Person"

CREATE TABLE "Audit"."Person" (
	idperson serial NOT NULL,
	"Name" varchar NULL,
	"Date" date NOT NULL,
	"Number" varchar NOT NULL
);

-- Permissions

ALTER TABLE "Audit"."Person" OWNER TO postgres;
GRANT ALL ON TABLE "Audit"."Person" TO postgres;

