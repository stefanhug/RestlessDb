# GenericDbRestApi
Create REST endpoints based on database queries within minutes with an ASP.net core based backend. Creating a new REST endpoint just requires one insert in a *QueryItem* table.
Multiple output formats like json, csv, excel and xml are provided.
Currently only SQL server as backend is supported, extension for other databases is planned

## Example Usage
The given exammples are based on the example database *Adventureworks* provided by Microsoft(R) which can be downloaded from here:
 [Adventureworks Example Db download](https://docs.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver15&tabs=ssms).
 All examples can be easily transformed to a different database schema.
 
- clone this repository:
 
```
git clone https://github.com/stefanhug/GenericDbRestApi.git
```
- configure the DB connection
Edit the connection string in *GenericDbRestApi/GenericDbRestApi/appsettings.Development.json* to match your database, user and password:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AppSettings": {
    "ConnectionString": "Data Source={MYDATABASE};Initial Catalog=Adventureworks2019;User Id={MYUSER};Password={MYPASSWORD};MultipleActiveResultSets=True"
  }
}
```

- build the repository:
```
cd .\GenericDbRestApi\
dotnet build
```
- run the SQL script *DbScripts/createqueryrepository.sql* with *sqlcmd* or *SQL server management studio* in your DB to create the query repository table *GQuery.QueryItem* 
- add exanple queries by running  *DbScripts/exampledb/FillQueryRepository4AdventureWorks.sql*. If another DB schema than adventureworks is used the examples need to be adapted.
- start the application:
```
cd ./bin/netcoreapp3.1/
GenericDbRestApi.exe --environment=Development
```
- open a browser a enter the URL *https://localhost:5001/dbapi/persons*
  You should see the following output of the *Adventureworks Person* table:
  ![browser json output person table](./doc/img/jsoninbrowser.PNG "Logo Title Text 1")

## Requirements
- .net Core 3.1

## supported output formats
- JSON
- Excel
- CSV
- XML

## Rest API call convention
- TBD

## REST query parameters
- TBD



