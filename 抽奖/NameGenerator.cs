using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace 抽奖
{
    class NameGenerator
    {
        public static Dictionary<int, Person> ReadFile()
        {
            Dictionary<int, Person> dic = new Dictionary<int, Person>();
            string[] lines = System.IO.File.ReadAllLines("总名单.txt", Encoding.UTF8);
            int i = 0;
            foreach (string line in lines)
            {
                if (line != "")
                {
                    string[] data = line.Split('\t');
                    Person p = new Person(data[0], data[1], data[2]);
                    dic.Add(i++, p);
                }
            }
            return dic;
        }

        public static List<Person> ReadHittedNames()
        {
            List<Person> hitted = new List<Person>();
            try
            {
                string[] lines = System.IO.File.ReadAllLines("获奖人员名单.txt", Encoding.UTF8);
                foreach (String line in lines)
                {
                    if (line != "" && (!line.StartsWith("--")))
                    {
                        string[] data = line.Split('\t');
                        Person p = new Person(data[0], data[1], data[2]);
                        hitted.Add(p);
                    }
                }
            }
            catch (FileNotFoundException)
            { 
                
            }
            return hitted;
        }

        public static List<int> getNames(Dictionary<int, Person> dic, int count, List<int> passby)
        {
            List<int> ret = new List<int>();
            Random rd = new Random();
            int r;
            while (true)
            {
                r = rd.Next(dic.Count);
                if (passby.Contains(r) || ret.Contains(r)) {
                    continue;
                }
                ret.Add(r);
                if (ret.Count == count)
                    break;
            }
            return ret;
        }
    }
}
