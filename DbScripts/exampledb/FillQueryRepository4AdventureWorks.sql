insert into GQuery.QueryRepository(NAME, LABEL, DESCRIPTION, SQL)
values('Persons', 'Persons', 
    'Query some fields of  in Adventureworks person.person table', 
	'select BusinessEntityID, Title, FirstName, MiddleName, LastName, ModifiedDate 
	 from Person.Person 
	 order by LastName')

insert into GQuery.QueryRepository(NAME, LABEL, DESCRIPTION, SQL)
values('PersonsByLastName', 'Persons filtered by last name', 
    'Parametrized Query of  in Adventureworks person.person table - with parameter lastName', 
	'select BusinessEntityID, Title, FirstName, MiddleName, LastName, ModifiedDate 
	 from Person.Person
	 where LastName = @LASTNAME
	 order by LastName')


insert into GQuery.QueryRepository(NAME, LABEL, DESCRIPTION, SQL)
values('JobCandidates', 'Job Candidates', 
    'Query of the Adventureworks HumanResources.vJobCandidate view', 
	'select * from HumanResources.vJobCandidate order by JobCandidateId')


