using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace DebugLogUploadMobile
{

    class Program
    {

        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            SqlConnection con = new SqlConnection();
            try
            {
                //Inicia a conexão com o banco de dados
                con.ConnectionString = @"Data Source=.;Initial Catalog=CadastroNetAssetDb_Demo;Integrated Security=True";
                if (con.State == System.Data.ConnectionState.Closed)
                {
                    con.Open();
                }

                //Obtendo parâmetro de filtro
                String serviceOrderTaskId;
                Console.Write("Informe o parâmetro serviceOrderTaskId: ");
                serviceOrderTaskId = Console.ReadLine();

                //Query String
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT [Id],[ServiceOrderTaskId],[UserId],[Date],[Json],[IsSuccess]" +
                                " FROM dbo.LogUploadMobile log  WHERE log.ServiceOrderTaskId =" +serviceOrderTaskId+
                                " ORDER BY log.id ASC";
                
                cmd.Connection = con;                
                SqlDataReader reader = cmd.ExecuteReader();

                var filepath = @"C:\Users\roger\Documents\Debug Log UploadMobile";
                int i = 1;
                while (reader.Read())
                {
                    var payload = Regex.Unescape(reader.GetString(4)).TrimEnd('\"').TrimStart('\"');
                    RequestServiceOrderTask obj = JsonConvert.DeserializeObject<RequestServiceOrderTask>(payload);
                    if (!string.IsNullOrEmpty(obj.ItemData))
                    {

                        var path = filepath + "\\" + obj.Id + "\\Fotos\\";
                        var bytes = Convert.FromBase64String(obj.ItemData);

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        var fullFilepath = path + obj.Id + "_" + i + ".jpg";                        
                        using (var imageFile = new FileStream(fullFilepath, FileMode.Create))
                        {
                            imageFile.Write(bytes, 0, bytes.Length);
                            imageFile.Flush();
                        }
                        Console.WriteLine("Imagem salva em: "+ fullFilepath);
                    }
                    else {
                        Console.WriteLine("Enviando dados para o endpoint: /api/mobile/synchronization");
                        PostApi(payload);                        
                    }                    
                    i++;
                }

                reader.Close();
                reader.Dispose();
                con.Close();               

            }
            catch (SqlException)
            {
                Console.WriteLine("Algum erro aconteceu.");
            }
        }
        

        public async static void PostApi(string payload)
        {           

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:60231/api/mobile/synchronization");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                
                streamWriter.Write(payload);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }           

        }            
        
    }
}
