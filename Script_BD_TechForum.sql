-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TechForumDB')
BEGIN
    CREATE DATABASE TechForumDB;
END
GO

USE TechForumDB;
GO

-- Tabla Usuarios
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios
    (
        UsuarioID INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(100) NOT NULL,
        Correo VARCHAR(150) NOT NULL UNIQUE,
        Password VARCHAR(200) NOT NULL
    );
END
GO

-- Tabla Preguntas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Preguntas')
BEGIN
    CREATE TABLE Preguntas
    (
        PreguntaID INT IDENTITY(1,1) PRIMARY KEY,
        Titulo VARCHAR(200) NOT NULL,
        Descripcion VARCHAR(MAX) NOT NULL,
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        UsuarioID INT NOT NULL,

        CONSTRAINT FK_Preguntas_Usuarios FOREIGN KEY (UsuarioID)
            REFERENCES Usuarios(UsuarioID)
    );
END
GO

-- Tabla Respuestas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Respuestas')
BEGIN
    CREATE TABLE Respuestas
    (
        RespuestaID INT IDENTITY(1,1) PRIMARY KEY,
        Contenido VARCHAR(MAX) NOT NULL,
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        UsuarioID INT NOT NULL,
        PreguntaID INT NOT NULL,

        CONSTRAINT FK_Respuestas_Usuarios FOREIGN KEY (UsuarioID)
            REFERENCES Usuarios(UsuarioID),
        CONSTRAINT FK_Respuestas_Preguntas FOREIGN KEY (PreguntaID)
            REFERENCES Preguntas(PreguntaID)
    );
END
GO

USE TechForumDB;
GO

IF COL_LENGTH('Preguntas', 'Codigo') IS NULL
BEGIN
    ALTER TABLE Preguntas ADD Codigo VARCHAR(MAX) NULL;
END
GO

IF COL_LENGTH('Preguntas', 'ImagenUrl') IS NULL
BEGIN
    ALTER TABLE Preguntas ADD ImagenUrl VARCHAR(300) NULL;
END
GO

USE TechForumDB;
GO

IF COL_LENGTH('dbo.Respuestas', 'Codigo') IS NULL
BEGIN
    ALTER TABLE Respuestas ADD Codigo VARCHAR(MAX) NULL;
END
GO

USE TechForumDB;
GO

IF COL_LENGTH('dbo.Respuestas', 'ImagenUrl') IS NULL
BEGIN
    ALTER TABLE Respuestas ADD ImagenUrl VARCHAR(300) NULL;
END
GO

USE TechForumDB;
GO

IF COL_LENGTH('Usuarios', 'Titular') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD Titular VARCHAR(150) NULL;
END
GO

IF COL_LENGTH('Usuarios', 'Biografia') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD Biografia VARCHAR(500) NULL;
END
GO

IF COL_LENGTH('Usuarios', 'Ubicacion') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD Ubicacion VARCHAR(150) NULL;
END
GO

IF COL_LENGTH('Usuarios', 'AvatarUrl') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD AvatarUrl VARCHAR(300) NULL;
END
GO

-- preguntas
USE TechForumDB;
GO

IF COL_LENGTH('Preguntas', 'Etiquetas') IS NULL
BEGIN
    ALTER TABLE Preguntas ADD Etiquetas VARCHAR(300) NULL;
END
GO

IF COL_LENGTH('Preguntas', 'TotalVistas') IS NULL
BEGIN
    ALTER TABLE Preguntas ADD TotalVistas INT NOT NULL DEFAULT 0;
END
GO

IF COL_LENGTH('Preguntas', 'Resuelta') IS NULL
BEGIN
    ALTER TABLE Preguntas ADD Resuelta BIT NOT NULL DEFAULT 0;
END
GO

-- Indices usados por los listados y el detalle de preguntas.
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Preguntas_FechaCreacion'
      AND object_id = OBJECT_ID('dbo.Preguntas')
)
BEGIN
    CREATE INDEX IX_Preguntas_FechaCreacion
        ON dbo.Preguntas (FechaCreacion DESC);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Respuestas_PreguntaID'
      AND object_id = OBJECT_ID('dbo.Respuestas')
)
BEGIN
    CREATE INDEX IX_Respuestas_PreguntaID
        ON dbo.Respuestas (PreguntaID);
END
GO
