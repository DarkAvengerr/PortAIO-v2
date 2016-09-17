using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Net;
using System.IO;
using System.Reflection;

namespace DevCommom
{
    public class AssemblyUtil
    {
        private OnGetVersionCompletedArgs versionCompletedArgs;

        public delegate void OnGetVersionCompleted(OnGetVersionCompletedArgs args);
        public event OnGetVersionCompleted onGetVersionCompleted;

        private WebRequest webRequest;
        private WebRequest webRequestCommom;

        private string AssemblyName;


        public AssemblyUtil(string pAssemblyName)
        {
        }
        
        public void GetLastVersionAsync()
        {
        }

        void FinishWebRequest(IAsyncResult result)
        {
        }

        void FinishWebRequestCommom(IAsyncResult result)
        {
        }

        private string GetVersionFromAssemblyInfo(string body)
        {
            return "";
        }

    }

    public class OnGetVersionCompletedArgs : EventArgs
    {
        public string LastAssemblyVersion;
        public string LastCommomVersion;

        public string CurrentCommomVersion;
    }
}
