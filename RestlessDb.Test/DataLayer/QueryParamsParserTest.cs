﻿using RestlessDb.DataLayer;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GenericDbRestApi.Test.DataLayer
{
    [Trait("Category", "UnitTest")]
    public class QueryParamsParserTest
    {
        [Fact]
        public void WhenSqlWithoutParametersGivenThenNoParamsFound()
        {
            var sql = "select * from HumanResources.vJobCandidate order by JobCandidateId";
            var sqlParameters = QueryParamsParser.GetQueryParams(sql);
            Assert.True(sqlParameters.Count == 0);
        }

        [Fact]
        public void WhenSqlWithOneParametersGivenThenParamFound()
        {
            var sql = @"select BusinessEntityID, Title, FirstName, MiddleName, LastName, ModifiedDate 
	                     from Person.Person
	                     where LastName = @LASTNAME
	                     order by LastName";
            var sqlParameters = QueryParamsParser.GetQueryParams(sql);
            Assert.True(sqlParameters.Count == 1);
            Assert.Collection(sqlParameters, item => Assert.Equal("LASTNAME", item));
        }

        [Fact]
        public void WhenSqlWithTwoParametersGivenThenParamsFound()
        {
            var sql = @"select SalesOrderID, OrderDate, CustomerID, SubTotal 
                        from Sales.SalesOrderHeader 
                        where OrderDate between @STARTDATE and @ENDDATE
                        order by SalesOrderID
                        ";
            var sqlParameters = QueryParamsParser.GetQueryParams(sql);
            Assert.Equal(2, sqlParameters.Count);
            Assert.Collection(sqlParameters, 
                              item => Assert.Equal("STARTDATE", item),
                              item => Assert.Equal("ENDDATE", item));
        }

        [Fact]
        public void WhenSqlWithOneParametersInTwoLocationsGivenThenOneParamFound()
        {
            var sql = @"select BusinessEntityID, Title, FirstName, MiddleName, LastName, ModifiedDate 
	                     from Person.Person
	                     where LastName = @LASTNAME or FirstNAme = @LASTNAME
	                     order by LastName";
            var sqlParameters = QueryParamsParser.GetQueryParams(sql);
            Assert.True(sqlParameters.Count == 1);
            Assert.Collection(sqlParameters, item => Assert.Equal("LASTNAME", item));
        }

        [Theory]
        [InlineData("select * from HumanResources.vJobCandidate order by JobCandidateId", true)]
        [InlineData("select * from HumanResources.vJobCandidate order     by JobCandidateId", true)]
        [InlineData("select * from HumanResources.vJobCandidate ORDER BY JobCandidateId", true)]
        [InlineData("select * from HumanResources.vJobCandidate Order By JobCandidateId", true)]
        [InlineData("select * from HumanResources.vJobCandidate ORDERBY JobCandidateId", false)]
        [InlineData("select * from HumanResources.vJobCandidate where JobCandidateId = 5", false)]
        public void WhenSqlGivenThenContainsOrderByCorrectlyParsed(string sql, bool expectedResult)
        {
            var containsOrderBy = QueryParamsParser.ContainsOrderBy(sql);
            Assert.Equal(expectedResult, containsOrderBy);
        }
    }
}
