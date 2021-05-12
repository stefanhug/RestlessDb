# GenericDbRestApi
Create REST endpoints based on database queries within minutes with an ASP.net core based backend. Creating a new REST endpoint just requires one insert in a *QueryItem* table.
Multiple output formats like json, csv, excel and xml are provided.
Currently only SQL server as backend is supported, extension for other databases is planned

## Prerequisites
- .net Core Framework 3.1 or higher
- Sql Server DB instance
- Adventureworks DB for the examples

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
dotnet restore
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

Let's compare this to the corresponding QueryItem inserted in *FillQueryRepository4AdventureWorks.sql*:
``` sql
insert into GQuery.QueryItem(Name, Label, Description, Sql)
values('Persons', 'Persons', 
    'Query some fields of  in Adventureworks person.person table', 
	'select BusinessEntityID, Title, FirstName, MiddleName, LastName, ModifiedDate 
	 from Person.Person 
	 order by BusinessEntityID')
```


TODO:specify corresponing query item and URL pattern


## Specify ranges for output and max rows to return
The following parameters can be used to specify maximum number of rows to return and the offsett from the begin of the query.
Per default only the first 8000 rows delivered by a query will be returned.

- maxrows (default: 8000)
- offset (default: 0)

Example:
```
https://localhost:44352/dbapi/persons?maxrows=10
https://localhost:44352/dbapi/persons?maxrows=10&offset=8000
```

## supported output formats

### JSON
### Excel

- CSV
- XML

## Rest API call convention
- TBD

## REST query parameters
- TBD

## Hierarchical queries



