using Evote360.Core.Enums;

namespace Evote360.Core.Common
{
    public abstract class BaseEntity
    {
        public virtual int Id { get; set; }
        public virtual EstadoEnum Estado { get; set; } = EstadoEnum.Activo;
    }
}
