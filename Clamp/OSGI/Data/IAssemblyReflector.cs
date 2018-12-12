using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data
{
    public interface IAssemblyReflector
    {
        /// <summary>
        /// Called to initialize the assembly reflector
        /// </summary>
        /// <param name='locator'>
        /// IAssemblyLocator instance which can be used to locate referenced assemblies.
        /// </param>
        void Initialize(IAssemblyLocator locator);

        /// <summary>
        /// Gets a list of custom attributes
        /// </summary>
        /// <returns>
        /// The custom attributes.
        /// </returns>
        /// <param name='obj'>
        /// An assembly, class or class member
        /// </param>
        /// <param name='type'>
        /// Type of the attribute to be returned. It will always be one of the attribute types
        /// defined in Mono.Bundles.
        /// </param>
        /// <param name='inherit'>
        /// 'true' if inherited attributes must be returned
        /// </param>
        object[] GetCustomAttributes(object obj, Type type, bool inherit);

        /// <summary>
        /// Gets a list of custom attributes
        /// </summary>
        /// <returns>
        /// The attributes.
        /// </returns>
        /// <param name='obj'>
        /// An assembly, class or class member
        /// </param>
        /// <param name='type'>
        /// Base type of the attribute to be returned
        /// </param>
        /// <param name='inherit'>
        /// 'true' if inherited attributes must be returned
        /// </param>
        List<CustomAttribute> GetRawCustomAttributes(object obj, Type type, bool inherit);

        /// <summary>
        /// 加载一个程序集文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        object LoadAssembly(string file);

        /// <summary>
        /// Loads the assembly specified in an assembly reference
        /// </summary>
        /// <returns>
        /// The assembly
        /// </returns>
        /// <param name='asmReference'>
        /// An assembly reference
        /// </param>
        object LoadAssemblyFromReference(object asmReference);

       /// <summary>
       /// 获得指定程序集的内部文件集
       /// </summary>
       /// <param name="asm"></param>
       /// <returns></returns>
        string[] GetResourceNames(object asm);

        /// <summary>
        /// Gets the data stream of a resource
        /// </summary>
        /// <returns>
        /// The stream.
        /// </returns>
        /// <param name='asm'>
        /// An assembly
        /// </param>
        /// <param name='resourceName'>
        /// The name of a resource
        /// </param>
        Stream GetResourceStream(object asm, string resourceName);

        /// <summary>
        /// Gets all types defined in an assembly
        /// </summary>
        /// <returns>
        /// The types
        /// </returns>
        /// <param name='asm'>
        /// An assembly
        /// </param>
        IEnumerable GetAssemblyTypes(object asm);

        /// <summary>
        /// Gets all assembly references of an assembly
        /// </summary>
        /// <returns>
        /// A list of assembly references
        /// </returns>
        /// <param name='asm'>
        /// An assembly
        /// </param>
        IEnumerable GetAssemblyReferences(object asm);

        /// <summary>
        /// Looks for a type in an assembly
        /// </summary>
        /// <returns>
        /// The type.
        /// </returns>
        /// <param name='asm'>
        /// An assembly
        /// </param>
        /// <param name='typeName'>
        /// Name of the type
        /// </param>
        object GetType(object asm, string typeName);


        /// <summary>
        /// Gets a custom attribute
        /// </summary>
        /// <returns>
        /// The custom attribute.
        /// </returns>
        /// <param name='obj'>
        /// An assembly, class or class member
        /// </param>
        /// <param name='type'>
        /// Base type of the attribute to be returned. It will always be one of the attribute types
        /// defined in Mono.Bundles.
        /// </param>
        /// <param name='inherit'>
        /// 'true' if inherited attributes must be returned
        /// </param>
        object GetCustomAttribute(object obj, Type type, bool inherit);

        /// <summary>
        /// Gets the name of a type (not including namespace)
        /// </summary>
        /// <returns>
        /// The type name.
        /// </returns>
        /// <param name='type'>
        /// A type
        /// </param>
        string GetTypeName(object type);

        /// <summary>
        /// Gets the full name of a type (including namespace)
        /// </summary>
        /// <returns>
        /// The full name of the type
        /// </returns>
        /// <param name='type'>
        /// A type
        /// </param>
        string GetTypeFullName(object type);

        /// <summary>
        /// Gets the assembly qualified name of a type
        /// </summary>
        /// <returns>
        /// The assembly qualified type name
        /// </returns>
        /// <param name='type'>
        /// A type
        /// </param>
        string GetTypeAssemblyQualifiedName(object type);

        /// <summary>
        /// Gets a list of all base types (including interfaces) of a type
        /// </summary>
        /// <returns>
        /// An enumeration of the full name of all base types of the type
        /// </returns>
        /// <param name='type'>
        /// A type
        /// </param>
        IEnumerable GetBaseTypeFullNameList(object type);

        /// <summary>
        /// Checks if a type is assignable to another type
        /// </summary>
        /// <returns>
        /// 'true' if the type is assignable
        /// </returns>
        /// <param name='baseType'>
        /// Expected base type.
        /// </param>
        /// <param name='type'>
        /// A type.
        /// </param>
        bool TypeIsAssignableFrom(object baseType, object type);

        /// <summary>
        /// Gets the fields of a type
        /// </summary>
        /// <returns>
        /// The fields.
        /// </returns>
        /// <param name='type'>
        /// A type
        /// </param>
        IEnumerable GetFields(object type);

        /// <summary>
        /// Gets the name of a field.
        /// </summary>
        /// <returns>
        /// The field name.
        /// </returns>
        /// <param name='field'>
        /// A field.
        /// </param>
        string GetFieldName(object field);

        /// <summary>
        /// Gets the full name of the type of a field
        /// </summary>
        /// <returns>
        /// The full type name
        /// </returns>
        /// <param name='field'>
        /// A field.
        /// </param>
        string GetFieldTypeFullName(object field);
    }
}
