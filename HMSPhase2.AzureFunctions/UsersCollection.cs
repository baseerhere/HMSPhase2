using HMSPhase2.AzureFunctions.DBModels;
using MongoDB.Driver;
using System;

namespace HMSPhase2.AzureFunctions
{
    public sealed class UsersCollection
    {
        private static volatile IMongoCollection<User> instance;
        private static object syncRoot = new Object();

        private UsersCollection() { }

        public static IMongoCollection<User> Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            string strMongoDBAtlasUri = @"mongodb://hms:JX8Cw7yb26WAmhhV228HXXpXQcuxAbSkXSMv0fHMnPvLnn3b9ElclVjld01cMmIisXUPSB7ZZP0Vncm434LHpA==@hms.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";
                            var client = new MongoClient(strMongoDBAtlasUri);
                            var db = client.GetDatabase("hms");
                            instance = db.GetCollection<User>("users");
                        }
                    }
                }
                return instance;
            }
        }
    }
}
