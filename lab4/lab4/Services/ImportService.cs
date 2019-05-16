using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using lab4.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;

namespace lab4.Services
{
    public class ImportService : ISourceService
    {
        private string connString = "Host=db.mirvoda.com;Port=5454;Username=developer;Password=rtfP@ssw0rd;Database=DashaSycheva";
        private string localhost = "";
        private string url = "https://raw.githubusercontent.com/SergeyMirvoda/IR-2019/master/data/IMDB%20Movie%20Titles.csv";
        
        public async ValueTask<bool> SetDatabase()
        {

            try
            {
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
                HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream());
                var data = sr.ReadToEnd();
                var array = data.Split(
                    new[] {Environment.NewLine},
                    StringSplitOptions.None
                );

                array = array.Skip(1).ToArray();
                if (array[array.Length - 1] == "")
                    Array.Resize(ref array, array.Length - 1);
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    foreach (var element in array)
                    {
                        //Somewhat Parse String
                        var id = element.Substring(0, element.IndexOf(",")).TrimStart('0');
                        var comma_index = element.IndexOf(",");
                        var yearFinder = new Regex(@".+\((\d{4})\)");
                        var title = element.Substring(comma_index + 1);
                        var match = yearFinder.Match(title);
                        var year = 0;
                        if (match.Success)
                            year = int.Parse(match.Groups[1].Value);
                        var brace_index = title.IndexOf(" (");
                        var name = brace_index == -1 ? title : title.Substring(0, brace_index);

                        //Execute query
                        var statement = "INSERT INTO table_name(id, name, year) VALUES (" + id + ", " + "\'" + name +
                                        "\'" + ", " + year + ") ON CONFLICT (id) DO NOTHING";
                        var command = new NpgsqlCommand(statement, conn);
                        command.ExecuteNonQuery();
                    }

                    conn.Close();

                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}