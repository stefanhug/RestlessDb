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
        private bool loading { get; set; }

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
            QueryResult = null;
            var (success, fetchQueryResult) = await FetchJsonData(Offset);
            if (success)
            {
                QueryResult = fetchQueryResult;
            }
        }

        public async Task FetchMoreRowsAndShow()
        {
            var (success, fetchQueryResult) = await FetchJsonData(QueryResult.Data.Count);
            if (success)
            {
                QueryResult.Data.AddRange(fetchQueryResult.Data);
                QueryResult.HasMoreRows = fetchQueryResult.HasMoreRows;
            }
        }

        public async Task ExportQueryResultsAsync()
        {
            ErrorMessage = null;
            try
            {
                loading = true;
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
                loading = false;
            }
        }

        private async Task<(bool success, QueryResult)> FetchJsonData(int offset)
        {
            ErrorMessage = null;
            QueryResult fetchQueryResult;
            loading = true;
            try
            {
                queryResultTable.Loading = true;
                var httpResponseMessage = await ClientModel.GatewayRestlessDb.FetchQueryContentAsync(QueryItem, JSON,
                                                                                                     offset, MaxRows, ParamValuesDict);
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                fetchQueryResult = JsonConvert.DeserializeObject<QueryResult>(content);
            }
            catch (HttpRequestException e)
            {
                ErrorMessage = $"HttpRequestException: httpstatus: {e.StatusCode}\nMessage:{e.Message}\n=====\nType: {e.GetType()}\nStack trace:\n==========\n{e.StackTrace}";
                return (false, null);
            }
            catch (Exception e)
            {
                ErrorMessage = $"{e.Message}\nType: {e.GetType()}\nStack trace:\n==========\n{e.StackTrace}";
                return (false, null);
            }
            finally
            {
                queryResultTable.Loading = false;
                loading = false;
            }
            return (true, fetchQueryResult);
        }
    }
}


