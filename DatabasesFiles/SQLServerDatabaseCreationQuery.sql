CREATE TABLE Clients(
	ClientId INT IDENTITY(1, 1) NOT NULL,
	FirstName NVarChar (20) NOT NULL,
	LastName NVarChar(20) NOT NULL,
	PhoneNumber NVarChar(15),
	Address XML NOT NULL,
	Country NVarChar(20)
)

CREATE TABLE Products(
	ProductId INT IDENTITY(1, 1) NOT NULL,
	ProductName NVarChar(50) NOT NULL,
	ProductDescription NVarChar(MAX),
	Price Decimal (18,2) NOT NULL,
	Supplier NVarChar(50) NOT NULL
)

CREATE TABLE Stores(
	StoreId INT IDENTITY(1, 1) NOT NULL,
	Address NVarChar(100) NOT NULL,
	Country NVarChar(100) NOT NULL
)

CREATE TABLE Positions (
	PositionId INT IDENTITY(1, 1) NOT NULL,
	PositionName NVarChar (100) NOT NULL,
	CarParkingPlace NVarChar (20),
	PositionBonus Double
)

CREATE TABLE Employees(
	EmployeeId INT IDENTITY(1, 1) NOT NULL,
	FirstName NVarChar (20) NOT NULL,
	LastName NVarChar(20) NOT NULL,
	PhoneNumber NVarChar(15),
	Adress XML NOT NULL,
	PositionId INT FOREIGN KEY REFRERENCES Positions(PositionId)
)

CREATE TABLE Orders(
	OrderId INT IDENTITY(1, 1) NOT NULL,
	ClientId INT NOT NULL FOREIGN KEY REFERENCES Clients(ClientId),
	EmployeeId INT NOT NULL FOREIGN KEY REFERENCES Employees(EmployeeId),
	OrderDate DateTime,
	OrderDetails XML NOT NULL,
	TotalCost Decimal (18,2) NOT NULL
)