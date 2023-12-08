using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public static class Projects
    {
        private static object _lockObject = new object();

        private static Func<string, Project> _createV13ProjectInstance;

        private static Func<string, Project> createV13ProjectInstance
        {
            get
            {
                if (_createV13ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV13ProjectInstance == null)
                        {
                            if (_createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V13 Project when you already have had opened a V14/V15/V15_1/V17/V18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV13.dll"));
                            //var assembly = Assembly.LoadFrom("DotNetSiemensPLCToolBoxLibrary.TIAV13.dll");
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.Step7ProjectV13");
                            _createV13ProjectInstance = (file) => (Project)Activator.CreateInstance(type, new object[] { file, null });
                        }
                    }
                }
                return _createV13ProjectInstance;
            }
        }

        private static Func<string, Project> _createV14SP1ProjectInstance;

        private static Func<string, Project> createV14SP1ProjectInstance
        {
            get
            {
                if (_createV14SP1ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV14SP1ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V14 Project when you already have had opened a V13/V15/V15_1/V17/V18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV14SP1.dll"));
                            //var assembly = Assembly.LoadFrom("DotNetSiemensPLCToolBoxLibrary.TIAV14SP1.dll");
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V14SP1.Step7ProjectV14SP1");
                            _createV14SP1ProjectInstance = (file) => (Project)Activator.CreateInstance(type, new object[] { file, null });
                            _attachV14SP1ProjectInstance = () => (Project)Activator.CreateInstance(type);
                        }
                    }
                }
                return _createV14SP1ProjectInstance;
            }
        }

        private static Func<Project> _attachV14SP1ProjectInstance;

        private static Func<Project> attachV14SP1ProjectInstance
        {
            get
            {
                if (_attachV14SP1ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV14SP1ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V14 Project when you already have had opened a V13/V15/V15_1/V17/V18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV14SP1.dll"));
                            //var assembly = Assembly.LoadFrom("DotNetSiemensPLCToolBoxLibrary.TIAV14SP1.dll");
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V14SP1.Step7ProjectV14SP1");
                            _createV14SP1ProjectInstance = (file) => (Project)Activator.CreateInstance(type, new object[] { file, null });
                            _attachV14SP1ProjectInstance = () => (Project)Activator.CreateInstance(type);
                        }
                    }
                }
                return _attachV14SP1ProjectInstance;
            }
        }

        private static Func<string, Credentials, Project> _createV15ProjectInstance;

        private static Func<string, Credentials, Project> createV15ProjectInstance
        {
            get
            {
                if (_createV15ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV15ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15_1ProjectInstance != null || _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V15 Project when you already have had opened a V13/V14/V15_1/V17/V18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV15.dll"));
                            //var assembly = Assembly.LoadFrom("DotNetSiemensPLCToolBoxLibrary.TIAV15.dll");
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V15.Step7ProjectV15");
                            _createV15ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                        }
                    }
                }
                return _createV15ProjectInstance;
            }
        }

        private static Func<string, Credentials, Project> _createV15_1ProjectInstance;

        private static Func<string, Credentials, Project> createV15_1ProjectInstance
        {
            get
            {
                if (_createV15_1ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV15_1ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V15.1 Project when you already have had opened a V13/V14/V15/V17/V18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV15_1.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V15_1.Step7ProjectV15_1");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV15_1ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV15_1ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV15_1ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _createV15_1ProjectInstance;
            }
        }

        private static Func<Project> _attachV15_1ProjectInstance;

        private static Func<Project> attachV15_1ProjectInstance
        {
            get
            {
                if (_attachV15_1ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV15_1ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V15.1 Project when you already have had opened a V13/V14/V15/V17/V18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV15_1.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V15_1.Step7ProjectV15_1");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV15_1ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV15_1ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV15_1ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV15_1ProjectInstance;
            }
        }

        private static Func<string, Project> _attachV15_1ProjectInstanceWithFilename;

        private static Func<string, Project> attachV15_1ProjectInstanceWithFilename
        {
            get
            {
                if (_attachV15_1ProjectInstanceWithFilename == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV15_1ProjectInstanceWithFilename == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V15.1 Project when you already have had opened a V13/V14/V15/V17/V18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV15_1.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V15_1.Step7ProjectV15_1");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV15_1ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV15_1ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV15_1ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV15_1ProjectInstanceWithFilename;
            }
        }

        private static Func<string, Credentials, Project> _createV16ProjectInstance;

        private static Func<string, Credentials, Project> createV16ProjectInstance
        {
            get
            {
                if (_createV16ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV16ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V16 Project when you already have had opened a V13/V14/V15/17/18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV16.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V16.Step7ProjectV16");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV16ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV16ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV16ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _createV16ProjectInstance;
            }
        }

        private static Func<Project> _attachV16ProjectInstance;

        private static Func<Project> attachV16ProjectInstance
        {
            get
            {
                if (_attachV16ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV16ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V16 Project when you already have had opened a V13/V14/V15/V17/V18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV16.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V16.Step7ProjectV16");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV16ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV16ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV16ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV16ProjectInstance;
            }
        }

        private static Func<string, Project> _attachV16ProjectInstanceWithFilename;

        private static Func<string, Project> attachV16ProjectInstanceWithFilename
        {
            get
            {
                if (_attachV16ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV16ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V16 Project when you already have had opened a V13/V14/V15/V17/V18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV16.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V16.Step7ProjectV16");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV16ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV16ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV16ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV16ProjectInstanceWithFilename;
            }
        }

        private static Func<string, Credentials, Project> _createV17ProjectInstance;

        private static Func<string, Credentials, Project> createV17ProjectInstance
        {
            get
            {
                if (_createV17ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV17ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV16ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V17 Project when you already have had opened a V13/V14/V15/V16/V18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV17.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V17.Step7ProjectV17");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV17ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV17ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV17ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _createV17ProjectInstance;
            }
        }

        private static Func<Project> _attachV17ProjectInstance;

        private static Func<Project> attachV17ProjectInstance
        {
            get
            {
                if (_attachV17ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV17ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV16ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V18 Project when you already have had opened a V13/V14/V15/16/18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV17.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V17.Step7ProjectV17");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV17ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV17ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV17ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV17ProjectInstance;
            }
        }

        private static Func<string, Project> _attachV17ProjectInstanceWithFilename;

        private static Func<string, Project> attachV17ProjectInstanceWithFilename
        {
            get
            {
                if (_attachV17ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV17ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV16ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V18 Project when you already have had opened a V13/V14/V15/16/18 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV17.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V17.Step7ProjectV17");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV17ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV17ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV17ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV17ProjectInstanceWithFilename;
            }
        }

        private static Func<string, Credentials, Project> _createV18ProjectInstance;

        private static Func<string, Credentials, Project> createV18ProjectInstance
        {
            get
            {
                if (_createV18ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV18ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV16ProjectInstance != null || _createV17ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V18 Project when you already have had opened a V13/V14/V15/V16/V17 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV18.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V18.Step7ProjectV18");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV18ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV18ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV18ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _createV18ProjectInstance;
            }
        }

        private static Func<Project> _attachV18ProjectInstance;

        private static Func<Project> attachV18ProjectInstance
        {
            get
            {
                if (_attachV18ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV18ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV16ProjectInstance != null || _createV17ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V18 Project when you already have had opened a V13/V14/V15/16/17 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV18.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V18.Step7ProjectV18");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV18ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV18ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV18ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV18ProjectInstance;
            }
        }

        private static Func<string, Project> _attachV18ProjectInstanceWithFilename;

        private static Func<string, Project> attachV18ProjectInstanceWithFilename
        {
            get
            {
                if (_attachV18ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV18ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || _createV16ProjectInstance != null || _createV17ProjectInstance != null)
                            {
                                throw new Exception("You can not open a V18 Project when you already have had opened a V13/V14/V15/16/17 Project. You need to close the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV18.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V18.Step7ProjectV18");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV18ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV18ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV18ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV18ProjectInstanceWithFilename;
            }
        }

        /// <summary>
        /// This Function Returens a Step7 Project Instance for every Project Folder in the Path.
        /// </summary>
        /// <param name="dirname"></param>
        /// <returns></returns>
        public static Project[] GetStep7ProjectsFromDirectory(string dirname)
        {
            return GetStep7ProjectsFromDirectory(dirname, null);
        }

        /// <summary>
        /// This Function Returens a Step7 Project Instance for every Project Folder in the Path.
        /// </summary>
        /// <param name="dirname"></param>
        /// <returns></returns>
        public static Project[] GetStep7ProjectsFromDirectory(string dirname, Credentials credentials)
        {
            List<Project> retVal = new List<Project>();

            try
            {
                string[] fls = System.IO.Directory.GetFiles(dirname, "*.s5d");
                foreach (var fl in fls)
                {
                    retVal.Add(new Step5Project(fl, false));
                }
            }
            catch (Exception)
            {
                Console.WriteLine("1 Projects.cs threw exception");
            }
            try
            {
                foreach (string subd in System.IO.Directory.GetDirectories(dirname))
                {
                    try
                    {
                        string[] fls = System.IO.Directory.GetFiles(subd, "*.s7p");
                        if (fls.Length > 0)
                            retVal.Add(new Step7ProjectV5(fls[0], false));

                        fls = System.IO.Directory.GetFiles(subd, "*.s7l");
                        if (fls.Length > 0)
                            retVal.Add(new Step7ProjectV5(fls[0], false));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap11");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap12");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap13");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap14");
                        if (fls.Length > 0)
                            retVal.Add(createV14SP1ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap15");
                        if (fls.Length > 0)
                            retVal.Add(createV15ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap15_1");
                        if (fls.Length > 0)
                            retVal.Add(createV15_1ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap16");
                        if (fls.Length > 0)
                            retVal.Add(createV16ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap17");
                        if (fls.Length > 0)
                            retVal.Add(createV17ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap18");
                        if (fls.Length > 0)
                            retVal.Add(createV18ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.al11");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.al12");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.al13");
                        if (fls.Length > 0)
                            retVal.Add(createV13ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.al14");
                        if (fls.Length > 0)
                            retVal.Add(createV14SP1ProjectInstance(fls[0]));

                        fls = System.IO.Directory.GetFiles(subd, "*.al15");
                        if (fls.Length > 0)
                            retVal.Add(createV15ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.al15_1");
                        if (fls.Length > 0)
                            retVal.Add(createV15_1ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.al16");
                        if (fls.Length > 0)
                            retVal.Add(createV16ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.al17");
                        if (fls.Length > 0)
                            retVal.Add(createV17ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.al18");
                        if (fls.Length > 0)
                            retVal.Add(createV18ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.s5d");
                        if (fls.Length > 0)
                            retVal.Add(new Step5Project(fls[0], false));
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("2 Projects.cs threw exception");
                    }
                }

                foreach (var ending in "*.zip;*.zap13".Split(';'))
                {
                    string[] zips = System.IO.Directory.GetFiles(dirname, ending);
                    foreach (string zip in zips)
                    {
                        var zipfile = ZipHelper.GetZipHelper(zip);
                        string entr = zipfile.GetFirstZipEntryWithEnding(".s7p");
                        if (entr != null)
                            retVal.Add(new Step7ProjectV5(zip, false));

                        entr = zipfile.GetFirstZipEntryWithEnding(".s7l");
                        if (entr != null)
                            retVal.Add(new Step7ProjectV5(zip, false));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap11");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap12");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap13");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap14");
                        if (entr != null)
                            retVal.Add(createV14SP1ProjectInstance(entr));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap15");
                        if (entr != null)
                            retVal.Add(createV15ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap15_1");
                        if (entr != null)
                            retVal.Add(createV15_1ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap16");
                        if (entr != null)
                            retVal.Add(createV16ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap17");
                        if (entr != null)
                            retVal.Add(createV17ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap18");
                        if (entr != null)
                            retVal.Add(createV18ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al11");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al12");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al13");
                        if (entr != null)
                            retVal.Add(createV13ProjectInstance(entr));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al14");
                        if (entr != null)
                            retVal.Add(createV14SP1ProjectInstance(entr));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al15");
                        if (entr != null)
                            retVal.Add(createV15ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al15_1");
                        if (entr != null)
                            retVal.Add(createV15_1ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al16");
                        if (entr != null)
                            retVal.Add(createV16ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al17");
                        if (entr != null)
                            retVal.Add(createV17ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al18");
                        if (entr != null)
                            retVal.Add(createV18ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding(".s5d");
                        if (entr != null)
                            retVal.Add(new Step5Project(zipfile, zip, false));
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("3 Projects.cs threw exception");
            }

            return retVal.ToArray();
        }

        public static Project AttachProject(string tiaVersion)
        {
            if (tiaVersion == "14SP1")
                return attachV14SP1ProjectInstance();
            if (tiaVersion == "15.1")
                return attachV15_1ProjectInstance();
            if (tiaVersion == "16")
                return attachV16ProjectInstance();
            if (tiaVersion == "17")
                return attachV17ProjectInstance();
            if (tiaVersion == "18")
                return attachV18ProjectInstance();

            return null;
        }

        public static Project AttachToInstanceWithFilename(string tiaVersion, string filename)
        {
            if (tiaVersion == "15.1")
                return attachV15_1ProjectInstanceWithFilename(filename);
            if (tiaVersion == "16")
                return attachV16ProjectInstanceWithFilename(filename);
            if (tiaVersion == "17")
                return attachV17ProjectInstanceWithFilename(filename);
            if (tiaVersion == "18")
                return attachV18ProjectInstanceWithFilename(filename);

            return null;
        }

        public static Project LoadProject(string file, bool showDeleted)
        {
            return LoadProject(file, showDeleted, null);
        }

        public static Project LoadProject(string file, bool showDeleted, Credentials credentials)
        {
            if (file.ToLower().EndsWith(".s5d"))
                return new Step5Project(file, showDeleted);
            else if (file.ToLower().EndsWith(".s7p"))
            {
                var test = new Step7ProjectV5(file, showDeleted);
                System.Console.WriteLine(test);
                return new Step7ProjectV5(file, showDeleted);
            }
            else if (file.ToLower().EndsWith(".s7l"))
                return new Step7ProjectV5(file, showDeleted);
            else if (file.ToLower().EndsWith(".ap11"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".ap12"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".ap13"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".ap14"))
                return createV14SP1ProjectInstance(file);
            else if (file.ToLower().EndsWith(".ap15"))
                return createV15ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".ap15_1"))
                return createV15_1ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".ap16"))
                return createV16ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".ap17"))
                return createV17ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".ap18"))
                return createV18ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".al11"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".al12"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".al13"))
                return createV13ProjectInstance(file);
            else if (file.ToLower().EndsWith(".al14"))
                return createV14SP1ProjectInstance(file);
            else if (file.ToLower().EndsWith(".al15"))
                return createV15ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".al15_1"))
                return createV15_1ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".al16"))
                return createV16ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".al17"))
                return createV17ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".al18"))
                return createV18ProjectInstance(file, credentials);
            else
            {
                var zh = ZipHelper.GetZipHelper(file);
                if (!string.IsNullOrEmpty(zh.GetFirstZipEntryWithEnding(".s5d")))
                    return new Step5Project(file, showDeleted);
                else if (!string.IsNullOrEmpty(zh.GetFirstZipEntryWithEnding(".s7p")))
                    return new Step7ProjectV5(file, showDeleted);
                else if (!string.IsNullOrEmpty(zh.GetFirstZipEntryWithEnding(".s7l")))
                    return new Step7ProjectV5(file, showDeleted);
            }
            return null;
        }

        public static ProjectFolder LoadProjectFolder(string projectAndStructuredFolderName)
        {
            var parts = projectAndStructuredFolderName.Split('|');
            var project = parts[0];
            var folder = parts[1];

            var prj = LoadProject(project, false);

            return prj.AllFolders.FirstOrDefault(x => x.StructuredFolderName == folder);
        }
    }
}