using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace StandardMVC.MessagingBus
{
    public class DoSomethingHandler : IHandleMessages<DoSomething>, IHandleMessages<DoSomethingElse>
    {
        

        public Task Handle(DoSomething message, IMessageHandlerContext context)
        {

            Console.WriteLine(message.SomeId);

            return Task.CompletedTask;
        }




        public Task Handle(DoSomethingElse message, IMessageHandlerContext context)
        {
            return Task.CompletedTask;
        }
    }


    public class PlaceHolderHandler : IHandleMessages<PlaceOrder>
    {
        static ILog log = LogManager.GetLogger<PlaceOrder>();




        public Task Handle(PlaceOrder message, IMessageHandlerContext context)
        {

            var command = new MessagingBus.DoSomething
             {
                 SomeId = 23
             };    



            Console.WriteLine(message.OrderId);
            log.Info($"Received PlaceOrder, OrderId = {message.OrderId}");
            context.SendLocal(command).ConfigureAwait(false);

            return Task.CompletedTask;
        }




    }


}
