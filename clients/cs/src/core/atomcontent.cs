/* Copyright (c) 2006 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
#region Using directives

#define USE_TRACING

using System;
using System.Xml;
using System.IO; 
using System.Globalization;
using System.ComponentModel;
using System.Runtime.InteropServices;

#endregion

//////////////////////////////////////////////////////////////////////
// Contains AtomContent, an object to represent the
// atom:content element.
// # atom:content
//
// atomInlineTextContent =
//    element atom:content {
//       atomCommonAttributes,
//       attribute type { "TEXT" | "HTML" | atomMediaType }?,
//       (text)*
//    }
//
// atomInlineXHTMLContent =
//    element atom:content {
//       atomCommonAttributes,
//       attribute type { "XHTML" | atomMediaType }?,
//       (text|anyElement)*
//    }
//
// atomOutOfLineContent =
//    element atom:content {
//       atomCommonAttributes,
//       attribute type { "TEXT" | "HTML" | "XHTML" | atomMediaType }?,
//       attribute src { atomUri },
//       empty
//    }
//
// atomContent = atomInlineTextContent
//  | atomInlineXHTMLContent
//  | atomOutOfLineContent
//////////////////////////////////////////////////////////////////////
namespace Google.GData.Client
{

#if WindowsCE
#else 
    //////////////////////////////////////////////////////////////////////
    /// <summary>TypeConverter, so that AtomContentConverter shows up in the property pages
    /// </summary> 
    //////////////////////////////////////////////////////////////////////
    [ComVisible(false)]
    public class AtomContentConverter : ExpandableObjectConverter
    {
        ///<summary>Standard type converter method</summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType) 
        {
            if (destinationType == typeof(AtomContent))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        ///<summary>Standard type converter method</summary>
        public override object ConvertTo(ITypeDescriptorContext context,CultureInfo culture, object value, System.Type destinationType) 
        {
            AtomContent content = value as AtomContent; 
            if (destinationType == typeof(System.String) && content != null)
            {
                return "Content-type: " + content.Type;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

    }
    /////////////////////////////////////////////////////////////////////////////
 


    //////////////////////////////////////////////////////////////////////
    /// <summary>atom:content object representation
    /// </summary> 
    //////////////////////////////////////////////////////////////////////

    [TypeConverterAttribute(typeof(AtomContentConverter)), DescriptionAttribute("Expand to see the content objectfor the entry.")]
#endif
    public class AtomContent : AtomBase
    {
        /// <summary>holds the  type attribute</summary> 
        private string type;
        /// <summary>holds the src URI attribute</summary> 
        private AtomUri src;
        /// <summary>holds teh content</summary> 
        private string content; 




        #region overloaded for persistence

        //////////////////////////////////////////////////////////////////////
        /// <summary>Returns the constant representing this XML element.</summary> 
        //////////////////////////////////////////////////////////////////////
        public override string XmlName 
        {
            get { return AtomParserNameTable.XmlContentElement; }
        }
        /////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////
        /// <summary>figures out if this object should be persisted</summary> 
        /// <returns> true, if it's worth saving</returns>
        //////////////////////////////////////////////////////////////////////
        public override bool ShouldBePersisted()
        {
            if (base.ShouldBePersisted() == false)
            {
                return Utilities.IsPersistable(this.src) || Utilities.IsPersistable(this.type) || Utilities.IsPersistable(this.content); 
            }
            return true;
        }
        /////////////////////////////////////////////////////////////////////////////



        //////////////////////////////////////////////////////////////////////
        /// <summary>overridden to save attributes for this(XmlWriter writer)</summary> 
        /// <param name="writer">the xmlwriter to save into </param>
        //////////////////////////////////////////////////////////////////////
        protected override void SaveXmlAttributes(XmlWriter writer)
        {
            base.SaveXmlAttributes(writer);
            WriteEncodedAttributeString(writer, AtomParserNameTable.XmlAttributeSrc, this.Src);
            WriteEncodedAttributeString(writer, AtomParserNameTable.XmlAttributeType, this.Type);
        }
        /////////////////////////////////////////////////////////////////////////////


        //////////////////////////////////////////////////////////////////////
        /// <summary>saves the inner state of the element. Note that if the 
        /// content type is xhtml, no encoding will be done by this object</summary> 
        /// <param name="writer">the xmlWriter to save into </param>
        //////////////////////////////////////////////////////////////////////
        protected override void SaveInnerXml(XmlWriter writer)
        {
            base.SaveInnerXml(writer);
            if (Utilities.IsPersistable(this.content))
            {
                if (this.type == "xhtml")
                {
                    // in this case we are not going to encode the inner content. 
                    // Developer has to take care of this
                    writer.WriteRaw(this.content); 
                }
                else 
                {
                    // per spec, text/html should be encoded. 
                    WriteEncodedString(writer, this.content);
                }
            }

        }
        /////////////////////////////////////////////////////////////////////////////


        #endregion


        //////////////////////////////////////////////////////////////////////
        /// <summary>accessor method public string Type</summary> 
        /// <returns> </returns>
        //////////////////////////////////////////////////////////////////////
        public string Type
        {
            get {return this.type;}
            set {this.Dirty = true;  this.type = value;}
        }
        /////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////
        /// <summary>accessor method public Uri Src</summary> 
        /// <returns> </returns>
        //////////////////////////////////////////////////////////////////////
        public AtomUri Src
        {
            get {return this.src;}
            set {this.Dirty = true;  this.src = value;}
        }
        /////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////
        /// <summary>public Uri AbsoluteUri</summary> 
        //////////////////////////////////////////////////////////////////////
        public string AbsoluteUri
        {
            get
            {
                return GetAbsoluteUri(this.Src.ToString()); 
            }
        }
        /////////////////////////////////////////////////////////////////////////////


        //////////////////////////////////////////////////////////////////////
        /// <summary>accessor method public string Content</summary> 
        /// <returns> </returns>
        //////////////////////////////////////////////////////////////////////
        public string Content
        {
            get {return this.content;}
            set {this.Dirty = true;  this.content = value;}
        }
        /////////////////////////////////////////////////////////////////////////////
    }
    /////////////////////////////////////////////////////////////////////////////
} 
/////////////////////////////////////////////////////////////////////////////
