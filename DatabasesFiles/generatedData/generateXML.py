import random

def makeWholeXml(paramterizedPart):
    return """<?xml version="1.0" encoding="UTF-16"?><Order xmlns="urn:OrdersInfoNamespace">{0}</Order>""".format(paramterizedPart)

def getParametrizedXmlPart(params):
    return """  <Product id = "{0}" > <Name>{1}</Name> <Quantity>{2}</Quantity> </Product>""".format(*params)

def getXml(produts,productsIds):
    result = ""
    j  = 0
    for i in productsIds:
        quantity= random.randrange(1,1000)
        result += (getParametrizedXmlPart([j, produts[i].replace("\n", ""), quantity])) +" "
        j+=1
    return makeWholeXml(result)

ROW_NUMBERS = 40000
PRODUCTS_NUMBERS = 1000
output = ""
with open('products.txt') as file:
    array = file.readlines()
    for i in range (ROW_NUMBERS):
        quantityOfProducts = random.randrange(1,100)
        productsIds = random.sample(range(1, PRODUCTS_NUMBERS), quantityOfProducts)
        output = (getXml(array, productsIds))+ "\n"
        with open("xmls5.csv", "a") as file:
            file.write(output)
        print(i)