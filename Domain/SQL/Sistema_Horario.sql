-- Crear la base de datos
USE master
GO
Drop Database Sistema_Horarios;
CREATE DATABASE Sistema_Horarios;
GO
USE Sistema_Horarios;
GO

-- Tabla: Profesor
CREATE TABLE Profesor (
    Id_Profesor INT IDENTITY(1,1) PRIMARY KEY,
    Cedula VARCHAR(20),
    Nombre VARCHAR(100),
    Apellido VARCHAR(100),
    Correo VARCHAR(100),
    Telefono VARCHAR(20),
	Carga_Horaria INT,
    Estado VARCHAR(1)
);

-- Tabla: Seguridad
CREATE TABLE Seguridad (
	Id_Usuario INT IDENTITY(1,1) PRIMARY KEY,
    Cedula VARCHAR(20),
    Correo VARCHAR(100),
    Estado VARCHAR(1)
);

-- Tabla: Profesor_X_Materia
CREATE TABLE Profesor_X_Materia (
	Id_Prof_Materia INT IDENTITY(1,1) PRIMARY KEY,
    Id_Profesor INT,
	Id_Materia INT,
    Estado VARCHAR(1)
);

-- Tabla: Materia
CREATE TABLE Materia (
    Id_Materia INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100),
	Area VARCHAR (50),
    Tipo VARCHAR(50),
	Color VARCHAR(10),
    Estado VARCHAR(1)
);

-- Tabla: Materia_X_Nivel
CREATE TABLE Materia_X_Nivel (
    Id_Mat_X_Nivel INT IDENTITY(1,1) PRIMARY KEY,
    Id_Materia INT,
	Id_Nivel_Academico INT,
	Prioridad INT,
    Carga_Horaria INT,
	Estado VARCHAR(1)
);


-- Tabla: Nivel_Academico
CREATE TABLE Nivel_Academico (
    Id_Nivel_Academico INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100),
	Estado VARCHAR(1)
)

-- Tabla: Grupo
CREATE TABLE Grupo (
    Id_Grupo INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100),
    Id_Nivel_Academico INT,
    Seccion VARCHAR(50),
    Id_Profesor_Guia INT,
	Estado VARCHAR(1)
);

-- Tabla: Horario
CREATE TABLE Horario (
	Id_Horario INT IDENTITY(1,1) PRIMARY KEY,
    Dia VARCHAR(1),
    Id_Profesor VARCHAR(20),
    Id_Materia INT,
    Id_Grupo INT,
    Hora_Inicio TIME,
    Hora_Fin TIME,
	Estado VARCHAR(1)
);

-- Tabla: Restriccion_Profesor
CREATE TABLE Restriccion_Profesor (
    Id_Restriccion INT IDENTITY(1,1) PRIMARY KEY,
    Id_Profesor INT,
	Razon VARCHAR(100),
    Dia VARCHAR(1),
    Hora_Inicio VARCHAR(10),
    Hora_Fin VARCHAR(10),
	Estado VARCHAR(1)
);

--Tabla: Reporteria_Progamada 
CREATE TABLE Reporteria_Progamada (
	Id_Rep_Programada INT IDENTITY(1,1) PRIMARY KEY,
	Correos VARCHAR (1000),
	Frecuencia VARCHAR(1),
	Fecha_Hora_Envio DATETIME,
	Estado VARCHAR(1)
);