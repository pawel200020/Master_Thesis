import json
import requests
import datetime   
import random
class Product:
    def __init__(self,id,name,description,price,category):
        self.id = id
        self.name = name
        self.description = description
        self.price = price
        self.category = category

    def to_dict(u):
        if isinstance(u, Product):
            dict = {
                "id": u.id,
                "name": u.name,
                'description':u.description,
                'price': u.price,
                'category': u.category,
                "id_int": u.id,
            }
        return dict


class Client:
    def __init__(self, id, name, last_name, age, favourite_product, recent_bought_products, salary,birth_date, description):
        self.id = id
        self.description = description
        self.name = name
        self.last_name = last_name
        self.age = age
        self.birth_date = birth_date
        self.favourite_product = favourite_product
        self.salary = salary
        self.recent_bought_products= recent_bought_products
    def to_dict(u):
        if isinstance(u, Client):
            dict = {
                "id": u.id,
                "name": u.name,
                "last_name": u.last_name,
                'salary': u.salary,
                'description':u.description,
                'age': u.age,
                'favourite_product': u.favourite_product,
                'recent_bought_products': u.recent_bought_products,
                'birth_date': f'{u.birth_date.year}-{u.birth_date.month}-{u.birth_date.day}T'
            }
        return dict
    
def convert_date(dateRaw):
    splitted = dateRaw.split("-")
    return datetime.datetime(int(splitted[0]), int(splitted[1]),int(splitted[2]))

def findAge(date):
    return datetime.datetime.now().year - date.year


def createFields():
    clientsUrl="http://localhost:8983/solr/Clients/schema"
    productsUrl="http://localhost:8983/solr/Products/schema"
    clientsJsons = [{
        "add-field":{
            "name":"age",
            "type": "pint",
            "indexed": True,
            "stored": True },
    },{
        "add-field":{
            "name":"birth_date",
            "type": "pdate",
            "indexed": True,
            "stored": True,
            "docValues": True }},{
     "add-field":{
            "name":"description",
            "type": "text_en",
            "indexed": True,
            "stored": True},},{
        "add-field":{
            "name":"favourite_product",
            "type": "pint",
            "indexed": True,
            "stored": True},},{
        "add-field":{
            "name":"name",
            "type": "string",
            "indexed": True,
            "stored": True,
            "docValues": True },},{
        "add-field":{
            "name":"last_name",
            "type": "string",
            "indexed": True,
            "stored": True,
            "docValues": True },},{
        "add-field":{
            "name":"personal_data",
            "type": "string",
            "indexed": True,
            "stored": True,
            "multiValued": True },},{
        "add-field":{
            "name":"recent_bought_products",
            "type": "string",
            "indexed": True,
            "stored": True,
            "multiValued": True },},{
        "add-field":{
            "name":"salary",
            "type": "pdouble",
            "indexed": True,
            "stored": True
            }},{
        "add-copy-field":{
            "source":"age",
            "dest": ["personal_data"]},},{
        "add-copy-field":{
            "source":"name",
            "dest": ["personal_data"]},},{
        "add-copy-field":{
            "source":"last_name",
           "dest": ["personal_data"]},},{
        "add-copy-field":{
            "source":"salary",
            "dest": ["personal_data"]}
    }]
    
    productsJsons = [{
        "add-field":{
            "name":"name",
            "type": "string",
            "indexed": True,
            "stored": True ,
            "docValues": True },},{
        "add-field":{
            "name":"description",
            "type": "text_en",
            "indexed": True,
            "stored": True},},{
        "add-field":{
            "name":"price",
            "type": "pdouble",
            "indexed": True,
            "stored": True},},{
        "add-field":{
            "name":"category",
            "type": "text_en",
            "indexed": True,
            "stored": True},
    },{
        "add-field":{
            "name":"id_int",
            "type": "pint",
            "indexed": True,
            "stored": True ,
            "docValues": True },}]
    for i in clientsJsons:
        response = requests.post(clientsUrl,json = i)
        print(f"{response.json()} Clients fields creation")
        if response.status_code != 200:
            return False
    for i in productsJsons:
        response = requests.post(productsUrl,json = i)
        print(f"{response.json()} Products fields creation")
        if response.status_code != 200:
            return False
    return True
    
def prepareSolr():
    if(createFields() == False):
        return
    url =   "http://localhost:8983/solr/Clients/update/json/docs"
    file2 = open("random_products_with_descriptions.txt")
    products = [line[:-2:].split(" - ") for line in file2]
    file2.close()
    id = 1
    file1 = open("processed_random_short_profiles.txt", "r")
    for line in file1:
        splitted = line[:-2].split(",")
        date = convert_date(splitted[2])
        client = Client(id, splitted[0],splitted[1],findAge(date),random.randint(0,10000),[products[random.randint(0,10000)][0],products[random.randint(0,10000)]][0],random.randint(100000,1000000)/100,date, splitted[3])
        response = requests.post(url, json.dumps(client, default=Client.to_dict))
        print(f"{response.json()} {id}")
        id= id+1
    file1.close()
    id =1
    file3 = open("categories.txt")
    categories = [line[:-1:] for line in file3]
    file3.close()
    url = "http://localhost:8983/solr/Products/update/json/docs"
    for product in products:
        product = Product(id,product[0],product[1],random.randint(100,100000)/100,categories[random.randint(0,344)])
        response = requests.post(url, json.dumps(product, default=Product.to_dict))
        print(f"{response.json()} {id}")
        id=id+1

def addNonStandardField(filedName,id,value):
    url =   "http://localhost:8983/solr/Clients/update/"
    json = [{
        "id":f"{id}",
        f"{filedName}": {"set": value}
    }]
    response = requests.post(url, json= json)
    print(f"{response.json()} {id}")
    return

def addNonStandardFields():
    for i in range(800):
        id = random.randint(1,1000)
        field = random.randint(1,3)
        if field == 1:
            addNonStandardField("has_children_b",id,i%2==0)
        elif field ==2:
            addNonStandardField("how_many_cats_i",id,random.randint(1,20))
        else:
            addNonStandardField("has_partner_b",id,i%2==0)

prepareSolr()
addNonStandardFields()