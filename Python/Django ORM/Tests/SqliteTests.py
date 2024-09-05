from Common.OrderXML import orderXml
from Common.TableWithNull import TableWithNull
from Common.TestResult import TestResult
from datetime import datetime
from tqdm import tqdm
import random
from db.models import *

class SqliteTests:

    def readTxt(self, fileName):
        with open(fileName) as single:
            return single.readlines()

    def removeRecordsSilently(self, idsToRemove):
        for i in tqdm(idsToRemove):
            Product.objects.filter(ProductId=i).delete()

    def addRecordsSilently(self, samplesQuantity):
        productNames = self.readTxt("real_product_names.txt")
        addedIds = []
        for i in tqdm(range(samplesQuantity)):
            name = random.choice(productNames)
            product = Product.objects.create(ProductName=name, ProductDescription="AddedByTest", Price=21.36,
                                             Supplier="None")
            addedIds.append(product.ProductId)
        return addedIds

    def singleRecordSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "singleRecordSearch")
        ordersCount = Employee.objects.count()
        for i in tqdm(range(samplesQuantity)):
            id = random.randrange(1, ordersCount)
            now = datetime.now()
            item = Employee.objects.get(EmployeeId=id)
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result

    def setOfDataSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "setOfDataSearch")
        positionsCount = Position.objects.count()
        for i in tqdm(range(samplesQuantity)):
            id = random.randrange(1, positionsCount)
            now = datetime.now()
            item = [x for x in Employee.objects.filter(Position_id=id)]
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result

    def setOfDataWithIsNullSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "setOfDataWithIsNullSearch")
        for i in tqdm(range(samplesQuantity)):
            table = TableWithNull(random.randrange(1, len(TableWithNull)))
            match table:
                case TableWithNull.ClientPhone:
                    now = datetime.now()
                    result = [x for x in Client.objects.filter(PhoneNumber__isnull=True)]
                    elapsed = datetime.now()
                    test_result.add_sample((elapsed - now).microseconds / 1000)
                case TableWithNull.ClientCountry:
                    now = datetime.now()
                    result = [x for x in Client.objects.filter(Country__isnull=True)]
                    elapsed = datetime.now()
                    test_result.add_sample((elapsed - now).microseconds / 1000)
                case TableWithNull.EmployeesPhone:
                    now = datetime.now()
                    result = [x for x in Employee.objects.filter(PhoneNumber__isnull=True)]
                    elapsed = datetime.now()
                    test_result.add_sample((elapsed - now).microseconds / 1000)
                case TableWithNull.EmployeesPositionId:
                    now = datetime.now()
                    result = [x for x in Employee.objects.filter(Position_id__isnull=True)]
                    elapsed = datetime.now()
                    test_result.add_sample((elapsed - now).microseconds / 1000)
                case TableWithNull.Orders:
                    now = datetime.now()
                    result = [x for x in Order.objects.filter(OrderDate__isnull=True)]
                    elapsed = datetime.now()
                    test_result.add_sample((elapsed - now).microseconds / 1000)
                case TableWithNull.Products:
                    now = datetime.now()
                    result = [x for x in Order.objects.filter(ProductDescription__isnull=True)]
                    elapsed = datetime.now()
                    test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result

    def addRecords(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "AddRecords")
        productNames = self.readTxt("real_product_names.txt")
        idsToRemove = []
        for _ in tqdm(range(samplesQuantity)):
            name = random.choice(productNames)
            now = datetime.now()
            product = Product.objects.create(ProductName=name, ProductDescription="AddedByTest", Price=21.36,
                                             Supplier="None")
            idsToRemove.append(product.ProductId)
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
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
            item = Product.objects.get(ProductId=id)
            item.ProductName = name
            item.save()
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        self.removeRecordsSilently(addedIds)
        return test_result

    def deleteRecords(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "DeleteRecords")
        addedIds = self.addRecordsSilently(samplesQuantity)
        for i in tqdm(addedIds):
            now = datetime.now()
            Product.objects.filter(ProductId=i).delete()
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result

    def searchTwoRelatedTables(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "SearchTwoRelatedTables")
        positionsCount = Position.objects.count()
        for i in tqdm(range(samplesQuantity)):
            position = Position.objects.get(PositionId=random.randrange(1, positionsCount)).PositionName
            now = datetime.now()
            result = [x for x in Employee.objects.filter(Position__PositionName=position)]
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result

    def serchFourRelatedTables(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "SearchFourRelatedTables")
        storesCount = Store.objects.count()
        positionsCount = Position.objects.count()
        for i in tqdm(range(samplesQuantity)):
            positionToFind = Position.objects.get(PositionId=random.randrange(1, positionsCount)).PositionName
            storeCountryToFind = Store.objects.get(StoreId=random.randrange(1, storesCount)).Country
            now = datetime.now()
            result = [ x for x in Order.objects.filter(Employee__Position__PositionName=positionToFind,
                                          Store__Country=storeCountryToFind)]
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result

    def searchRecordsWhichDoesNotHaveConnection(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "searchRecordsWhichDoesNotHaveConnection")
        for i in tqdm(range(samplesQuantity)):
            if i%2 != 0:
                now = datetime.now()
                usedPosition =  set([position.Position_id for position in Employee.objects.filter(Position__isnull=False)])
                result = [x for x in Position.objects.exclude(PositionId__in=usedPosition)]
                elapsed = datetime.now()
                test_result.add_sample((elapsed - now).microseconds / 1000)
            else:
                now = datetime.now()
                usedStores = set([order.Store_id for order in Order.objects.filter()])
                result = [x for x in Store.objects.exclude(StoreId__in=usedStores)]
                elapsed = datetime.now()
                test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result

    def searchWithSubQuery(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "searchWithSubQuery")
        countries = self.readTxt("Country.txt")
        for i in tqdm(range(samplesQuantity)):
            country = random.choice(countries)
            now = datetime.now()
            result = [ x for x in Order.objects.filter(Client__Country=country)]
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result

    def removeRelatedRecords(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "removeRelatedRecords")
        clientsCount = Client.objects.count()
        employeesCount = Employee.objects.count()
        for i in tqdm(range(samplesQuantity)):
            client =  Client.objects.get(ClientId=random.randrange(1, clientsCount))
            employee = Employee.objects.get(EmployeeId=random.randrange(1, employeesCount))
            store = Store.objects.create(Address="Norymberska 1 30-412 Krakow", Country = "Poland")
            order = Order.objects.create(Employee=employee, Store=store, Client=client, OrderDate=str(datetime.now()), OrderDetails=orderXml, TotalCost = 998)
            now = datetime.now()
            Store.objects.filter(StoreId=store.StoreId).delete()
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result
