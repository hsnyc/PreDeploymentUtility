using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTPre
{
    class sources
    {
        private string _sourceP;

        public string sourceP
        {
            get { return _sourceP; }
            set { _sourceP = value; }
        }

        private string _sourceFolder;

        public string sourceFolder
        {
            get { return _sourceFolder; }
            set { _sourceFolder = value; }
        }


        private string _prtPortName;
        public string prtPortName
        {
            get { return _prtPortName; }
            set { _prtPortName = value; }
        }

        private string _prtDriveName;
        public string prtDriveName
        {
            get { return _prtDriveName; }
            set { _prtDriveName = value; }
        }
    }


}
