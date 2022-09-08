using ExchangeSimulatorBackend.Dtos;
using ExchangeSimulatorBackend.HubConfig;
using ExchangeSimulatorBackend.Services;
using MatchingEngineApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ExchangeSimulatorBackend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IHubContext<MatchingEngineHub> _hub;
        private readonly MatchingEngineService _matchingEngineService;

        public OrdersController(IHubContext<MatchingEngineHub> hub, MatchingEngineService matchingEngineService)
        {
            _hub = hub;
            _matchingEngineService = matchingEngineService;
        }


        //[HttpGet]
        //public IActionResult GetPriceLevels()
        //{
            
        //}

        [HttpPost]
        public IActionResult AddOrder([FromBody] AddOrderDto addOrderDto)
        {
            if (addOrderDto.OrderType == Dtos.OrderType.MarketOrder)
            {
                _matchingEngineService.AddOrder(new MarketOrder(Guid.NewGuid(), (uint)addOrderDto.Quantity, (MatchingEngineApp.OrderType)addOrderDto.OrderSide!));
            }
            else if (addOrderDto.OrderType == Dtos.OrderType.LimitOrder)
            {
                _matchingEngineService.AddOrder(new LimitOrder(Guid.NewGuid(), (uint)addOrderDto.Quantity, addOrderDto.Price, (MatchingEngineApp.OrderType)addOrderDto.OrderSide!));
            }

            return Ok();
        }
    }
}
