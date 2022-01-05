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
        [Parameter]
        public string QueryItem { get; set; }
        public TableDisplayOptions TableDisplayOptions { get; } = new();

        private int Offset { get; set; } = 0;
        private int MaxRows { get; set; } = 500;
        private string OutputFormat { get; set; }
        private string ErrorMessage { get; set; }
        private QueryMetaData QueryMetaData { get; set; }
        private QueryResult QueryResult { get; set; }


        private Dictionary<string, string> ParamValuesDict = new();
        private string PageDataContent { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ClientModel.CheckInitAsync();
            OutputFormat = ClientModel.FormatterInfos.FirstOrDefault(f => f.Disposition == Disposition.EMBEDDED).OutputFormat;
        }

        protected override async Task OnParametersSetAsync()
        {
            ErrorMessage = null;
            PageDataContent = null;
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
            PageDataContent = "Loading...";
            try
            {
                var httpResponseMessage = await ClientModel.GatewayRestlessDb.FetchQueryContentAsync(QueryItem, OutputFormat,
                                                                                                     Offset, MaxRows, ParamValuesDict);
                var formatterInfo = ClientModel.FormatterInfos.First(f => f.OutputFormat == OutputFormat);
                if (formatterInfo.Disposition == Disposition.EMBEDDED)
                {
                    var content = await httpResponseMessage.Content.ReadAsStringAsync();
                    QueryResult = JsonConvert.DeserializeObject<QueryResult>(content);
                }
                else
                {
                    var fileName = $"{QueryMetaData.Name}.{formatterInfo.FileExtension}";
                    var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    PageDataContent = $"File {fileName} downloaded";
                    using (var streamRef = new DotNetStreamReference(stream: stream))
                    {
                        await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
                    }
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
        }
    }

}

