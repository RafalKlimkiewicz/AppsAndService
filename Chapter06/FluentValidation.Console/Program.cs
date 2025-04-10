using System.Globalization;
using System.Text;

using FluentValidation.Results;
using FluentValidation.Validators; 

OutputEncoding = Encoding.UTF8; 

Thread t = Thread.CurrentThread;
t.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
t.CurrentUICulture = t.CurrentCulture;
WriteLine($"Current culture: {t.CurrentCulture.DisplayName}");
WriteLine();

Order order = new()
{
    //OrderId = 10001,
    //CustomerName = "Abcdef",
    //CustomerEmail = "abc@example.com",
    //CustomerLevel = CustomerLevel.Gold,
    //OrderDate = new(2022, month: 12, day: 1),
    //ShipDate = new(2022, month: 12, day: 5),
    //// CustomerLevel is Gold so Total can be >20.
    //Total = 49.99M
};

var validator = new OrderValidator();

var result = validator.Validate(order);

// Output the order data.
WriteLine($"CustomerName:  {order.CustomerName}");
WriteLine($"CustomerEmail: {order.CustomerEmail}");
WriteLine($"CustomerLevel: {order.CustomerLevel}");
WriteLine($"OrderId:       {order.OrderId}");
WriteLine($"OrderDate:     {order.OrderDate}");
WriteLine($"ShipDate:      {order.ShipDate}");
WriteLine($"Total:         {order.Total:C}");
WriteLine();

// Output if the order is valid and any rules that were broken.
WriteLine($"IsValid:  {result.IsValid}");
foreach (var item in result.Errors)
{
    WriteLine($"  {item.Severity}: {item.ErrorMessage}");
}
