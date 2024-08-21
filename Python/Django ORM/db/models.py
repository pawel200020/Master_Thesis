import sys

try:
    from django.db import models
except Exception:
    print('Exception: Django Not Found, please install it with "pip install django".')
    sys.exit()
################################### SQLite ######################################

class Product(models.Model):
    class Meta:
        db_table = "Products"

    ProductId = models.AutoField(primary_key=True)
    ProductName = models.TextField()
    ProductDescription = models.TextField()
    Price = models.FloatField()
    Supplier = models.TextField()


class Client(models.Model):
    class Meta:
        db_table = "Clients"

    ClientId = models.AutoField(primary_key=True)
    FirstName = models.TextField()
    LastName = models.TextField()
    PhoneNumber = models.TextField()
    Address = models.TextField()
    Country = models.TextField()


class Store(models.Model):
    class Meta:
        db_table = "Stores"

    StoreId = models.AutoField(primary_key=True)
    Address = models.TextField()
    Country = models.TextField()

class Position(models.Model):
    class Meta:
        db_table = "Positions"

    PositionId = models.AutoField(primary_key=True)
    PositionName = models.TextField()
    CarParkingPlace = models.TextField()
    PositionBonus = models.TextField()

class Employee(models.Model):
    class Meta:
        db_table = "Employees"

    EmployeeId = models.AutoField(primary_key=True)
    FirstName = models.TextField()
    LastName = models.TextField()
    PhoneNumber = models.TextField()
    Adress = models.TextField()
    Position = models.ForeignKey(Position, db_column="PositionId", on_delete=models.CASCADE)

class Order(models.Model):
    class Meta:
        db_table = "Orders"

    OrderId = models.AutoField(primary_key=True)
    Client = models.OneToOneField(Client, db_column="ClientId", on_delete=models.CASCADE)
    Employee = models.ForeignKey(Employee, db_column="EmployeeId", on_delete=models.CASCADE)
    OrderDate = models.TextField()
    TotalCost = models.FloatField()
    Store = models.ForeignKey(Store, db_column="StoreId", on_delete= models.CASCADE)
    OrderDetails = models.TextField()

################################### SQL Server ######################################

class ProductServ(models.Model):
    class Meta:
        db_table = "Products"

    ProductId = models.AutoField(primary_key=True)
    ProductName = models.TextField()
    ProductDescription = models.TextField()
    Price = models.FloatField()
    Supplier = models.TextField()

class ClientServ(models.Model):
    class Meta:
        db_table = "Clients"

    ClientId = models.AutoField(primary_key=True)
    FirstName = models.TextField()
    LastName = models.TextField()
    PhoneNumber = models.TextField()
    Address = models.TextField()
    Country = models.TextField()


class StoreServ(models.Model):
    class Meta:
        db_table = "Stores"

    StoreId = models.AutoField(primary_key=True)
    Address = models.TextField()
    Country = models.TextField()

class PositionServ(models.Model):
    class Meta:
        db_table = "Positions"

    PositionId = models.AutoField(primary_key=True)
    PositionName = models.TextField()
    CarParkingPlace = models.TextField()
    PositionBonus = models.FloatField()

class EmployeeServ(models.Model):
    class Meta:
        db_table = "Employees"

    EmployeeId = models.AutoField(primary_key=True)
    FirstName = models.TextField()
    LastName = models.TextField()
    PhoneNumber = models.TextField()
    Address = models.TextField()
    Position = models.ForeignKey(PositionServ, db_column="PositionId", on_delete=models.CASCADE)

class OrderServ(models.Model):
    class Meta:
        db_table = "Orders"

    OrderId = models.AutoField(primary_key=True)
    Client = models.OneToOneField(ClientServ, db_column="ClientId", on_delete=models.CASCADE)
    Employee = models.ForeignKey(EmployeeServ, db_column="EmployeeId", on_delete=models.CASCADE)
    OrderDate = models.DateField()
    TotalCost = models.FloatField()
    Store = models.ForeignKey(StoreServ, db_column="StoreId", on_delete= models.CASCADE)
    OrderDetails = models.TextField()