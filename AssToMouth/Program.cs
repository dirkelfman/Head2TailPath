using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace AssToMouth
{
    class MainClass
    {
        NetSpell.SpellChecker.Dictionary.WordDictionary _wordDict;
        NetSpell.SpellChecker.Spelling _spelling;
        public static void Main(string[] args)
        {
            var mc = new MainClass();

                
        }
        public MainClass()
        {
            _wordDict = new NetSpell.SpellChecker.Dictionary.WordDictionary();
            _wordDict.Initialize();
            _spelling = new NetSpell.SpellChecker.Spelling();
            var ass = "head";
            var mouth = "tail";
            _spelling.Dictionary = _wordDict;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var set = GetComboAnagrams(ass, mouth);
            sw.Stop();
            Console.WriteLine($"found {set.Count} words in {sw.ElapsedMilliseconds} mili");
            sw.Reset();
            sw.Start();
            var g = GetNearestWordGraph(set.ToList());
            var path = g.shortest_path(ass, mouth);
            //add the mouth
            path.Add(mouth);
            Console.WriteLine($"found {string.Join("-->",path)} in {sw.ElapsedMilliseconds} mili");
            int f = 0;
        }

        Graph GetNearestWordGraph ( List<string> words)
        {
            
            var g = new Graph();

            for (int i = 0; i < words.Count;i++)
            {
                
                var word = words[i];
                var edges = words.Where(x => OneDiff(x, word)).ToDictionary((_ => _), (_ => 1));
                g.add_vertex(word,edges);
            }
            return g;
        }

        bool OneDiff ( string word1 , string word2)
        {
            bool flg = false;
            if ( word1 == word2 )
            {
                return false;
            }
            for (int i = 0; i < word1.Length;i++)
            {
                if ( word1[i] != word2[i]  )
                {
                    if (flg)
                    {
                        return false;
                    }
                    else
                    {
                        flg = true;
                    }

                }

            }
            return true;
        }




        HashSet<string> GetComboAnagrams ( string word1, string word2)
        {
            var words = new HashSet<string>();
            var combo = word1 + word2;
            var targetLen = word1.Length;
            for (int i = 0; i < combo.Length;i++)
            {
                for (int j = 0; j < combo.Length;j++)
                {
                    for (int k = 0; k < combo.Length; k++)
                    {
                        for (int l = 0; l < combo.Length; l++)
                        {
                            var word = new string(new char[] { combo[i], combo[j], combo[k], combo[l] });
                            if ( !words.Contains(word) && IsWord(word))
                            {
                                words.Add(word);
                            }
                        }
                    }    
                }
            }
            return words;

        }


        private bool IsWord(string wordToCheck)
        {
            return _spelling.TestWord(wordToCheck);

        }

    }




    class Graph
    {
        Dictionary<string, Dictionary<string, int>> vertices = new Dictionary<string, Dictionary<string, int>>();

        public void add_vertex(string name, Dictionary<string, int> edges)
        {
            vertices[name] = edges;
        }

        public List<string> shortest_path(string start, string finish)
        {
            var previous = new Dictionary<string, string>();
            var distances = new Dictionary<string, int>();
            var nodes = new List<string>();

            List<string> path = null;

            foreach (var vertex in vertices)
            {
                if (vertex.Key == start)
                {
                    distances[vertex.Key] = 0;
                }
                else
                {
                    distances[vertex.Key] = int.MaxValue;
                }

                nodes.Add(vertex.Key);
            }

            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => distances[x] - distances[y]);

                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest == finish)
                {
                    path = new List<string>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == int.MaxValue)
                {
                    break;
                }

                foreach (var neighbor in vertices[smallest])
                {
                    var alt = distances[smallest] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }

            return path;
        }
    }
}
