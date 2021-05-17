using GenericDbRestApi.Lib.DataLayer;
using GenericDbRestApi.Lib.Managers;
using GenericDbRestApi.Lib.Repositories;
using GenericDBRestApi.Lib.Formatters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace GenericDbRestApi.Lib.Services
{
    public static class GenericDbRestApiServiceCollectionExtensions
    {
        public static void AddDbRestApi(this IServiceCollection services, string dbConnectionString)
        {
            services.AddScoped<SqlConnection, SqlConnection>(p =>
            {
                var conn = new SqlConnection(dbConnectionString);
                conn.Open();
                return conn;
            });
            services.AddScoped<IGenericSqlHelper, GenericSqlHelper>();
            services.AddScoped<QueryItemProvider, QueryItemProvider>();
            services.AddScoped<QueryParamsProvider, QueryParamsProvider>();
            services.AddScoped<QueryRepository, QueryRepository>();
            services.AddScoped<GenericQueryManager, GenericQueryManager>();

            //Formatters
            services.AddSingleton<IQueryFormatter, QueryJsonFormatter>();
            services.AddSingleton<IQueryFormatter, QueryXmlFormatter>();
            services.AddSingleton<IQueryFormatter, QueryCsvFormatter>();
            services.AddSingleton<IQueryFormatter, QueryExcelFormatter>();
            services.AddSingleton<IQueryFormatter, QueryHtmlFormatter>();
        }
    }
}
