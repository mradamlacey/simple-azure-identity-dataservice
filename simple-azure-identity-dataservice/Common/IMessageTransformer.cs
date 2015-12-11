using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.SimpleAzureIdentityDataService.Common
{
    public interface IMessageTransformer
    {
        bool CanTransform(HttpResponseMessage response);

        HttpResponseMessage Transform(HttpResponseMessage response);
    }
}
