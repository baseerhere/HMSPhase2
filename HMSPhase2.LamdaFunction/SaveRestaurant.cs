using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using System.Net;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HMSPhase2.LamdaFunction
{
    public class SaveRestaurant
    {

        IDynamoDBContext DDBContext { get; set; }

        public SaveRestaurant()
        {
            // Check to see if a table name was passed in through environment variables and if so 
            // add the table mapping.
            var tableName = "users";
            AWSConfigsDynamoDB.Context.TypeMappings[typeof(Restaurant)] = new Amazon.Util.TypeMapping(typeof(Restaurant), tableName);
            

            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            this.DDBContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse AddRestaurant(APIGatewayProxyRequest request, ILambdaContext context)
        {

            context.Logger.LogLine($"Request {request?.Body}");

            var user = JsonConvert.DeserializeObject<user>(request?.Body);
            
            context.Logger.LogLine($"Saving User with name {user.Name}");
            Task task = DDBContext.SaveAsync<user>(user);
            task.Wait();

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = user.Name.ToString(),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };
            return response;
        }
    }
}
