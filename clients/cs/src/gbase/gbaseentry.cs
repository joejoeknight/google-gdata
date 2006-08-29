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
using System;
using System.Xml;
using System.IO;
using System.Collections;
using Google.GData.Client;
#endregion


namespace Google.GData.GoogleBase
{

    ///////////////////////////////////////////////////////////////////////
    /// <summary>A Google Base entry.
    ///
    /// Google Base entries in the items and snippets feed will
    /// contain tags in the g: namespace that can be accessed
    /// using <see cref="GBaseAttributes"/>.
    ///
    /// Entries in the attributes feed will contain
    /// <see cref="AttributeHistogram"/> objects instead.
    ///
    /// Entries in the item type feed will contain
    /// <see cref="ItemTypeDefinition"/> objects instead.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class GBaseEntry : AtomEntry, GBaseAttributeContainer
    {
        private readonly GBaseAttributes attributes;
        private readonly ItemTypeDefinition itemTypeDefinition;

        ///////////////////////////////////////////////////////////////////////
        /// <summary>Creates a new entry</summary>
        ///////////////////////////////////////////////////////////////////////
        public GBaseEntry() : base()
        {
            attributes = new GBaseAttributes(ExtensionElements);
            itemTypeDefinition = new ItemTypeDefinition(ExtensionElements);
        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>Access the g: tags in this entry.</summary>
        ///////////////////////////////////////////////////////////////////////
        public GBaseAttributes GBaseAttributes
        {
            get
            {
                return attributes;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>Access the gm:attribute tags in this entry, useful if
        /// the current entry is part of the histogram feed, may be null.</summary>
        ///////////////////////////////////////////////////////////////////////
        public AttributeHistogram AttributeHistogram
        {
            get
            {
                return GBaseUtilities.GetExtension(ExtensionElements,
                                                   typeof(AttributeHistogram))
                       as AttributeHistogram;
            }
            set
            {
                GBaseUtilities.SetExtension(ExtensionElements,
                                            typeof(AttributeHistogram), value);
            }
        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>Access the gm:attributs tags in this entry, useful if
        /// the current entry is part of the item types feed.</summary>
        ///////////////////////////////////////////////////////////////////////
        public ItemTypeDefinition ItemTypeDefinition
        {
            get
            {
                return itemTypeDefinition;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>Adds element from the gm: namespace</summary>
        ///////////////////////////////////////////////////////////////////////
        internal bool AddFromMetaNamespace(XmlNode node)
        {
            if (String.Compare(node.NamespaceURI,
                               GBaseNameTable.NSGBaseMeta, true) == 0)
            {
                switch(node.LocalName)
                {
                case "item_type":
                    ExtensionElements.Add(MetadataItemType.Parse(node));
                    break;

                case "attributes":
                    ExtensionElements.Add(ItemTypeAttributes.Parse(node));
                    break;

                case "attribute":
                    ExtensionElements.Add(AttributeHistogram.Parse(node));
                    break;

                default:
                    return false;
                }
                return true;
            }
            return false;
        }
    }

}
