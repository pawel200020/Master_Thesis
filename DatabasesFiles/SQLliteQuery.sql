CREATE TABLE IF NOT EXISTS Clients(
		ClientId INTEGER PRIMARY KEY,
		FirstName TEXT NOT NULL,
		LastName TEXT NOT NULL,
		PhoneNumber TEXT,
		Address TEXT NOT NULL,
		Country TEXT
	)

CREATE TABLE IF NOT EXISTS Products(
		ProductId INTEGER PRIMARY KEY NOT NULL,
		ProductName TEXT NOT NULL,
		ProductDescription TEXT,
		Price REAL NOT NULL,
		Supplier TEXT NOT NULL
	)


CREATE TABLE IF NOT EXISTS Stores(
		StoreId INTEGER PRIMARY KEY NOT NULL,
		Address TEXT NOT NULL,
		Country TEXT NOT NULL
	)

CREATE TABLE IF NOT EXISTS Positions (
		PositionId INTEGER PRIMARY KEY NOT NULL,
		PositionName TEXT NOT NULL,
		CarParkingPlace TEXT,
		PositionBonus REAL
	)

CREATE TABLE IF NOT EXISTS Employees(
		EmployeeId INTEGER PRIMARY KEY NOT NULL,
		FirstName TEXT NOT NULL,
		LastName TEXT NOT NULL,
		PhoneNumber TEXT,
		Adress TEXT NOT NULL,
		PositionId INT NOT NULL, 
            FOREIGN KEY (PositionId) REFERENCES Positions (PositionId)
	)

CREATE TABLE IF NOT EXISTS Orders(
		OrderId INTEGER PRIMARY KEY NOT NULL,
		ClientId INTEGER NOT NULL,
		EmployeeId INTEGER NOT NULL,
		OrderDate TEXT,
		OrderDetails TEXT NOT NULL,
		TotalCost NUMERIC NOT NULL,
            FOREIGN KEY (ClientId) REFERENCES Clients(ClientId) ON DELETE CASCADE,
            FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
	)