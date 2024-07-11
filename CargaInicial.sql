use db_gestion_cuentas;
go

-- Carga de data inicial

--insert into tb_moneda values ('Soles','S/'), ('Dólares','$')

--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Comida', NULL)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Compras', NULL)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Vivienda', NULL)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Transporte', NULL)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Vehiculo', NULL)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Vida y Entretenimiento', NULL)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Comunicaciones', NULL)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Gastos Financieros', NULL)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Ingreso', NULL)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Restaurante', 1)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Delivery', 1)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Ropa y calzado', 2)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Mascotas', 2)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Farmacia', 2)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Servicios', 3)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Transporte Público', 4)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Taxi', 4)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Vuelos', 4)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Combustible', 5)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Estacionamiento', 5)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Cine', 6)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Streaming', 6)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Conciertos', 6)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Deporte', 6)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Teléfono movil', 7)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES ( N'Juegos', 7)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Aplicaciones', 7)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Impuestos', 8)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Seguro de Desgravamen', 8)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Intereses', 8)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Salario', 9)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Cafeteria / jugos', 1)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Compras Supermercado', 2)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Bodega', 1)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Mantenimiento del Hogar', 2)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Educación', 6)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Hoteles', 6)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Tecnología', 7)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Libreria', 2)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Comisiones Bancarias', 8)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Otros', 2)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Museos, arte, etc', 6)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Trámites', 6)
--INSERT [dbo].[tb_categoria] ([descripcion], [idCategoriaPadre]) VALUES (N'Salud', 7)
	
--insert into tb_pais values('Perú'), ('Estados Unidos')

--insert into tb_banco values ('BCP', 1, 'BCP'),('Interbank',1, 'INTERBANK'),('BBVA',1, 'BBVA'),('Diners Club', 1, 'DINERS')

--insert into tb_tarjeta_tipo values
--('Débito'),
--('Crédito')

--INSERT INTO [dbo].[tb_persona] VALUES ('Raul', 'Dario', 'Castañeda', 'Najar', '', '', '', '')
--INSERT INTO [dbo].[tb_persona] VALUES ('Jesus', 'Enrique', 'Castañeda', 'Najar', '', '', '', '')
--INSERT INTO [dbo].[tb_persona] VALUES ('Jack', 'Enamuel', 'Garcia', 'Castañeda', '', '', '', '')
--INSERT INTO [dbo].[tb_persona] VALUES ('Raul', '', 'Castañeda', 'Achuy', '', '', '', '')
--INSERT INTO [dbo].[tb_persona] VALUES ('Jean', 'Deynis', 'Valenzuela', 'Najar', '', '', '', '')
--INSERT INTO [dbo].[tb_persona] VALUES ('Swan', '', 'Alaca', 'Najar', '', '', '', '')
--INSERT INTO [dbo].[tb_persona] VALUES ('Rosamaria', 'Yaneth', 'Rodriguez', 'Chabasunco', '', '', '', '')
--INSERT INTO [dbo].[tb_persona] VALUES ('Paul', '', 'Garcia', 'Castañeda', '', '', '', '')

--insert into tb_tarjeta values
--('Bcp Visa Crédito',1,1,'',GETDATE(), 2, null),
--('Interbank Signatura',2,1,'',GETDATE(), 2, null),
--('BBVA Visa Crédito',3,1,'',GETDATE(), 2, null),
--('Diners Club',4,1,'',GETDATE(), 2, null),
--('Bcp Visa Débito',1,1,'',GETDATE(), 1, null),
--('Interbank Visa Débito',2,1,'',GETDATE(), 1, null),
--('BBVA Visa Débito',3,1,'',GETDATE(), 1, null)

--insert into tb_cuenta values
--('Bcp Soles',5,1),
--('Bcp Dolares',5,2),
--('Interbank  Soles',6, 1),
--('Interbank  Dolares', 6,2),
--('BBVA Soles',7, 1)

--,[idTarjeta]
--      ,[diaInicio]
--      ,[diaCorte]
--      ,[minDiaPago]
--      ,[maxDiaPago]
--      ,[estado]

--INSERT INTO tb_periodo_configuracion values
--(1,27,26,27,21,1,0),
--(2,10,11,27,21,1,1),
--(3,21,20,27,21,1,0),
--(4,27,26,27,21,1,0)

--insert into [tb_movimiento_tipo] values ('Tarjetas de Crédito'), ('Cuentas Bancarias'), ('Efectivo')