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
    public class OrderController : ControllerBase
    {
        private OrderHandler _orderHandler { get; set; }

        public OrderController (OrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
        }

        [HttpGet("all/{customerId}")]
        public async Task<ActionResult<List<Order>>> GetAllCustomersOrders(string customerId)
        {
            if (!Int16.TryParse(customerId, out var id))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Provided not valid customer id");
            }
            try
            {
                var result = await _orderHandler.GetOrdersForCustomer(id);
                return result;
            }
            catch(ApiException ae)
            {
                return StatusCode((int)ae.StatusCode, ae.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(string orderId)
        {
            if (!Int16.TryParse(orderId, out var id))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Provided not valid order id");
            }

            try
            {
                var result = await _orderHandler.GetOrderById(id);
                return result;
            }
            catch (ApiException ae)
            {
                return StatusCode((int)ae.StatusCode, ae.Message);
            }
            catch(Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateOrder([FromBody]Order newOrder)
        {
            try
            {
                var results = await _orderHandler.CreateOrder(newOrder);
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
    }
}
