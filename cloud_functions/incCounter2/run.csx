
#r "Microsoft.Azure.EventHubs"
#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"
using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;

public class Item: TableEntity
{
    public int Value1 { get; set; }
    public int Value2 { get; set; }
    public string Name { get; set; }
}
public static async Task Run(EventData[] myEvent,  ILogger log, CloudTable inputTable, CloudTable outputTable)
{   
    log.LogInformation($"test print" + myEvent);
    //var rowId = myEvent[0];
    //var countId = myEvent[1];
    //log.LogInformation(rowId + ", " + countId);

    TableResult result = await inputTable.ExecuteAsync(TableOperation.Retrieve<Item>("12","12"));    
    // if (result.Result == null) {
    //    log.LogInformation("No result"); 
    //    throw new HttpResponseMessage(HttpStatusCode.NoContent);
    // }
    var item = result.Result as Item;
    log.LogInformation($"{item.Name}");

    item.Value1 += 1;
    item.Name = "Foo Bar";
    item.ETag = "*";

    // Use Insert to insert...
    var operation = TableOperation.Replace(item);
    await outputTable.ExecuteAsync(operation);



}

