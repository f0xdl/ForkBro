using ForkBro.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForkBro.Clients
{
    class ApiClient : IApi
    {
        IApiMediator hub;
        public ApiClient(IApiMediator mediator)
        {
            hub = mediator;
        }
        //TODO подписка на добавление форка в хабе
        //Rx.NET - медленно
        //TPL - более быстрый подход
    }
}
