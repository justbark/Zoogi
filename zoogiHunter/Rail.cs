using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zoogiHunter
{
    public class Rail
    {
        #region Vars

        private string _name;
        private List<Tuple<int, int, int>> _path;

        #endregion Vars

        #region Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public List<Tuple<int, int,int>> Path
        {
            get { return _path; }
            set { _path = value; }
        }

        #endregion Properties

        #region Constructs

        public Rail()
        {
            //parameterless constructor for XML serialization
        }

        public Rail(string Name, uint RunebookID, int RuneNumber)
        {
            this.Name = Name;
            this.Path = new List<Tuple<int, int, int>>();
        }

        #endregion Constructs
    }
}
