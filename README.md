# <h1>Audit with Entity Framework V 1.0</h1>
<h3>(Example with C# Core MVC)</h3>
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

# <h1>Well, is simple (or not)</h1>

This Audit get all properties in your model, all values in it and save in the table Audit. (This is simple, right?)

<h3>Save With Audit</h3>

In Context, i'm create the method apart for save changes with Audit because you can save some records without Entity,
gives you more freedom to take advantage of the feature. (Do not be required to fill the table every time :P)

This method take all the records that you add (with Add or AddRange) and separate with EntityState (Modified, Added, Deleted) and 
save according to your State;

<h3>EntityState.Added</h3>

For Added, i'm get all the ChangeTracker and separate in List, because you need the id of column to save record in Database (To get better consultation).

After first SaveChanges, i'm get the ChangeTracker with all the information what i need and save in table.

<h3>EntityState.Modified(Working)</h3>
<h3>EntityState.Deleted(Working)</h3>


