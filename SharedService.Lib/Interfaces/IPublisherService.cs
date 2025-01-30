using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedService.Lib.Interfaces
{
    public interface IPublisherService
    {
        public Task PublishMessageAsync(string topic, object message, string subject = "");
    }
}
