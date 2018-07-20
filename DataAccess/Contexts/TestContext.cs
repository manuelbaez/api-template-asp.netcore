using DataAccessUtil.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBaseAccess.Contexts
{
   public class TestContext : SelfGeneratedDbContext
    {
        public TestContext(DbContextOptions options) : base(options)
        {
        }
    }
}
