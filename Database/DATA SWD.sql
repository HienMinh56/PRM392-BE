INSERT INTO [CampusFoodSystem].[dbo].[Area] (AreaId, Name, Status) VALUES 
('AREA001', 'Area 1', 1),
('AREA002', 'Area 2', 1),
('AREA003', 'Area 3', 2);

INSERT INTO [CampusFoodSystem].[dbo].[Campus] (CampusId, AreaId, Name, Status) VALUES 
('CAMP001', 'AREA001', 'Campus A', 1),
('CAMP002', 'AREA002', 'Campus B', 1),
('CAMP003', 'AREA003', 'Campus C', 2);

INSERT INTO [CampusFoodSystem].[dbo].[Session] (SessionId, StartTime, EndTime) VALUES 
('SESSION001', '05:00', '8:00'),
('SESSION002', '9:00', '11:00');

INSERT INTO [CampusFoodSystem].[dbo].[AreaSession] (AreaSessionId, SessionId, AreaId) VALUES 
('AREASESSION001', 'SESSION001', 'AREA001'),
('AREASESSION002', 'SESSION002', 'AREA002'),
('AREASESSION003', 'SESSION002', 'AREA002');

INSERT INTO [CampusFoodSystem].[dbo].[Store] (StoreId, AreaId, Name, Address, Status, Phone) VALUES 
('STORE001', 'AREA001', 'Quán Cơm chay', '123 Street A', 1, 123456789),
('STORE002', 'AREA002', 'Quán Nem Nướng', '456 Street B', 1, 987654321),
('STORE003', 'AREA003', 'Quán Bia', '789 Street C', 2, 111222333);

INSERT INTO [CampusFoodSystem].[dbo].[Food] (FoodId, Name, StoreId, Price, Title, Description, Cate, Image, Status) VALUES 
('FOOD001', 'Cơm thập cẩm', 'STORE001', 10, 'Ăn chay tịnh tâm', 'Gồm abcdxyz', 1, NULL,1),
('FOOD002', 'Combo nem nướng', 'STORE002', 15, 'Nem nướng nhà làm', 'Description for Food', 1, NULL,1),
('FOOD003', 'Combo vui vẻ', 'STORE003', 20, 'Bia + gà', 'Description for Food ', 1, NULL, 1);

INSERT INTO [CampusFoodSystem].[dbo].[User] (UserId, Name, UserName, Password, Email, CampusId, Phone, Role, Balance, Status,AccessToken, RefreshToken) VALUES 
('USER001', 'admin', 'admin123', '123', 'admin1@example.com', 'CAMP001', 123456789, 1, 100, 1, NULL, NULL),
('USER002', 'customer', 'customer1', '123', 'customer1@example.com', 'CAMP001', 987654321, 2, 200, 1, NULL, NULL),
('USER003', 'shiper', 'shiper123', '123', 'shipper1@example.com', 'CAMP001', 111222333, 3, 150, 1, NULL, NULL),
('USER004', 'shiper', 'shiper123', '123', 'shipper2@example.com', 'CAMP002', 123456789, 3, 100, 1, NULL, NULL),
('USER005', 'customer', 'customer2', '123', 'customer2@example.com', 'CAMP002', 1567890, 2, 200, 1,NULL, NULL),
('USER006', 'customer', 'customer3', '123', 'customer3@example.com', 'CAMP002', 1345678, 2, 1000, 1, NULL, NULL),
('USER007', 'customer', 'customer4', '123', 'customer4@example.com', 'CAMP001', 1234567, 2, 200, 1, NULL, NULL);

INSERT INTO [CampusFoodSystem].[dbo].[Transaction] (TransationId, UserId, Type, Amonut, Status, Time) VALUES 
('TRANS001', 'USER002', 2, 50, 1, GETDATE()),
('TRANS002', 'USER005', 1, 100, 1, GETDATE()),
('TRANS003', 'USER006', 1, 70, 1, GETDATE()),
('TRANS004', 'USER007', 1, 200, 1, GETDATE());

INSERT INTO [CampusFoodSystem].[dbo].[Order] (OrderId, UserId, AreaSessionId, Price, Quantity, StoreId, TransationId, Status) VALUES 
('ORDER001', 'USER005', 'AREASESSION003', 100, 2, 'STORE001', 'TRANS002', 1),
('ORDER002', 'USER006', 'AREASESSION002', 200, 3, 'STORE002', 'TRANS003', 1),
('ORDER003', 'USER007', 'AREASESSION001', 100, 4, 'STORE002', 'TRANS004', 1);

INSERT INTO [CampusFoodSystem].[dbo].[OrderDetail] (OrderDetailId, OrderId, FoodId, Status, Price, Quantity, Note) VALUES 
('DETAIL001', 'ORDER001', 'FOOD001', 1, 10, 1, NULL),
('DETAIL002', 'ORDER001', 'FOOD002', 1, 20, 1, NULL),
('DETAIL003', 'ORDER002', 'FOOD002', 1, 15, 1, NULL),
('DETAIL004', 'ORDER002', 'FOOD003', 1, 30, 2, NULL),
('DETAIL005', 'ORDER003', 'FOOD001', 1, 10, 2, NULL),
('DETAIL006', 'ORDER003', 'FOOD002', 1, 15, 2, NULL),
('DETAIL007', 'ORDER003', 'FOOD003', 1, 20, 1, NULL);