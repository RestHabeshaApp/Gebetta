using System;
using System.Collections.Generic;
using NServiceBus;

namespace StandardMVC.MessagingBus
{
    public class DoSomethingElse: ICommand
    {
        public int SomeId { get; set; }

        public ChildClass ChildStuff { get; set; }

        public List<ChildClass> ListOfStuff { get; set; } = new List<ChildClass>();


        public DoSomethingElse()
        {
        }
    }
}
