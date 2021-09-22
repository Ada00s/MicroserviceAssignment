using ClientApi.Handlers;
using ClientApi.Handlers.Helpers;
using ClientApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ClientApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private ICustomerHandler _customerHandler { get; set; }

        public CustomerController(ICustomerHandler customerHandler)
        {
            _customerHandler = customerHandler;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Customer>>> GetAllCustomers()
        {
            try
            {
                var result = await _customerHandler.GetAllCustomers();
                return result;
            }
            catch (ApiException ae)
            {
                return StatusCode((int)ae.StatusCode, ae.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateNewCustomer([FromBody] Customer newCustomer)
        {
            if (newCustomer != null)
            {
                try
                {
                    var results = await _customerHandler.CreateNewCustomer(newCustomer);
                    return results;
                }
                catch (ApiException ae)
                {
                    return StatusCode((int)ae.StatusCode, ae.Message);
                }
                catch (Exception e)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
                }
            }
            return false;
        }

        [HttpPut]
        public async Task<ActionResult<bool>> UpdateCustomer(Customer existingCustomer)
        {
            try
            {
                var results = await _customerHandler.UpdateCustomer(existingCustomer);
                return results;
            }
            catch (ApiException ae)
            {
                return StatusCode((int)ae.StatusCode, ae.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet("{customerId}")]
        public async Task<ActionResult<Customer>> GetCustomerById(string customerId)
        {
            if (!Int16.TryParse(customerId, out var id))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "It was not provided a valid Id");
            }

            try
            {
                var result = await _customerHandler.GetCustomerById(id);
                return result;
            }
            catch (ApiException ae)
            {
                return StatusCode((int)ae.StatusCode, ae.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
        
    }
}
