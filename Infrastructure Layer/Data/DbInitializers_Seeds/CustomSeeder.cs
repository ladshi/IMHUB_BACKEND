using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Infrastructure.Data.DbInitializers_Seeds
{
    public interface ICustomSeeder
    {
        Task InitializeAsync();
    }
}
