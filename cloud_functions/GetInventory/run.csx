//code returns list of products from table inventory using http result
#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Text;
using System.Collections.Generic;

//entrys is table
public class Item: TableEntity
{
    public string name { get; set; }
    public string barcode { get; set; }
    public string type { get; set; }
    public int amount {get; set; }

    string PartitionKey {get; set; }
    string RowKey {get; set; }
}

public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log, CloudTable inputTable)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    TableContinuationToken ct = null;
    var results = new List<Item>();

    var query = new TableQuery<Item>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "product"));
    //var results = await inputTable.ExecuteQuerySegmentedAsync(query, ct);


    do {
        var response = await inputTable.ExecuteQuerySegmentedAsync(query, ct);
        ct = response.ContinuationToken;
        log.LogInformation("*****"+response.Results[0].name);
        results.AddRange(response.Results);
    } while(ct != null);


    //var myObj = new { products = results};
    var jsonToReturn = JsonConvert.SerializeObject(results);
    log.LogInformation(jsonToReturn);
    return new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
    };
   
}
