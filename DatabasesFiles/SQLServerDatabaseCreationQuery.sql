IF NOT EXISTS (SELECT name FROM master.sys.databases WHERE name = N'FrameworkPerformanceMT')
Begin
 CREATE DATABASE FrameworkPerformanceMT
	COLLATE Latin1_General_100_CI_AI_SC_UTF8
End
Go

USE FrameworkPerformanceMT
GO

IF OBJECT_ID('OrdersInfoCollection') IS NOT NULL
DROP XML SCHEMA COLLECTION OrdersInfoCollection;
GO
CREATE XML SCHEMA COLLECTION OrdersInfoCollection AS 
'<xsd:schema xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
xmlns="urn:OrdersInfoNamespace" 
targetNamespace="urn:OrdersInfoNamespace" 
elementFormDefault="qualified">
  <xsd:element name="Order">
    <xsd:complexType>
      <xsd:sequence>
        <xsd:element name="Product" minOccurs="1" maxOccurs="unbounded">
          <xsd:complexType>
            <xsd:sequence>
              <xsd:element name="Name" type="xsd:string" minOccurs="1" maxOccurs="1" />
              <xsd:element name="Quantity" type="xsd:integer" minOccurs="1" maxOccurs="1" />
            </xsd:sequence>
            <xsd:attribute name="id" type="xsd:integer" use="required"/>
          </xsd:complexType>
        </xsd:element>
      </xsd:sequence>
    </xsd:complexType>
  </xsd:element>
</xsd:schema>';
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='Clients') 
BEGIN
	CREATE TABLE Clients(
		ClientId INT IDENTITY(1, 1) NOT NULL,
		FirstName NVarChar (20) NOT NULL,
		LastName NVarChar(20) NOT NULL,
		PhoneNumber NVarChar(15),
		Address NVarChar(100) NOT NULL,
		Country NVarChar(20),
		PRIMARY KEY (ClientId)
	)
END
GO
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='Products')
BEGIN
	CREATE TABLE Products(
		ProductId INT IDENTITY(1, 1) NOT NULL,
		ProductName NVarChar(50) NOT NULL,
		ProductDescription NVarChar(MAX),
		Price Decimal (18,2) NOT NULL,
		Supplier NVarChar(50) NOT NULL,
		PRIMARY KEY (ProductId)
	)
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='Stores')
BEGIN
	CREATE TABLE Stores(
		StoreId INT IDENTITY(1, 1) NOT NULL,
		Address NVarChar(100) NOT NULL,
		Country NVarChar(100) NOT NULL,
		PRIMARY KEY (StoreId)
	)
END
GO
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='Positions')
BEGIN
	CREATE TABLE Positions (
		PositionId INT IDENTITY(1, 1) NOT NULL,
		PositionName NVarChar (100) NOT NULL,
		CarParkingPlace NVarChar (20),
		PositionBonus Decimal (18,2),
		PRIMARY KEY (PositionId)
	)
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='Employees')
BEGIN
	CREATE TABLE Employees(
		EmployeeId INT IDENTITY(1, 1) NOT NULL,
		FirstName NVarChar (20) NOT NULL,
		LastName NVarChar(20) NOT NULL,
		PhoneNumber NVarChar(15),
		Address NVarChar(100) NOT NULL,
		PositionId INT FOREIGN KEY (PositionId) REFERENCES dbo.Positions (PositionId),
		PRIMARY KEY (EmployeeId)
	)
END 
GO
DROP TABLE Orders 
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='Orders')
BEGIN
	CREATE TABLE Orders(
		OrderId INT IDENTITY(1, 1) NOT NULL,
		ClientId INT NOT NULL FOREIGN KEY REFERENCES Clients(ClientId) ON DELETE CASCADE,
		EmployeeId INT NOT NULL FOREIGN KEY REFERENCES Employees(EmployeeId),
		OrderDate DateTime,
		OrderDetails XML (OrdersInfoCollection) NOT NULL,
		TotalCost Decimal (18,2) NOT NULL,
		StoreId INT  FOREIGN KEY (StoreId) REFERENCES dbo.Stores (StoreId) ON DELETE CASCADE,
		PRIMARY KEY (OrderId)
	)
END
GO