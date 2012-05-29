using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Configuration.Provider;
using System.Windows.Forms;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.Xml;
using System.IO;


namespace Kopplungstester
{

    public class CustomSettingsProvider : SettingsProvider
    {
        private static string _settingsFileName;
        public static string SettingsFileName
        {
            get { return _settingsFileName; }
            set { _settingsFileName = value; }
        }

        private static string _originalSettingsFileName;
        public static string OriginalSettingsFileName
        {
            get { return _originalSettingsFileName; }
        }

        const string SETTINGSROOT = "Settings";
        //XML Root Node

        public override void Initialize(string name, NameValueCollection col)
        {
            base.Initialize(this.ApplicationName, col);
            SettingsFileName = ApplicationName + ".settings";
            _originalSettingsFileName = SettingsFileName;
        }

        public override string ApplicationName
        {
            get
            {
                if (Application.ProductName.Trim().Length > 0)
                {
                    return Application.ProductName;
                }
                else
                {
                    FileInfo fi = new FileInfo(Application.ExecutablePath);
                    return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                }
            }
            set { }
            //Do nothing
        }

        public override string Name
        {
            get { return "CustomSettingsProvider"; }
        }
        public virtual string GetAppSettingsPath()
        {
            //Used to determine where to store the settings
            System.IO.FileInfo fi = new System.IO.FileInfo(Application.ExecutablePath);
            return fi.DirectoryName;
        }

        public virtual string GetAppSettingsFilename()
        {
            //Used to determine the filename to store the settings
            //return ApplicationName + ".settings";
            return SettingsFileName;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals)
        {
            //Iterate through the settings to be stored
            //Only dirty settings are included in propvals, and only ones relevant to this provider
            foreach (SettingsPropertyValue propval in propvals)
            {
                SetValue(propval);
            }

            try
            {
                SettingsXML.Save(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
            }
            catch (Exception)
            {
            }
            //Ignore if cant save, device been ejected
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            //Create new collection of values
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            //Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in props)
            {

                SettingsPropertyValue value = new SettingsPropertyValue(setting);
                value.IsDirty = false;
                value.SerializedValue = GetValue(setting);
                values.Add(value);
            }
            return values;
        }


        private string _oldSettingsFileName = "";
        private XmlDocument _settingsXML = null;

        private XmlDocument SettingsXML
        {
            get
            {
                //If we dont hold an xml document, try opening one.  
                //If it doesnt exist then create a new one ready.
                

                if (_settingsXML == null || _settingsFileName != _oldSettingsFileName)
                {
                    _oldSettingsFileName = _settingsFileName;

                    _settingsXML = new XmlDocument();

                    try
                    {
                        _settingsXML.Load(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
                    }
                    catch (Exception)
                    {
                        //Create new document
                        XmlDeclaration dec = _settingsXML.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
                        _settingsXML.AppendChild(dec);

                        XmlNode nodeRoot = default(XmlNode);

                        nodeRoot = _settingsXML.CreateNode(XmlNodeType.Element, SETTINGSROOT, "");
                        _settingsXML.AppendChild(nodeRoot);
                    }
                }

                return _settingsXML;
            }
        }

        private string GetValue(SettingsProperty setting)
        {
            string ret = "";

            try
            {
                if (IsRoaming(setting))
                {
                    ret = SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + setting.Name).InnerText;
                }
                else
                {
                    ret = SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + "PC_" + Environment.MachineName + "/" + setting.Name).InnerText;
                }
            }

            catch (Exception)
            {
                if ((setting.DefaultValue != null))
                {
                    ret = setting.DefaultValue.ToString();
                }
                else
                {
                    ret = "";
                }
            }

            return ret;
        }

        private void SetValue(SettingsPropertyValue propVal)
        {

            XmlElement MachineNode = default(XmlElement);
            XmlElement SettingNode = default(XmlElement);

            //Determine if the setting is roaming.
            //If roaming then the value is stored as an element under the root
            //Otherwise it is stored under a machine name node 
            try
            {
                if (IsRoaming(propVal.Property))
                {
                    SettingNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + propVal.Name);
                }
                else
                {
                    SettingNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + "PC_" + Environment.MachineName + "/" + propVal.Name);
                }
            }
            catch (Exception)
            {
                SettingNode = null;
            }

            //Check to see if the node exists, if so then set its new value
            if ((SettingNode != null))
            {
                SettingNode.InnerText = propVal.SerializedValue != null ? propVal.SerializedValue.ToString() : "";
            }
            else
            {
                if (IsRoaming(propVal.Property))
                {
                    //Store the value as an element of the Settings Root Node
                    SettingNode = SettingsXML.CreateElement(propVal.Name);
                    SettingNode.InnerText = propVal.SerializedValue.ToString();
                    SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(SettingNode);
                }
                else
                {
                    //Its machine specific, store as an element of the machine name node,
                    //creating a new machine name node if one doesnt exist.
                    try
                    {

                        MachineNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + "PC_" + Environment.MachineName);
                    }
                    catch (Exception)
                    {
                        MachineNode = SettingsXML.CreateElement("PC_" + Environment.MachineName);
                        SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(MachineNode);
                    }

                    if (MachineNode == null)
                    {
                        MachineNode = SettingsXML.CreateElement("PC_" + Environment.MachineName);
                        SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(MachineNode);
                    }

                    SettingNode = SettingsXML.CreateElement(propVal.Name);
                    SettingNode.InnerText = propVal.SerializedValue != null ? propVal.SerializedValue.ToString() : "";
                    MachineNode.AppendChild(SettingNode);
                }
            }
        }

        private bool IsRoaming(SettingsProperty prop)
        {
            //Determine if the setting is marked as Roaming
            foreach (DictionaryEntry d in prop.Attributes)
            {
                Attribute a = (Attribute)d.Value;
                if (a is System.Configuration.SettingsManageabilityAttribute)
                {
                    return true;
                }
            }
            return false;
        }
    }
}