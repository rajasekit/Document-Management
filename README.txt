This project "Document Management" was developed using the following technology stack:

1. **Front End**
   - Angular - version 16
   - Developed in Visual Studio Code

2. **Back End**
   - ASP.NET Core Web API with C# - version .NET 8
   - Developed in Visual Studio 2022

3. **Database**
   - Microsoft SQL Server - Version 19



**Prerequisites:**

Ensure the following are installed:

1. [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
2. [Visual Studio Code](https://code.visualstudio.com/)
3. [SQL Server Management Studio (SSMS)](https://www.microsoft.com/en-ca/sql-server/sql-server-downloads)
4. [Node.js](https://nodejs.org/en)
5. Angular CLI - Execute the following command in the command prompt:
   ```
   npm install -g @angular/cli@16.0.2
   ```



**Steps to Run the Application:**

1. Download the project from this GitHub link: [Document Management Project](https://github.com/rajasekit/Document-Management)
2. Open `DocumentManagement.sln` in Visual Studio 2022.
3. In Solution Explorer, set "DocumentManagement.Server" as the default project and build the solution.
4. Navigate to `appsettings.json` and update the default connection string `"DefaultConnection"` with the correct server, then save the file.
5. Go to `Tools` -> `NuGet Package Manager` -> `Package Manager Settings` -> `NuGet Package Manager` -> `Package Sources` and ensure the following package sources exist. If not, add them:
   - `https://api.nuget.org/v3/index.json`
   - `https://www.nuget.org/api/v2`
6. Navigate to `Tools` -> `NuGet Package Manager` -> `Package Manager Console` and ensure "DocumentManagement.Server" is selected in the Default project dropdown in the console window.
7. In the console window, run the following command:
   ```
   update-database
   ```
8. Open SSMS and verify that the DocumentManagement database is created along with the following tables:
   - CaseTransactions
   - Documents
9. Run the `DocumentManagement.Server` in Visual Studio and ensure the API is up and running with Swagger.
10. Open `DocumentManagement.Client` in Visual Studio Code.
11. Navigate to `src` -> `environments` -> `environment.development.ts` and ensure the API port numbers are configured correctly.
12. Open the terminal window and run the following command:
    ```
    ng build --configuration=development
    ```
13. In the terminal, run the following command:
    ```
    ng serve
    ```
14. Click on the URL displayed in the terminal.



**Application Flow:**

1. Choose one or more files and click on "Update Files". Once uploaded, the grid below will be updated with the uploaded documents.
2. Click the "Download" button provided in the grid to find the document in the download folder.
3. Use pagination to navigate and view all uploaded documents.