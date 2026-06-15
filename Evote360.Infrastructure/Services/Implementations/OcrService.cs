using Evote360.Application.Services.Interfaces;
using Tesseract;
using System.IO;

namespace Evote360.Infrastructure.Services.Implementations
{
    public class OcrService : IOcrService
    {
        public string ExtraerTextoDeImagen(byte[] imagenBytes)
        {
            try
            {
                var tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tessdata");
                if (!Directory.Exists(tessDataPath))
                {
                    // Fallback to Evote360.Web/tessdata
                    tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
                }
                
                using var engine = new TesseractEngine(tessDataPath, "spa", EngineMode.Default);
                using var img = Pix.LoadFromMemory(imagenBytes);
                using var page = engine.Process(img);
                return page.GetText();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
