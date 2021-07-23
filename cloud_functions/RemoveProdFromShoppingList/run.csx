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
   
    string to_romove_name = req.Query["to_romove_name"];
    
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    to_romove_name = to_romove_name ?? data?.to_romove_name;
   
    //reading table entry we want 
    TableOperation retrieveOperation = TableOperation.Retrieve<Item>("product",to_romove_name);
    TableResult result = await inputTable.ExecuteAsync(retrieveOperation);
    
    log.LogInformation(to_romove_name);
     
    if (result.Result == null) {   
        //shouldnt happen- error
    } else {
        var item = result.Result as Item;
        var operation = TableOperation.Delete(item); //instead of Replace there is Insert/Delete/Replace and some more
        await outputTable.ExecuteAsync(operation); // wait till works
        log.LogInformation("del");
      
    }

    var myObj = new {productName = to_romove_name};
    var jsonToReturn = JsonConvert.SerializeObject(myObj);

    return new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
    };
 
}


//https://functionappinportal.azurewebsites.net/api/UpdateCounter?counterID=1&newValue=10
