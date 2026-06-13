using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Evote360.Web.Helpers;

public static class UploadFile
{
    public static string Upload(IFormFile file, int id, string folderName, bool isEditMode = false, string imagePath = "")
    {
        // Si esta editando y el usuario no subio un archivo nuevo, retorna la ruta de la imagen vieja
        if (isEditMode && file == null)
        {
            return imagePath;
        }

        if (file == null || file.Length == 0)
        {
            return string.Empty;
        }

        // Define la estructura de carpetas 
        string basePath = $"/images/{folderName}/{id}";
        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + basePath.Replace('/', '\\'));

        // Crea el directorio si no existe en el servidor
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // Genera un id unico para el archivo
        Guid guid = Guid.NewGuid();
        FileInfo fileInfo = new FileInfo(file.FileName);
        string fileName = guid + fileInfo.Extension;

        string fullFilePath = Path.Combine(path, fileName);

        using (var stream = new FileStream(fullFilePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }
        
        return $"{basePath}/{fileName}";
    }
}