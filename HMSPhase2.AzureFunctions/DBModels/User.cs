using MongoDB.Bson;

namespace HMSPhase2.AzureFunctions.DBModels
{
    public class User
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
    }
}
