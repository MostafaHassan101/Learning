using Domain;
using Nest;

Console.ForegroundColor = ConsoleColor.Green;
//var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
//    .DefaultIndex("your_index_name_1"); // Replace with your desired index name

var connectionsettings = new ConnectionSettings(new Uri("https://localhost:9200"))
    .ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true) // Disable certificate validation (not recommended for production)
    .EnableTcpKeepAlive(TimeSpan.FromMinutes(2), TimeSpan.FromSeconds(10))
    //.PrettyJson()
    //.OnRequestDataCreated()
    .DefaultIndex("your_index_name_1").BasicAuthentication("elastic", "cp-hY3MfSa2QTxz5XmP1");


var client = new ElasticClient(connectionsettings);

// Step 1: check index is Exists & Create the index and define the mapping

if (!(client.Indices.Exists("your_index_name_1").Exists))
{
    var createIndexResponse = client.Indices.Create("your_index_name_1", c => c
   .Map<Person>(d => d
       .AutoMap()
   ));
    if (createIndexResponse.IsValid) Console.WriteLine("Created Index Successfully.");
    else Console.WriteLine(createIndexResponse.OriginalException.Message);
}

// Step 2: Store data in the index
var document = new Person
{

    Id = new Random().Next(1, 999999),
    Name = "Mostafa Hassan",
    Age = 310
};

// Insert
var indexDocResponseAsync = await client.IndexDocumentAsync(document);
//var indexResponseAsync = await client.IndexAsync(document,"");

// Get doc by Id 
var getasync = await client.GetAsync<Person>(1, idx => idx.Index("your_index_name_1"));


// Update doc in index
var updateResponseAsync = await client.UpdateAsync<Person>(1, up => 
                          up.Doc(document)
                          .Index("your_index_name_1"));

// Delete doc in index
var deleteResponseAsync = await client.DeleteAsync<Person>(913663, d => d
                            .Index("your_index_name_1"));

if (!deleteResponseAsync.IsValid)
{
    // Handle delete failure
    //throw new Exception("Failed to delete document");
    Console.WriteLine(deleteResponseAsync.OriginalException.Message);
}

if (indexDocResponseAsync.IsValid)
{
    Console.WriteLine("Document indexed successfully.");

    // Step 3: Perform a query on the index
    var searchResponseAsync = await client.SearchAsync<Person>(s => s
        .Query(q => q
            .Match(m => m
                .Field(f => f.Name)
                .Query("Mostafa Hassan")
                    )
        ).Size(100).Sort(s => s.Field(f => f.Age, SortOrder.Ascending))
    );
    // Aggregation Functions
    var res = await client.SearchAsync<Person>(s => s.Aggregations(a => a
            .Max("Max_Age", sa => sa
            .Field(f => f.Age) // Field to Max
        )));
    Console.WriteLine(res.Aggregations.Max("Max_Age").Value);

    if (searchResponseAsync.IsValid)
    {
        Console.WriteLine("Query executed successfully. Results:");

        foreach (var hit in searchResponseAsync.Hits)
        {
            Console.WriteLine($"Id: {hit.Id}, Name: {hit.Source.Name}, Age: {hit.Source.Age}");
        }
    }
    else
    {
        Console.WriteLine("Query failed. Error: " + searchResponseAsync.OriginalException.Message);
    }
}
else
{
    Console.WriteLine("Document indexing failed. Error: " + indexDocResponseAsync.OriginalException.Message);
    Console.WriteLine("Document indexing failed. Error: " + indexDocResponseAsync.OriginalException.InnerException?.Message);
}

Console.ReadKey();