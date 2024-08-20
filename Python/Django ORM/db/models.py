import sys

try:
    from django.db import models
except Exception:
    print('Exception: Django Not Found, please install it with "pip install django".')
    sys.exit()


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
    Orders = models.ManyToOneRel("Orders",field_name="StoreId",to='StoreId', on_delete=models.CASCADE)

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
    Position = models.ForeignKey(Position, db_column="PositionId", on_delete=models.DO_NOTHING)

class Order(models.Model):
    class Meta:
        db_table = "Orders"

    OrderId = models.AutoField(primary_key=True)
    Client = models.OneToOneField(Client, db_column="ClientId", on_delete=models.CASCADE)
    Employee = models.ForeignKey(Employee, db_column="EmployeeId", on_delete=models.CASCADE)
    OrderDate = models.TextField()
    TotalCost = models.FloatField()
    Store = models.ForeignKey(Store, db_column="StoreId", on_delete=models.DO_NOTHING)
