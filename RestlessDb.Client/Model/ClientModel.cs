using Newtonsoft.Json;
using RestlessDb.Common.Gateway;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RestlessDb.Client.Model
{
    public class ClientModel
    {
        private readonly GatewayRestlessDb gatewayRestlessDb;

        private List<QueryConfigItem> queryConfigItems;
        public List<QueryConfigItem> QueryConfigItems
        {
            get => queryConfigItems;
        }
        public GatewayRestlessDb GatewayRestlessDb
        {
            get => gatewayRestlessDb;
        }

        public string ErrorMessage { get; private set;  } = null;

        public ClientModel(HttpClient httpClient)
        {
            this.gatewayRestlessDb = new GatewayRestlessDb(httpClient);
        }

        public async Task CheckInitAsync()
        {
            if (queryConfigItems != null)
            {
                return;
            }
            try
            {
                ErrorMessage = null;
                queryConfigItems = (await GatewayRestlessDb.GetQueryConfigAsync());
            }
            catch(Exception e)
            {
                ErrorMessage = $"Failed to fetch query configuration: {e.Message}";
            }
            
        }

        public async Task<QueryConfigItem> GetConfigItemAsync(string name)
        {
            await CheckInitAsync();
            return queryConfigItems.Find(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public void InvalidateCaches()
        {
            queryConfigItems = null;
        }
    }
}
