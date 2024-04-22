using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikiApplicationFINAL
{
    internal class Information : IComparable<Information>
    {
        // 6.1 Create a separate class file to hold the four data items of the Data Structure (use the Data Structure Matrix as a guide). 
        // Use private properties for the fields which must be of type “string”. The class file must have separate setters and getters, add an appropriate IComparable for the Name attribute.
        // Save the class as “Information.cs”.
        // Attributes
        // Name, Category, Structure & Definition
        private string name;
        private string category;
        private string structure;
        private string definition;

        public Information()
        { }
        public Information(string newName, string newCategory, string newStructure, string newDefinition)
        {
            name = newName;
            category = newCategory;
            structure = newStructure;
            definition = newDefinition;
        }
        public string GetName()
        {
            return name;
        }

        public void SetName(string _Name)
        {
            name = _Name; 
        }

        public string GetCategory()
        {
            return category;
        }

        public void SetCategory(string _Category)
        {
            category = _Category;
        }

        public string GetStructure()
        {
            return structure;
        }

        public void SetStructure(string _Structure)
        {
            structure = _Structure;
        }

        public string GetDefinition()
        {
            return definition;
        }

        public void SetDefinition(string _Definition)
        {
            definition = _Definition;
        }

        public int CompareTo(Information other)
        {
            return name.CompareTo(other.name); 
        }


    }
}
