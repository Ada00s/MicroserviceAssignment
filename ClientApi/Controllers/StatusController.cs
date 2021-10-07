using ClientApi.Handlers;
using CommonLib.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ClientApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        private IOrderHandler _orderHandler { get; set; }

        public StatusController(IOrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
        }

        [HttpPut("{orderId}")]
        public async Task<ActionResult<bool>> UpdateOrderStatus(string orderId, [FromBody] int newStatus)
        {
            try
            {
                if (!Int16.TryParse(orderId, out var id))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "Provided not valid customer id");
                }
                var result = await _orderHandler.UpdateOrderStatus(id, newStatus);
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

        [HttpGet("{orderId}")]
        public async Task<ActionResult<string>> CheckOrderStatus(string orderId)
        {
            try
            {
                if (!Int16.TryParse(orderId, out var id))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "Provided not valid customer id");
                }
                var result = await _orderHandler.GetOrderById(id);
                return result.Status.ToString();
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
