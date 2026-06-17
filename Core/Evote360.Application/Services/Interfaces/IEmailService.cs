using System.Threading.Tasks;

namespace Evote360.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpo);
    }
}
