using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HMSPhase2Functions
{
    public static class CreatePortal
    {
        [FunctionName("CreatePortal")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequestMessage req, [Table("portal", Connection = "")]ICollector<Portal> outTable, TraceWriter log)
        {
            dynamic data = await req.Content.ReadAsAsync<object>();
            string portalname = data?.portalname;
            string administrator = data?.administrator;
            string email = data?.email;
            string password = data?.password;
            string urlsuffix = data?.urlsuffix;

            if (portalname == null || administrator == null || email == null || password == null || urlsuffix == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass the parameters in the request body");
            }
            
            outTable.Add(new Portal()
            {
                PartitionKey = "Functions",
                RowKey = Guid.NewGuid().ToString(),
                PortalName = portalname,
                Administrator = administrator,
                Email = email,
                Password = password, 
                UrlSuffix = urlsuffix
            });
            return req.CreateResponse(HttpStatusCode.Created);
        }
        
    }
}
