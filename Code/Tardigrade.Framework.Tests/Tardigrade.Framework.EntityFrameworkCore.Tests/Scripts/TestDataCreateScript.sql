CREATE TABLE "Users" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY,
    "Email" TEXT NULL,
    "FirstName" TEXT NULL,
    "LastName" TEXT NULL,
    "CreatedBy" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "ModifiedBy" TEXT NULL,
    "ModifiedDate" TEXT NOT NULL
);


CREATE TABLE "UserCredentials" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_UserCredentials" PRIMARY KEY,
    "Status" TEXT NULL,
    "UserId" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "ModifiedBy" TEXT NULL,
    "ModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_UserCredentials_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Credentials" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Credentials" PRIMARY KEY,
    "Name" TEXT NULL,
    "UserCredentialId" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "ModifiedBy" TEXT NULL,
    "ModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_Credentials_UserCredentials_UserCredentialId" FOREIGN KEY ("UserCredentialId") REFERENCES "UserCredentials" ("Id") ON DELETE CASCADE
);


CREATE TABLE "CredentialIssuers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_CredentialIssuers" PRIMARY KEY,
    "Name" TEXT NULL,
    "CredentialId" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "ModifiedBy" TEXT NULL,
    "ModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_CredentialIssuers_Credentials_CredentialId" FOREIGN KEY ("CredentialId") REFERENCES "Credentials" ("Id") ON DELETE CASCADE
);


CREATE INDEX "IX_CredentialIssuers_CredentialId" ON "CredentialIssuers" ("CredentialId");


CREATE INDEX "IX_Credentials_UserCredentialId" ON "Credentials" ("UserCredentialId");


CREATE INDEX "IX_UserCredentials_UserId" ON "UserCredentials" ("UserId");



