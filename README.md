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
- build the repository:
```
cd .\GenericDbRestApi\
dotnet build
```
- run the SQL script *DbScripts/createqueryrepository.sql* with *sqlcmd* or *SQL server management studio* in your DB to create the query repository table *GQuery.QueryItem* 
- add exanple tables with *notyetthere.sql*
- add example query repository entries using *fillqueryrepository.sql*
- compile and install 

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



