using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lab4.Interfaces;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Npgsql;

namespace lab4.Services
{
    public class LuceneService : ILuceneService
    {
        private static LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

        // Ensures index backwards compatibility (I guess)
        private static String indexLocation = System.IO.Directory.GetCurrentDirectory();
        private static FSDirectory dir = FSDirectory.Open(indexLocation);
        //create an analyzer to process the text
        private static StandardAnalyzer analyzer = new StandardAnalyzer(AppLuceneVersion);

        //create an index writer
        private static IndexWriterConfig indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
        private static IndexWriter writer = new IndexWriter(dir, indexConfig);
     
        private string connectionString = "Host=db.mirvoda.com;Port=5454;Username=developer;Password=rtfP@ssw0rd;Database=DashaSycheva";
        private IndexSearcher searcher = new IndexSearcher(writer.GetReader(applyAllDeletes: false));


        public async ValueTask<IEnumerable<string>> Search(string text, bool viaLucene)
        {;
            var query = text;
            var array = query.Split(' ').ToList();
            var res_list = new List<string>();

            
                if (!viaLucene) 
                    using (var conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();
                        var statement = "";

                        //Поиск по точному названию
                        statement = "SELECT * " +
                                "FROM table_name " +
                                "WHERE name = \'" + query + "\'";
                        var command = new NpgsqlCommand(statement, conn);
                        int id;
                        var year = 0;
                        var name = "";
                        var counter = 0;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read() && counter < 10)
                            {
                                id = reader.GetInt32(0);
                                year = reader.GetInt32(2);
                                name = reader.GetString(1);
                                counter += 1;
                                res_list.Add("ID: " + id.ToString() + " YEAR: " + year.ToString() + " NAME: " + name);
                            }
                        }

                        //Поиск по году и по названию  //, если предыдущий ничего не дал
                        
                        //Ищем год в запросе
                        string year_to_find = "";
                        int number = 0;
                        foreach (var word in array) {
                            bool result = Int32.TryParse(word, out number);
                            if (result && number > 1800 && number <= 9999) {
                                year_to_find = word;
                                array.RemoveAt(array.IndexOf(word));
                                break;
                            }

                            number = 0;
                        }

                        //Если нашли
                        if (number != 0)
                            foreach (var word in array)
                            {
                                if (!String.IsNullOrEmpty(word))
                                {
                                    statement = "SELECT * " +
                                        "FROM table_name " +
                                        "WHERE year = " + year_to_find + " AND name ILIKE \'%" + word + "%\' ";
                                    command = new NpgsqlCommand(statement, conn);
                                    using (var reader = command.ExecuteReader())
                                    {
                                        while (reader.Read() && counter < 10)
                                        {
                                            counter += 1;
                                            id = reader.GetInt32(0);
                                            year = reader.GetInt32(2);
                                            name = reader.GetString(1);
                                            res_list.Add("ID: " + id.ToString() + " YEAR: " + year.ToString() + " NAME: " + name);
                                        }
                                    }
                                }
                            }

                        foreach (var word in array)
                        {
                            if (!String.IsNullOrEmpty(word))
                            {
                                statement = "SELECT * " +
                                "FROM table_name " +
                                "WHERE name ILIKE \'" + word + " %\' " +
                                    "OR name = \'" + word + "\' " +
                                    "OR  name ILIKE \'% " + word + "\'";
                                command = new NpgsqlCommand(statement, conn);
                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read() && counter < 10)
                                    {
                                        counter += 1;
                                        id = reader.GetInt32(0);
                                        year = reader.GetInt32(2);
                                        name = reader.GetString(1);
                                        res_list.Add("ID: " + id.ToString() + " YEAR: " + year.ToString() + " NAME: " + name);
                                    }
                                }
                            }
                        }

                        foreach (var word in array)
                        {
                            if (!String.IsNullOrEmpty(word))
                            {
                                statement = "SELECT * " +
                                "FROM table_name " +
                                "WHERE name ILIKE \'%" + word + "%\' ";
                                command = new NpgsqlCommand(statement, conn);
                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read() && counter < 10)
                                    {
                                        counter += 1;
                                        id = reader.GetInt32(0);
                                        year = reader.GetInt32(2);
                                        name = reader.GetString(1);
                                        res_list.Add("ID: " + id.ToString() + " YEAR: " + year.ToString() + " NAME: " + name);
                                    }
                                }
                            }
                        }

                        res_list = res_list.Select(x => x).Distinct().ToList();
                        conn.Close();
                    }
                else
                {
                    UpsertWriter();
                    QueryParser parser = new QueryParser(AppLuceneVersion, "name", analyzer);
                    var phrase = new MultiPhraseQuery();
                    foreach (var word in array)
                    {
                        var q = parser.Parse(query);
                        if (!string.IsNullOrEmpty(word))
                        {
                            var res = searcher.Search(q, 10).ScoreDocs;
                            foreach (var hit in res)
                            {
                                var foundDoc = searcher.Doc(hit.Doc);
                                var score = hit.Score;
                                res_list.Add("Score: " + score + " ID: " + foundDoc.GetField("id").GetInt32Value() +
                                    " YEAR: " + foundDoc.GetField("year").GetInt32Value() + " NAME: " + foundDoc.GetValues("name")[0]);
                            }
                        }
                    }

                    //Ищем полное название
                    phrase.Add(new Term("name", query));
                    var hits = searcher.Search(phrase, 10).ScoreDocs;
                    foreach (var hit in hits)
                    {
                        var foundDoc = searcher.Doc(hit.Doc);
                        var score = hit.Score;
                        res_list.Add("Score: " + score + " ID: " + foundDoc.GetField("id").GetInt32Value().ToString() +
                            " YEAR: " + foundDoc.GetField("year").GetInt32Value().ToString() + " NAME: " + foundDoc.GetValues("name")[0]);
                    }

                    //Ищем части слов
                    foreach (var word in array)
                    {
                        if (!string.IsNullOrEmpty(word))
                        {
                            var wild = new WildcardQuery(new Term("name", word));
                            var res = searcher.Search(wild, 10).ScoreDocs;
                            foreach (var hit in res)
                            {
                                var foundDoc = searcher.Doc(hit.Doc);
                                var score = hit.Score;
                                res_list.Add("Score: " + score + " ID: " + foundDoc.GetField("id").GetInt32Value().ToString() +
                                    " YEAR: " + foundDoc.GetField("year").GetInt32Value().ToString() + " NAME: " + foundDoc.GetValues("name")[0]);
                            }
                        }
                    }

                    //Ищем год и часть слова
                    string year_to_find = "";
                    int number = 0;
                    foreach (var word in array)
                    {
                        bool result = Int32.TryParse(word, out number);
                        if (result && number > 1800 && number <= 9999)
                        {
                            year_to_find = word;
                            array.RemoveAt(array.IndexOf(word));
                            break;
                        }
                        else number = 0;
                    }

                    //Если нашли
                    if (number != 0)
                    {
                        phrase = new MultiPhraseQuery();
                        foreach (var word in array)
                        {
                            if (!string.IsNullOrEmpty(word))
                            {
                                BooleanQuery booleanQuery = new BooleanQuery();
                                var wild = new WildcardQuery(new Term("name", word));
                                var num = NumericRangeQuery.NewInt32Range("year", 1, number, number, true, true);
                                booleanQuery.Add(wild, Occur.MUST);
                                booleanQuery.Add(num, Occur.MUST);
                                var res = searcher.Search(booleanQuery, 10).ScoreDocs;
                                foreach (var hit in res)
                                {
                                    var foundDoc = searcher.Doc(hit.Doc);
                                    var score = hit.Score;
                                    res_list.Add("Score: " + score + " ID: " + foundDoc.GetField("id").GetInt32Value().ToString() +
                                        " YEAR: " + foundDoc.GetField("year").GetInt32Value().ToString() + " NAME: " + foundDoc.GetValues("name")[0]);
                                }
                            }
                        }
                    }
                }

                return res_list;

            
            
        }


        private void UpsertWriter()
        {
            writer.DeleteAll();
            writer.Flush(triggerMerge: true, applyAllDeletes:true);
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    var statement = "";
                    var id = 0;
                    var year = 0;
                    var name = "";

                    statement = "SELECT * FROM table_name";
                    var command = new NpgsqlCommand(statement, conn);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            id = reader.GetInt32(0);
                            year = reader.GetInt32(2);
                            name = reader.GetString(1);
                            var source = new
                            {
                                id = id,
                                name = name,
                                year = year
                            };
                            var doc = new Document();
                            doc.Add(new TextField("name", source.name, Field.Store.YES));
                            doc.Add(new StoredField("id", source.id));
                            doc.Add(new Int32Field("year", source.year, Field.Store.YES));
                            writer.AddDocument(doc);
                        }
                    }
                }
                writer.Flush(triggerMerge: false, applyAllDeletes: false);
                writer.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}