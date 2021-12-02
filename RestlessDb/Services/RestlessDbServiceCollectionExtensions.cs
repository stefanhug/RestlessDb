using RestlessDb.DataLayer;
using RestlessDb.Managers;
using RestlessDb.Repositories;
using RestlessDb.Formatters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace RestlessDb.Services
{
    public static class RestlessDbServiceCollectionExtensions
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
            services.AddScoped<QueryConfigRepository, QueryConfigRepository>();
            services.AddScoped<QueryConfigManager, QueryConfigManager>();

            //Formatters
            services.AddSingleton<IQueryFormatter, QueryJsonFormatter>();
            services.AddSingleton<IQueryFormatter, QueryXmlFormatter>();
            services.AddSingleton<IQueryFormatter, QueryCsvFormatter>();
            services.AddSingleton<IQueryFormatter, QueryExcelFormatter>();
            services.AddSingleton<IQueryFormatter, QueryHtmlFormatter>();
        }
    }
}
