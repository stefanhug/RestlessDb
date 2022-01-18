insert into GQuery.QueryItem(Name, Label, Description, Sql)
values('Persons', 'Persons', 
    'Query some fields of  in Adventureworks person.person table', 
	'select BusinessEntityID, Title, FirstName, MiddleName, LastName, ModifiedDate 
	 from Person.Person 
	 order by BusinessEntityID')

insert into GQuery.QueryItem(Name, Label, Description, Sql)
values('PersonsByLastName', 'Persons filtered by last name', 
       'Parametrized Query of  in Adventureworks person.person table - with parameter lastName', 
	   'select BusinessEntityID, Title, FirstName, MiddleName, LastName, ModifiedDate 
	    from Person.Person
	    where LastName = @LASTNAME
	    order by FirstName')

insert into GQuery.QueryItem(Name, Label, Description, Sql)
values('JobCandidates', 'Job Candidates', 
    'Query of the Adventureworks HumanResources.vJobCandidate view', 
	'select * from HumanResources.vJobCandidate order by JobCandidateId')

insert into GQuery.QueryItem(Name, Label, Description, Sql)
values('SalesOrders', 'Sales Orders', 
    'Query of some fields from the the Adventureworks table Sales.SalesOrderHeader',
    'select SalesOrderID, OrderDate, CustomerID, SubTotal from Sales.SalesOrderHeader order by SalesOrderID')

insert into GQuery.QueryItem(Name, Label, Description, Parent, Pos, Sql)
values('SalesOrderDetails', 'Sales Order Details', 
    'Query of some fields from the the Adventureworks table Sales.SalesOrderDetail as detail query',
	'SalesOrders', 0,
    'select SalesOrderDetailID, ProductID, OrderQty, UnitPrice, LineTotal from sales.SalesOrderDetail where SalesOrderID = @SalesOrderID order by SalesOrderDetailID')

insert into GQuery.QueryItem(Name, Label, Description, Pos, Sql)
values('EMail', 'E-Mails', 
    'E-Mail adresses in example db', 0,
    'select EmailAddressid, Emailaddress from person.emailaddress order by EmailAddressid')
	
	
	 
