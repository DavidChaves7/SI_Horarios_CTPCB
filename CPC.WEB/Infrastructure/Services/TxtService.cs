using Blazorise;
using CsvHelper;
using Microsoft.AspNetCore.Hosting.Server;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace SI_Horarios_CTPCB.Infrastructure.Services
{
    public class TxtService(IConfiguration config)
    {
        private readonly IConfiguration Config = config;
        private string? rutaDirectorio;

        public void CreateReport<T>(IEnumerable<T> data, string fileName)
        {
            rutaDirectorio = Config.GetValue<string>("CargaXmlReporteDirectory");
            int BufferSize = 8192;

            if (rutaDirectorio is null)
            {
                return;
            }
            string rutaCompletaArchivo = Path.Combine(rutaDirectorio, fileName);

            if (!Directory.Exists(rutaDirectorio))
            {
                Directory.CreateDirectory(rutaDirectorio);
            }

            //if (!File.Exists(rutaCompletaArchivo))
            //{
            //    File.Create(rutaCompletaArchivo);
            //}

            using StreamWriter writer = new(rutaCompletaArchivo);
            var tipoEntidad = data.FirstOrDefault()?.GetType();
            IEnumerable<PropertyInfo> propiedades = tipoEntidad?.GetProperties().Where(prop => prop.Name != "ExtensionData") ?? [];

            IEnumerable<string> columnNames = propiedades.Select(prop => prop.Name);
            writer.WriteLine(string.Join("|", columnNames));

            foreach (var entidad in data)
            {
                // Obtener los valores de las propiedades de la entidad, excluyendo ExtensionData
                IEnumerable<string> fields = propiedades.Select(prop => prop.GetValue(entidad)?.ToString() ?? string.Empty);

                string line = string.Join("|", fields);
                writer.WriteLine(line);

                // Si se alcanza el tamaño del búfer le hace flush
                if (writer.BaseStream.Length > BufferSize)
                {
                    writer.Flush();
                }
            }
            writer.Dispose();
        }

        public byte[] CreateZip(string zipFileName)
        {
            rutaDirectorio = Config.GetValue<string>("CargaXmlReporteDirectory");
            if (rutaDirectorio is null)
            {
                return [];
            }
            string route = Path.Combine(rutaDirectorio, zipFileName);

            if (File.Exists(route))
            {
                File.Delete(route);
            }

            using (FileStream zipFile = new FileStream(route, FileMode.Create))
            {
                using (ZipArchive zip = new ZipArchive(zipFile, ZipArchiveMode.Create))
                {
                    if (AgregarArchivoAlZip(zip, "Deudor.txt"))
                    {
                        return [];
                    }
                    if (AgregarArchivoAlZip(zip, "Activas.txt"))
                    {
                        return [];
                    }
                    if (AgregarArchivoAlZip(zip, "Historicas.txt"))
                    {
                        return [];
                    }
                }
            }
            return File.ReadAllBytes(route);
        }

        private bool AgregarArchivoAlZip(ZipArchive zip, string nombreArchivo)
        {
            rutaDirectorio = Config.GetValue<string>("CargaXmlReporteDirectory");
            if (rutaDirectorio is null)
            {
                return false;
            }
            string rutaCompletaArchivo = Path.Combine(rutaDirectorio, nombreArchivo);

            if (File.Exists(rutaCompletaArchivo))
            {
                ZipArchiveEntry entry = zip.CreateEntry(nombreArchivo);

                using (Stream stream = entry.Open())
                {
                    using (FileStream archivo = new FileStream(rutaCompletaArchivo, FileMode.Open, FileAccess.Read))
                    {
                        archivo.CopyTo(stream);
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        public static byte[] CreateReport<T>(IEnumerable<T> data)
        {
            StringBuilder FileData = new();

            IEnumerable<PropertyInfo> propiedades = typeof(T)?.GetProperties().Where(prop => prop.Name != "ExtensionData") ?? [];
            string header = string.Join("|", propiedades.Select(p => p.Name));

            FileData.AppendLine(header);

            foreach (var item in data)
            {
                string row = string.Join("|", propiedades.Select(p => p.GetValue(item)?.ToString() ?? string.Empty));
                FileData.AppendLine(row);
            }

            byte[] fileBytes = [];
            using MemoryStream ms = new();
            StreamWriter writer = new(ms, Encoding.UTF8);
            writer.Write(FileData.ToString());
            writer.Flush();
            ms.Position = 0;
            fileBytes = ms.ToArray();
            writer.Dispose();
            ms.Dispose();

            return fileBytes;
        }
    }
}
