using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using RestlessDb.DataLayer;
using RestlessDb.Formatters;
using RestlessDb.Managers;
using RestlessDb.Repositories;

namespace RestlessDb.Services
{
    public static class RestlessDbServiceCollectionExtensions
    {
        public static void AddDbRestApi(this IServiceCollection services, string dbConnectionString)
        {
            AddDependencies(services, dbConnectionString);
            AddFormatters(services);
        }

        private static void AddFormatters(IServiceCollection services)
        {
            services.AddSingleton<IQueryFormatter, QueryJsonFormatter>();
            services.AddSingleton<IQueryFormatter, QueryXmlFormatter>();
            services.AddSingleton<IQueryFormatter, QueryCsvFormatter>();
            services.AddSingleton<IQueryFormatter, QueryExcelFormatter>();
            services.AddSingleton<IQueryFormatter, QueryHtmlFormatter>();
            services.AddSingleton<IQueryFormatter, QueryEmbeddedFormatter>();
        }

        private static void AddDependencies(IServiceCollection services, string dbConnectionString)
        {
            services.AddScoped<SqlConnection, SqlConnection>(p =>
            {
                var conn = new SqlConnection(dbConnectionString);
                conn.Open();
                return conn;
            });
            services.AddScoped<IGenericSqlHelper, GenericSqlHelper>();
            services.AddScoped<QueryParamsProvider, QueryParamsProvider>();
            services.AddScoped<QueryRepository, QueryRepository>();
            services.AddScoped<GenericQueryManager, GenericQueryManager>();
            services.AddScoped<QueryConfigManager, QueryConfigManager>();
            services.AddScoped<QueryItemsManager, QueryItemsManager>();
            services.AddScoped<QueryItemsRepository, QueryItemsRepository>();
        }
    }
}
