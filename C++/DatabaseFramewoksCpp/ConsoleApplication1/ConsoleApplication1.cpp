#include <iostream>
#include "SqliteManager.cpp"


int main()
{
	std::string operations = "Choose operation :\n\
\n\
	A - Run All Tests and save to file\n\
\n\
		Single table tests :\n\
	        B - Single record search\n\
		C - Set of records search\n\
		D - Set of records search with isNull command\n\
		E - Adding single records\n\
		F - Edit single record\n\
		G - Delete single record\n\
\n\
		Multiple table tests :\n\
	        H - Searching in two connected tables\n\
		I - Searching in four connected tables\n\
		J - Searching for records which does not have connection\n\
		K - Searching with subquery\n\
		L - Remove related records\n\
		X - Main menu\n\n";

	std::cout << "Welcome to Database Performance Tester - C++\n";
	SqliteManager manager;
	while (true)
	{
		std::cout << "Select framework which you want to test:\n";
		std::cout << "2. SQLiteCPP\n";
		std::cout << "X - Close program\n\n";
		std::string input;
		std::cin >> input;
		if (input == "2")
		{
			std::cout << std::format("Selected framework {}", "SQLiteCPP\n");
			std::cout << operations;
			std::cout << "Choose operation:\n\n";
			std::cin >> input;
			std::string repetitions;

			if (input != "X") {
				std::cout << "Number of repetitions:\n\n";
				std::cin >> repetitions;
			}

			if (input == "A")
			{
				std::stringstream ss;
				ss << "\"Language\": \"C++\"\n";
				ss <<"\"Results\": [\n";
				std::cout << "SingleRecordSearch\n";
				ss << manager.SingleRecordSearch(std::stoi(repetitions)) << '\n';
				std::cout << "SetOfDataSearch\n";
				ss << manager.SetOfDataSearch(std::stoi(repetitions)) << '\n';
				std::cout << "SetOfDataWithIsNullSearch\n";
				ss << manager.SetOfDataWithIsNullSearch(std::stoi(repetitions)) << '\n';
				std::cout << "AddRecords\n";
				ss << manager.AddRecords(std::stoi(repetitions)) << '\n';
				std::cout << "EditRecords\n";
				ss << manager.EditRecords(std::stoi(repetitions)) << '\n';
				std::cout << "DeleteRecords\n";
				ss << manager.DeleteRecords(std::stoi(repetitions)) << '\n';
				std::cout << "SearchTwoRelatedRecords\n";
				ss << manager.SearchTwoRelatedRecords(std::stoi(repetitions)) << '\n';
				std::cout << "SearchFourRelatedRecords\n";
				ss << manager.SearchFourRelatedRecords(std::stoi(repetitions)) << '\n';
				std::cout << "SearchRecordsWhichDoesNotHaveConnection\n";
				ss << manager.SearchRecordsWhichDoesNotHaveConnection(std::stoi(repetitions)) << '\n';
				std::cout << "SearchWithSubquery\n";
				ss << manager.SearchWithSubquery(std::stoi(repetitions)) << '\n';
				std::cout << "RemoveRelatedRecords\n";
				ss << manager.RemoveRelatedRecords(std::stoi(repetitions)) << '\n';
				ss << "]\n}";
				std::string result = ss.str();
				std::cout << result;
				std::ofstream myfile;
				myfile.open("results.txt");
				myfile << result;
				myfile.close();
			}
			else if (input == "B")
			{
				std::cout << manager.SingleRecordSearch(std::stoi(repetitions)) << '\n';
			}
			else if (input == "C")
			{
				std::cout << manager.SetOfDataSearch(std::stoi(repetitions)) << '\n';
			}
			else if (input == "D")
			{
				std::cout << manager.SetOfDataWithIsNullSearch(std::stoi(repetitions)) << '\n';
			}
			else if (input == "E")
			{
				std::cout << manager.AddRecords(std::stoi(repetitions)) << '\n';
			}
			else if (input == "F")
			{
				std::cout << manager.EditRecords(std::stoi(repetitions)) << '\n';
			}
			else if (input == "G")
			{
				std::cout << manager.DeleteRecords(std::stoi(repetitions)) << '\n';
			}
			else if (input == "H")
			{
				std::cout << manager.SearchTwoRelatedRecords(std::stoi(repetitions)) << '\n';
			}
			else if (input == "I")
			{
				std::cout << manager.SearchFourRelatedRecords(std::stoi(repetitions)) << '\n';
			}
			else if (input == "J")
			{
				std::cout << manager.SearchRecordsWhichDoesNotHaveConnection(std::stoi(repetitions)) << '\n';
			}
			else if (input == "K")
			{
				std::cout << manager.SearchWithSubquery(std::stoi(repetitions)) << '\n';
			}
			else if (input == "L")
			{
				std::cout << manager.RemoveRelatedRecords(std::stoi(repetitions)) << '\n';
			}
			else if (input == "X")
			{
				break;
			}
		}
		system("pause");
	}
	return 0;
}
