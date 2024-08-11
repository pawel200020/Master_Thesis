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
                'category': u.category
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
    print(response.json())
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
    print(response.json())
    id=id+1