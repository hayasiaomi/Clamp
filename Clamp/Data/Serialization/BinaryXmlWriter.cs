using System;
using System.Collections;
using System.Text;
using System.IO;

namespace Clamp.Data.Serialization
{
	internal class BinaryXmlWriter
	{
		BinaryWriter writer;
		BinaryXmlTypeMap typeMap;
		Hashtable stringTable = new Hashtable ();
		
		public BinaryXmlWriter (Stream stream, BinaryXmlTypeMap typeMap)
		{
			this.typeMap = typeMap;
			writer = new BinaryWriter (stream);
		}
		
		public void WriteBeginElement (string name)
		{
			writer.Write (BinaryXmlReader.TagBeginElement);
			WriteString (name);
		}
		
		public void WriteEndElement ()
		{
			writer.Write (BinaryXmlReader.TagEndElement);
		}
		
		void WriteValueHeader (string name, TypeCode type)
		{
			writer.Write (BinaryXmlReader.TagValue);
			WriteString (name);
			writer.Write ((byte) type);
		}
		
		public void WriteValue (string name, bool b)
		{
			WriteValueHeader (name, TypeCode.Boolean);
			writer.Write (b);
		}
		
		public void WriteValue (string name, string s)
		{
			WriteValueHeader (name, TypeCode.String);
			WriteString (s);
		}
		
		public void WriteValue (string name, char value)
		{
			WriteValueHeader (name, TypeCode.Char);
			writer.Write (value);
		}
		
		public void WriteValue (string name, byte value)
		{
			WriteValueHeader (name, TypeCode.Byte);
			writer.Write (value);
		}
		
		public void WriteValue (string name, short value)
		{
			WriteValueHeader (name, TypeCode.Int16);
			writer.Write (value);
		}
		
		public void WriteValue (string name, int value)
		{
			WriteValueHeader (name, TypeCode.Int32);
			writer.Write (value);
		}
		
		public void WriteValue (string name, long value)
		{
			WriteValueHeader (name, TypeCode.Int64);
			writer.Write (value);
		}
		
		public void WriteValue (string name, DateTime value)
		{
			WriteValueHeader (name, TypeCode.DateTime);
			writer.Write (value.Ticks);
		}
		
		public void WriteValue (string name, object ob)
		{
			TypeCode t = ob != null ? Type.GetTypeCode (ob.GetType ()) : TypeCode.Empty;
			WriteValueHeader (name, t);
			if (t != TypeCode.Empty)
				WriteValue (ob, t);
		}
		
		public void WriteValue (string name, IBinaryXmlElement ob)
		{
			if (ob == null)
				WriteValueHeader (name, TypeCode.Empty);
			else {
				WriteValueHeader (name, TypeCode.Object);
				WriteObject (ob);
			}
		}
		
		void WriteValue (object ob)
		{
			if (ob == null)
				writer.Write ((byte) TypeCode.Empty);
			else {
				TypeCode t = Type.GetTypeCode (ob.GetType ());
				writer.Write ((byte) t);
				WriteValue (ob, t);
			}
		}
		
		void WriteValue (object ob, TypeCode t)
		{
			switch (t) {
				case TypeCode.Boolean: writer.Write ((bool)ob); break;
				case TypeCode.Char: writer.Write ((char)ob); break;
				case TypeCode.SByte: writer.Write ((sbyte)ob); break;
				case TypeCode.Byte: writer.Write ((byte)ob); break;
				case TypeCode.Int16: writer.Write ((short)ob); break;
				case TypeCode.UInt16: writer.Write ((ushort)ob); break;
				case TypeCode.Int32: writer.Write ((int)ob); break;
				case TypeCode.UInt32: writer.Write ((uint)ob); break;
				case TypeCode.Int64: writer.Write ((long)ob); break;
				case TypeCode.UInt64: writer.Write ((ulong)ob); break;
				case TypeCode.Single: writer.Write ((float)ob); break;
				case TypeCode.Double: writer.Write ((double)ob); break;
				case TypeCode.DateTime: writer.Write (((DateTime)ob).Ticks); break;
				case TypeCode.String: WriteString ((string)ob); break;
				case TypeCode.Object: WriteObject (ob); break;
				default:
					throw new InvalidOperationException ("Unexpected value type: " + t);
			}
		}
		
		void WriteObject (object ob)
		{
			if (ob == null)
				writer.Write (BinaryXmlReader.TagObjectNull);
			else {
				IBinaryXmlElement elem = ob as IBinaryXmlElement;
				if (elem != null) {
					writer.Write (BinaryXmlReader.TagObject);
					WriteString (typeMap.GetTypeName (elem));
					elem.Write (this);
					WriteEndElement ();
				}
				else if (ob is IDictionary) {
					IDictionary dict = (IDictionary) ob;
					writer.Write (BinaryXmlReader.TagObjectDictionary);
					writer.Write (dict.Count);
					foreach (DictionaryEntry e in dict) {
						WriteValue (e.Key);
						WriteValue (e.Value);
					}
				}
				else if (ob is ICollection) {
					ICollection col = (ICollection) ob;
					writer.Write (BinaryXmlReader.TagObjectArray);
					if (ob is Array)
						writer.Write ((byte) Type.GetTypeCode (ob.GetType().GetElementType ()));
					else
						writer.Write ((byte) TypeCode.Object);
					writer.Write (col.Count);
					foreach (object e in col) {
						WriteValue (e);
					}
				}
				else
					throw new InvalidOperationException ("Invalid object type: " + ob.GetType ());
			}
		}
		
		void WriteString (string s)
		{
			if (s == null)
				writer.Write (-1);
			else {
				object ind = stringTable [s];
				if (ind == null) {
					stringTable.Add (s, stringTable.Count);
					byte[] bytes = Encoding.UTF8.GetBytes (s);
					writer.Write (bytes.Length);
					writer.Write (bytes);
				} else {
					// +2 because -1 is reserved for null, and 0 is considered positive
					writer.Write (-((int)ind + 2));
				}
			}
		}
	}
}
