from Common import OrderXML
from Common.TableWithNull import TableWithNull
from db.models import *
from Common.TestResult import TestResult
from datetime import datetime
from tqdm import tqdm
import random

class SqlServerTests:

    def readTxt(self, fileName):
        with open(fileName) as single:
            return single.readlines()

    def removeRecordsSilently(self, idsToRemove):
        for i in tqdm(idsToRemove):
            ProductServ.objects.using('sql-server').filter(ProductId=i).delete()

    def addRecordsSilently(self, samplesQuantity):
        productNames = self.readTxt("real_product_names.txt")
        addedIds = []
        for i in tqdm(range(samplesQuantity)):
            name = random.choice(productNames)
            product = ProductServ.objects.using('sql-server').create(ProductName=name, ProductDescription="AddedByTest", Price=21.36,
                                             Supplier="None")
            addedIds.append(product.ProductId)
        return addedIds

    def singleRecordSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "singleRecordSearch")
        employeeCount = EmployeeServ.objects.using('sql-server').count()
        for i in tqdm(range(samplesQuantity)):
            id = random.randrange(1, employeeCount)
            now = datetime.now()
            item = EmployeeServ.objects.using('sql-server').get(EmployeeId=id)
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        return test_result

    def setOfDataSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "setOfDataSearch")
        positionsCount = PositionServ.objects.using('sql-server').count()
        for i in tqdm(range(samplesQuantity)):
            id = random.randrange(1, positionsCount)
            now = datetime.now()
            item = EmployeeServ.objects.using('sql-server').filter(Position_id=id)
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        return test_result

    def setOfDataWithIsNullSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "setOfDataWithIsNullSearch")
        for i in tqdm(range(samplesQuantity)):
            table = TableWithNull(random.randrange(1, len(TableWithNull)))
            match table:
                case TableWithNull.ClientPhone:
                    now = datetime.now()
                    ClientServ.objects.using('sql-server').filter(PhoneNumber__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)
                case TableWithNull.ClientCountry:
                    now = datetime.now()
                    ClientServ.objects.using('sql-server').filter(Country__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)
                case TableWithNull.EmployeesPhone:
                    now = datetime.now()
                    EmployeeServ.objects.using('sql-server').filter(PhoneNumber__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)
                case TableWithNull.EmployeesPositionId:
                    now = datetime.now()
                    EmployeeServ.objects.using('sql-server').filter(Position_id__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)
                case TableWithNull.Orders:
                    now = datetime.now()
                    OrderServ.objects.using('sql-server').filter(OrderDate__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)
                case TableWithNull.Products:
                    now = datetime.now()
                    OrderServ.objects.using('sql-server').filter(ProductDescription__isnull=True)
                    elapsed = now.microsecond // 1000
                    test_result.add_sample(elapsed)
        return test_result

    def addRecords(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "AddRecords")
        productNames = self.readTxt("real_product_names.txt")
        idsToRemove = []
        for _ in tqdm(range(samplesQuantity)):
            name = random.choice(productNames)
            now = datetime.now()
            product = ProductServ.objects.using('sql-server').create(ProductName=name, ProductDescription="AddedByTest", Price=21.36,
                                             Supplier="None")
            idsToRemove.append(product.ProductId)
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        self.removeRecordsSilently(idsToRemove)
        return test_result

    def editRecords(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "EditRecords")
        addedIds = self.addRecordsSilently(samplesQuantity)
        productNames = self.readTxt("real_product_names.txt")
        for _ in tqdm(range(samplesQuantity)):
            name = random.choice(productNames)
            id = random.choice(addedIds)
            now = datetime.now()
            item = ProductServ.objects.using('sql-server').get(ProductId=id)
            item.ProductName = name
            item.save()
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        self.removeRecordsSilently(addedIds)
        return test_result

    def deleteRecords(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "DeleteRecords")
        addedIds = self.addRecordsSilently(samplesQuantity)
        for i in tqdm(addedIds):
            now = datetime.now()
            ProductServ.objects.using('sql-server').filter(ProductId=i).delete()
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        return test_result

    def searchTwoRelatedTables(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "SearchTwoRelatedTables")
        positionsCount = PositionServ.objects.using('sql-server').count()
        for i in tqdm(range(samplesQuantity)):
            position = PositionServ.objects.using('sql-server').get(PositionId=random.randrange(1, positionsCount)).PositionName
            now = datetime.now()
            result = EmployeeServ.objects.using('sql-server').filter(Position__PositionName=position)
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        return test_result

    def serchFourRelatedTables(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "SearchFourRelatedTables")
        storesCount = StoreServ.objects.using('sql-server').count()
        positionsCount = PositionServ.objects.using('sql-server').count()
        for i in tqdm(range(samplesQuantity)):
            positionToFind = PositionServ.objects.using('sql-server').get(PositionId=random.randrange(1, positionsCount)).PositionName
            storeCountryToFind = StoreServ.objects.using('sql-server').get(StoreId=random.randrange(1, storesCount)).Country
            now = datetime.now()
            result = OrderServ.objects.using('sql-server').filter(Employee__Position__PositionName=positionToFind,
                                          Store__Country=storeCountryToFind)
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        return test_result

    def searchRecordsWhichDoesNotHaveConnection(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "searchRecordsWhichDoesNotHaveConnection")
        for i in tqdm(range(samplesQuantity)):
            if i%2 != 0:
                now = datetime.now()
                usedPosition =  set([position.Position_id for position in EmployeeServ.objects.using('sql-server').filter(Position__isnull=False)])
                result = PositionServ.objects.using('sql-server').exclude(PositionId__in=usedPosition)
                elapsed = now.microsecond // 1000
                test_result.add_sample(elapsed)
            else:
                now = datetime.now()
                usedStores = set([order.Store_id for order in Order.objects.filter()])
                result = StoreServ.objects.using('sql-server').exclude(StoreId__in=usedStores)
                elapsed = now.microsecond // 1000
                test_result.add_sample(elapsed)
        return test_result

    def searchWithSubQuery(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "searchWithSubQuery")
        countries = self.readTxt("Country.txt")
        for i in tqdm(range(samplesQuantity)):
            country = random.choice(countries)
            now = datetime.now()
            OrderServ.objects.using('sql-server').filter(Client__Country=country)
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        return test_result

    def removeRelatedRecords(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "removeRelatedRecords")
        clientsCount = ClientServ.objects.using('sql-server').count()
        employeesCount = EmployeeServ.objects.using('sql-server').count()
        for i in tqdm(range(samplesQuantity)):
            client =  ClientServ.objects.using('sql-server').get(ClientId=random.randrange(1, clientsCount))
            employee = EmployeeServ.objects.using('sql-server').get(EmployeeId=random.randrange(1, employeesCount))
            store = StoreServ.objects.using('sql-server').create(Address="Norymberska 1 30-412 Krakow", Country = "Poland")
            order = OrderServ.objects.using('sql-server').create(Employee=employee, Store=store, Client=client, OrderDate=datetime.now(), TotalCost = 998)
            now = datetime.now()
            Store.objects.filter(StoreId=store.StoreId).delete()
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        return test_result