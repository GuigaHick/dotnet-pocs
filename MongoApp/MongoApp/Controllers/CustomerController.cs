using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoApp.Models;
using MongoApp.Services;

namespace MongoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;
        public CustomerController(CustomerService bookService)
        {
            _customerService = bookService;
        }

        [HttpGet]
        public ActionResult<List<Customer>> Get() =>
            _customerService.Get();

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        public ActionResult<Customer> Get(string id)
        {
            var book = _customerService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        [Route("Create")]
        public ActionResult<Customer> Create(Customer customer)
        {
            _customerService.Create(customer);

            return CreatedAtRoute("GetBook", new { id = customer.Id.ToString() }, customer);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Customer bookIn)
        {
            var book = _customerService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _customerService.Update(id, bookIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var book = _customerService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _customerService.Remove(book.Id);

            return NoContent();
        }

    }
}