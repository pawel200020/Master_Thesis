import random
from datetime import datetime
import pysolr
from tqdm import tqdm
from Common.TestResult import TestResult

class PySolrTests:
    def __init__(self):
        self.solrProducts = pysolr.Solr('http://localhost:8983/solr/Products', always_commit=True, timeout = 10)
        self.solrClients = pysolr.Solr('http://localhost:8983/solr/Clients', always_commit=True, timeout = 10)

    def readTxt(self, fileName):
        with open(fileName) as single:
            return single.readlines()

    def removeRecordsSilently(self, idsToRemove):
        for i in tqdm(idsToRemove):
            self.solrProducts.delete(i)

    def addRecordsSilently(self, samplesQuantity):
        cartegories = self.readTxt("categories.txt")
        productsCount = (self.solrProducts.search(q="*:*", fl='id name category price description', qf='id', defType='edismax')).raw_response["response"]["numFound"]
        idsToAdd = []
        for i in tqdm(range(samplesQuantity)):
            category = random.choice(cartegories)
            price = random.randrange(1, 9999999) / 100
            id = productsCount + i + 1
            name = "example product {}".format(i)
            self.solrProducts.add([
                {
                    "id": id,
                    "name": name,
                    "category": category,
                    "price": price,
                    "description": "addedByTests"
                }
            ])
            idsToAdd.append(id)
        return idsToAdd


    def singleRecordSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "singleRecordSearch")
        productsCount = (self.solrProducts.search(q="*:*", fl='id name category price description', qf='id',defType='edismax')).raw_response["response"]["numFound"]
        for i in tqdm(range(samplesQuantity)):
            idToFind = random.randrange(1, productsCount)
            now = datetime.now()
            result = self.solrProducts.search(idToFind,fl='id name category price description', qf='id',defType='edismax').docs
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result

    def setOfDataSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "setOfDataSearch")
        names = self.readTxt("first_words.txt")
        for i in tqdm(range(samplesQuantity)):
            nameToFind = random.choice(names)
            now = datetime.now()
            result = self.solrClients.search(nameToFind,fl="id name age salary description favourite_product recent_bought_products birth_date has_children_b how_many_cats_i has_partner_b", qf="name",defType="edismax", rows=10000).docs
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result

    def setOfDataWithIsNullSearch(self,samplesQuantity):
        test_result = TestResult(samplesQuantity, "setOfDataWithIsNullSearch")
        for i in tqdm(range(samplesQuantity)):
            field =  "has_children_b" if i%3 == 0 else "how_many_cats_i" if i % 3 == 1 else "has_partner_b"
            now = datetime.now()
            query = "NOT {}:*".format(field)
            result = self.solrClients.search(query,fl="id name age salary description favourite_product recent_bought_products birth_date has_children_b how_many_cats_i has_partner_b", qf="name",defType="edismax",  rows=10000).docs
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result

    def addRecords (self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "addRecords")
        cartegories = self.readTxt("categories.txt")
        productsCount = (self.solrProducts.search(q="*:*", fl='id name category price description', qf='id',defType='edismax')).raw_response["response"]["numFound"]
        idsToRemove = []
        for i in tqdm(range(samplesQuantity)):
            category = random.choice(cartegories)
            price = random.randrange(1,9999999)/100
            id = productsCount + i + 1
            name = "example product {}".format(i)
            now = datetime.now()
            self.solrProducts.add([
                {
                    "id": id,
                    "name": name,
                    "category": category,
                    "price": price,
                    "description": "addedByTests"
                }
            ])
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
            idsToRemove.append(id)
        self.removeRecordsSilently(idsToRemove)
        return test_result

    def editRecords (self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "editRecords")
        addedIds = self.addRecordsSilently(samplesQuantity)
        for i in tqdm(range(samplesQuantity)):
            idToEdit = random.choice(addedIds)
            now = datetime.now()
            doc = {'id': idToEdit, 'description' : "Edited value set for this by iteration {}".format(i)}
            self.solrProducts.add([doc],fieldUpdates={'description':'set'})
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        self.removeRecordsSilently(addedIds)
        return test_result

    def deleteRecords (self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "deleteRecords")
        addedIds = self.addRecordsSilently(samplesQuantity)
        for i in tqdm(addedIds):
            now = datetime.now()
            self.solrProducts.delete(i)
            elapsed = datetime.now()
            test_result.add_sample((elapsed - now).microseconds / 1000)
        return test_result