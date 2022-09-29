using ExchangeSimulatorBackend.Dtos;
using ExchangeSimulatorBackend.HubConfig;
using ExchangeSimulatorBackend.Services;
using MatchingEngineApp;
using MatchingEngineApp.Dtos;
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
        
        private readonly MatchingEngineService _matchingEngineService;
        private readonly IUserContextService _userContextService;

        public OrdersController(MatchingEngineService matchingEngineService, IUserContextService userContextService)
        {
            _matchingEngineService = matchingEngineService;
            _userContextService = userContextService;
        }

        [HttpGet("orderbook")]
        public ActionResult<Dictionary<double, OrderBookLevel>> GetOrderBook() => Ok(_matchingEngineService.GetOrderBook());

        [HttpPost]
        public IActionResult AddOrder([FromBody] AddOrderDto addOrderDto)
        {
            var userId = (Guid)_userContextService.GetUserId!;

            if (addOrderDto.OrderType == Dtos.OrderType.MarketOrder)
            {
                _matchingEngineService.AddOrder(new MarketOrder(userId,
                                                (uint)addOrderDto.Quantity,
                                                (MatchingEngineApp.OrderType)addOrderDto.OrderSide!));
            }
            else if (addOrderDto.OrderType == Dtos.OrderType.LimitOrder)
            {
                _matchingEngineService.AddOrder(new LimitOrder(userId,
                                                (uint)addOrderDto.Quantity,
                                                addOrderDto.Price,
                                                (MatchingEngineApp.OrderType)addOrderDto.OrderSide!));
            }

            return Ok();
        }
    }
}
