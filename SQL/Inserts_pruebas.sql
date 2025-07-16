USE Sistema_Horarios;
GO

INSERT INTO Nivel_Academico (Nombre, Estado)
VALUES 
('Sétimo', 'A'),
('Octavo', 'A'),
('Noveno', 'A'),
('Décimo', 'A');

INSERT INTO Materia (Nombre, Area, Tipo, Color, Estado)
VALUES 
('Matemáticas', 'Académica', 'Teórica', '#FF5733', 'A'),
('Ciencias', 'Académica', 'Teórica', '#33C1FF', 'A'),
('Inglés', 'Lenguas', 'Teórica', '#85FF33', 'A'),
('Educación Física', 'Deportiva', 'Práctica', '#FF33A8', 'A');

INSERT INTO Profesor (Cedula, Nombre, Apellido, Correo, Telefono, Carga_Horaria, Estado)
VALUES 
('123456789', 'Laura', 'Sánchez', 'laura.sanchez@colegio.edu', '8888-1234', 40, 'A'),
('234567890', 'José', 'Ramírez', 'jose.ramirez@colegio.edu', '8888-2345', 40, 'A'),
('345678901', 'Ana', 'Morales', 'ana.morales@colegio.edu', '8888-3456', 40, 'A'),
('456789012', 'Carlos', 'Méndez', 'carlos.mendez@colegio.edu', '8888-4567', 40, 'A');


INSERT INTO Profesor_X_Materia (Id_Profesor, Id_Materia, Estado)
VALUES 
(1, 1, 'A'), -- Laura: Matemáticas
(2, 2, 'A'), -- José: Ciencias
(3, 3, 'A'), -- Ana: Inglés
(4, 4, 'A'); -- Carlos: Educación Física


INSERT INTO Materia_X_Nivel (Id_Materia, Id_Nivel_Academico, Prioridad, Carga_Horaria, Estado)
VALUES 
(1, 1, 1, 25, 'A'), -- Matemáticas - Sétimo
(2, 1, 2, 20, 'A'), -- Ciencias - Sétimo
(3, 1, 3, 20, 'A'), -- Inglés - Sétimo
(4, 1, 4, 6, 'A'), -- Educación Física - Sétimo

(1, 2, 1, 20, 'A'), -- Matemáticas - Octavo
(2, 2, 2, 25, 'A'),
(3, 2, 3, 25, 'A'),
(4, 2, 4, 6, 'A');


INSERT INTO Grupo (Nombre, Id_Nivel_Academico, Seccion, Id_Profesor_Guia, Estado)
VALUES 
('7-1', 1, 'A', '1', 'A'),
('7-2', 1, 'B', '2', 'A'),
('8-1', 2, 'A', '3', 'A'),
('8-2', 2, 'B', '4', 'A');

INSERT INTO Restriccion_Profesor (Id_Profesor, Razon, Dia, Hora_Inicio, Hora_Fin, Estado)
VALUES 
(1, 'Clases universitarias', 'L', '07:00', '11:20', 'A');

