CREATE TABLE "Persons" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Persons" PRIMARY KEY,
    "Name" TEXT NULL,
    "CreatedBy" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "ModifiedBy" TEXT NULL,
    "ModifiedDate" TEXT NOT NULL
);


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


CREATE TABLE "Blogs" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Blogs" PRIMARY KEY,
    "Name" TEXT NULL,
    "Rating" INTEGER NOT NULL,
    "Url" TEXT NULL,
    "OwnerId" TEXT NULL,
    "CreatedBy" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "ModifiedBy" TEXT NULL,
    "ModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_Blogs_Persons_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES "Persons" ("Id") ON DELETE RESTRICT
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


CREATE TABLE "Posts" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY,
    "Content" TEXT NULL,
    "Title" TEXT NULL,
    "AuthorId" TEXT NULL,
    "BlogId" TEXT NULL,
    "CreatedBy" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "ModifiedBy" TEXT NULL,
    "ModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_Posts_Blogs_BlogId" FOREIGN KEY ("BlogId") REFERENCES "Blogs" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Posts_Persons_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Persons" ("Id") ON DELETE RESTRICT
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


CREATE UNIQUE INDEX "IX_Blogs_OwnerId" ON "Blogs" ("OwnerId");


CREATE INDEX "IX_CredentialIssuers_CredentialId" ON "CredentialIssuers" ("CredentialId");


CREATE INDEX "IX_Credentials_UserCredentialId" ON "Credentials" ("UserCredentialId");


CREATE INDEX "IX_Posts_AuthorId" ON "Posts" ("AuthorId");


CREATE INDEX "IX_Posts_BlogId" ON "Posts" ("BlogId");


CREATE INDEX "IX_UserCredentials_UserId" ON "UserCredentials" ("UserId");



