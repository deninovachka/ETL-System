# ETL-System
University project

ETLSystem
An ETL (Extract, Transform, Load) console application built with C# and Entity Framework Core.
It reads transactional data from CSV files, applies transformations, cleans invalid data, exports the results to a new CSV, and upserts the processed data into a SQL database.

Features
-Import transactions from CSV (transactions.csv)
-Transform/Clean invalid or inconsistent records
-Upsert data into SQL Server via EF Core (insert new or update existing by TransactionId)
-Export clean dataset to clean_transactions.csv
-Reports through menu options:Total number of transactions, Sum of transaction amounts, Largest transaction details (ID, Customer, Amount, Date)

Installation
Requirements
-.NET 9 SDK (or project’s target version)
-SQL Server (LocalDB, Express, or Developer)
-EF Core
-CSV file named transactions.csv (see Usage)

Setup
Make sure to configure your connection string in ApplicationDBContext.cs.
git clone https://github.com/yourusername/ETLSystem.git
cd ETLSystem
dotnet restore
dotnet build
dotnet run --project ETLSystem

Usage
1.Place your transactions.csv in the working directory.
2.Run the app:
dotnet run --project ETLSystem
3.Choose menu option 1 to import and clean data.
4.The cleaned data will be exported to clean_transactions.csv.
5.Use options 2–4 for basic transaction statistics.

CSV format
TransactionId,CustomerName,Amount,TransactionDate
1,John Doe,250.00,2024-05-14
2,Jane Smith,300.50,2024-06-01
...
1,John Doe,250.00,2024-05-14
2,Jane Smith,300.50,2024-06-01
