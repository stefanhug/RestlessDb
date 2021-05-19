using RestlessDb.DataLayer;
using RestlessDb.Types;
using Moq;
using System.Collections.Generic;

namespace GenericDbRestApi.Test.DataLayer
{
    public class GenericSqlHelperMoq
    {
        public IGenericSqlHelper SetupMoq4GenericSqlHelper()
        {
            var genericSqlHelperMoq = new Mock<IGenericSqlHelper>();

            genericSqlHelperMoq.Setup(
                p => p.QueryAsDictList(QueryItemProvider.QRY_QRY_REPOSITORY,
                                       0, QueryItemProvider.MAXCHILDQUERIES,
                                       new Dictionary<string, object> { { "NAME", "salesorders" } }))
                     .Returns((qryQueryItemReturnValue, false));

            genericSqlHelperMoq.Setup(
                p => p.QueryResultColumns(qryQueryItemReturnValue[0]["Sql"].ToString()))
                     .Returns(SalesOrderColumns);

            genericSqlHelperMoq.Setup(
                p => p.QueryResultColumns(qryQueryItemReturnValue[1]["Sql"].ToString()))
                     .Returns(SalesOrderDetailColumns);

            return genericSqlHelperMoq.Object;
        }

        public List<Dictionary<string, object>> qryQueryItemReturnValue = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    { "Name", "SalesOrders" },
                    { "Label", "Sales Orders" },
                    { "Description", "Query of some fields from the the Adventureworks table Sales.SalesOrderHeader" },
                    { "Parent", null },
                    { "Pos", 0 },
                    { "Sql", "select SalesOrderID, OrderDate, CustomerID, SubTotal from Sales.SalesOrderHeader order by SalesOrderID" }
                },
                new Dictionary<string, object>()
                {
                    { "Name", "SalesOrderDetails" },
                    { "Label", "Sales Order Details" },
                    { "Description", "Query of some fields from the the Adventureworks table Sales.SalesOrderDetail as detail query" },
                    { "Parent", "SalesOrders" },
                    { "Pos", 0 },
                    { "Sql", "select SalesOrderDetailID, ProductID, OrderQty, UnitPrice, LineTotal from sales.SalesOrderDetail where SalesOrderID = @SalesOrderID order by SalesOrderDetailID" }
                }
            };

        public List<QueryColumn> SalesOrderColumns = new List<QueryColumn>()
            {
                new QueryColumn() { Label = "SalesOrderID", ColumnType = QueryColumnType.INT},
                new QueryColumn() { Label = "OrderDate", ColumnType = QueryColumnType.DATETIME},
                new QueryColumn() { Label = "CustomerID", ColumnType = QueryColumnType.INT},
                new QueryColumn() { Label = "SubTotal", ColumnType = QueryColumnType.DECIMAL}
            };

        public List<QueryColumn> SalesOrderDetailColumns = new List<QueryColumn>()
            {
                new QueryColumn() { Label = "SalesOrderDetailID", ColumnType = QueryColumnType.INT},
                new QueryColumn() { Label = "ProductID", ColumnType = QueryColumnType.DATETIME},
                new QueryColumn() { Label = "OrderQty", ColumnType = QueryColumnType.DECIMAL},
                new QueryColumn() { Label = "UnitPrice", ColumnType = QueryColumnType.DECIMAL},
                new QueryColumn() { Label = "LineTotal", ColumnType = QueryColumnType.DECIMAL}
            };

    }
}
