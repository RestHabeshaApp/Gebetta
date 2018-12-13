using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NServiceBus;

namespace StandardMVC.MessagingBus
{
    [Serializable]
    public class DoSomething: ICommand
    {
        [JsonProperty(Order =1)]
        public int SomeId { get; set; }

        [JsonProperty(Order =2)]
        public ChildClass ChildStuff { get; set; }

        public List<ChildClass> ListOfStuff { get; set; } = new List<ChildClass>();

    }


    public class ChildClass{

        public string SomeProperty { get; set; }
    }
}
