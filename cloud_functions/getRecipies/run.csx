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

public class Recipie
{
   public string name;  

   public Recipie(string para)
   {
     name = para;
   }
}
public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log)
{
    //log.LogInformation("C# HTTP trigger function processed a request!");

    string name = req.Query["name"];
    string t;
    string[] y;
    string[] x;

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    name = name ?? data?.name; 
    var results = new List<Recipie>();

	var client = new HttpClient();
	var request = new HttpRequestMessage
	{
    	Method = HttpMethod.Get,
    	RequestUri = new Uri("https://api.spoonacular.com/recipes/findByIngredients?apiKey=9436c18753be46ef94b7c4299481a035&number=5&ingredients="+ name)
   	};

   
	using (var response = await client.SendAsync(request))
	{
		response.EnsureSuccessStatusCode();
		var body = await response.Content.ReadAsStringAsync();
    

		    body.ToString();
            //log.LogInformation("body is:" + body.ToString);
			x=body.Split("\"title\":");
			for (int i = 1; i < x.Length; i++)
            {
				t="";
				y=x[i].Split("\"original\":\"");
				for (int j=0; j<y.Length; j++)
				{
					t= t+ "\n"+ (y[j].Split("\",")[0]);
				}   
            results.Add(new Recipie (t));
            }	
    

    var jsonToReturn = JsonConvert.SerializeObject(results);
		log.LogInformation(jsonToReturn);
		return new HttpResponseMessage(HttpStatusCode.OK) 
		{
			Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
		};
    }
}