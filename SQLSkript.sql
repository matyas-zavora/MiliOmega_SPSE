USE [PV_projekt]
GO
INSERT [dbo].[predmety] ([zkratka], [nazev]) VALUES (N'A', N'Anglický jazyk')
INSERT [dbo].[predmety] ([zkratka], [nazev]) VALUES (N'C', N'Český jazyk a literatura')
INSERT [dbo].[predmety] ([zkratka], [nazev]) VALUES (N'CIT', N'Cvičení ze správy IT')
INSERT [dbo].[predmety] ([zkratka], [nazev]) VALUES (N'DS', N'Databázové systémy')
INSERT [dbo].[predmety] ([zkratka], [nazev]) VALUES (N'EK', N'Ekonomika')
INSERT [dbo].[predmety] ([zkratka], [nazev]) VALUES (N'M', N'Matematika')
INSERT [dbo].[predmety] ([zkratka], [nazev]) VALUES (N'MVH', N'Multimédia a vývoj her')
INSERT [dbo].[predmety] ([zkratka], [nazev]) VALUES (N'PSS', N'Počítačové systémy a sítě')
INSERT [dbo].[predmety] ([zkratka], [nazev]) VALUES (N'PV', N'Programovací vybavení')
INSERT [dbo].[predmety] ([zkratka], [nazev]) VALUES (N'TV', N'Tělesná výchova')
INSERT [dbo].[predmety] ([zkratka], [nazev]) VALUES (N'WA', N'Webové aplikace')
GO
INSERT [dbo].[trida] ([nazev]) VALUES (N'C1a')
INSERT [dbo].[trida] ([nazev]) VALUES (N'C2c')
INSERT [dbo].[trida] ([nazev]) VALUES (N'C3c')
INSERT [dbo].[trida] ([nazev]) VALUES (N'E1')
GO
INSERT [dbo].[uzivatele] ([username], [_password], [isTeacher], [firstName], [lastName], [trida]) VALUES (N'doe', NULL, 0, N'John', N'Doe', N'C1a')
INSERT [dbo].[uzivatele] ([username], [_password], [isTeacher], [firstName], [lastName], [trida]) VALUES (N'prochazka', N'prochazka', 0, N'Adam', N'Procházka', N'C3c')
INSERT [dbo].[uzivatele] ([username], [_password], [isTeacher], [firstName], [lastName], [trida]) VALUES (N'studenkova', N'studenkova', 1, N'Kristina', N'Studenkova', N'C1a')
INSERT [dbo].[uzivatele] ([username], [_password], [isTeacher], [firstName], [lastName], [trida]) VALUES (N'zavora', N'Matyas321', 0, N'Matyáš', N'Závora', N'C3c')
GO
SET IDENTITY_INSERT [dbo].[znamky] ON 

INSERT [dbo].[znamky] ([ID], [predmetZkratka], [usernameZak], [usernameUcitel], [znamka], [datum], [description]) VALUES (9, N'C', N'zavora', N'studenkova', 1, CAST(N'2023-06-05' AS Date), N'Čtvrtletní písemná práce')
INSERT [dbo].[znamky] ([ID], [predmetZkratka], [usernameZak], [usernameUcitel], [znamka], [datum], [description]) VALUES (11, N'C', N'zavora', N'studenkova', 2, CAST(N'2023-06-05' AS Date), N'Seš frajer :)')
INSERT [dbo].[znamky] ([ID], [predmetZkratka], [usernameZak], [usernameUcitel], [znamka], [datum], [description]) VALUES (12, N'C', N'zavora', N'studenkova', 1, CAST(N'2023-06-06' AS Date), N'')
INSERT [dbo].[znamky] ([ID], [predmetZkratka], [usernameZak], [usernameUcitel], [znamka], [datum], [description]) VALUES (13, N'C', N'zavora', N'studenkova', 1, CAST(N'2023-06-06' AS Date), N'')
INSERT [dbo].[znamky] ([ID], [predmetZkratka], [usernameZak], [usernameUcitel], [znamka], [datum], [description]) VALUES (15, N'C', N'doe', N'studenkova', 1, CAST(N'2023-06-06' AS Date), N'')
SET IDENTITY_INSERT [dbo].[znamky] OFF
GO
