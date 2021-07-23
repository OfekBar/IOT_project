#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;

public class Item: TableEntity
{
    public int Value1 { get; set; }
    public int Value2 { get; set; }
    public string Name { get; set; }
}

public static async Task<HttpResponseMessage> Run(HttpRequest req, CloudTable outTable, TraceWriter log)
{
    var item = new Item{
        Value1 = 1,
        Value2 = 2,
        PartitionKey = "partKey",
        RowKey = "rowKey",
        Name = "Neo's entity"
    };

    item.ETag = "*";

    // Use Insert to insert...
    var operation = TableOperation.Insert(item);
    await outTable.ExecuteAsync(operation);

    return new HttpResponseMessage(HttpStatusCode.NoContent);
}
