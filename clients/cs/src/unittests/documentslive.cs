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
#define USE_TRACING
#define DEBUG

using System;
using System.IO;
using NUnit.Framework;
using Google.GData.Client.UnitTests;
using Google.GData.Documents;


namespace Google.GData.Client.LiveTests
{
    [TestFixture]
    [Category("LiveTest")]
    public class DocumentsTestSuite : BaseLiveTestClass
    {
      

        //////////////////////////////////////////////////////////////////////
        /// <summary>default empty constructor</summary> 
        //////////////////////////////////////////////////////////////////////
        public DocumentsTestSuite()
        {
        }

   
        public override string ServiceName
        {
            get {
                return "writely"; 
            }
        }


        //////////////////////////////////////////////////////////////////////
        /// <summary>runs an authentication test</summary> 
        //////////////////////////////////////////////////////////////////////
        [Test] public void GoogleAuthenticationTest()
        {
            Tracing.TraceMsg("Entering Documents List Authentication Test");

            DocumentsListQuery query = new DocumentsListQuery();
            DocumentsService service = new DocumentsService(this.ApplicationName);
            if (this.userName != null)
            {
                service.Credentials = new GDataCredentials(this.userName, this.passWord);
            }
            service.RequestFactory = this.factory; 

            DocumentsFeed feed = service.Query(query) as DocumentsFeed;

            ObjectModelHelper.DumpAtomObject(feed,CreateDumpFileName("AuthenticationTest")); 
                service.Credentials = null; 
        }
        /////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Tests word processor document creation and deletion
        /// </summary>
        [Test] public void CreateDocumentTest()
        {
            //set up a text/plain file
            string tempFile = Directory.GetCurrentDirectory();
            tempFile = tempFile + Path.DirectorySeparatorChar + "docs_live_test.txt";

            //Console.WriteLine("Creating temporary document at: " + tempFile);

            using (StreamWriter sw = File.CreateText(tempFile))
            {
                sw.WriteLine("My name is Ozymandias, king of kings:");
                sw.WriteLine("Look on my works, ye mighty, and despair!");
                sw.Close();
            }
            


            DocumentsService service = new DocumentsService(this.ApplicationName);
            if (this.userName != null)
            {
                service.Credentials = new GDataCredentials(this.userName, this.passWord);
            }
            service.RequestFactory = this.factory;

            //pick a unique document name
            string documentTitle = "Ozy " + Guid.NewGuid().ToString();

            DocumentEntry entry = service.UploadDocument(tempFile, documentTitle);

            Assert.IsNotNull(entry, "Should get a valid entry back from the server.");
            Assert.AreEqual(documentTitle, entry.Title.Text, "Title on document should be what we provided.");
            Assert.IsTrue(entry.IsDocument, "What we uploaded should come back as a text document type.");
            Assert.IsTrue(entry.AccessControlList != null, "We should get an ACL list back");

            try
            {
                Uri uri = new Uri(entry.AccessControlList);
                Assert.IsTrue(uri.AbsoluteUri == entry.AccessControlList);
    
            } catch (Exception e)
            {
                throw e;
            }
            
            
            //try to delete the document we created
            entry.Delete();

            //clean up the file we created
            File.Delete(tempFile);
            
        }

        /// <summary>
        /// Tests spreadsheet creation and deletion
        /// </summary>
        
        [Ignore("Currently delete is broken")]
        [Test] public void CreateSpreadsheetTest()
        {
            //set up a text/csv file
            string tempFile = Directory.GetCurrentDirectory();
            tempFile = tempFile + Path.DirectorySeparatorChar + "docs_live_test.csv";

            //Console.WriteLine("Creating temporary document at: " + tempFile);

            using (StreamWriter sw = File.CreateText(tempFile))
            {
                sw.WriteLine("foo,bar,baz");
                sw.WriteLine("1,2,3");
                sw.Close();
            }



            DocumentsService service = new DocumentsService(this.ApplicationName);
            if (this.userName != null)
            {
                service.Credentials = new GDataCredentials(this.userName, this.passWord);
            }
            service.RequestFactory = this.factory;

            //pick a unique document name
            string documentTitle = "Simple " + Guid.NewGuid().ToString();

            DocumentEntry entry = service.UploadDocument(tempFile, documentTitle);

            Assert.IsNotNull(entry, "Should get a valid entry back from the server.");
            Assert.AreEqual(documentTitle, entry.Title.Text, "Title on document should be what we provided.");
            Assert.IsTrue(entry.IsSpreadsheet, "What we uploaded should come back as a spreadsheet document type.");

            //try to delete the document we created
            entry.Delete();

            //clean up the file we created
            File.Delete(tempFile);
        }

    
    } /////////////////////////////////////////////////////////////////////////////
}




