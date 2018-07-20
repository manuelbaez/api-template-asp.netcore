using DataAccessUtil.Entity.General;
using DataBaseAccess.Contexts;

namespace DataBaseAccess.Entities
{
    public class TestEntity : Entity<TestEntity, TestContext>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

    }
}
