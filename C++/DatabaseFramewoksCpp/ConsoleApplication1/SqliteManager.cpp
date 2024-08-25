#include <chrono>
#include <iostream>
#include <sqlite3.h>
#include <format>
#include <fstream>
#include <string>
#include "TestResult.cpp"
#include <string_view>
struct Order
{
	int OrderId;
	int ClientId;
	int EmployeeId;
	std::string OrderDate;
	std::string OrderDetails;
	double TotalCost;
	int StoreId;
};

struct Employee
{
	int EmployeeId;
	std::string FirstName;
	std::string LastName;
	std::string PhoneNumber;
	std::string Adress;
	int PositionId;
};

struct Store
{
	int StoreId;
	std::string Address;
	std::string Country;
};

struct Position
{
	int PositionId;
	std::string PositionName;
	std::string CarParkingPlace;
	double PositionBonus;
};

struct Client
{
	int ClientId;
	std::string FirstName;
	std::string LastName;
	std::string PhoneNumber;
	std::string Address;
	std::string Country;
};
class SqliteManager
{
private:
	sqlite3* DB;
	std::string _dbLocation = R"(D:\Code\UJ\Master_Thesis\Master_Thesis\C++\DatabaseFramewoksCpp\FrameworkPerformanceSqlLite.db)";

	std::string MergeArrayAsString(std::vector<int>* values)
	{
		std::stringstream ss;
		ss << "(";
		for (int i = 0; i < values->size(); i++)
			if (i - 1 == values->size())
				ss << std::to_string(values->at(i)) + ")";
			else
				ss << std::to_string(values->at(i)) + ", ";
	}

	static int EmptyCallback(void* param, int args, char** argv, char** azColName)
	{
		for (int i = 0; i < args; i++)
			auto a = argv[i];
		return 0;
	}
	static int OrderCallbackNoneResult(void* param, int args, char** argv, char** azColName)
	{
		/*for(int i = 0; i < args; i ++)
		{
			std::cout << azColName[i] << " "<< argv[i] <<"\n";
		}*/
		Order o;
		o.OrderId = std::stoi(argv[0]);
		o.ClientId = std::stoi(argv[1]);
		o.EmployeeId = std::stoi(argv[2]);
		o.OrderDate = argv[3];
		o.OrderDetails = argv[4];
		o.TotalCost = atof(argv[5]);
		return 0;
	}

	static int EmployeeCallback(void* list, int args, char** argv, char** azColName)
	{
		std::vector<Employee>* p = reinterpret_cast<std::vector<Employee>*>(list);
		Employee e;
		e.EmployeeId = std::stoi(argv[0]);
		e.FirstName = argv[1];
		e.LastName = argv[2];
		e.PhoneNumber = argv[3];
		e.Adress = argv[4];
		e.PositionId = std::stoi(argv[5]);
		p->push_back(e);
		return 0;
	}

	static int ClientCallback(void* list, int args, char** argv, char** azColName)
	{
		std::vector<Client>* cl = reinterpret_cast<std::vector<Client>*>(list);
		Client c;
		c.ClientId = std::stoi(argv[0]);
		c.FirstName = argv[1];
		c.LastName = argv[2];
		c.PhoneNumber = argv[3];
		c.Address = argv[4];
		c.Country = argv[5];
		cl->push_back(c);
		return 0;
	}

	static int OrderCallback(void* list, int args, char** argv, char** azColName)
	{
		std::vector<Order>* orders = reinterpret_cast<std::vector<Order>*>(list);
		Order o;
		o.OrderId = std::stoi(argv[0]);
		o.ClientId = std::stoi(argv[1]);
		o.EmployeeId = std::stoi(argv[2]);
		o.OrderDate = argv[3];
		o.TotalCost = atof(argv[4]);
		o.StoreId = atof(argv[5]);
		o.OrderDetails = argv[6];
		orders->push_back(o);
		return 0;
	}

	static int CollectIdsCallback(void* list, int args, char** argv, char** azColName)
	{
		std::vector<int>* p = reinterpret_cast<std::vector<int>*>(list);
		p->push_back(std::stoi(argv[0]));
		return 0;
	}

	static int CollectOneColumnCallback(void* list, int args, char** argv, char** azColName)
	{
		std::vector<std::string>* p = reinterpret_cast<std::vector<std::string>*>(list);
		p->push_back(argv[0]);
		return 0;
	}

	static int PositionsCallback(void* list, int args, char** argv, char** azColName)
	{
		std::vector<Position>* pa = reinterpret_cast<std::vector<Position>*>(list);
		Position p;
		p.PositionId = std::stoi(argv[0]);
		p.PositionName = argv[1];
		p.CarParkingPlace = argv[2];
		p.PositionBonus = atof(argv[3]);
		return 0;
	}

	static int StoresCallback(void* list, int args, char** argv, char** azColName)
	{
		Store s;
		s.StoreId = std::stoi(argv[0]);
		s.Address = argv[1];
		s.Country = argv[2];
		std::vector<Store>* p = reinterpret_cast<std::vector<Store>*>(list);
		p->emplace_back(s);
		return 0;
	}

	int ExecuteSqlWithEmptyCallback(std::string sql)
	{
		auto start_time = std::chrono::high_resolution_clock::now();
		sqlite3_open(_dbLocation.c_str(), &DB);
		sqlite3_exec(DB, sql.c_str(), EmptyCallback, nullptr, nullptr);
		auto end_time = std::chrono::high_resolution_clock::now();
		auto time = end_time - start_time;
		return time / std::chrono::milliseconds(1);
	}
	int ExecuteSqlForUpdate(std::string sql)
	{
		auto start_time = std::chrono::high_resolution_clock::now();
		sqlite3_open(_dbLocation.c_str(), &DB);
		auto result = sqlite3_exec(DB, sql.c_str(), nullptr, 0, nullptr);
		auto end_time = std::chrono::high_resolution_clock::now();
		auto time = end_time - start_time;
		return time / std::chrono::milliseconds(1);
	}

	static std::vector<std::string> ReadDataFromTxtFile(std::string fileName)
	{
		std::ifstream fin(fileName);

		std::vector<std::string> data;

		std::string element;
		while (std::getline(fin, element))
		{
			data.push_back(element);
		}
		return data;
	}

	void AddRecordsSilently(int samplesQuantity)
	{
		auto records = ReadDataFromTxtFile("real_product_names.txt");
		srand((unsigned)time(nullptr));
		for (int i = 0; i < samplesQuantity; i++)
		{
			int productName = rand() % records.size();
			std::string sql1 = "INSERT INTO Products ([ProductName],[ProductDescription],[Price],[Supplier]) values ('";
			std::string sql = sql1 + records[productName] + R"(','AddedByTest',223.55,'None'))";
			ExecuteSqlForUpdate(sql);
		}
	}

	std::vector<int>* GetAddedIds()
	{
		std::vector<int>* ids = new std::vector<int>();
		std::string sql = "SELECT ProductId From Products Where ProductDescription = 'AddedByTest'";
		sqlite3_open(_dbLocation.c_str(), &DB);
		sqlite3_exec(DB, sql.c_str(), CollectIdsCallback, ids, nullptr);
		return ids;
	}

	std::vector<int>* GetUsedStoresIds()
	{
		std::vector<int>* ids = new std::vector<int>();
		std::string sql = "Select Distinct StoreId From Orders";
		sqlite3_open(_dbLocation.c_str(), &DB);
		sqlite3_exec(DB, sql.c_str(), CollectIdsCallback, ids, nullptr);
		return ids;
	}

	std::vector<int>* GetUsedPostionsIds()
	{
		std::vector<int>* ids = new std::vector<int>();
		std::string sql = "Select Distinct PositionId From Employees";
		sqlite3_open(_dbLocation.c_str(), &DB);
		sqlite3_exec(DB, sql.c_str(), CollectIdsCallback, ids, nullptr);
		return ids;
	}

	std::vector<std::string>* GetPositionNames()
	{
		std::vector<std::string>* ids = new std::vector<std::string>();
		std::string sql = "SELECT PositionName From Positions";
		sqlite3_open(_dbLocation.c_str(), &DB);
		sqlite3_exec(DB, sql.c_str(), CollectOneColumnCallback, ids, nullptr);
		return ids;
	}

	std::vector<Store>* GetStores()
	{
		std::vector<Store>* stores = new std::vector<Store>();
		std::string sql = "SELECT StoreId,Address,Country From Stores";
		sqlite3_open(_dbLocation.c_str(), &DB);
		sqlite3_exec(DB, sql.c_str(), StoresCallback, stores, nullptr);
		return stores;
	}
public:

	std::string SingleRecordSearch(int samplesQuantity)
	{
		TestResult* result = new TestResult(samplesQuantity, "SingleRecordSearch");
		sqlite3* DB;
		try
		{
			for (int i = 0; i < samplesQuantity; i++)
			{
				srand((unsigned)time(nullptr));
				int orderId = rand() % 40000;
				auto start_time = std::chrono::high_resolution_clock::now();
				std::string sqlpart1 = "SELECT [OrderId], [ClientId], [EmployeeId], [OrderDate], [OrderDetails], [TotalCost], [StoreId] FROM[Orders] Where OrderID = ";
				std::string sqlpart2 = " Limit 1";
				std::string sql = sqlpart1 + std::to_string(orderId) + sqlpart2;
				sqlite3_open(_dbLocation.c_str(), &DB);
				sqlite3_exec(DB, sql.c_str(), OrderCallbackNoneResult, nullptr, nullptr);
				auto end_time = std::chrono::high_resolution_clock::now();
				auto time = end_time - start_time;
				result->AddMeasure(time / std::chrono::milliseconds(1));
			}
		}
		catch (std::exception ex)
		{
			std::cerr << ex.what();
		}
		return result->GetTestResultAsString();
	}
	std::string SetOfDataSearch(int samplesQuantity)
	{
		TestResult* result = new TestResult(samplesQuantity, "SetOfDataSearch");
		sqlite3* DB;
		try
		{
			for (int i = 0; i < samplesQuantity; i++)
			{
				srand((unsigned)time(nullptr));
				int postionId = rand() % 105;
				auto start_time = std::chrono::high_resolution_clock::now();
				std::string sqlpart1 = "SELECT [EmployeeId],[FirstName],[LastName],[PhoneNumber],[Adress],[PositionId] FROM [Employees] WHERE PositionId = ";
				std::string sql = sqlpart1 + std::to_string(postionId);
				sqlite3_open(_dbLocation.c_str(), &DB);
				std::vector<Employee>* emp = new std::vector<Employee>();
				sqlite3_exec(DB, sql.c_str(), EmployeeCallback, emp, nullptr);
				auto end_time = std::chrono::high_resolution_clock::now();
				auto time = end_time - start_time;
				result->AddMeasure(time / std::chrono::milliseconds(1));
				delete emp;
			}
		}
		catch (std::exception ex)
		{
			std::cerr << ex.what();
		}
		return result->GetTestResultAsString();
	}
	std::string SetOfDataWithIsNullSearch(int samplesQuantity)
	{
		TestResult* result = new TestResult(samplesQuantity, "SetOfDataWithIsNullSearch");
		sqlite3* DB;
		try
		{
			for (int i = 0; i < samplesQuantity; i++)
			{
				srand((unsigned)time(nullptr));
				int table = rand() % 6;
				std::string sql;
				switch (static_cast<TableWithNull>(table))
				{
				case ClientPhone:
					sql = "SELECT [ClientId],[FirstName],[LastName],[PhoneNumber],[Address],[Country] FROM [Clients] where PhoneNumber is NULL";
					result->AddMeasure(ExecuteSqlWithEmptyCallback(sql));
					break;
				case ClientCountry:
					sql = "SELECT [ClientId],[FirstName],[LastName],[PhoneNumber],[Address],[Country] FROM [Clients] where Country is NULL";
					result->AddMeasure(ExecuteSqlWithEmptyCallback(sql));
					break;
				case EmployeesPhone:
					sql = "SELECT [EmployeeId],[FirstName],[LastName],[PhoneNumber],[Address],[PositionId] FROM [Employees] Where PhoneNumber is NULL";
					result->AddMeasure(ExecuteSqlWithEmptyCallback(sql));
					break;
				case EmployeesPositionId:
					sql = "SELECT [EmployeeId],[FirstName],[LastName],[PhoneNumber],[Address],[PositionId] FROM [Employees] Where PositionId is NULL";
					result->AddMeasure(ExecuteSqlWithEmptyCallback(sql));
					break;
				case Orders:
					sql = "SELECT [OrderId], [ClientId], [EmployeeId], [OrderDate], [OrderDetails], [TotalCost], [StoreId] FROM[Orders] Where OrderDate is NULL";
					result->AddMeasure(ExecuteSqlWithEmptyCallback(sql));
					break;
				case Products:
					sql = "SELECT [ProductId],[ProductName],[ProductDescription],[Price],[Supplier] FROM [Products] where ProductDescription is null";
					result->AddMeasure(ExecuteSqlWithEmptyCallback(sql));
					break;
				}
			}
		}
		catch (std::exception ex)
		{
			std::cerr << ex.what();
		}
		return result->GetTestResultAsString();
	}
	std::string AddRecords(int samplesQuantity)
	{
		TestResult* result = new TestResult(samplesQuantity, "AddRecords");
		auto records = ReadDataFromTxtFile("real_product_names.txt");
		srand((unsigned)time(nullptr));
		for (int i = 0; i < samplesQuantity; i++)
		{
			int productName = rand() % records.size();
			std::string sql1 = "INSERT INTO Products ([ProductName],[ProductDescription],[Price],[Supplier]) values ('";
			std::string sql = sql1 + records[productName] + R"(','AddedByTest',223.55,'None'))";
			result->AddMeasure(ExecuteSqlForUpdate(sql));

		}
		std::string sql = "Delete From Products Where [ProductDescription] = 'AddedByTest'";
		ExecuteSqlForUpdate(sql);
		return result->GetTestResultAsString();
	}

	std::string EditRecords(int samplesQuantity)
	{
		TestResult* result = new TestResult(samplesQuantity, "EditRecords");
		AddRecordsSilently(samplesQuantity);
		std::vector<int>* AddedIds = GetAddedIds();
		auto records = ReadDataFromTxtFile("real_product_names.txt");
		srand((unsigned)time(nullptr));
		for (int i = 0; i < samplesQuantity; i++)
		{
			int idToEdit = AddedIds->at(rand() % AddedIds->size());
			std::string name = records.at(rand() % records.size());
			std::string sql = std::format("Update Products Set ProductName = '{}' Where ProductId = {}", name, idToEdit);
			result->AddMeasure(ExecuteSqlForUpdate(sql));
		}
		std::string sql = "Delete From Products Where [ProductDescription] = 'AddedByTest'";
		ExecuteSqlForUpdate(sql);
		delete AddedIds;
		return result->GetTestResultAsString();
	}

	std::string DeleteRecords(int samplesQuantity)
	{
		TestResult* result = new TestResult(samplesQuantity, "DeleteRecords");
		AddRecordsSilently(samplesQuantity);
		std::vector<int>* AddedIds = GetAddedIds();
		for (int i = 0; i < AddedIds->size(); i++)
		{
			std::string sql = std::format("Delete FROM Products Where ProductId = {}", AddedIds->at(i));
			result->AddMeasure(ExecuteSqlForUpdate(sql));
		}
		delete AddedIds;
		return result->GetTestResultAsString();
	}

	std::string SearchTwoRelatedRecords(int samplesQuantity)
	{
		std::vector<std::string>* positionNames = GetPositionNames();
		TestResult* result = new TestResult(samplesQuantity, "SearchTwoRelatedRecords");
		sqlite3* DB;
		try
		{
			for (int i = 0; i < samplesQuantity; i++)
			{
				srand((unsigned)time(nullptr));
				std::string positionToFind = positionNames->at(rand() % positionNames->size());
				auto start_time = std::chrono::high_resolution_clock::now();
				std::vector<Employee>* emp = new std::vector<Employee>();
				std::string sql = std::format(
					R"(SELECT "Employees"."EmployeeId", "Employees"."FirstName", "Employees"."LastName", "Employees"."PhoneNumber", "Employees"."Adress", "Employees"."PositionId" FROM "Employees" INNER JOIN "Positions" ON ("Employees"."PositionId" = "Positions"."PositionId") WHERE "Positions"."PositionName" = '{}')", positionToFind);
				sqlite3_open(_dbLocation.c_str(), &DB);
				sqlite3_exec(DB, sql.c_str(), EmployeeCallback, emp, nullptr);
				auto end_time = std::chrono::high_resolution_clock::now();
				auto time = end_time - start_time;
				result->AddMeasure(time / std::chrono::milliseconds(1));
				delete emp;
			}
		}
		catch (std::exception ex)
		{
			std::cerr << ex.what();
		}
		delete positionNames;
		return result->GetTestResultAsString();
	}
	std::string SearchFourRelatedRecords(int samplesQuantity)
	{
		std::vector<std::string>* positionNames = GetPositionNames();
		std::vector<Store>* stores = GetStores();

		TestResult* result = new TestResult(samplesQuantity, "SearchTwoRelatedRecords");
		sqlite3* DB;
		try
		{
			for (int i = 0; i < samplesQuantity; i++)
			{
				srand((unsigned)time(nullptr));
				std::string positionToFind = positionNames->at(rand() % positionNames->size());
				std::string storeCountryToFind = stores->at(rand() % positionNames->size()).Country;
				auto start_time = std::chrono::high_resolution_clock::now();
				std::vector<Order>* ord = new std::vector<Order>();
				std::string sql = std::format(
					R"(SELECT "Orders"."OrderId", "Orders"."ClientId", "Orders"."EmployeeId", "Orders"."OrderDate", "Orders"."TotalCost", "Orders"."StoreId", "Orders"."OrderDetails" FROM "Orders" INNER JOIN "Employees" ON ("Orders"."EmployeeId" = "Employees"."EmployeeId") INNER JOIN "Positions" ON ("Employees"."PositionId" = "Positions"."PositionId") INNER JOIN "Stores" ON ("Orders"."StoreId" = "Stores"."StoreId") WHERE ("Positions"."PositionName" = '{}' AND "Stores"."Country" = '{}')", positionToFind, storeCountryToFind);
				sqlite3_open(_dbLocation.c_str(), &DB);
				sqlite3_exec(DB, sql.c_str(), OrderCallback, ord, nullptr);
				auto end_time = std::chrono::high_resolution_clock::now();
				auto time = end_time - start_time;
				result->AddMeasure(time / std::chrono::milliseconds(1));
				delete ord;
			}
		}
		catch (std::exception ex)
		{
			std::cerr << ex.what();
		}
		delete positionNames;
		delete stores;
		return result->GetTestResultAsString();
	}

	std::string SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity)
	{
		TestResult* result = new TestResult(samplesQuantity, "SearchRecordsWhichDoesNotHaveConnection");
		sqlite3* DB;
		try
		{
			for (int i = 0; i < samplesQuantity; i++)
			{
				if (i % 2 == 0)
				{
					auto start_time = std::chrono::high_resolution_clock::now();
					std::string sql = "Select [PositionId], [PositionName], [CarParkingPlace], [PositionBonus] from Positions where PositionId not in (Select DISTINCT [PositionId] from Employees)";
					sqlite3_open(_dbLocation.c_str(), &DB);
					std::vector<Position>* pos = new std::vector<Position>();
					sqlite3_exec(DB, sql.c_str(), PositionsCallback, pos, nullptr);
					auto end_time = std::chrono::high_resolution_clock::now();
					auto time = end_time - start_time;
					result->AddMeasure(time / std::chrono::milliseconds(1));
					delete pos;
				}
				else
				{
					auto start_time = std::chrono::high_resolution_clock::now();
					std::string sql = "Select [StoreId], [Address] ,[Country] from Stores where StoreId not in (Select DISTINCT [StoreId] from Orders)";
					sqlite3_open(_dbLocation.c_str(), &DB);
					std::vector<Store>* store = new std::vector<Store>();
					sqlite3_exec(DB, sql.c_str(), StoresCallback, store, nullptr);
					auto end_time = std::chrono::high_resolution_clock::now();
					auto time = end_time - start_time;
					result->AddMeasure(time / std::chrono::milliseconds(1));
					delete store;
				}
			}
		}
		catch (std::exception ex)
		{
			std::cerr << ex.what();
		}
		return result->GetTestResultAsString();
	}

	std::string SearchWithSubquery(int samplesQuantity)
	{
		TestResult* result = new TestResult(samplesQuantity, "SearchWithSubquery");
		sqlite3* DB;
		std::string sql = std::format("Select Distinct Country From Stores");
		sqlite3_open(_dbLocation.c_str(), &DB);
		std::vector<std::string>* countries = new std::vector<std::string>();
		sqlite3_exec(DB, sql.c_str(), CollectOneColumnCallback, countries, nullptr);
		try
		{
			for (int i = 0; i < samplesQuantity; i++)
			{
				srand((unsigned)time(nullptr));
				std::string countryToFind = countries->at(rand() % countries->size());
				auto start_time = std::chrono::high_resolution_clock::now();
				std::string sql = "Insert into Stores ([Address] ,[Country]) Values ('Norymberska 1 30-412 Krakow','Poland')";
				std::vector<Order>* orders = new std::vector<Order>();
				sqlite3_exec(DB, sql.c_str(), OrderCallback, orders, nullptr);
				auto end_time = std::chrono::high_resolution_clock::now();
				auto time = end_time - start_time;
				result->AddMeasure(time / std::chrono::milliseconds(1));
				delete orders;
			}
		}
		catch (std::exception ex)
		{
			std::cerr << ex.what();
		}
		delete countries;
		return result->GetTestResultAsString();
	}

	std::string RemoveRelatedRecords(int samplesQuantity)
	{
		srand((unsigned)time(nullptr));
		TestResult* result = new TestResult(samplesQuantity, "RemoveRelatedRecords");
		sqlite3* DB;
		auto clients = new std::vector<Client>();
		auto employee = new std::vector<Employee>();
		std::string sql = "SELECT[EmployeeId],[FirstName],[LastName] ,[PhoneNumber] ,[Adress] ,[PositionId] FROM [Employees]";
		std::string sql2 = "SELECT[ClientId],[FirstName] ,[LastName]  ,[PhoneNumber] ,[Address]  ,[Country] FROM [Clients]";
		sqlite3_open(_dbLocation.c_str(), &DB);
		sqlite3_exec(DB, sql2.c_str(), ClientCallback, clients, nullptr);
		sqlite3_exec(DB, sql.c_str(), EmployeeCallback, employee, nullptr);
		for (int i = 0; i < samplesQuantity; i++)
		{
			auto client = clients->at(rand() % clients->size());
			auto emp = employee->at(rand() % employee->size());
			std::string sql3 = "Insert into Stores ([Address] ,[Country]) Values ('Norymberska 1 30-412 Krakow','Poland')";
			sqlite3_exec(DB, sql3.c_str(), nullptr, 0, nullptr);
			sql3 = "Select StoreId From Stores Where Address ='Norymberska 1 30-412 Krakow' Limit 1";
			std::vector<int>* ids = new std::vector<int>();
			sqlite3_exec(DB, sql3.c_str(), CollectIdsCallback, ids, nullptr);
			int idStoreToOrder = ids->front();
			sql3 = std::format("Insert into Orders ([ClientId], [EmployeeId], [OrderDate], [OrderDetails], [TotalCost], [StoreId]) values ({},{},'2018-10-24 07:58:21.340','sample bad detalis data',331.33,{})", client.ClientId, emp.EmployeeId, idStoreToOrder);
			sqlite3_exec(DB, sql3.c_str(), nullptr, 0, nullptr);
			sql3 = "Select OrderId From Orders Where OrderDetails = 'sample bad detalis data'";
			ids->clear();
			sqlite3_exec(DB, sql3.c_str(), CollectIdsCallback, ids, nullptr);
			int id = ids->front();

			auto start_time = std::chrono::high_resolution_clock::now();
			sql3 = std::format("Delete From Orders Where OrderId = {} ", id);
			sqlite3_exec(DB, sql3.c_str(), nullptr, 0, nullptr);
			auto end_time = std::chrono::high_resolution_clock::now();
			auto time = end_time - start_time;
			result->AddMeasure(time / std::chrono::milliseconds(1));
		}
		return result->GetTestResultAsString();
	}

	enum TableWithNull
	{
		ClientPhone = 1,
		ClientCountry = 2,
		EmployeesPhone = 3,
		EmployeesPositionId = 4,
		Orders = 5,
		Products = 6,
	};

};



