//includes, imports
#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"
using System.Net.Http;
using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Text;
using System.Collections.Generic;

static HttpClient client = new HttpClient();
//table entry, values other than row/partition key
public class Item: TableEntity
{
    public string name { get; set; }
    public string barcode { get; set; }
    public string type { get; set; }
    public int amount {get; set; }

    string PartitionKey {get; set; }
    string RowKey {get; set; }
}


public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log, CloudTable inputTable, CloudTable outputTable)
{
    
    
    log.LogInformation("C# HTTP trigger function processed a request..");
 
    string barcode_input_number = req.Query["barcode_input_number"];  
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    barcode_input_number = barcode_input_number ?? data?.barcode_input_number;

    //getting name of product from barcode
        string BaseURL = "https://functionappinportal.azurewebsites.net/api/barcodeToName?barcode_input_number=";
        string myUrl = BaseURL + barcode_input_number;
        string prodName = null;

        HttpResponseMessage httpResult = client.GetAsync(myUrl).Result;
        Console.WriteLine(httpResult.StatusCode);
        string jsonData = await httpResult.Content.ReadAsStringAsync();
        prodName = jsonData;
        prodName = prodName.Substring(1,prodName.Length-2);
  

    //reading table entry we want 
    TableOperation retrieveOperation = TableOperation.Retrieve<Item>("product",barcode_input_number);
    TableResult result = await inputTable.ExecuteAsync(retrieveOperation);
 
     
    if (result.Result == null) {
       var item = new Item();
       item.name = prodName;
       item.barcode = barcode_input_number;
       item.RowKey = barcode_input_number;
       item.PartitionKey = "product";
       item.amount = 1;
       item.ETag = "*";      
       var operation = TableOperation.Insert(item); //instead of Replace there is Insert/Delete/Replace and some more
     
       await outputTable.ExecuteAsync(operation); // wait till works
       
    } else {
        //log.LogInformation("found");
        log.LogInformation("1");
        var item = result.Result as Item;
        item.amount = item.amount+1;
        item.ETag = "*";
        // Use Insert to Insert/Replace table entry with our updated one
        var operation = TableOperation.Replace(item); //instead of Replace there is Insert/Delete/Replace and some more
         log.LogInformation("1");
        await outputTable.ExecuteAsync(operation); // wait till works
         log.LogInformation("1");
        log.LogInformation("5");

    }
    var myObj = new {productName = prodName};
    var jsonToReturn = JsonConvert.SerializeObject(myObj);

    return new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
    };
 
}


//https://functionappinportal.azurewebsites.net/api/UpdateCounter?counterID=1&newValue=10
