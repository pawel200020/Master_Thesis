############################################################################
## Django ORM Standalone Python Template
############################################################################
""" Here we'll import the parts of Django we need. It's recommended to leave
these settings as is, and skip to START OF APPLICATION section below """

# Turn off bytecode generation
import sys

from PySolr.PySolrTests import PySolrTests
from Tests.FrameworkMenu import FrameworkMenu

sys.dont_write_bytecode = True

# Django specific settings
import os
os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'settings')
import django
django.setup()

# Import your models for use in your script
from Tests.RelationalFrameworkMenu import RelationalFrameworkMenu
from Tests.SqliteTests import SqliteTests
from Tests.SqlServerTests import SqlServerTests
############################################################################
## START OF APPLICATION
############################################################################
sqliteTests = SqliteTests()
sqlServerTests = SqlServerTests()
pySolrTests = PySolrTests()

sqliteFrameworkMenu = RelationalFrameworkMenu("Sqlite",sqliteTests)
sqlServerFrameworkMenu = RelationalFrameworkMenu("SQL Server",sqlServerTests)
solrServerFrameworkMenu = FrameworkMenu("PySolr",pySolrTests)
print("Welcome to Database Performance Tester - Python")
while True:
    print("Select framework which you want to test:")
    print("1.Django ORM SQL Server\n2. DjangoOrm Sqlite\n3.PySolr\nX - Close program")

    intput = input()
    match intput:
         case "1":
            sqlServerFrameworkMenu.showMenu()
         case "2":
             sqliteFrameworkMenu.showMenu()
         case "3":
             solrServerFrameworkMenu.showMenu()
         case "X":
             break