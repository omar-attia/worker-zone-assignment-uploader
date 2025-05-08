# Worker Zone Assignment Uploader

This solution provides a backend-only implementation with RESTful API endpoints for uploading and analyzing **bulk Worker Zone assignments** via CSV file input.

It follows an **N-tier architecture** to ensure clean separation of concerns and scalability. The main functionality revolves around parsing CSV files and storing assignment data reliably in the database.

## 📌 Project Overview

- ✅ **Backend only** – no frontend included  
- ✅ **N-tier architecture** for maintainability  
- ✅ Accepts **CSV files** to upload multiple assignments at once  
- ✅ API endpoints built with **ASP.NET Core**  
- ✅ Postman collection included for easy testing  
- ✅ Sample CSV files included under `TestCSVFiles` folder  

## 📦 Technologies Used

- **.NET 8**  
- **ASP.NET Core Web API**  
- **Entity Framework Core**  
- **SQL Server**  
- **Postman Collection** for API testing  

## 📁 Folder Structure

WorkerZoneAssignmentUploader/
├── Service/ # Application layer (services)
├── Models/ # DTOs and Result Models
├── DAL/ # Database context, Entities, and repositories
├── API/ # API controllers
├── TestCSVFiles/ # Sample CSV files for testing
├── WorkerZoneAssignmentUploader.sln
└── README.md

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK  
- PostgreSQL  

### Run the API

1. Clone the repository:
   ```bash
   git clone https://github.com/omar-attia/worker-zone-assignment-uploader.git
   cd worker-zone-assignment-uploader

Restore and run:

dotnet restore
dotnet ef database update
dotnet run
Make sure your appsettings.json contains the correct SQL Server connection string.

🔍 API Documentation
The API supports CSV file uploads via a dedicated endpoint. You can test the API using the Postman collection linked below.

🔗 Postman Collection:
https://api.postman.com/collections/34292382-38fd6066-a9a1-4972-a86c-9b4caea30805?access_key=PMAT-01JTR4MT7XNBAQQZDFNZ57SRWX

📂 Test CSV Files
Sample input files are available under the TestCSVFiles directory in the root of the solution. These files demonstrate the expected format and structure for the CSV imports.
