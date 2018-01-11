using MongoDB.Bson;
using System;

namespace HMSPhase2.AzureFunctions.DBModels
{
    class Restaurant
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string ShortCode { get; set; }
        public User Administrator { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedBy { get; set; }
    }
}
