

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
    public string barcode { get; set; } 
    public string name {get; set; }
    string PartitionKey {get; set; }
    string RowKey {get; set; }
}



public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log, CloudTable inputTable)
{
    
   // log.LogInformation("1");
    string barcode_input_number = req.Query["barcode_input_number"];
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    barcode_input_number = barcode_input_number ?? data?.barcode_input_number;
    TableContinuationToken ct = null;
    string prodName = "not found";
    // log.LogInformation("2");
      var query = new TableQuery<Item>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "product"));

   // var query = new TableQuery<Item>().Where(TableQuery.GenerateFilterCondition("barcode", QueryComparisons.Equal, barcode_input_number));
    // log.LogInformation("3");

    do {
        var response = await inputTable.ExecuteQuerySegmentedAsync(query, ct);
        ct = response.ContinuationToken;
        // log.LogInformation("4");
        
        //log.LogInformation(response.Results.Count.ToString());
        for(int i = 0; i < response.Results.Count; i++){
            //log.LogInformation(response.Results[i].barcode);
            if(response.Results[i].barcode == barcode_input_number){
                prodName = response.Results[i].name;
                log.LogInformation("***");
                log.LogInformation(prodName);
            }
        }

         //log.LogInformation("5");
    } while(ct != null);

    var jsonToReturn = JsonConvert.SerializeObject(prodName);
    log.LogInformation(jsonToReturn);
    return new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
    };
}