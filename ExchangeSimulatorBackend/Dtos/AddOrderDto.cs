using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ExchangeSimulatorBackend.Dtos
{
    public enum OrderType
    {
        LimitOrder,
        MarketOrder
    }

    public enum OrderSide
    {
        BUY,
        SELL
    }

    public class AddOrderDto
    {
        public OrderType? OrderType { get; set; }
        public OrderSide? OrderSide { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }


    public class AddOrderDtoValidator : AbstractValidator<AddOrderDto>
    {
        public AddOrderDtoValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;


            RuleFor(x => x.OrderType).IsInEnum();
            RuleFor(x => x.OrderSide).IsInEnum();

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .When(x => x.OrderType == OrderType.LimitOrder)
                .WithMessage("invalid price, must be grather than 0");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("invalid quantity, must be grather than 0");


            //var orderTypes = new List<string>() { "limit", "market" };
            //var orderSides = new List<string>() { "buy", "sell" };



            //RuleFor(x => x.Quantity)
            //    .NotNull()
            //    .GreaterThan(0)
            //    .WithMessage("invalid quantity, quantity must be greather than 0");


            //RuleFor(x => x.OrderType)
            //    .NotEmpty()
            //    .Must(x => orderTypes.Contains(x))
            //    .WithMessage("orderType must be one of: " + String.Join(", ", orderTypes));

            //RuleFor(x => x.Side)
            //    .Must(x => orderSides.Contains(x))
            //    .WithMessage("side must be one of: " + String.Join(", ", orderSides));

            //RuleFor(x=>x.Price)
            //    .Matches(@"^[+]?([1-9]+\.?[0-9]*|\.[-9]+)$")
            //    .WithMessage("Price must be number grather than 0");


            //.GreaterThan(0)
            //.WithMessage("Price cannot be smaller or equal 0")
            //.When(x=>x.OrderType =="limit")
        }
    }
}
