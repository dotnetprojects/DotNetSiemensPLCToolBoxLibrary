using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.General;

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
                                throw new Exception("You cannot open a project in V13 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V14 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V14 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V15 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V15.1 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V15.1 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V15.1 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V16 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V16 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V16 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V17 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V17 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V17 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V18 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V18 if you already have a project open in another version. You need to restart the Application!");
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
                                throw new Exception("You cannot open a project in V18 if you already have a project open in another version. You need to restart the Application!");
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

        private static Func<string, Credentials, Project> _createV19ProjectInstance;

        private static Func<string, Credentials, Project> createV19ProjectInstance
        {
            get
            {
                if (_createV19ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV19ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || 
                                _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You cannot open a project in V19 if you already have a project open in another version. You need to restart the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV19.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V19.Step7ProjectV19");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV19ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV19ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV19ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _createV19ProjectInstance;
            }
        }

        private static Func<Project> _attachV19ProjectInstance;

        private static Func<Project> attachV19ProjectInstance
        {
            get
            {
                if (_attachV19ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV19ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null || 
                                _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You cannot open a project in V19 if you already have a project open in another version. You need to restart the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV19.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V19.Step7ProjectV19");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV19ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV19ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV19ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV19ProjectInstance;
            }
        }

        private static Func<string, Project> _attachV19ProjectInstanceWithFilename;

        private static Func<string, Project> attachV19ProjectInstanceWithFilename
        {
            get
            {
                if (_attachV19ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV19ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null ||
                                _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You cannot open a project in V19 if you already have a project open in another version. You need to restart the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV19.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V19.Step7ProjectV19");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV19ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV19ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV19ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV19ProjectInstanceWithFilename;
            }
        }

        private static Func<string, Credentials, Project> _createV20ProjectInstance;

        private static Func<string, Credentials, Project> createV20ProjectInstance
        {
            get
            {
                if (_createV20ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV20ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null ||
                                _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You cannot open a project in V20 if you already have a project open in another version. You need to restart the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV20.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V20.Step7ProjectV20");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV20ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV20ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV20ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _createV20ProjectInstance;
            }
        }

        private static Func<Project> _attachV20ProjectInstance;

        private static Func<Project> attachV20ProjectInstance
        {
            get
            {
                if (_attachV20ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV20ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null ||
                                _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You cannot open a project in V20 if you already have a project open in another version. You need to restart the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV20.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V20.Step7ProjectV20");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV20ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV20ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV20ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV20ProjectInstance;
            }
        }

        private static Func<string, Project> _attachV20ProjectInstanceWithFilename;

        private static Func<string, Project> attachV20ProjectInstanceWithFilename
        {
            get
            {
                if (_attachV20ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV20ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null ||
                                _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null)
                            {
                                throw new Exception("You cannot open a project in V20 if you already have a project open in another version. You need to restart the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV20.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V20.Step7ProjectV20");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV20ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV20ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV20ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV20ProjectInstanceWithFilename;
            }
        }

        private static Func<string, Credentials, Project> _createV21ProjectInstance;

        private static Func<string, Credentials, Project> createV21ProjectInstance
        {
            get
            {
                if (_createV21ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_createV21ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null ||
                                _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null || _createV19ProjectInstance != null || _createV20ProjectInstance != null)
                            {
                                throw new Exception("You cannot open a project in V21 if you already have a project open in another version. You need to restart the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV21.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V21.Step7ProjectV21");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV21ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV21ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV21ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _createV21ProjectInstance;
            }
        }

        private static Func<Project> _attachV21ProjectInstance;

        private static Func<Project> attachV21ProjectInstance
        {
            get
            {
                if (_attachV21ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV21ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null ||
                                _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null || _createV19ProjectInstance != null || _createV20ProjectInstance != null)
                            {
                                throw new Exception("You cannot open a project in V21 if you already have a project open in another version. You need to restart the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV21.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V21.Step7ProjectV21");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV21ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV21ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV21ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV21ProjectInstance;
            }
        }

        private static Func<string, Project> _attachV21ProjectInstanceWithFilename;

        private static Func<string, Project> attachV21ProjectInstanceWithFilename
        {
            get
            {
                if (_attachV21ProjectInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_attachV21ProjectInstance == null)
                        {
                            if (_createV13ProjectInstance != null || _createV14SP1ProjectInstance != null || _createV15ProjectInstance != null || _createV15_1ProjectInstance != null ||
                                _createV16ProjectInstance != null || _createV17ProjectInstance != null || _createV18ProjectInstance != null || _createV19ProjectInstance != null || _createV20ProjectInstance != null)
                            {
                                throw new Exception("You cannot open a project in V21 if you already have a project open in another version. You need to restart the Application!");
                            }
                            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "";
                            var assembly = Assembly.LoadFrom(Path.Combine(path, "DotNetSiemensPLCToolBoxLibrary.TIAV21.dll"));
                            var type = assembly.GetType("DotNetSiemensPLCToolBoxLibrary.Projectfiles.V21.Step7ProjectV21");
                            var mth = type.GetMethod("AttachToInstanceWithFilename");
                            _createV21ProjectInstance = (file, credentials) => (Project)Activator.CreateInstance(type, new object[] { file, null, credentials });
                            _attachV21ProjectInstance = () => (Project)Activator.CreateInstance(type);
                            _attachV21ProjectInstanceWithFilename = (file) => (Project)mth.Invoke(null, new object[] { file });
                        }
                    }
                }
                return _attachV21ProjectInstanceWithFilename;
            }
        }

        /// <summary>
        /// This Function Returens a Step7 Project Instance for every Project Folder in the Path.
        /// </summary>
        /// <param name="dirname"></param>
        /// <returns></returns>
        static public Project[] GetStep7ProjectsFromDirectory(string dirname)
        {
            return GetStep7ProjectsFromDirectory(dirname, null);
        }

        /// <summary>
        /// This Function Returens a Step7 Project Instance for every Project Folder in the Path.
        /// </summary>
        /// <param name="dirname"></param>
        /// <returns></returns>
        static public Project[] GetStep7ProjectsFromDirectory(string dirname, Credentials credentials)
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

                        fls = System.IO.Directory.GetFiles(subd, "*.ap19");
                        if (fls.Length > 0)
                            retVal.Add(createV19ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap20");
                        if (fls.Length > 0)
                            retVal.Add(createV20ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.ap21");
                        if (fls.Length > 0)
                            retVal.Add(createV21ProjectInstance(fls[0], credentials));

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

                        fls = System.IO.Directory.GetFiles(subd, "*.al19");
                        if (fls.Length > 0)
                            retVal.Add(createV19ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.al20");
                        if (fls.Length > 0)
                            retVal.Add(createV20ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.al21");
                        if (fls.Length > 0)
                            retVal.Add(createV21ProjectInstance(fls[0], credentials));

                        fls = System.IO.Directory.GetFiles(subd, "*.s5d");
                        if (fls.Length > 0)
                            retVal.Add(new Step5Project(fls[0], false));
                    }
                    catch (Exception)
                    { }
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

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap19");
                        if (entr != null)
                            retVal.Add(createV19ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap20");
                        if (entr != null)
                            retVal.Add(createV20ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.ap21");
                        if (entr != null)
                            retVal.Add(createV21ProjectInstance(entr, credentials));

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

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al19");
                        if (entr != null)
                            retVal.Add(createV19ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al20");
                        if (entr != null)
                            retVal.Add(createV20ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding("*.al21");
                        if (entr != null)
                            retVal.Add(createV21ProjectInstance(entr, credentials));

                        entr = zipfile.GetFirstZipEntryWithEnding(".s5d");
                        if (entr != null)
                            retVal.Add(new Step5Project(zipfile, zip, false));
                    }
                }
            }
            catch (Exception)
            { }

            return retVal.ToArray();
        }

        static public Project AttachProject(string tiaVersion)
        {
            if (tiaVersion == "14SP1")
                return attachV14SP1ProjectInstance();
            else if (tiaVersion == "15.1")
                return attachV15_1ProjectInstance();
            else if (tiaVersion == "16")
                return attachV16ProjectInstance();
            else if (tiaVersion == "17")
                return attachV17ProjectInstance();
            else if (tiaVersion == "18")
                return attachV18ProjectInstance();
            else if (tiaVersion == "19")
                return attachV19ProjectInstance();
            else if (tiaVersion == "20")
                return attachV20ProjectInstance();
            else if (tiaVersion == "21")
                return attachV21ProjectInstance();

            return null;
        }

        static public Project AttachToInstanceWithFilename(string tiaVersion, string filename)
        {
            if (tiaVersion == "15.1")
                return attachV15_1ProjectInstanceWithFilename(filename);
            if (tiaVersion == "16")
                return attachV16ProjectInstanceWithFilename(filename);
            if (tiaVersion == "17")
                return attachV17ProjectInstanceWithFilename(filename);
            if (tiaVersion == "18")
                return attachV18ProjectInstanceWithFilename(filename);
            if (tiaVersion == "19")
                return attachV19ProjectInstanceWithFilename(filename);
            if (tiaVersion == "20")
                return attachV20ProjectInstanceWithFilename(filename);
            if (tiaVersion == "21")
                return attachV21ProjectInstanceWithFilename(filename);

            return null;
        }

        static public Project LoadProject(string file, bool showDeleted)
        {
            return LoadProject(file, showDeleted, null);
        }

        static public Project LoadProject(string file, bool showDeleted, Credentials credentials)
        {
            if (file.ToLower().EndsWith(".s5d"))
                return new Step5Project(file, showDeleted);
            else if (file.ToLower().EndsWith(".s7p"))
                return new Step7ProjectV5(file, showDeleted);
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
            else if (file.ToLower().EndsWith(".ap19"))
                return createV19ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".ap20"))
                return createV20ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".ap21"))
                return createV21ProjectInstance(file, credentials);
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
            else if (file.ToLower().EndsWith(".al19"))
                return createV19ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".al20"))
                return createV20ProjectInstance(file, credentials);
            else if (file.ToLower().EndsWith(".al21"))
                return createV21ProjectInstance(file, credentials);
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
