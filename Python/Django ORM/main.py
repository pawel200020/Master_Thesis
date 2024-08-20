############################################################################
## Django ORM Standalone Python Template
############################################################################
""" Here we'll import the parts of Django we need. It's recommended to leave
these settings as is, and skip to START OF APPLICATION section below """

# Turn off bytecode generation
import sys

from Tests.RelationalFrameworkMenu import RelationalFrameworkMenu

sys.dont_write_bytecode = True

# Django specific settings
import os
os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'settings')
import django
django.setup()

# Import your models for use in your script
from db.models import *
from Tests.SqliteTests import SqliteTests
############################################################################
## START OF APPLICATION
############################################################################
""" Replace the code below with your own """
# Seed a few users in the database
# User.objects.create(name='Dan')
# User.objects.create(name='Robert')
#
# for u in User.objects.all():
#     print(f'ID: {u.id} \tUsername: {u.name}')
#Product.objects.using('sql-server').all()
sqliteTests = SqliteTests()
sqliteFrameworkMenu = RelationalFrameworkMenu("Sqlite",sqliteTests)
print("Welcome to Database Performance Tester - Python")
while True:
    print("Select framework which you want to test:")
    print("1.Django ORM SQL Server\n2. DjangoOrm Sqlite\nX - Close program")

    intput = input()
    match intput:
         case "2":
             sqliteFrameworkMenu.showMenu()
         case "X":
             break