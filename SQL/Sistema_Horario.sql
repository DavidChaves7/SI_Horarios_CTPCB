-- Crear la base de datos
Drop Database Sistema_Horarios;
CREATE DATABASE Sistema_Horarios;
GO
USE Sistema_Horarios;
GO

-- Tabla: Profesor
CREATE TABLE Profesor (
    Cedula VARCHAR(20) PRIMARY KEY,
    Nombre VARCHAR(100),
    Apellido VARCHAR(100),
    Correo VARCHAR(100),
    Telefono VARCHAR(20)
);

-- Tabla: Seguridad
CREATE TABLE Seguridad (
    Cedula VARCHAR(20) PRIMARY KEY,
    Correo VARCHAR(100),
    FOREIGN KEY (Cedula) REFERENCES Profesor(Cedula)
);

-- Tabla: Materia
CREATE TABLE Materia (
    Id_Materia INT PRIMARY KEY,
    Nombre VARCHAR(100),
    Tipo VARCHAR(50)
);

-- Tabla: Nivel_Academico
CREATE TABLE Nivel_Academico (
    Id_nivel_Academico INT PRIMARY KEY,
    Id_Materia INT,
    Carga_Horaria INT,
    FOREIGN KEY (Id_Materia) REFERENCES Materia(Id_Materia)
);

-- Tabla: Grupo
CREATE TABLE Grupo (
    Id_Grupo INT PRIMARY KEY,
    Nombre VARCHAR(100),
    Nivel_academico INT,
    Seccion VARCHAR(50),
    Profesor_Guia VARCHAR(20),
    FOREIGN KEY (Nivel_academico) REFERENCES Nivel_Academico(Id_nivel_Academico),
    FOREIGN KEY (Profesor_Guia) REFERENCES Profesor(Cedula)
);

-- Tabla: Horario
CREATE TABLE Horario (
    Dia VARCHAR(20),
    Cedula_Profesor VARCHAR(20),
    Id_Materia INT,
    Id_Grupo INT,
    Nombre_Dia VARCHAR(10) NOT NULL,
    Hora_Inicio TIME,
    Hora_Fin TIME,
    PRIMARY KEY (Cedula_Profesor, Id_Materia, Id_Grupo, Nombre_Dia),
    FOREIGN KEY (Cedula_Profesor) REFERENCES Profesor(Cedula),
    FOREIGN KEY (Id_Materia) REFERENCES Materia(Id_Materia),
    FOREIGN KEY (Id_Grupo) REFERENCES Grupo(Id_Grupo)
);

-- Tabla: Restriccion_Horaria
CREATE TABLE Restriccion_Horaria (
    Id_Restriccion INT PRIMARY KEY,
    Cedula_Profesor VARCHAR(20),
    Dia VARCHAR(20),
    Hora_Inicio TIME,
    Hora_Fin TIME,
    FOREIGN KEY (Cedula_Profesor) REFERENCES Profesor(Cedula)
);