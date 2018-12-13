using System;
using NServiceBus;

namespace StandardMVC.MessagingBus
{
    public class PlaceOrder: ICommand
    {
        public string OrderId { get; set; }
    }
}
