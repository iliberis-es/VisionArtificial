using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace VisionArtificial
{
    class Program
    {
        /*
         * Datos necesarios para el uso del servicio REST
         */

        // Esta key no es válida. Se debe generar una válida en la web de Vision Services
        const string subscriptionKey = "09e763cd572143cea8bff47db625b499"; 
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze";

        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                // Recibimos el nombre del archivo como parámetro en la consola
                string fichero = args[0];

                HttpClient cliente = new HttpClient();

                // Parámetros y cabecera de la llamada.
                cliente.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                string Parametros = "visualFeatures=Categories,Description,Color&language=en";

                string uri = uriBase + "?" + Parametros;

                HttpResponseMessage resultado;
                byte[] byteArray = null;

                // Cuerpo de la petición. Enviamos el archivo codificado como array de bytes
                try
                {
                    FileStream fileStream = new FileStream(fichero, FileMode.Open, FileAccess.Read);
                    BinaryReader binaryReader = new BinaryReader(fileStream);
                    byteArray = binaryReader.ReadBytes((int)fileStream.Length);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"** ERROR: {ex.Message}");
                    Console.ResetColor();
                }

                if (byteArray != null)
                {
                    // Ejecutamos la petición y procesamos los resultados del análisis
                    using (ByteArrayContent content = new ByteArrayContent(byteArray))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        resultado = cliente.PostAsync(uri, content).Result;

                        // Merece la pena detenerse a ver esta cadena. Contiene el resultado del análisis en formato JSON
                        string cadenaResultado = resultado.Content.ReadAsStringAsync().Result;

                        // Mostramos la descripción de lo que se ha encontrado en la imagen
                        dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(cadenaResultado);
                        Console.Write("\nResultado del análisis: ");
                        Console.WriteLine(jsonObj.description.captions[0].text);
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("** Falta parámetro requerido. Uso: VisionArtificial.exe [fichero.jpg]");
                Console.ResetColor();
            }
            Console.WriteLine("\nPulse ENTER para finalizar...");
            Console.ReadLine();
        }
    }
}
