using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HMSPhase2.AzureFunctions.DBModels;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using HMSPhase2.AzureFunctions.Collections;

namespace HMSPhase2.AzureFunctions
{
    public static class User
    {
        [FunctionName("CreateUser")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("CreateUser function processed a request.");
            var itemId = ObjectId.Empty;
            var jsonContent = string.Empty;
            try
            {
                //retrieving the content from the request's body
                jsonContent = await req.Content.ReadAsStringAsync().ConfigureAwait(false);
                //assuming we have valid JSON content, convert to BSON
                var doc = BsonSerializer.Deserialize<DBModels.User>(jsonContent);
                var collection = UsersCollection.Instance;
                //store new document in MongoDB collection
                await collection.InsertOneAsync(doc).ConfigureAwait(false);
                //retrieve the _id property created document
                itemId = doc._id;
            }
            catch (System.FormatException fex)
            {
                //thrown if there's an error in the parsed JSON
                log.Error($"A format exception occurred, check the JSON document is valid: {jsonContent}", fex);
            }
            catch (System.TimeoutException tex)
            {
                log.Error("A timeout error occurred", tex);
            }
            catch (MongoException mdbex)
            {
                log.Error("A MongoDB error occurred", mdbex);
            }
            catch (System.Exception ex)
            {
                log.Error("An error occurred", ex);
            }
            return itemId == ObjectId.Empty
                ? req.CreateResponse(HttpStatusCode.BadRequest, "An error occurred, please check the function log")
                : req.CreateResponse(HttpStatusCode.OK, $"The created item's _id is  {itemId}");
        }
        
    }
}
