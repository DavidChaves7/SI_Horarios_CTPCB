USE Sistema_Horarios;
GO

INSERT INTO Nivel_Academico (Nombre, Estado)
VALUES 
('Séptimo', 'A'),
('Octavo', 'A'),
('Noveno', 'A'),
('Décimo', 'A'),
('Undécimo', 'A'),
('Duodécimo', 'A');

INSERT INTO Materia (Nombre, Area, Tipo, Color, Estado)
VALUES 
('Matemáticas', 'Académica', 'N/A', '#FF5733', 'A'),            -- Rojo anaranjado
('Ciencias', 'Académica', 'N/A', '#33C1FF', 'A'),               -- Celeste
('Español', 'Académica', 'N/A', '#2ECC71', 'A'),                -- Verde
('Cocina', 'Tecnica', 'N/A', '#F39C12', 'A'),                   -- Naranja
('Mantenimiento', 'Tecnica', 'N/A', '#D35400', 'A'),            -- Naranja oscuro
('Tecnologia', 'Académica', 'N/A', '#9B59B6', 'A'),             -- Morado
('Informatica', 'Académica', 'N/A', '#2980B9', 'A'),            -- Azul
('Orientación', 'Académica', 'N/A', '#1ABC9C', 'A'),            -- Turquesa
('Musica', 'Académica', 'N/A', '#E74C3C', 'A'),                 -- Rojo
('Educación Religiosa', 'Académica', 'N/A', '#34495E', 'A'),               -- Gris azulado
('Hogar', 'Académica', 'N/A', '#7D3C98', 'A'),                  -- Morado oscuro
('Estudios Sociales', 'Académica', 'N/A', '#27AE60', 'A'),      -- Verde fuerte
('Habilidades', 'Académica', 'N/A', '#F1C40F', 'A'),            -- Amarillo
('Artes Industriales', 'Académica', 'N/A', '#E67E22', 'A'),     -- Naranja suave
('Artesanias', 'Tecnica', 'N/A', '#BA4A00', 'A'),               -- Marrón quemado
('Artes Plasticas', 'Académica', 'N/A', '#A569BD', 'A'),        -- Lila
('Educación Fisica', 'Académica', 'N/A', '#3498DB', 'A'),       -- Azul brillante
('Ingles', 'Académica', 'N/A', '#58D68D', 'A'),                 -- Verde menta
('Afectividad y Sexualidad', 'Académica', 'N/A', '#F06292', 'A'), -- Rosado coral
('GUIA', 'Académica', 'N/A', '#16A085', 'A'),           -- Verde azulado
('Maderas', 'Tecnica', 'N/A', '#8B4513', 'A'); -- Marrón madera
 
 
INSERT INTO Profesor (Cedula, Nombre, Apellido, Correo, Telefono, Carga_Horaria, Estado)
VALUES 
('111111111', 'MARÍA FERNANDA', 'BRENES CHACÓN', 'example@colegio.edu', '8888-8888', 23, 'A'),
('222222222', 'MONSERRAT', 'CARBONELL RAMÍREZ', 'example@colegio.edu', '8888-2345', 20, 'A'),
('333333333', 'LIZETTE', 'RETANA BARBOZA', 'example@colegio.edu', '8888-2345', 41, 'A'),
('444444444', 'ANGIE', 'RAMÍREZ RIVAS', 'example@colegio.edu', '8888-2345', 42, 'A'),
('555555555', 'WENDY', 'ÁLVAREZ QUESADA', 'example@colegio.edu', '8888-2345', 19, 'A'),
('666666666', 'FRANCINI', 'ANGULO HERNÁNDEZ', 'example@colegio.edu', '8888-2345', 41, 'A'),
('777777777', 'GERARDINA', 'MURILLO AZOFEIFA', 'example@colegio.edu', '8888-2345', 33, 'A'),
('888888888', 'YENDRI', 'CALDERÓN RAMÍREZ', 'example@colegio.edu', '8888-2345', 10, 'A'),
('999999999', 'GUSTAVO', 'SILES RÍOS', 'example@colegio.edu', '8888-2345', 28, 'A'),
('123123123', 'MARCO', 'ROVERSSI ALVARADO', 'example@colegio.edu', '8888-2345', 15, 'A'),
('321321321', 'ROBERTO', 'PICADO ALCOCER', 'example@colegio.edu', '8888-2345', 30, 'A'),
('456456456', 'ALEXANDER', 'CRUZ SALGUERA', 'example@colegio.edu', '8888-2345', 12, 'A'),
('456456457', 'ESTEBAN ', 'GARRO B', 'example@colegio.edu', '8888-2345', 16, 'A'),
('654654654', 'ALEJANDRA', 'CHAVEZ UMAÑA', 'example@colegio.edu', '8888-2345', 16, 'A'),
('789789789', 'JÉSSICA', 'SALAZAR MARÍN', 'example@colegio.edu', '8888-2345', 30, 'A'),
('987987987', 'EVELYN', 'BOGANTES AGUILAR', 'example@colegio.edu', '8888-2345', 1, 'A'),
('147147147', ' YERLY ', 'CASTILLO LINARES', 'example@colegio.edu', '8888-2345', 2, 'A'),
('258258258', 'RAQUEL', 'CHINCHILLA ARIAS', 'example@colegio.edu', '8888-2345', 3, 'A'),
('369369369', 'ADRIANA', 'ALVARADO CAMBRONERO', 'example@colegio.edu', '8888-2345', 3, 'A'),
('000000000', 'Sin Asignar', '', '', '', 0, 'A');



INSERT INTO Profesor_X_Materia (Id_Profesor, Id_Materia, Estado)
VALUES 
(1, 12, 'A'), -- MARÍA	FERNANDA	BRENES	CHACÓN : ESTUDIOS SOCIALES
(2, 3, 'A'), -- MONSERRAT CARBONELL RAMÍREZ : ESPAÑOL 
(2, 2, 'A'), -- MONSERRAT CARBONELL RAMÍREZ : CIENCIAS
(3, 2, 'A'), -- LIZETTE RETANA BARBOZA : CIENCIAS
(3, 1, 'A'), -- LIZETTE RETANA BARBOZA : MATEMATICAS
(4, 3, 'A'), --  ANGIE RAMÍREZ RIVAS : ESPAÑOL
(4, 13, 'A'), --  ANGIE RAMÍREZ RIVAS : HABILIDADES
(4, 2, 'A'), --  ANGIE RAMÍREZ RIVAS : CIENCIAS 
(5, 1, 'A'), --  WENDY ÁLVAREZ QUESADA : MATEMATICAS
(5, 2, 'A'), --  WENDY ÁLVAREZ QUESADA : CIENCIAS
(5, 3, 'A'), --  WENDY ÁLVAREZ QUESADA : ESPAÑOL
(5, 13, 'A'), --  WENDY ÁLVAREZ QUESADA : HABILIDADES
(5, 12, 'A'), --  WENDY ÁLVAREZ QUESADA : ESTUDIOS SOCIALES
(6, 11, 'A'), --  FRANCINI ÁNGULO HERNÁNDEZ : HOGAR
(6, 15, 'A'), --  FRANCINI ÁNGULO HERNÁNDEZ : ARTESANIA
(7, 11, 'A'), --  GERARDINA MURILLO AZOFEIFA : HOGAR
(7, 4, 'A'), --  GERARDINA MURILLO AZOFEIFA : COCINA
(8, 11, 'A'), --  YENDRI CALDERÓN RAMÍREZ : HOGAR
(9, 17, 'A'), --  GUSTAVO SILES RÍOS : EDUCACIÓN FISICA
(10, 17, 'A'), --  MARCO ROVERSSI ALVARADO : EDUCACIÓN FISICA
(11, 14, 'A'), --  ROBERTO PICADO ALCOCER : ARTES INDUSTRIALES
(12, 17, 'A'), --  ALEXANDER CRUZ SALGUERA : EDUCACIÓN FISICA
(13, 16, 'A'), --  ESTEBAN GARRO B : ARTES PLASTICAS
(14, 18, 'A'), --  ALEJANDRA CHAVEZ UMAÑA : INGLES
(15, 6, 'A'), --  JÉSSICA SALAZAR MARÍN : TECNOLOGIA
(15, 7, 'A'), --  JÉSSICA SALAZAR MARÍN : INFORMATICA
(16, 19, 'A'), --  EVELYN BOGANTES AGUILAR : AFECTIVIDAD Y SEXUALIDAD
(17, 8, 'A'), --   YERLY CASTILLO LINARES : ORIENTACIÓN
(18, 8, 'A'), --   RAQUEL CHINCHILLA ARIAS : ORIENTACIÓN
(19, 8, 'A'); --   ADRIANA ALVARADO CAMBRONERO : ORIENTACIÓN



INSERT INTO Materia_X_Nivel (Id_Materia, Id_Nivel_Academico, Prioridad, Carga_Horaria, Estado)
VALUES
-- Matemáticas
(1, 1, 1, 4, 'A'), -- Sétimo - Matemáticas - 4 lecciones,
(1, 2, 1, 4, 'A'), -- Octavo - Matemáticas - 4 lecciones,
(1, 3, 1, 4, 'A'), -- Noveno - Matemáticas - 4 lecciones,
(1, 4, 1, 4, 'A'), -- Décimo - Matemáticas - 4 lecciones,
(1, 5, 1, 4, 'A'), -- Undécimo - Matemáticas - 4 lecciones,
(1, 6, 1, 4, 'A'), -- Duodécimo - Matemáticas - 4 lecciones,

-- Estudios Sociales
(12, 1, 1, 3, 'A'), -- Sétimo - Estudios Sociales - 3 lecciones,
(12, 2, 1, 3, 'A') ,-- Octavo - Estudios Sociales - 3 lecciones,
(12, 3, 1, 3, 'A') ,-- Noveno - Estudios Sociales - 3 lecciones,
(12, 4, 1, 3, 'A'), -- Décimo - Estudios Sociales - 3 lecciones,
(12, 5, 1, 3, 'A'), -- Undécimo - Estudios Sociales - 3 lecciones,
(12, 6, 1, 3, 'A'), -- Duodécimo - Estudios Sociales - 3 lecciones,

-- Ciencias
(2, 1, 1, 3, 'A'), -- Sétimo - Ciencias - 3 lecciones,
(2, 2, 1, 3, 'A'), -- Octavo - Ciencias - 3 lecciones,
(2, 3, 1, 3, 'A'), -- Noveno - Ciencias - 3 lecciones,
(2, 4, 1, 3, 'A'), -- Décimo - Ciencias - 3 lecciones,
(2, 5, 1, 3, 'A'), -- Undécimo - Ciencias - 3 lecciones,
(2, 6, 1, 3, 'A'), -- Duodécimo - Ciencias - 3 lecciones,

--Español
(3, 1, 1, 4, 'A'), -- Sétimo - Español - 4 lecciones,
(3, 2, 1, 4, 'A'), -- Octavo - Español - 4 lecciones,
(3, 3, 1, 4, 'A'), -- Noveno - Español - 4 lecciones,
(3, 4, 1, 4, 'A'), -- Décimo - Español - 4 lecciones,
(3, 5, 1, 4, 'A'), -- Undécimo - Español - 4 lecciones,
(3, 6, 1, 4, 'A'), -- Duodécimo - Español - 4 lecciones,

--Habilidades
(13, 1, 1, 3, 'A'), -- Sétimo - Habilidades y destrezas para la vida - 3 lecciones,
(13, 2, 1, 3, 'A'), -- Octavo - Habilidades y destrezas para la vida - 3 lecciones,
(13, 3, 1, 3, 'A'), -- Noveno - Habilidades y destrezas para la vida - 3 lecciones,
(13, 4, 1, 4, 'A'), -- Décimo - Habilidades y destrezas para la vida - 4 lecciones,
(13, 5, 1, 4, 'A'), -- Undécimo - Habilidades y destrezas para la vida - 4 lecciones,
(13, 6, 1, 4, 'A'), -- Duodécimo - Habilidades y destrezas para la vida - 4 lecciones,

--Ingles
(18, 1, 1, 2, 'A'), -- Sétimo - Inglés - 2 lecciones,
(18, 2, 1, 2, 'A'), -- Octavo - Inglés - 2 lecciones,
(18, 3, 1, 2, 'A'), -- Noveno - Inglés - 2 lecciones,
(18, 4, 1, 2, 'A'), -- Décimo - Inglés - 2 lecciones,
(18, 5, 1, 2, 'A'), -- Undécimo - Inglés - 2 lecciones,
(18, 6, 1, 2, 'A'), -- Duodécimo - Inglés - 2 lecciones,

--Educación Física
(17, 1, 1, 2, 'A'), -- Sétimo - Educación Física - 2 lecciones,
(17, 2, 1, 2, 'A'), -- Octavo - Educación Física - 2 lecciones,
(17, 3, 1, 2, 'A'), -- Noveno - Educación Física - 2 lecciones,
(17, 4, 1, 2, 'A'), -- Décimo - Educación Física - 2 lecciones,
(17, 5, 1, 2, 'A'), -- Undécimo - Educación Física - 2 lecciones,
(17, 6, 1, 2, 'A'), -- Duodécimo - Educación Física - 2 lecciones,

-- MUSICa
(9, 1, 1, 2, 'A'), -- Sétimo - Educación Musical - 2 lecciones,
(9, 2, 1, 2, 'A'), -- Octavo - Educación Musical - 2 lecciones,
(9, 3, 1, 2, 'A'), -- Noveno - Educación Musical - 2 lecciones,
(9, 4, 1, 2, 'A'), -- Décimo - Educación Musical - 2 lecciones,
(9, 5, 1, 2, 'A'), -- Undécimo - Educación Musical - 2 lecciones,
(9, 6, 1, 2, 'A'), -- Duodécimo - Educación Musical - 2 lecciones,

-- GUIA
(20, 1, 1, 1, 'A'), -- Sétimo - GUIA - 1 lecciones,
(20, 2, 1, 1, 'A'), -- Octavo - GUIA - 1 lecciones,
(20, 3, 1, 1, 'A'), -- Noveno - GUIA - 1 lecciones,
(20, 4, 1, 1, 'A'), -- Décimo - GUIA - 1 lecciones,
(20, 5, 1, 1, 'A'), -- Undécimo - GUIA - 1 lecciones,
(20, 6, 1, 1, 'A'), -- Duodécimo - GUIA - 1 lecciones,

-- Informática Educativa
(7, 1, 1, 2, 'A'), -- Sétimo - Informática Educativa - 2 lecciones,
(7, 2, 1, 2, 'A'), -- Octavo - Informática Educativa - 2 lecciones,
(7, 3, 1, 2, 'A'), -- Noveno - Informática Educativa - 2 lecciones,
(7, 4, 1, 2, 'A'), -- Décimo - Informática Educativa - 2 lecciones,
(7, 5, 1, 2, 'A'), -- Undécimo - Informática Educativa - 2 lecciones,
(7, 6, 1, 2, 'A'), -- Duodécimo - Informática Educativa - 2 lecciones,

-- Orientación
(8, 1, 1, 1, 'A'), -- Sétimo - Orientación - 1 lecciones,
(8, 2, 1, 1, 'A'), -- Octavo - Orientación - 1 lecciones,
(8, 3, 1, 1, 'A'), -- Noveno - Orientación - 1 lecciones,
(8, 4, 1, 1, 'A'), -- Décimo - Orientación - 1 lecciones,
(8, 5, 1, 1, 'A'), -- Undécimo - Orientación - 1 lecciones,
(8, 6, 1, 1, 'A'), -- Duodécimo - Orientación - 1 lecciones,

--Artes Plásticas
(16, 1, 1, 2, 'A'), -- Sétimo - Artes Plásticas - 2 lecciones,
(16, 2, 1, 2, 'A'), -- Octavo - Artes Plásticas - 2 lecciones,
(16, 3, 1, 2, 'A'), -- Noveno - Artes Plásticas - 2 lecciones,
(16, 4, 1, 2, 'A'), -- Décimo - Artes Plásticas - 2 lecciones,
(16, 5, 1, 2, 'A'), -- Undécimo - Artes Plásticas - 2 lecciones,
(16, 6, 1, 2, 'A'), -- Duodécimo - Artes Plásticas - 2 lecciones,

--Educación Religiosa
(10, 1, 1, 1, 'A'), -- Sétimo - Educación Religiosa - 1 lecciones,
(10, 2, 1, 1, 'A'), -- Octavo - Educación Religiosa - 1 lecciones,
(10, 3, 1, 1, 'A'), -- Noveno - Educación Religiosa - 1 lecciones,
(10, 4, 1, 1, 'A'), -- Décimo - Educación Religiosa - 1 lecciones,
(10, 5, 1, 1, 'A'), -- Undécimo - Educación Religiosa - 1 lecciones,
(10, 6, 1, 1, 'A'), -- Duodécimo - Educación Religiosa - 1 lecciones,

--Afectividad y sexualidad
(19, 4, 1, 1, 'A'), -- Décimo - Afectividad y sexualidad - 1 lecciones,

--Cocina
(4, 4, 1, 18, 'A'), -- Décimo - Técnica 1 - Cocina - 18 lecciones,
(4, 5, 1, 18, 'A'), -- Undécimo - Técnica 1 - Cocina - 18 lecciones,
(4, 6, 1, 22, 'A'), -- Duodécimo - Técnica 1 - Cocina - 22 lecciones,

--Madera
(21, 4, 1, 18, 'A'), -- Décimo - Madera - 18 lecciones,
(21, 5, 1, 18, 'A'), -- Undécimo - Madera - 18 lecciones,
(21, 6, 1, 22, 'A'), -- Duodécimo - Madera - 22 lecciones,

--Artesania
(15, 4, 1, 18, 'A'), -- Décimo - Artesania - 18 lecciones,
(15, 5, 1, 18, 'A'), -- Undécimo - Artesania - 18 lecciones,
(15, 6, 1, 22, 'A'), -- Duodécimo - Artesania - 22 lecciones,

--Mantenimiento
(5, 4, 1, 18, 'A'), -- Décimo - Mantenimiento - 18 lecciones,
(5, 5, 1, 18, 'A'), -- Undécimo - Mantenimiento - 18 lecciones,
(5, 6, 1, 22, 'A'), -- Duodécimo - Mantenimiento - 22 lecciones,

--Hogar
(11, 1, 1, 10, 'A'), -- Sétimo - Hogar - 10 lecciones,
(11, 2, 1, 10, 'A'), -- Octavo - Hogar - 10 lecciones,
(11, 3, 1, 10, 'A'), -- Noveno - Hogar - 10 lecciones;

--Artes Industriales
(14, 1, 1, 10, 'A'), -- Sétimo - Artes Industriales - 10 lecciones,
(14, 2, 1, 10, 'A'), -- Octavo - Artes Industriales - 10 lecciones,
(14, 3, 1, 10, 'A'), -- Noveno - Artes Industriales - 10 lecciones;

--Tecnologia
(6, 1, 1, 6, 'A'), -- Sétimo - Tecnología - 6 lecciones,
(6, 2, 1, 6, 'A'), -- Octavo - Tecnología - 6 lecciones,
(6, 3, 1, 6, 'A'); -- Noveno - Tecnología - 6 lecciones;


INSERT INTO Grupo (Nombre, Id_Nivel_Academico, Seccion, Id_Profesor_Guia, Estado)
VALUES 
('7-1', 1, 'Grupo 1', 1, 'A'),
('7-2', 1, 'Grupo 2', 4, 'A'),
('8-1', 2, 'Grupo 3', 2, 'A'),
('8-2', 2, 'Grupo 4', 6, 'A'),
('9-1', 3, 'Grupo 5', 4, 'A'),
('9-2', 3, 'Grupo 8', 5, 'A'),
('10-1', 4, 'Grupo 6', -1, 'A'),
('12-1', 6, 'Grupo 7', 7, 'A');


INSERT INTO Restriccion_Profesor (Id_Profesor, Razon, Dia, Hora_Inicio, Hora_Fin, Estado)
VALUES 
(9, 'Trabajo con la parte de Educación Curricular', 'L', '07:00', '8:20', 'A'),
(9, 'Trabajo con la parte de Educación Curricular', 'L', '12:00', '15:40', 'A'),
(9, 'Trabajo con la parte de Educación Curricular', 'K', '10:00', '11:20', 'A'),
(9, 'Trabajo con la parte de Educación Curricular', 'V', '07:00', '16:20', 'A'),
(10, 'Trabajo con la parte de Educación Curricular', 'K', '07:00', '07:40', 'A'),
(10, 'Trabajo con la parte de Educación Curricular', 'K', '09:20', '10:40', 'A'),
(10, 'Trabajo con la parte de Educación Curricular', 'K', '12:40', '16:20', 'A'),
(10, 'Trabajo con la parte de Educación Curricular', 'V', '08:20', '13:20', 'A'),
(10, 'Trabajo con la parte de Educación Curricular', 'V', '15:40', '16:20', 'A'),
(10, 'Trabajo en otra institución', 'L', '07:00', '16:20', 'A'),
(10, 'Trabajo en otra institución', 'M', '07:00', '16:20', 'A'),
(10, 'Trabajo en otra institución', 'J', '07:00', '16:20', 'A'),
(13, 'Trabajo en otra institución', 'M', '07:00', '16:20', 'A'),
(13, 'Trabajo en otra institución', 'J', '07:00', '16:20', 'A'),
(14, 'Trabajo con la parte de Educación Curricular', 'L', '07:00', '09:00', 'A'),
(14, 'Trabajo con la parte de Educación Curricular', 'L', '14:00', '16:20', 'A'),
(14, 'Trabajo con la parte de Educación Curricular', 'K', '07:00', '09:00', 'A'),
(14, 'Trabajo en otra institución', 'M', '07:00', '16:20', 'A'),
(14, 'Trabajo con la parte de Educación Curricular', 'J', '12:00', '16:20', 'A'),
(15, 'Trabajo en otra institución', 'L', '07:00', '16:20', 'A'),
(15, 'Trabajo en otra institución', 'K', '07:00', '16:20', 'A');

INSERT INTO Reporteria_Progamada(Frecuencia,Correos,Estado)
VALUES ('T', 'dchavesd.07@gmail.com,camposcespedesd@gmail.com', 'A');