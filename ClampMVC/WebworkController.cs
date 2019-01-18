using ClampMVC.Annotation;
using ClampMVC.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace ClampMVC
{
    public class WebworkController : Controller
    {
        private readonly Dictionary<string, MethodInfo> routeMethodInfoCaches = new Dictionary<string, MethodInfo>();

        protected WebworkController() : this(String.Empty)
        {

        }


        protected WebworkController(string modulePath) : base(modulePath)
        {
            var methods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

                if (attribute == null)
                {
                    continue;
                }

                string httpMethod = attribute.Method;
                string httpPath = attribute.Path;

                if (string.IsNullOrWhiteSpace(httpPath))
                {
                    httpPath = method.Name;
                }

                RouteBuilder routeBuilder = new RouteBuilder(httpMethod, this);

                string fullPath = routeBuilder.GetFullPath(httpPath);

                this.routeMethodInfoCaches.Add($"{httpMethod}-{fullPath}".ToLower(), method);

                routeBuilder[httpPath] = x =>
                {
                    return this.HandleRequestMethod($"{this.Request.Method}-{this.Request.Path}".ToLower());
                };

            }

        }

        private dynamic HandleRequestMethod(string path)
        {
            MethodInfo method = this.routeMethodInfoCaches[path];

            //Dictionary<string, string> keyValuePairs = this.Bind<Dictionary<string, string>>();

            ParameterInfo[] parameterInfos = method.GetParameters();

            object[] argments;

            if (parameterInfos != null && parameterInfos.Length > 0)
            {
                argments = new object[parameterInfos.Length];


                foreach (ParameterInfo pi in parameterInfos)
                {
                    object value = null;

                    if (this.Request.Query != null && this.Request.Query.Count > 0)
                    {
                        foreach (string keyName in this.Request.Query.Keys)
                        {
                            if (EqualsParameterValue(keyName, pi.Name, pi.ParameterType.IsClass))
                            {
                                value = this.ConvertParameterToValue(this.Request.Query[keyName], keyName, pi.ParameterType);
                                break;
                            }
                        }
                    }

                    if (value == null)
                    {
                        if (this.Request.Form != null && this.Request.Form.Count > 0)
                        {
                            foreach (string keyName in this.Request.Form.Keys)
                            {
                                if (EqualsParameterValue(keyName, pi.Name, pi.ParameterType.IsClass))
                                {
                                    value = this.ConvertParameterToValue(this.Request.Form[keyName], keyName, pi.ParameterType);

                                    break;
                                }
                            }
                        }
                    }

                    argments[pi.Position] = value;
                }
            }
            else
            {
                argments = new object[0];
            }

            return method.Invoke(this, argments);
        }

        private object ConvertParameterToValue(dynamic data, string keyName, Type targetType)
        {
            if (!targetType.IsClass || targetType.IsAssignableFrom(typeof(string)))
            {
                if (targetType.IsAssignableFrom(typeof(string)))
                {
                    if (data.HasValue)
                        return Convert.ToString(data.Value);
                    return string.Empty;
                }
                else if (targetType.IsAssignableFrom(typeof(int)))
                {
                    if (data.HasValue)
                        return Convert.ToInt32(data.Value);
                    return 0;
                }
                else if (targetType.IsAssignableFrom(typeof(long)))
                {
                    if (data.HasValue)
                        return Convert.ToInt64(data.Value);
                    return 0;
                }
                else if (targetType.IsAssignableFrom(typeof(bool)))
                {
                    if (data.HasValue)
                        return Convert.ToBoolean(data.Value);
                    return false;
                }
                else if (targetType.IsAssignableFrom(typeof(float)) || targetType.IsAssignableFrom(typeof(double)))
                {
                    if (data.HasValue)
                        return Convert.ToDouble(data.Value);
                    return false;
                }
            }
            else
            {
                string[] fields = keyName.Split('.');

                object instance = Activator.CreateInstance(targetType);

                if (fields.Length > 1)
                {
                    string fieldName = fields[1];

                    PropertyInfo propertyInfo = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .FirstOrDefault(p => string.Equals(fieldName, p.Name, StringComparison.CurrentCultureIgnoreCase));

                    if (propertyInfo != null)
                    {
                        object pValue = this.ConvertParameterToValue(data, keyName, propertyInfo.PropertyType);

                        if (propertyInfo.CanWrite)
                            propertyInfo.SetValue(instance, pValue, null);
                    }
                }

                return instance;
            }

            return null;
        }

        private bool EqualsParameterValue(string keyName, string name, bool isClass)
        {
            if (!isClass)
                return string.Equals(keyName, name, StringComparison.CurrentCultureIgnoreCase);
            else
                return keyName.StartsWith(name, StringComparison.CurrentCultureIgnoreCase);
        }


    }
}
