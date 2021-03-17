using MediatR;
using System;

namespace Worldex.Core.Entities.Configuration.FeedConfiguration
{
    public class InvokeAPIPlanCroneObj : IRequest
    {
        public DateTime Date { get; set; }
    }
}
