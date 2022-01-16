using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using RestlessDb.Client.Shared;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestlessDb.Client.Pages
{
    public partial class ShowQuery
    {
        const string JSON = "json";

        [Parameter]
        public string QueryItem { get; set; }
        public TableDisplayOptions TableDisplayOptions { get; } =
            new()
            {
                Bordered = true,
                Dense = true,
                Hover = true,
                Striped = true
            };

        private int Offset { get; set; } = 0;
        private int MaxRows { get; set; } = 500;
        private string ExportFormat { get; set; }
        private string ErrorMessage { get; set; }
        private QueryMetaData QueryMetaData { get; set; }
        private QueryResult QueryResult { get; set; }
        private Dictionary<string, string> ParamValuesDict = new();
        private QueryResultTable queryResultTable { get; set; }
        private bool exporting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ClientModel.CheckInitAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            ErrorMessage = null;
            QueryResult = null;

            QueryMetaData = await ClientModel.GetConfigItemAsync(QueryItem);
            ParamValuesDict.Clear();
            if (QueryMetaData.Parameters?.Count > 0)
            {
                int numParams = QueryMetaData.Parameters.Count;

                foreach (var param in QueryMetaData.Parameters)
                {
                    ParamValuesDict.Add(param, string.Empty);
                }
            }
        }

        public async Task ShowQueryResultsAsync()
        {
            ErrorMessage = null;
            QueryResult = null;
            try
            {
                queryResultTable.Loading = true;
                var httpResponseMessage = await ClientModel.GatewayRestlessDb.FetchQueryContentAsync(QueryItem, JSON,
                                                                                                     Offset, MaxRows, ParamValuesDict);
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                QueryResult = JsonConvert.DeserializeObject<QueryResult>(content);
            }
            catch (HttpRequestException e)
            {
                ErrorMessage = $"HttpRequestException: httpstatus: {e.StatusCode}\nMessage:{e.Message}\n=====\nType: {e.GetType()}\nStack trace:\n==========\n{e.StackTrace}";
            }
            catch (Exception e)
            {
                ErrorMessage = $"{e.Message}\nType: {e.GetType()}\nStack trace:\n==========\n{e.StackTrace}";
            }
            finally
            {
                queryResultTable.Loading = false;
            }
        }
        public async Task ExportQueryResultsAsync()
        {
            ErrorMessage = null;
            try
            {
                exporting = true;
                var httpResponseMessage = await ClientModel.GatewayRestlessDb.FetchQueryContentAsync(QueryItem, ExportFormat,
                                                                                                     Offset, MaxRows, ParamValuesDict);
                var formatterInfo = ClientModel.FormatterInfos.First(f => f.OutputFormat == ExportFormat);
                var fileName = $"{QueryMetaData.Name}.{formatterInfo.FileExtension}";
                var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
                using (var streamRef = new DotNetStreamReference(stream: stream))
                {
                    await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
                }

            }
            catch (HttpRequestException e)
            {
                ErrorMessage = $"HttpRequestException: httpstatus: {e.StatusCode}\nMessage:{e.Message}\n=====\nType: {e.GetType()}\nStack trace:\n==========\n{e.StackTrace}";
            }
            catch (Exception e)
            {
                ErrorMessage = $"{e.Message}\nType: {e.GetType()}\nStack trace:\n==========\n{e.StackTrace}";
            }
            finally
            {
                exporting = false;
            }
        }
    }
}


