USE BDFisioterapia;
GO

DROP TABLE IF EXISTS Factura;
DROP TABLE IF EXISTS Atencion;
DROP TABLE IF EXISTS Usuario;
DROP TABLE IF EXISTS Tratamiento;
DROP TABLE IF EXISTS Fisioterapeuta;
DROP TABLE IF EXISTS Paciente;
DROP TABLE IF EXISTS Rol;
GO

CREATE TABLE Rol (
    IdRol INT IDENTITY(1,1) NOT NULL,
    NombreRol VARCHAR(50) NOT NULL,
    CONSTRAINT PK_Rol PRIMARY KEY (IdRol),
    CONSTRAINT UQ_Rol_NombreRol UNIQUE (NombreRol)
);
GO

CREATE TABLE Usuario (
    IdUsuario INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Correo VARCHAR(150) NOT NULL,
    Celular VARCHAR(10) NOT NULL,
    Pais VARCHAR(50) NULL,
    Genero VARCHAR(20) NULL,
    NombreUsuario VARCHAR(150) NOT NULL,
    Contrasena VARCHAR(100) NOT NULL,
    IdRol INT NOT NULL,
    CONSTRAINT PK_Usuario PRIMARY KEY (IdUsuario),
    CONSTRAINT FK_Usuario_Rol FOREIGN KEY (IdRol) REFERENCES Rol(IdRol),
    CONSTRAINT UQ_Usuario_Correo UNIQUE (Correo),
    CONSTRAINT UQ_Usuario_NombreUsuario UNIQUE (NombreUsuario)
);
GO

CREATE TABLE Paciente (
    IdPaciente INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    CI VARCHAR(20) NOT NULL,
    Telefono VARCHAR(10) NOT NULL,
    FechaNacimiento DATE NULL,
    Diagnostico VARCHAR(200) NULL,
    CONSTRAINT PK_Paciente PRIMARY KEY (IdPaciente),
    CONSTRAINT UQ_Paciente_CI UNIQUE (CI)
);
GO

CREATE TABLE Fisioterapeuta (
    IdFisioterapeuta INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Especialidad VARCHAR(100) NOT NULL,
    Telefono VARCHAR(10) NOT NULL,
    Estado VARCHAR(20) NOT NULL,
    CONSTRAINT PK_Fisioterapeuta PRIMARY KEY (IdFisioterapeuta)
);
GO

CREATE TABLE Tratamiento (
    IdTratamiento INT IDENTITY(1,1) NOT NULL,
    NombreTratamiento VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(200) NULL,
    Costo DECIMAL(10,2) NOT NULL,
    CONSTRAINT PK_Tratamiento PRIMARY KEY (IdTratamiento)
);
GO

CREATE TABLE Atencion (
    IdAtencion INT IDENTITY(1,1) NOT NULL,
    IdPaciente INT NOT NULL,
    IdFisioterapeuta INT NULL,
    IdTratamiento INT NOT NULL,
    Fecha DATE NOT NULL,
    Hora VARCHAR(10) NOT NULL,
    Estado VARCHAR(30) NOT NULL,
    Observacion VARCHAR(300) NULL,
    CONSTRAINT PK_Atencion PRIMARY KEY (IdAtencion),
    CONSTRAINT FK_Atencion_Paciente FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente),
    CONSTRAINT FK_Atencion_Fisioterapeuta FOREIGN KEY (IdFisioterapeuta) REFERENCES Fisioterapeuta(IdFisioterapeuta),
    CONSTRAINT FK_Atencion_Tratamiento FOREIGN KEY (IdTratamiento) REFERENCES Tratamiento(IdTratamiento)
);
GO

CREATE TABLE Factura (
    IdFactura INT IDENTITY(1,1) NOT NULL,
    IdAtencion INT NOT NULL,
    FechaEmision DATETIME NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    Descuento DECIMAL(10,2) NOT NULL DEFAULT 0,
    Total DECIMAL(10,2) NOT NULL,
    MetodoPago VARCHAR(50) NULL,
    CONSTRAINT PK_Factura PRIMARY KEY (IdFactura),
    CONSTRAINT FK_Factura_Atencion FOREIGN KEY (IdAtencion) REFERENCES Atencion(IdAtencion),
    CONSTRAINT UQ_Factura_Atencion UNIQUE (IdAtencion)
);
GO

INSERT INTO Rol (NombreRol)
VALUES 
('Administrador'),
('Recepcionista');
GO

INSERT INTO Usuario
(Nombre, Apellido, Correo, Celular, Pais, Genero, NombreUsuario, Contrasena, IdRol)
VALUES
('Administrador', 'Principal', 'admin@fisiozens.com', '70000000', 'Bolivia', 'Otro',
 'admin@fisiozens.com', '123456',
 (SELECT IdRol FROM Rol WHERE NombreRol = 'Administrador')),

('Recepcionista', 'Principal', 'recepcionista@fisiozens.com', '70000001', 'Bolivia', 'Femenino',
 'recepcionista@fisiozens.com', '123456',
 (SELECT IdRol FROM Rol WHERE NombreRol = 'Recepcionista'));
GO

INSERT INTO Tratamiento (NombreTratamiento, Descripcion, Costo)
VALUES
('Masoterapia', 'Masajes terapéuticos musculares', 80.00),
('Electroterapia', 'Uso de corrientes eléctricas terapéuticas', 100.00),
('Rehabilitación muscular', 'Ejercicios para recuperación muscular', 120.00),
('Fisioterapia Deportiva', 'Recuperación de lesiones deportivas', 150.00),
('Kinesiología', 'Movimiento corporal terapéutico', 130.00),
('Fisioterapia Neurológica', 'Rehabilitación neurológica', 160.00),
('Terapia Manual', 'Manipulación manual terapéutica', 110.00),
('Drenaje Linfático', 'Estimulación del sistema linfático', 140.00),
('Punción Seca', 'Tratamiento de puntos gatillo', 170.00),
('Vendaje Neuromuscular', 'Vendaje funcional muscular', 90.00),
('Terapia Respiratoria', 'Mejora de la función pulmonar', 125.00),
('Ultrasonido Terapéutico', 'Ondas ultrasónicas terapéuticas', 135.00),
('Crioterapia', 'Aplicación de frío terapéutico', 75.00),
('Termoterapia', 'Aplicación de calor terapéutico', 75.00),
('Estiramiento Asistido', 'Flexibilidad muscular asistida', 85.00),
('Reeducación Postural', 'Corrección de postura corporal', 145.00),
('Movilización Articular', 'Mejora movilidad articular', 115.00),
('Fortalecimiento Lumbar', 'Ejercicios para zona lumbar', 105.00),
('Recuperación de Rodilla', 'Terapia post lesión de rodilla', 150.00),
('Fisioterapia Pediátrica', 'Tratamientos para niños', 155.00);

INSERT INTO Fisioterapeuta (Nombre, Apellido, Especialidad, Telefono, Estado)
VALUES
('Carlos', 'Mamani', 'Masoterapia', '70000001', 'Disponible'),
('Ana', 'Rojas', 'Electroterapia', '70000002', 'Disponible'),
('Luis', 'Quispe', 'Rehabilitación muscular', '70000003', 'Ocupado'),
('Maria', 'Flores', 'Fisioterapia Deportiva', '70000004', 'Disponible'),
('Juan', 'Condori', 'Kinesiología', '70000005', 'Disponible'),
('Laura', 'Mendoza', 'Fisioterapia Neurológica', '70000006', 'Ocupado'),
('Pedro', 'Vargas', 'Electroterapia', '70000007', 'Disponible'),
('Elena', 'Cruz', 'Masoterapia', '70000008', 'Disponible'),
('Diego', 'Choque', 'Terapia Manual', '70000009', 'Disponible'),
('Sofia', 'Gutierrez', 'Fisioterapia Respiratoria', '70000010', 'Ocupado'),
('Miguel', 'Villca', 'Drenaje Linfático', '70000011', 'Disponible'),
('Lucia', 'Apaza', 'Vendaje Neuromuscular', '70000012', 'Disponible'),
('Jorge', 'Pinto', 'Punción Seca', '70000013', 'Ocupado'),
('Camila', 'Rivera', 'Kinesiología', '70000014', 'Disponible'),
('Fernando', 'Suarez', 'Masoterapia', '70000015', 'Disponible'),
('Natalia', 'Lema', 'Fisioterapia Pediátrica', '70000016', 'Ocupado'),
('Ricardo', 'Salazar', 'Electroterapia', '70000017', 'Disponible'),
('Valeria', 'Mora', 'Terapia Manual', '70000018', 'Disponible'),
('Andres', 'Torrez', 'Rehabilitación muscular', '70000019', 'Disponible'),
('Paola', 'Navarro', 'Fisioterapia Deportiva', '70000020', 'Ocupado');

SELECT * FROM Rol;
SELECT * FROM Usuario;
SELECT * FROM Fisioterapeuta;
SELECT * FROM Tratamiento;
GO