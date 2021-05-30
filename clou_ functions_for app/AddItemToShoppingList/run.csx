//includes, imports
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



//table entry, values other than row/partition key
public class Item: TableEntity
{
    string PartitionKey {get; set; }
    string RowKey {get; set; }
}



public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log, CloudTable inputTable, CloudTable outputTable)
{     
    log.LogInformation("C# HTTP trigger function processed a request.");
   
    string name_to_add = req.Query["name_to_add"];
    
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    name_to_add = name_to_add ?? data?.barcode_inputname_to_add_number;
   

    //reading table entry we want 
    TableOperation retrieveOperation = TableOperation.Retrieve<Item>("product",name_to_add);
    TableResult result = await inputTable.ExecuteAsync(retrieveOperation);
    bool added;
     
    if (result.Result == null) {
       
       var item = new Item(); 
       item.RowKey = name_to_add;
       item.PartitionKey = "product"; 
       item.ETag = "*";      
       var operation = TableOperation.Insert(item); //instead of Replace there is Insert/Delete/Replace and some more
       await outputTable.ExecuteAsync(operation); // wait till works
       added = true;
    } else {
      added = false;
    }

   // var myObj = new {added = added};
    var jsonToReturn = JsonConvert.SerializeObject(added);

    return new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
    };
 
}


//https://functionappinportal.azurewebsites.net/api/UpdateCounter?counterID=1&newValue=10
