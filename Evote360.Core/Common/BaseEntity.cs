namespace Evote360.Core.Common
{
    public abstract class BaseEntity
    {
        public virtual int Id { get; set; }
        public virtual bool Estado { get; set; } = true;
    }
}
