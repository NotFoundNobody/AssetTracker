using AssetTracker.Data;
using Core.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class PersonService(AppDbContext context)  : IPersonService
    {

    }
}
