using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Clamp.Cfg
{
    public class ExtendedProperties : Hashtable
    {
        private static readonly Byte DEFAULT_BYTE = 0;
        private static readonly Boolean DEFAULT_BOOLEAN = false;
        private static readonly Int32 DEFAULT_INT32 = 0;
        private static readonly Single DEFAULT_SINGLE = 0;
        private static readonly Int64 DEFAULT_INT64 = 0;
        private static readonly Double DEFAULT_DOUBLE = 0;

        /// <summary> Default configurations repository.
        /// </summary>
        private ExtendedProperties defaults;

        /// <summary>
        /// The file connected to this repository (holding comments and such).
        /// </summary>
        protected internal string file;

        /// <summary>
        /// Base path of the configuration file used to create
        /// this ExtendedProperties object.
        /// </summary>
        protected internal string basePath;

        /// <summary>
        /// File separator.
        /// </summary>
        protected internal string fileSeparator = Path.AltDirectorySeparatorChar.ToString();

        /// <summary>
        /// Has this configuration been initialized.
        /// </summary>
        protected internal bool isInitialized = false;

        /// <summary>
        /// This is the name of the property that can point to other
        /// properties file for including other properties files.
        /// </summary>
        protected internal static string include = "include";

        /// <summary>
        /// These are the keys in the order they listed
        /// in the configuration file. This is useful when
        /// you wish to perform operations with configuration
        /// information in a particular order.
        /// </summary>
        protected internal ArrayList keysAsListed = new ArrayList();

        /// <summary>
        /// Creates an empty extended properties object.
        /// </summary>
        public ExtendedProperties()
        {
        }

        /// <summary>
        /// Creates and loads the extended properties from the specified
        /// file.
        /// </summary>
        /// <param name="file">A String.</param>
        /// <exception cref="IOException" />
        public ExtendedProperties(string file) : this(file, null)
        {
        }

        /// <summary>
        /// Creates and loads the extended properties from the specified
        /// file.
        /// </summary>
        /// <param name="file">A String.</param>
        /// <param name="defaultFile">File to load defaults from.</param>
        /// <exception cref="IOException" />
        public ExtendedProperties(string file, string defaultFile)
        {
            this.file = file;

            basePath = new FileInfo(file).FullName;
            basePath = basePath.Substring(0, (basePath.LastIndexOf(fileSeparator) + 1) - (0));

            Load(new FileStream(file, FileMode.Open, FileAccess.Read));

            if (defaultFile != null)
            {
                defaults = new ExtendedProperties(defaultFile);
            }
        }

        /// <summary>
        /// Indicate to client code whether property
        /// resources have been initialized or not.
        /// </summary>
        public bool IsInitialized()
        {
            return isInitialized;
        }

        public string Include
        {
            get { return include; }
            set { include = value; }
        }

        public new IEnumerable Keys
        {
            get { return keysAsListed; }
        }

        public void Load(Stream input)
        {
            Load(input, null);
        }

        /// <summary>
        /// Load the properties from the given input stream
        /// and using the specified encoding.
        /// </summary>
        /// <param name="input">An InputStream.
        /// </param>
        /// <param name="encoding">An encoding.
        /// </param>
        /// <exception cref="IOException" />
        public void Load(Stream input, string encoding)
        {
            lock (this)
            {
                PropertiesReader reader = null;
                if (encoding != null)
                {
                    try
                    {
                        reader = new PropertiesReader(new StreamReader(input, Encoding.GetEncoding(encoding)));
                    }
                    catch (IOException)
                    {
                        // Get one with the default encoding...
                    }
                }

                if (reader == null)
                {
                    reader = new PropertiesReader(new StreamReader(input));
                }

                try
                {
                    while (true)
                    {
                        string line = reader.ReadProperty();

                        if (line == null)
                        {
                            break;
                        }

                        int equalSignIndex = line.IndexOf('=');

                        if (equalSignIndex > 0)
                        {
                            string key = line.Substring(0, (equalSignIndex) - (0)).Trim();
                            string value = line.Substring(equalSignIndex + 1).Trim();

                            if (string.Empty.Equals(value))
                            {
                                continue;
                            }

                            if (Include != null && key.ToUpper().Equals(Include.ToUpper()))
                            {
                              
                                FileInfo file;

                                if (value.StartsWith(fileSeparator))
                                {
                                    file = new FileInfo(value);
                                }
                                else
                                {
                                    if (value.StartsWith(string.Format(".{0}", fileSeparator)))
                                    {
                                        value = value.Substring(2);
                                    }
                                    file = new FileInfo(basePath + value);
                                }

                                bool tmpBool;
                                if (File.Exists(file.FullName))
                                {
                                    tmpBool = true;
                                }
                                else
                                {
                                    tmpBool = Directory.Exists(file.FullName);
                                }
                                
                                if (tmpBool)
                                {
                                    Load(new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
                                }
                            }
                            else
                            {
                                AddProperty(key, value);
                            }
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    return;
                }
                reader.Close();
            }
        }

        /// <summary>  Gets a property from the configuration.
        /// *
        /// </summary>
        /// <param name="key">property to retrieve
        /// </param>
        /// <returns>value as object. Will return user value if exists,
        /// if not then default value if exists, otherwise null
        ///
        /// </returns>
        public Object GetProperty(string key)
        {
           
            Object o = this[key];

            if (o == null)
            {
                // if there isn't a value there, get it from the
                // defaults if we have them
                if (defaults != null)
                {
                    o = defaults[key];
                }
            }

            return o;
        }

        /// <summary> Add a property to the configuration. If it already
        /// exists then the value stated here will be added
        /// to the configuration entry. For example, if
        /// *
        /// resource.loader = file
        /// *
        /// is already present in the configuration and you
        /// *
        /// addProperty("resource.loader", "classpath")
        /// *
        /// Then you will end up with a Vector like the
        /// following:
        /// *
        /// ["file", "classpath"]
        /// *
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        public void AddProperty(string key, Object token)
        {
            Object o = this[key];

            if (o is String)
            {
                ArrayList v = new ArrayList(2);
                v.Add(o);
                v.Add(token);
                CfgUtil.PutElement(this, key, v);
            }
            else if (o is ArrayList)
            {
                ((ArrayList)o).Add(token);
            }
            else
            {
                if (token is string && ((string)token).IndexOf(PropertiesTokenizer.DELIMITER) > 0)
                {
                    PropertiesTokenizer tokenizer = new PropertiesTokenizer((string)token);

                    while (tokenizer.HasMoreTokens())
                    {
                        string s = tokenizer.NextToken();


                        AddStringProperty(key, s);
                    }
                }
                else
                {

                    AddPropertyDirect(key, token);
                }
            }
        }

        /// <summary>   Adds a key/value pair to the map.  This routine does
        /// no magic morphing.  It ensures the keyList is maintained
        /// *
        /// </summary>
        /// <param name="key">key to use for mapping
        /// </param>
        /// <param name="obj">object to store
        ///
        /// </param>
        private void AddPropertyDirect(string key, Object obj)
        {
           
            if (!ContainsKey(key))
            {
                keysAsListed.Add(key);
            }

         
            CfgUtil.PutElement(this, key, obj);
        }

        /// <summary>  Sets a string property w/o checking for commas - used
        /// internally when a property has been broken up into
        /// strings that could contain escaped commas to prevent
        /// the inadvertent vectorization.
        ///
        /// Thanks to Leon Messerschmidt for this one.
        ///
        /// </summary>
        private void AddStringProperty(string key, string token)
        {
            Object o = this[key];

            if (o is string)
            {
                ArrayList v = new ArrayList(2);
                v.Add(o);
                v.Add(token);
                CfgUtil.PutElement(this, key, v);
            }
            else if (o is ArrayList)
            {
                ((ArrayList)o).Add(token);
            }
            else
            {
                AddPropertyDirect(key, token);
            }
        }

        /// <summary> Set a property, this will replace any previously
        /// set values. Set values is implicitly a call
        /// to clearProperty(key), addProperty(key,value).
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetProperty(string key, Object value)
        {
            ClearProperty(key);
            AddProperty(key, value);
        }

        /// <summary> Save the properties to the given outputStream.
        /// </summary>
        /// <param name="output">An OutputStream.
        /// </param>
        /// <param name="Header">A String.
        /// </param>
        /// <exception cref="IOException">
        /// </exception>
        public void Save(TextWriter output, string Header)
        {
            lock (this)
            {
                if (output != null)
                {
                    TextWriter textWriter = output;
                    if (Header != null)
                    {
                        textWriter.WriteLine(Header);
                    }

                    foreach (string key in Keys)
                    {
                        Object value = this[key];
                        if (value == null)
                        {
                            continue;
                        }

                        if (value is string)
                        {
                            WriteKeyOutput(textWriter, key, (string)value);
                        }
                        else if (value is IEnumerable)
                        {
                            foreach (string currentElement in (IEnumerable)value)
                                WriteKeyOutput(textWriter, key, currentElement);
                        }

                        textWriter.WriteLine();
                        textWriter.Flush();
                    }
                }
            }
        }

        private void WriteKeyOutput(TextWriter textWriter, string key, string value)
        {
            StringBuilder currentOutput = new StringBuilder();
            currentOutput.Append(key).Append("=").Append(value);
            textWriter.WriteLine(currentOutput.ToString());
        }

        /// <summary> Combines an existing Hashtable with this Hashtable.
        /// *
        /// Warning: It will overwrite previous entries without warning.
        /// *
        /// </summary>
        /// <param name="c">ExtendedProperties
        ///
        /// </param>
        public void Combine(ExtendedProperties c)
        {
            foreach (string key in c.Keys)
            {
                Object o = c[key];
                // if the value is a String, escape it so that if there are delimiters that the value is not converted to a list
                if (o is string)
                {
                    o = ((string)o).Replace(",", @"\,");
                }

                SetProperty(key, o);
            }
        }

        /// <summary> Clear a property in the configuration.
        /// *
        /// </summary>
        /// <param name="key">key to remove along with corresponding value.
        ///
        /// </param>
        public void ClearProperty(string key)
        {
            if (ContainsKey(key))
            {
                for (int i = 0; i < keysAsListed.Count; i++)
                {
                    if (((String)keysAsListed[i]).Equals(key))
                    {
                        keysAsListed.RemoveAt(i);
                        break;
                    }
                }

                Remove(key);
            }
        }

        /// <summary> Get the list of the keys contained in the configuration
        /// repository.
        /// *
        /// </summary>
        /// <returns>An Iterator.
        ///
        /// </returns>
        /// <summary> Get the list of the keys contained in the configuration
        /// repository that match the specified prefix.
        /// *
        /// </summary>
        /// <param name="prefix">The prefix to test against.
        /// </param>
        /// <returns>An Iterator of keys that match the prefix.
        ///
        /// </returns>
        public IEnumerable GetKeys(string prefix)
        {
            ArrayList matchingKeys = new ArrayList();

            foreach (Object key in Keys)
            {
                if (key is string && ((string)key).StartsWith(prefix))
                {
                    matchingKeys.Add(key);
                }
            }
            return matchingKeys;
        }

        /// <summary> Create an ExtendedProperties object that is a subset
        /// of this one. Take into account duplicate keys
        /// by using the setProperty() in ExtendedProperties.
        /// *
        /// </summary>
        /// <param name="prefix">prefix
        ///
        /// </param>
        public ExtendedProperties Subset(string prefix)
        {
            ExtendedProperties c = new ExtendedProperties();
            bool validSubset = false;

            foreach (Object key in Keys)
            {
                if (key is string && ((string)key).StartsWith(prefix))
                {
                    if (!validSubset)
                        validSubset = true;

                    string newKey;

                  
                    if (((string)key).Length == prefix.Length)
                    {
                        newKey = prefix;
                    }
                    else
                    {
                        newKey = ((string)key).Substring(prefix.Length + 1);
                    }

                    c.AddPropertyDirect(newKey, this[key]);
                }
            }

            if (validSubset)
            {
                return c;
            }
            else
            {
                return null;
            }
        }

        /// <summary> Display the configuration for debugging
        /// purposes.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (String key in Keys)
            {
                Object value = this[key];
                sb.AppendFormat("{0} => {1}", key, ValueToString(value)).Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        private string ValueToString(Object value)
        {
            if (value is ArrayList)
            {
                string s = "ArrayList :: ";
                foreach (Object o in (ArrayList)value)
                {
                    if (!s.EndsWith(", "))
                    {
                        s += ", ";
                    }
                    s += string.Format("[{0}]", o);
                }
                return s;
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary> Get a string associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <returns>The associated string.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a String.
        ///
        /// </exception>
        public string GetString(string key)
        {
            return GetString(key, null);
        }

        /// <summary> Get a string associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <param name="defaultValue">The default value.
        /// </param>
        /// <returns>The associated string if key is found,
        /// default value otherwise.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a String.
        ///
        /// </exception>
        public string GetString(string key, string defaultValue)
        {
            Object value = this[key];

            if (value is string)
            {
                return (string)value;
            }
            else if (value == null)
            {
                if (defaults == null)
                {
                    return defaultValue;
                }
                else
                {
                    return defaults.GetString(key, defaultValue);
                }
            }
            else if (value is ArrayList)
            {
                return (string)((ArrayList)value)[0];
            }
            else
            {
                throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a String object", '\'', key));
            }
        }

        /// <summary> Get a list of properties associated with the given
        /// configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <returns>The associated properties if key is found.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a String/Vector.
        /// </exception>
        /// <exception cref="ArgumentException"> if one of the tokens is
        /// malformed (does not contain an equals sign).
        ///
        /// </exception>
        public Hashtable GetProperties(string key)
        {
            //UPGRADE_TODO: Format of property file may need to be changed. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1089"'
            return GetProperties(key, new Hashtable());
        }

        /// <summary> Get a list of properties associated with the given
        /// configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <param name="defaultProps">Default property values.
        /// </param>
        /// <returns>The associated properties if key is found.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a String/Vector.
        /// </exception>
        /// <exception cref="ArgumentException"> if one of the tokens is
        /// malformed (does not contain an equals sign).
        ///
        /// </exception>
        public Hashtable GetProperties(string key, Hashtable defaultProps)
        {
            /*
	    * Grab an array of the tokens for this key.
	    */
            string[] tokens = GetStringArray(key);

            /*
	    * Each token is of the form 'key=value'.
	    */
            Hashtable props = new Hashtable(defaultProps);
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                int equalSign = token.IndexOf('=');
                if (equalSign > 0)
                {
                    string pkey = token.Substring(0, (equalSign) - (0)).Trim();
                    string pvalue = token.Substring(equalSign + 1).Trim();
                    CfgUtil.PutElement(props, pkey, pvalue);
                }
                else
                {
                    throw new ArgumentException(string.Format("{0}{1}' does not contain an equals sign", '\'', token));
                }
            }
            return props;
        }

        /// <summary> Get an array of strings associated with the given configuration
        /// key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <returns>The associated string array if key is found.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a String/Vector.
        ///
        /// </exception>
        public string[] GetStringArray(string key)
        {
            Object value = this[key];

            // What's your vector, Victor?
            ArrayList vector;
            if (value is string)
            {
                vector = new ArrayList(1);
                vector.Add(value);
            }
            else if (value is ArrayList)
            {
                vector = (ArrayList)value;
            }
            else if (value == null)
            {
                if (defaults == null)
                {
                    return new string[0];
                }
                else
                {
                    return defaults.GetStringArray(key);
                }
            }
            else
            {
                throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a String/Vector object", '\'', key));
            }

            string[] tokens = new string[vector.Count];
            for (int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = (string)vector[i];
            }

            return tokens;
        }

        /// <summary> Get a Vector of strings associated with the given configuration
        /// key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <returns>The associated Vector.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Vector.
        ///
        /// </exception>
        public ArrayList GetVector(string key)
        {
            return GetVector(key, null);
        }

        /// <summary>
        /// Gets the string list.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public List<string> GetStringList(string key)
        {
            Object value = this[key];
            return (List<string>)value;
        }

        /// <summary> Get a Vector of strings associated with the given configuration
        /// key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <param name="defaultValue">The default value.
        /// </param>
        /// <returns>The associated Vector.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Vector.
        ///
        /// </exception>
        public ArrayList GetVector(string key, ArrayList defaultValue)
        {
            Object value = this[key];

            if (value is ArrayList)
            {
                return (ArrayList)value;
            }
            else if (value is string)
            {
                ArrayList v = new ArrayList(1);
                v.Add(value);
                CfgUtil.PutElement(this, key, v);
                return v;
            }
            else if (value == null)
            {
                if (defaults == null)
                {
                    return (defaultValue ?? new ArrayList());
                }
                else
                {
                    return defaults.GetVector(key, defaultValue);
                }
            }
            else
            {
                throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Vector object", '\'', key));
            }
        }

        /// <summary> Get a boolean associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <returns>The associated boolean.
        /// </returns>
        /// <exception cref="Exception"> is thrown if the key doesn't
        /// map to an existing object.
        /// </exception>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Boolean.
        ///
        /// </exception>
        public bool GetBoolean(string key)
        {
            Boolean b = GetBoolean(key, DEFAULT_BOOLEAN);
            if ((Object)b == null)
            {
                throw new Exception(string.Format("{0}{1}' doesn't map to an existing object", '\'', key));
            }
            else
            {
                return b;
            }
        }

        /// <summary> Get a boolean associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <param name="defaultValue">The default value.
        /// </param>
        /// <returns>The associated boolean if key is found and has valid
        /// format, default value otherwise.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Boolean.
        ///
        /// </exception>
        public Boolean GetBoolean(string key, Boolean defaultValue)
        {
            Object value = this[key];

            if (value is Boolean)
            {
                return (Boolean)value;
            }
            else if (value is string)
            {
                String s = TestBoolean((string)value);
                Boolean b = s.ToUpper().Equals("TRUE");
                CfgUtil.PutElement(this, key, b);
                return b;
            }
            else if (value == null)
            {
                if (defaults == null)
                {
                    return defaultValue;
                }
                else
                {
                    return defaults.GetBoolean(key, defaultValue);
                }
            }
            else
            {
                throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Boolean object", '\'', key));
            }
        }

        /// <summary> Test whether the string represent by value maps to a boolean
        /// value or not. We will allow <code>true</code>, <code>on</code>,
        /// and <code>yes</code> for a <code>true</code> boolean value, and
        /// <code>false</code>, <code>off</code>, and <code>no</code> for
        /// <code>false</code> boolean values.  Case of value to test for
        /// boolean status is ignored.
        /// *
        /// </summary>
        /// <param name="value">The value to test for boolean state.
        /// </param>
        /// <returns><code>true</code> or <code>false</code> if the supplied
        /// text maps to a boolean value, or <code>null</code> otherwise.
        ///
        /// </returns>
        public string TestBoolean(string value)
        {
            string s = value.ToLower();

            if (s.Equals("true") || s.Equals("on") || s.Equals("yes"))
            {
                return "true";
            }
            else if (s.Equals("false") || s.Equals("off") || s.Equals("no"))
            {
                return "false";
            }
            else
            {
                return null;
            }
        }

        /// <summary> Get a byte associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <returns>The associated byte if key is found and has valid
        /// format, <see cref="DEFAULT_BYTE"/> otherwise.
        /// </returns>
        /// <exception cref="Exception"> is thrown if the key doesn't
        /// map to an existing object.
        /// </exception>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Byte.
        /// </exception>
        public sbyte GetByte(string key)
        {
            if (ContainsKey(key))
            {
                Byte b = GetByte(key, DEFAULT_BYTE);
                return (sbyte)b;
            }
            else
            {
                throw new Exception(string.Format("{0}{1} doesn't map to an existing object", '\'', key));
            }
        }

        /// <summary> Get a byte associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <param name="defaultValue">The default value.
        /// </param>
        /// <returns>The associated byte if key is found and has valid
        /// format, default value otherwise.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Byte.
        /// </exception>
        public sbyte GetByte(string key, sbyte defaultValue)
        {
            return GetByte(key, defaultValue);
        }

        /// <summary> Get a byte associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <param name="defaultValue">The default value.
        /// </param>
        /// <returns>The associated byte if key is found and has valid
        /// format, default value otherwise.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Byte.
        /// </exception>
        public Byte GetByte(string key, Byte defaultValue)
        {
            Object value = this[key];

            if (value is Byte)
            {
                return (Byte)value;
            }
            else if (value is string)
            {
                Byte b = Byte.Parse((string)value);
                CfgUtil.PutElement(this, key, b);
                return b;
            }
            else if (value == null)
            {
                if (defaults == null)
                {
                    return defaultValue;
                }
                else
                {
                    return defaults.GetByte(key, defaultValue);
                }
            }
            else
            {
                throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Byte object", '\'', key));
            }
        }

        /// <summary> The purpose of this method is to get the configuration resource
        /// with the given name as an integer.
        /// *
        /// </summary>
        /// <param name="name">The resource name.
        /// </param>
        /// <returns>The value of the resource as an integer.
        ///
        /// </returns>
        public Int32 GetInt(string name)
        {
            return GetInteger(name);
        }

        /// <summary> The purpose of this method is to get the configuration resource
        /// with the given name as an integer, or a default value.
        /// *
        /// </summary>
        /// <param name="name">The resource name
        /// </param>
        /// <param name="def">The default value of the resource.
        /// </param>
        /// <returns>The value of the resource as an integer.
        ///
        /// </returns>
        public Int32 GetInt(string name, int def)
        {
            return GetInteger(name, def);
        }

        /// <summary> Get a int associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <returns>The associated int if key is found and has valid
        /// format, <see cref="DEFAULT_INT32"/> otherwise.
        /// </returns>
        /// <exception cref="Exception"> is thrown if the key doesn't
        /// map to an existing object.
        /// </exception>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Integer.
        /// </exception>
        public Int32 GetInteger(string key)
        {
            Int32 i = GetInteger(key, DEFAULT_INT32);
            if ((Object)i == null)
            {
                throw new Exception(string.Format("{0}{1}' doesn't map to an existing object", '\'', key));
            }
            else
            {
                return i;
            }
        }

        /// <summary> Get a int associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <param name="defaultValue">The default value.
        /// </param>
        /// <returns>The associated int if key is found and has valid
        /// format, <see cref="DEFAULT_INT32"/> otherwise.
        /// </returns>
        /// <returns>The associated int if key is found and has valid
        /// format, default value otherwise.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Integer.
        /// </exception>
        public Int32 GetInteger(string key, Int32 defaultValue)
        {
            Object value = this[key];

            if (value is Int32)
            {
                return (Int32)value;
            }
            else if (value is string)
            {
                Int32 i = Int32.Parse((string)value);
                CfgUtil.PutElement(this, key, i);
                return i;
            }
            else if (value == null)
            {
                if (defaults == null)
                {
                    return defaultValue;
                }
                else
                {
                    return defaults.GetInteger(key, defaultValue);
                }
            }
            else
            {
                throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Integer object", '\'', key));
            }
        }

        /// <summary> Get a long associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <returns>The associated long if key is found and has valid
        /// format, <see cref="DEFAULT_INT64"/> otherwise.
        /// </returns>
        /// <exception cref="Exception"> is thrown if the key doesn't
        /// map to an existing object.
        /// </exception>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Long.
        /// </exception>
        public Int64 GetLong(string key)
        {
            Int64 l = GetLong(key, DEFAULT_INT64);
            if ((Object)l == null)
            {
                throw new Exception(string.Format("{0}{1}' doesn't map to an existing object", '\'', key));
            }
            else
            {
                return l;
            }
        }

        /// <summary> Get a long associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <param name="defaultValue">The default value.
        /// </param>
        /// <returns>The associated long if key is found and has valid
        /// format, <see cref="DEFAULT_INT64"/> otherwise.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Long.
        /// </exception>
        public Int64 GetLong(string key, Int64 defaultValue)
        {
            Object value = this[key];

            if (value is Int64)
            {
                return (Int64)value;
            }
            else if (value is string)
            {
                Int64 l = Int64.Parse((string)value);
                CfgUtil.PutElement(this, key, l);
                return l;
            }
            else if (value == null)
            {
                if (defaults == null)
                {
                    return defaultValue;
                }
                else
                {
                    return defaults.GetLong(key, defaultValue);
                }
            }
            else
            {
                throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Long object", '\'', key));
            }
        }

        /// <summary> Get a float associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <returns>The associated float if key is found and has valid
        /// format, <see cref="DEFAULT_SINGLE"/> otherwise.
        /// </returns>
        /// <exception cref="Exception"> is thrown if the key doesn't
        /// map to an existing object.
        /// </exception>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Float.
        /// </exception>
        public float GetFloat(string key)
        {
            Single f = GetFloat(key, DEFAULT_SINGLE);
            if ((Object)f == null)
            {
                throw new Exception(string.Format("{0}{1}' doesn't map to an existing object", '\'', key));
            }
            else
            {
                return f;
            }
        }

        /// <summary> Get a float associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <param name="defaultValue">The default value.
        /// </param>
        /// <returns>The associated float if key is found and has valid
        /// format, <see cref="DEFAULT_SINGLE"/> otherwise.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Float.
        /// </exception>
        public Single GetFloat(string key, Single defaultValue)
        {
            Object value = this[key];

            if (value is Single)
            {
                return (Single)value;
            }
            else if (value is string)
            {
                //UPGRADE_TODO: Format of parameters of constructor 'java.lang.Float.Float' are different in the equivalent in .NET. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1092"'
                Single f = Single.Parse((string)value);
                CfgUtil.PutElement(this, key, f);
                return f;
            }
            else if (value == null)
            {
                if (defaults == null)
                {
                    return defaultValue;
                }
                else
                {
                    return defaults.GetFloat(key, defaultValue);
                }
            }
            else
            {
                throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Float object", '\'', key));
            }
        }

        /// <summary> Get a double associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <returns>The associated double if key is found and has valid
        /// format, <see cref="DEFAULT_DOUBLE"/> otherwise.
        /// </returns>
        /// <exception cref="Exception"> is thrown if the key doesn't
        /// map to an existing object.
        /// </exception>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Double.
        /// </exception>
        public Double GetDouble(string key)
        {
            Double d = GetDouble(key, DEFAULT_DOUBLE);
            if ((Object)d == null)
            {
                throw new Exception(string.Format("{0}{1}' doesn't map to an existing object", '\'', key));
            }
            else
            {
                return d;
            }
        }

        /// <summary> Get a double associated with the given configuration key.
        /// *
        /// </summary>
        /// <param name="key">The configuration key.
        /// </param>
        /// <param name="defaultValue">The default value.
        /// </param>
        /// <returns>The associated double if key is found and has valid
        /// format, <see cref="DEFAULT_DOUBLE"/> otherwise.
        /// </returns>
        /// <exception cref="InvalidCastException"> is thrown if the key maps to an
        /// object that is not a Double.
        /// </exception>
        public Double GetDouble(string key, Double defaultValue)
        {
            Object value = this[key];

            if (value is Double)
            {
                return (Double)value;
            }
            else if (value is string)
            {
                //UPGRADE_TODO: Format of parameters of constructor 'java.lang.Double.Double' are different in the equivalent in .NET. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1092"'
                Double d = Double.Parse((string)value);
                CfgUtil.PutElement(this, key, d);
                return d;
            }
            else if (value == null)
            {
                if (defaults == null)
                {
                    return defaultValue;
                }
                else
                {
                    return defaults.GetDouble(key, defaultValue);
                }
            }
            else
            {
                throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Double object", '\'', key));
            }
        }

        /// <summary>
        /// Convert a standard properties class into a configuration class.
        /// </summary>
        /// <param name="p">properties object to convert into a ExtendedProperties object.</param>
        /// <returns>ExtendedProperties configuration created from the properties object.</returns>
        public static ExtendedProperties ConvertProperties(ExtendedProperties p)
        {
            ExtendedProperties c = new ExtendedProperties();

            foreach (string key in p.Keys)
            {
                Object value = p.GetProperty(key);

                // if the value is a String, escape it so that if there are delimiters that the value is not converted to a list
                if (value is string)
                    value = value.ToString().Replace(",", @"\,");
                c.SetProperty(key, value);
            }

            return c;
        }
    }
}