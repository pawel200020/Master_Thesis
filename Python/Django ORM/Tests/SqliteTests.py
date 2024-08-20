from Common.TestResult import TestResult
from datetime import datetime
from tqdm import tqdm
import random
from db.models import *
from enum import Enum

class SqliteTests:

    def readTxt(self, fileName):
        with open(fileName) as single:
            return single.readlines()

    def singleRecordSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "singleRecordSearch")
        ordersCount = Employee.objects.count()
        for i in tqdm(range(samplesQuantity)):
            id = random.randrange(1, ordersCount)
            now = datetime.now()
            item = Employee.objects.get(EmployeeId=id)
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        return test_result

    def setOfDataSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "setOfDataSearch")
        positionsCount = Position.objects.count()
        for i in tqdm(range(samplesQuantity)):
            id = random.randrange(1, positionsCount)
            now = datetime.now()
            item = Employee.objects.filter(Position_id=id)
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        return test_result

    def setOfDataWithIsNullSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "setOfDataWithIsNullSearch")
        for i in tqdm(range(samplesQuantity)):
            table = TableWithNull(random.randrange(1,len(TableWithNull)))
            match table:
                case TableWithNull.ClientPhone:
                    now = datetime.now()
                    Client.objects.filter(PhoneNumber__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)
                case TableWithNull.ClientCountry:
                    now = datetime.now()
                    Client.objects.filter(Country__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)
                case TableWithNull.EmployeesPhone:
                    now = datetime.now()
                    Employee.objects.filter(PhoneNumber__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)
                case TableWithNull.EmployeesPositionId:
                    now = datetime.now()
                    Employee.objects.filter(Position_id__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)
                case TableWithNull.Orders:
                    now = datetime.now()
                    Order.objects.filter(OrderDate__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)
                case TableWithNull.Products:
                    now = datetime.now()
                    Order.objects.filter(ProductDescription__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)

    def addRecords(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "AddRecords")
        productNames = self.readTxt("real_product_names.txt")
        for i in tqdm(range(samplesQuantity)):
            Product.objects.create(name=productNames[i],   )

        return test_result



class TableWithNull(Enum):
    ClientPhone = 1,
    ClientCountry = 2,
    EmployeesPhone = 3,
    EmployeesPositionId = 4,
    Orders = 5,
    Products = 6,