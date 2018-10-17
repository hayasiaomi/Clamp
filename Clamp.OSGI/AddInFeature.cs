using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI
{
    public class AddInFeature
    {
        private string name;
        private Bundle addIn;
        private List<List<Codon>> codons = new List<List<Codon>>();

        public Bundle AddIn
        {
            get
            {
                return addIn;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public IEnumerable<Codon> Codons
        {
            get
            {
                return
                    from list in codons
                    from c in list
                    select c;
            }
        }

        public IEnumerable<IEnumerable<Codon>> GroupedCodons
        {
            get
            {
                return codons.AsReadOnly();
            }
        }

        public AddInFeature(string name, Bundle addIn)
        {
            this.addIn = addIn;
            this.name = name;
        }

        public static void SetUp(AddInFeature addInPath, XmlReader reader, string endElement)
        {
            addInPath.DoSetUp(reader, endElement, addInPath.addIn);
        }

        void DoSetUp(XmlReader reader, string endElement, Bundle addIn)
        {
            Stack<ICondition> conditionStack = new Stack<ICondition>();
            List<Codon> innerCodons = new List<Codon>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == "Condition" || reader.LocalName == "ComplexCondition")
                        {
                            conditionStack.Pop();
                        }
                        else if (reader.LocalName == endElement)
                        {
                            if (innerCodons.Count > 0)
                                this.codons.Add(innerCodons);
                            return;
                        }
                        break;
                    case XmlNodeType.Element:
                        string elementName = reader.LocalName;
                        if (elementName == "Condition")
                        {
                            conditionStack.Push(AddInCondition.Read(reader, addIn));
                        }
                        else if (elementName == "ComplexCondition")
                        {
                            conditionStack.Push(AddInCondition.ReadComplexCondition(reader, addIn));
                        }
                        else
                        {
                            Codon newCodon = new Codon(this.AddIn, elementName, AddInProperties.ReadFromAttributes(reader), conditionStack.ToList().AsReadOnly());

                            innerCodons.Add(newCodon);

                            if (!reader.IsEmptyElement)
                            {
                                AddInFeature subPath = this.AddIn.GetExtensionPath(this.Name + "/" + newCodon.Id);
                                subPath.DoSetUp(reader, elementName, addIn);
                            }
                        }
                        break;
                }
            }
            if (innerCodons.Count > 0)
                this.codons.Add(innerCodons);
        }
    }
}
