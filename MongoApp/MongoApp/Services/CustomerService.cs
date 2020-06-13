using MongoApp.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoApp.Services
{
    public class CustomerService
    {
        private readonly IMongoCollection<Customer> _customers;

        public CustomerService(ICustomerDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _customers = database.GetCollection<Customer>(settings.CustomerCollectionName);
        }

        public List<Customer> Get() =>
            _customers.Find(book => true).ToList();

        public Customer Get(string id) =>
            _customers.Find<Customer>(book => book.Id == id).FirstOrDefault();

        public Customer Create(Customer book)
        {
            _customers.InsertOne(book);
            return book;
        }

        public void Update(string id, Customer bookIn) =>
            _customers.ReplaceOne(book => book.Id == id, bookIn);

        public void Remove(Customer bookIn) =>
            _customers.DeleteOne(book => book.Id == bookIn.Id);

        public void Remove(string id) =>
            _customers.DeleteOne(book => book.Id == id);
    }
}

