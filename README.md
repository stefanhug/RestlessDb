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
 
``` powershell
git clone https://github.com/stefanhug/GenericDbRestApi.git
```
- configure the DB connection
Edit the connection string in *GenericDbRestApi/GenericDbRestApi/appsettings.Development.json* to match your database, user and password:
``` js
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
``` pwsh
cd ./GenericDbRestApi/
dotnet restore
dotnet build
```
- run the SQL script *DbScripts/createqueryrepository.sql* with *sqlcmd* or *SQL server management studio* in your DB to create the query repository table *GQuery.QueryItem* 
- add exanple queries by running  *DbScripts/exampledb/FillQueryRepository4AdventureWorks.sql*. If another DB schema than adventureworks is used the examples need to be adapted.
- start the application:
``` pwsh
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

The SQL statement to execute is specified in the *QueryItem.Sql* column, *QueryItem.Name* is used to specifiy the endpoint url of the query:

```
https://{{SERVER}:{PORT}/dbapi/{QueryItem.Name}
```

**Note: the SQL statement needs to contain an order by clause to make range selection working. But this is anyway reequired to ensure that the results of a DB query are predictable** 


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
- TBD

## Integrating *GenericDbRestApi.Lib.dll* in an existing application
The proided example application GenericDbRestApi is a minimum example appserver without authentication and other features.
To integrate GenericDbRestApi in an existing project the following step is necessary (nuget package is not yet available).
- reference *GenericDbRestApi.Lib.dll* in your project
- Add the following code to you *Startup.cs*

``` csharp
...
using GenericDbRestApi.Lib.Services;  //ADD
...
namespace GenericDbRestApi
{
    public class Startup
    {
	...
	
	public void ConfigureServices(IServiceCollection services)
        {
            ...
	    // needed for string serialiation of enum values
            services.AddControllers().AddJsonOptions
            (opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // ADD to provide correct JSON serialization pf enum values as string, not as ordinal
            });

            // DbRestApi registration
            var connectionString = Configuration.GetSection("AppSettings").GetValue<string>("ConnectionString"); //ADD
            services.AddDbRestApi(connectionString);    //ADD
	    ...
        }
	
	...
    }
}
```

## REST query parameters
- TBD

## Hierarchical queries



