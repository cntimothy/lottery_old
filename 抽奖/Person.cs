using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 抽奖
{
    class Person
    {
        private String id;
        private String name;
        private String work;

        public Person(String id, String name, String work)
        {
            this.id = id;
            this.name = name;
            this.work = work;
        }

        public override String ToString()
        {
            return id + "\t" + name + "\t" + work;
        }

        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj.ToString());
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public String Id
        {
            get { return id; }
            set { id = value; }
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public String Work
        {
            get { return work; }
            set { work = value; }
        }
    }
}
