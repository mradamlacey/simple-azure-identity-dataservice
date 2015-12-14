using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using DataServices.SimpleAzureIdentityDataService.Models;

namespace DataServices.SimpleAzureIdentityDataService.Repositories
{
    public class EimDataRepository
    {

        public Task<int> SubmitNewProperty(Property property)
        {
            var taskSource = new TaskCompletionSource<int>();
            taskSource.SetResult(0);

            return taskSource.Task;
        }

        public Task<int> SubmitUpdateToProperty(Property property)
        {
            var taskSource = new TaskCompletionSource<int>();
            taskSource.SetResult(0);

            return taskSource.Task;
        }

        public EimDataRepository()
        {

        }
    }
}