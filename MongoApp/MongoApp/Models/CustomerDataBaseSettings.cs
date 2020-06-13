using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoApp.Models
{
    public class CustomerDataBaseSettings
    {
    }

    public class CustomerDatabaseSettings : ICustomerDatabaseSettings
    {
        public string CustomerCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface ICustomerDatabaseSettings
    {
        string CustomerCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
