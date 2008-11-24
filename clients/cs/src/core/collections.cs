/* Copyright (c) 2006-2008 Google Inc.
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
/* Change history
* Oct 13 2008  Joe Feser       joseph.feser@gmail.com
* Converted ArrayLists and other .NET 1.1 collections to use Generics
* Combined IExtensionElement and IExtensionElementFactory interfaces
* 
*/
#region Using directives

#define USE_TRACING

using System;
using System.Collections.Generic;
using System.Collections;

#endregion

//////////////////////////////////////////////////////////////////////
// contains typed collections based on the 1.1 .NET framework
// using typed collections has the benefit of additional code reliability
// and using them in the collection editor
// 
//////////////////////////////////////////////////////////////////////
namespace Google.GData.Client
{
    //////////////////////////////////////////////////////////////////////
    /// <summary>standard typed collection based on 1.1 framework for FeedEntries
    /// </summary> 
    //////////////////////////////////////////////////////////////////////
    public class AtomEntryCollection : AtomCollectionBase<AtomEntry>
    {
        /// <summary>holds the owning feed</summary> 
        private AtomFeed feed;

        /// <summary>private default constructor</summary> 
        private AtomEntryCollection()
        {
        }
        /// <summary>constructor</summary> 
        public AtomEntryCollection(AtomFeed feed)
            : base()
        {
            this.feed = feed;
        }

        /// <summary>Fins an atomEntry in the collection 
        /// based on it's ID. </summary> 
        /// <param name="value">The atomId to look for</param> 
        /// <returns>Null if not found, otherwise the entry</returns>
        public AtomEntry FindById(AtomId value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            foreach (AtomEntry entry in List)
            {
                if (entry.Id.AbsoluteUri == value.AbsoluteUri)
                {
                    return entry;
                }
            }
            return null;
        }


        /// <summary>standard typed accessor method </summary> 
        public override AtomEntry this[int index]
        {
            get
            {
                return ((AtomEntry)List[index]);
            }
            set
            {
                if (value != null)
                {
                    if (value.Feed == null || value.Feed != this.feed)
                    {
                        value.setFeed(this.feed);
                    }
                }
                List[index] = value;
            }
        }

        /// <summary>standard typed add method </summary> 
        public override void Add(AtomEntry value)
        {
            if (value != null)
            {
                if (value.Feed == null)
                {
                    value.setFeed(this.feed);
                }
                else
                {
                    if (this.feed != null && value.Feed == this.feed)
                    {
                        // same object, already in here. 
                        throw new ArgumentException("The entry is already part of this collection");
                    }
                    // now we need to see if this is the same feed. If not, copy
                    if (AtomFeed.IsFeedIdentical(value.Feed, this.feed) == false)
                    {
                        AtomEntry newEntry = AtomEntry.ImportFromFeed(value);
                        newEntry.setFeed(this.feed);
                        value = newEntry;
                    }
                }
                value.ProtocolMajor = this.feed.ProtocolMajor;
                value.ProtocolMinor = this.feed.ProtocolMinor;
            }
            base.Add(value);
        }

        public AtomEntry CopyOrMove(AtomEntry value)
        {
            if (value != null)
            {
                if (value.Feed == null)
                {
                    value.setFeed(this.feed);
                }
                else
                {
                    if (this.feed != null && value.Feed == this.feed)
                    {
                        // same object, already in here. 
                        throw new ArgumentException("The entry is already part of this collection");
                    }
                    // now we need to see if this is the same feed. If not, copy
                    if (AtomFeed.IsFeedIdentical(value.Feed, this.feed) == false)
                    {
                        AtomEntry newEntry = AtomEntry.ImportFromFeed(value);
                        newEntry.setFeed(this.feed);
                        value = newEntry;
                    }
                }
                value.ProtocolMajor = this.feed.ProtocolMajor;
                value.ProtocolMinor = this.feed.ProtocolMinor;
            }
            base.Add(value);
            return value; 
        }
    }
    /////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////
    /// <summary>standard typed collection based on 1.1 framework for AtomLinks
    /// </summary> 
    //////////////////////////////////////////////////////////////////////
    public class AtomLinkCollection : AtomCollectionBase<AtomLink>
    {
        /// <summary>standard typed accessor method </summary> 
        public override void Add(AtomLink value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            // Remove link with same relation to avoid duplication.
            AtomLink oldLink = FindService(value.Rel, value.Type);
            if (oldLink != null)
            {
                Remove(oldLink);
            }
            base.Add(value);
        }
       
        //////////////////////////////////////////////////////////////////////
        /// <summary>public AtomLink FindService(string service,string type)
        ///   Retrieves the first link with the supplied 'rel' and/or 'type' value.
        ///   If either parameter is null, the corresponding match isn't needed.
        /// </summary> 
        /// <param name="service">the service entry to find</param>
        /// <param name="type">the link type to find</param>
        /// <returns>the found link or NULL </returns>
        //////////////////////////////////////////////////////////////////////
        public AtomLink FindService(string service, string type)
        {
            foreach (AtomLink link in List)
            {
                string linkRel = link.Rel;
                string linkType = link.Type;

                if ((service == null || (linkRel != null && linkRel == service)) &&
                    (type == null || (linkType != null && linkType == type)))
                {

                    return link;
                }
            }
            return null;
        }
        /////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////
        /// <summary>public AtomLink FindService(string service,string type)
        ///   Retrieves the first link with the supplied 'rel' and/or 'type' value.
        ///   If either parameter is null, the corresponding match isn't needed.
        /// </summary> 
        /// <param name="service">the service entry to find</param>
        /// <param name="type">the link type to find</param>
        /// <returns>the found link or NULL </returns>
        //////////////////////////////////////////////////////////////////////
        public List<AtomLink> FindServiceList(string service, string type)
        {
            List<AtomLink> foundLinks = new List<AtomLink>();

            foreach (AtomLink link in List)
            {
                string linkRel = link.Rel;
                string linkType = link.Type;

                if ((service == null || (linkRel != null && linkRel == service)) &&
                    (type == null || (linkType != null && linkType == type)))
                {

                    foundLinks.Add(link);
                    ;
                }
            }
            return foundLinks;
        }
        /////////////////////////////////////////////////////////////////////////////
    }
    /////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////
    /// <summary>standard typed collection based on 1.1 framework for AtomCategory
    /// </summary> 
    //////////////////////////////////////////////////////////////////////
    public class AtomCategoryCollection : AtomCollectionBase<AtomCategory>
    {
        /// <summary>standard typed accessor method </summary> 
        public override void Add(AtomCategory value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            // Remove category with the same term to avoid duplication.
            AtomCategory oldCategory = Find(value.Term, value.Scheme);
            if (oldCategory != null)
            {
                Remove(oldCategory);
            }
            base.Add(value);
        }

        /// <summary>
        /// finds the first category with this term
        /// ignoring schemes
        /// </summary>
        /// <param name="term">the category term to search for</param>
        /// <returns>AtomCategory</returns>
        public AtomCategory Find(string term)
        {
            return Find(term, null);
        }

        /// <summary>
        /// finds a category with a given term and scheme
        /// </summary>
        /// <param name="term"></param>
        /// <param name="scheme"></param>
        /// <returns>AtomCategory or NULL</returns>
        public AtomCategory Find(string term, AtomUri scheme)
        {
            foreach (AtomCategory category in List)
            {
                if (scheme == null || scheme == category.Scheme)
                {
                    if (term == category.Term)
                    {
                        return category;
                    }
                }
            }
            return null;
        }

        /// <summary>standard typed accessor method </summary> 
        public override bool Contains(AtomCategory value)
        {
            if (value == null)
            {
                return (List.Contains(value));
            }
            // If value is not of type AtomCategory, this will return false.
            if (Find(value.Term, value.Scheme) != null)
            {
                return true;
            }
            return false;

        }
    }
    /////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////
    /// <summary>standard typed collection based on 1.1 framework for AtomPerson
    /// </summary> 
    //////////////////////////////////////////////////////////////////////
    public class QueryCategoryCollection : AtomCollectionBase<QueryCategory>
    {

    }
    /////////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////////////////////////////
    /// <summary>standard typed collection based on 1.1 framework for AtomPerson
    /// </summary> 
    //////////////////////////////////////////////////////////////////////
    public class AtomPersonCollection : AtomCollectionBase<AtomPerson>
    {        

    }
    /////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Generic collection base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AtomCollectionBase<T> : IList<T>
    {
        protected List<T> List = new List<T>();
        /// <summary>standard typed accessor method </summary> 
        public virtual T this[int index]
        {
            get
            {
                return (T)List[index];
            }
            set
            {
                List[index] = value;
            }
        }
        
        /// <summary>standard typed accessor method </summary> 
        public virtual void Add(T value)
        {
            List.Add(value);
        }

        public virtual void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        public virtual void Clear()
        {
            List.Clear();
        }

        public virtual void CopyTo(T[] arr, int index)
        {
            List.CopyTo(arr, index);
        }

        /// <summary>standard typed accessor method </summary> 
        public virtual int IndexOf(T value)
        {
            return (List.IndexOf(value));
        }
        /// <summary>standard typed accessor method </summary> 
        public virtual void Insert(int index, T value)
        {
            List.Insert(index, value);
        }
        /// <summary>standard typed accessor method </summary> 
        public virtual bool Remove(T value)
        {
            return List.Remove(value);
        }
        /// <summary>standard typed accessor method </summary> 
        public virtual bool Contains(T value)
        {
            // If value is not of type AtomPerson, this will return false.
            return (List.Contains(value));
        }

        public virtual int Count
        {
            get 
            {
                return List.Count;
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }



    }

    /// <summary>
    ///  internal list to override the add and the constructor
    /// </summary>
    /// <returns></returns>
    public class ExtensionList : IList<IExtensionElementFactory>
    {
        IVersionAware container;

        List<IExtensionElementFactory> _list = new List<IExtensionElementFactory>();

        /// <summary>
        /// Return a new collection that is not version aware.
        /// </summary>
        public static ExtensionList NotVersionAware()
        {
            return new ExtensionList(NullVersionAware.Instance);
        }

        /// <summary>
        /// returns an extensionlist that belongs to a version aware
        /// container
        /// </summary>
        /// <param name="container"></param>
        public ExtensionList(IVersionAware container)
        {
            this.container = container;
        }

        public int Add(IExtensionElementFactory value)
        {
            IVersionAware target = value as IVersionAware;

            if (target != null)
            {
                target.ProtocolMajor = this.container.ProtocolMajor;
                target.ProtocolMinor = this.container.ProtocolMinor;
            }

            _list.Add(value);
            return _list.Count - 1;
            //return _list.IndexOf(value);
        }

        public int IndexOf(IExtensionElementFactory item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, IExtensionElementFactory item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public IExtensionElementFactory this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        void ICollection<IExtensionElementFactory>.Add(IExtensionElementFactory item)
        {
            Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(IExtensionElementFactory item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(IExtensionElementFactory[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(IExtensionElementFactory item)
        {
            return _list.Remove(item);
        }

        public IEnumerator<IExtensionElementFactory> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
