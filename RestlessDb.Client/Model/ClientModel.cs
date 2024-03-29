﻿using Newtonsoft.Json;
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

        public List<QueryMetaData> QueryMetaDatas { get; private set; }
        public List<FormatterInfo> FormatterInfos { get; private set; }

        public GatewayRestlessDb GatewayRestlessDb
        {
            get => gatewayRestlessDb;
        }

        public string ErrorMessage { get; private set;  } = null;

        public ClientModel(HttpClient httpClient)
        {
            this.gatewayRestlessDb = new GatewayRestlessDb(httpClient);
        }

        public async Task<bool> CheckInitAsync()
        {
            if (QueryMetaDatas != null && FormatterInfos != null)
            {
                return true;
            }
            try
            {
                ErrorMessage = null;
                QueryMetaDatas = await GatewayRestlessDb.GetQueryConfigAsync();
                FormatterInfos = await GatewayRestlessDb.GetAllFormatters();
                return true;
            }
            catch(Exception e)
            {
                ErrorMessage = $"Failed to fetch query configuration: {e.Message}";
            }
            return false;
            
        }

        public async Task<QueryMetaData> GetConfigItemAsync(string name)
        {
            await CheckInitAsync();
            return QueryMetaDatas.Find(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public void InvalidateCaches()
        {
            QueryMetaDatas = null;
        }
    }
}
