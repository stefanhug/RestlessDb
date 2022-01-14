using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestlessDb.Client.Shared
{
    public class TableDisplayOptions
    {
        public bool Hover { get; set; }
        public bool Dense { get; set; }
        public bool Striped { get; set; }
        public bool Bordered { get; set; }

    }
    public partial class QueryResultInnerTable
    {
        [Parameter, EditorRequired]
        public QueryMetaData MetaData { get; set; }
        [Parameter, EditorRequired]
        public IEnumerable<IDictionary<string, object>> Data { get; set; }
        [Parameter]
        public TableDisplayOptions TableDisplayOptions { get; set; } = new();
        [Parameter]
        public int NestingLevel { get; set; } = 0;

        /// <summary>
        /// This is a dirty hack to resolve the problem of the Newtonsoft Deserialization. Due to the definition of 
        /// QueryResult Newtonsoft cannot determine the correct type List<Dictionary<string,object> and deserializes as JArray
        /// </summary>
        /// <param name="objectToConvert"></param>
        public static IEnumerable<IDictionary<string, object>> CastDataToListOfDicts(object objectToConvert)
        {
            List<IDictionary<string, object>> ret = new();
            try
            {
                var enumerable = (IEnumerable<object>)objectToConvert;
                foreach(var itm in enumerable)
                {
                    if (itm is IDictionary<string, object>)
                    {
                        ret.Add(itm as IDictionary<string, object>);
                    }
                    else if (itm is IDictionary<string, JToken>)
                    {
                        var newDict = new Dictionary<string, object>();
                        foreach(var kvp in (IDictionary<string, JToken>)itm)
                        {
                            newDict.Add(kvp.Key, kvp.Value);
                        }
                        ret.Add(newDict);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"CastDataToListOfDicts: cast failed {e.Message}");
            }
            return ret;
        }
    }
}