using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HMSPhase2.Lambda
{
    public class Functions
    {
        // This const is the name of the environment variable that the serverless.template will use to set
        // the name of the DynamoDB table used to store Restaurant posts.
        const string TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP = "Restaurants";

        public const string ID_QUERY_STRING_NAME = "Id";
        IDynamoDBContext DDBContext { get; set; }

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            // Check to see if a table name was passed in through environment variables and if so 
            // add the table mapping.
            var tableName = System.Environment.GetEnvironmentVariable(TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP);
            if(!string.IsNullOrEmpty(tableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(Restaurant)] = new Amazon.Util.TypeMapping(typeof(Restaurant), tableName);
            }

            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            this.DDBContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
        }

        /// <summary>
        /// Constructor used for testing passing in a preconfigured DynamoDB client.
        /// </summary>
        /// <param name="ddbClient"></param>
        /// <param name="tableName"></param>
        public Functions(IAmazonDynamoDB ddbClient, string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(Restaurant)] = new Amazon.Util.TypeMapping(typeof(Restaurant), tableName);
            }

            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            this.DDBContext = new DynamoDBContext(ddbClient, config);
        }

        /// <summary>
        /// A Lambda function that returns back a page worth of Restaurant posts.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of Restaurants</returns>
        public async Task<APIGatewayProxyResponse> GetRestaurantsAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Getting Restaurants");
            var search = this.DDBContext.ScanAsync<Restaurant>(null);
            var page = await search.GetNextSetAsync();
            context.Logger.LogLine($"Found {page.Count} Restaurants");

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(page),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }

        /// <summary>
        /// A Lambda function that returns the Restaurant identified by restaurantId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> GetRestaurantAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string restaurantId = null;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(ID_QUERY_STRING_NAME))
                restaurantId = request.PathParameters[ID_QUERY_STRING_NAME];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(ID_QUERY_STRING_NAME))
                restaurantId = request.QueryStringParameters[ID_QUERY_STRING_NAME];

            if (string.IsNullOrEmpty(restaurantId))
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Body = $"Missing required parameter {ID_QUERY_STRING_NAME}"
                };
            }

            context.Logger.LogLine($"Getting Restaurant {restaurantId}");
            var Restaurant = await DDBContext.LoadAsync<Restaurant>(restaurantId);
            context.Logger.LogLine($"Found Restaurant: {Restaurant != null}");

            if (Restaurant == null)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(Restaurant),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
            return response;
        }

        /// <summary>
        /// A Lambda function that adds a Restaurant post.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> AddRestaurantAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var Restaurant = JsonConvert.DeserializeObject<Restaurant>(request?.Body);
            Restaurant.Id = Guid.NewGuid().ToString();
            Restaurant.CreatedTimestamp = DateTime.Now;

            context.Logger.LogLine($"Saving Restaurant with id {Restaurant.Id}");
            await DDBContext.SaveAsync<Restaurant>(Restaurant);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = Restaurant.Id.ToString(),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };
            return response;
        }

        /// <summary>
        /// A Lambda function that removes a Restaurant post from the DynamoDB table.
        /// </summary>
        /// <param name="request"></param>
        public async Task<APIGatewayProxyResponse> RemoveRestaurantAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string restaurantId = null;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(ID_QUERY_STRING_NAME))
                restaurantId = request.PathParameters[ID_QUERY_STRING_NAME];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(ID_QUERY_STRING_NAME))
                restaurantId = request.QueryStringParameters[ID_QUERY_STRING_NAME];

            if (string.IsNullOrEmpty(restaurantId))
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Body = $"Missing required parameter {ID_QUERY_STRING_NAME}"
                };
            }

            context.Logger.LogLine($"Deleting Restaurant with id {restaurantId}");
            await this.DDBContext.DeleteAsync<Restaurant>(restaurantId);

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
