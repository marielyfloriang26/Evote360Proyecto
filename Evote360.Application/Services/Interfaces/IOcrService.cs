namespace Evote360.Application.Services.Interfaces
{
    public interface IOcrService
    {
        string ExtraerTextoDeImagen(byte[] imagenBytes);
    }
}
