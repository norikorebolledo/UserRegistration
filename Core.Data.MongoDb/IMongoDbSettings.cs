using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.MongoDb
{
    public interface IMongoDbSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
    }
}
