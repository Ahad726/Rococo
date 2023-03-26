﻿using Rococo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rococo.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
    }
}
