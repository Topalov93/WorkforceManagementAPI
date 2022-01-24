# The Workforce Management API is a platform for tracking who is in and out of the office and general management of sick leaves, requests for vacations (paid and non-paid) and the respective approvals.
# The system orchestrates the workforce availability, tracking time offs, approvals and sick leaves.
# Before starting the API, change connection string in WorkforceManagementAPI.Web.appsettings.json.


# Running the code coverage commands

### 1. Oppen PowerShell and go to "src" folder or the folder with your solution file and execute

### 2. Run the command
 ```PowerShell
 dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura  /p:CoverletOutput='./testresults/' /p:Exclude="[RateApp.Models]*%2c[*]RateApp.DAL.Entities*" 
 ```
This command will collect code coverage information and store it in  "testresults" folder of each unit test project.


### 3. Run the command
```PowerShell
reportgenerator "-reports:.\RateApp.BLL.UnitTests\testresults\coverage.cobertura.xml;.\RateApp.WEB.UnitTests\testresults\coverage.cobertura.xml" -targetdir:.\testresults   
```
This command will merge the collected information of the two test result files into a single report and output it in the "src\testresults". The test results folder should have an "index.html" that could be opened with any browser. To see the results.

### 4. Combine the two commands in a single step
```PowerShell
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura  /p:CoverletOutput='./testresults/' /p:Exclude="[RateApp.Models]*%2c[*]RateApp.DAL.Entities*"; reportgenerator "-reports:.\RateApp.BLL.UnitTests\testresults\coverage.cobertura.xml;.\RateApp.WEB.UnitTests\testresults\coverage.cobertura.xml" -targetdir:.\testresults   
```