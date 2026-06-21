-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DevSpaceDB')
BEGIN
    CREATE DATABASE DevSpaceDB;
END
GO

USE DevSpaceDB;
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