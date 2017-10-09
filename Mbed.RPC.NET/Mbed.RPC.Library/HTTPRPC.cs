// * This library is revised by Kehinde Aina, November 2016

using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

/**
 *  This class is used to create an object which can communicate using RPC to an mbed Connected over HTTP
 *  
 * @author Stanislaus Eichstaedt
 * @license
 * Copyright (c) 2010 ARM Ltd
 *
 *Permission is hereby granted, free of charge, to any person obtaining a copy
 *of this software and associated documentation files (the "Software"), to deal
 *in the Software without restriction, including without limitation the rights
 *to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *copies of the Software, and to permit persons to whom the Software is
 *furnished to do so, subject to the following conditions:
 * <br>
 *The above copyright notice and this permission notice shall be included in
 *all copies or substantial portions of the Software.
 * <br>
 *THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *THE SOFTWARE.
 *
 */
namespace org.mbed.RPC
{
    public class HTTPRPC // : SerialRPC
    {
        private bool bWEB_Ok = false;
        private Uri mbedAddr = null;
        private String Address = String.Empty;

        #region HTTP RPC constructor
        /// <summary>
        /// HTTP RPC constructor
        /// </summary>
        /// <param name="Addr">the internet address - eg. http://xxx.xxx.xxx.xxx</param>
	    public HTTPRPC(String Addr)
        {
            try
            {
                String ipAddress = String.Empty;
                Address = Addr;

                mbedAddr = new Uri(Address);

                if (mbedAddr.IsWellFormedOriginalString() == true)
                {
                    ipAddress = mbedAddr.Host;                              // read the ip address

                    Ping pingMBED = new Ping();
                    PingReply reply = pingMBED.Send(ipAddress);             // ping it to check if alive?

                    if (reply.Status == IPStatus.Success)                   // some status infos...
                    {
                        bWEB_Ok = true;
                        Debug.WriteLine("Address: {0}", ipAddress);
                        Debug.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                        Debug.WriteLine("Time to live: {0}", reply.Options.Ttl);

                        StatusMessage = reply.Status.ToString();
                    }
                    else
                    {
                         StatusMessage = reply.Status.ToString();
                    }
                }
            }
            catch (PingException pingEx)
            {
                bWEB_Ok = false;
                StatusMessage = pingEx.Message;
                Debug.Print("Ping HTTPRPC error: " + pingEx.Message);
            }
            catch (Exception ex)
            {
                bWEB_Ok = false;
                StatusMessage = ex.Message;
                Debug.Print("Constructor HTTPRPC error: " + ex.Message);
            }
	    }
        #endregion

        #region HTTP RPC function
        /// <summary>
	    /// RPC call over HTTP
	    /// </summary>
	    /// <param name="Name">name of the object to call</param>
	    /// <param name="Method">the method to access</param>
	    /// <param name="Args">the arguments needed</param>
	    /// <returns></returns>
	    public String RPC(String Name, String Method, String[] Args)
        {
            if (bWEB_Ok == false) 
                return "Error";

            HttpWebResponse response = null;                  
            Stream dataStream = null;                         
            StreamReader reader = null;                       

		    //Execute RPC GET command and get result back
            String Response = "empty";
		    String Arguments = String.Empty;

		    if(Args != null)
            {
			    int s = Args.Count();

			    for(int i = 0; i < s; i++)
                {
				    Arguments = Arguments + "%20" + Args[i];
			    }
		    }
		    String Command = "/rpc/" + Name + "/" + Method + Arguments;
		    Debug.Print(Address + Command);

            try
            {
                mbedAddr = new Uri(Address + Command);

                if (mbedAddr.IsWellFormedOriginalString() == true)
                {
                    WebRequest request = WebRequest.Create(mbedAddr);                   // Create a request for the URL. 	
                    request.Credentials = CredentialCache.DefaultCredentials;           // If required by the server, set the credentials.
                    response = (HttpWebResponse)request.GetResponse();                  // Get the response.
                    Debug.WriteLine(response.StatusDescription);                        // Display the status.

                    dataStream = response.GetResponseStream();                          // Get the stream containing content returned by the server.
                    reader = new StreamReader(dataStream);                              // Open the stream using a StreamReader for easy access.

                    Response = reader.ReadToEnd();                                      // Read the content.
                    Debug.WriteLine(Response);                                          // Display the content for debug purpose.

                    bWEB_Ok = true;
                    StatusMessage = "Success";
                }
            }
            catch (WebException e)
            {
                Response = "Error";

                // try to get more details
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    bWEB_Ok = false;
                    Debug.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Debug.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    StatusMessage = "Status Description : " + ((HttpWebResponse)e.Response).StatusDescription;
                }
                else
                {
                    bWEB_Ok = false;
                    Debug.Print("Malformed URL Exception: " + e.Message);
                    StatusMessage = e.Message;
                }
            }
            catch (IOException e)
            {
                bWEB_Ok = false;
                Response = "Error";
                Debug.Print("IO Exception in RPC: " + e.Message);
                StatusMessage = e.Message;
            }
            catch (NullReferenceException e)
            {
                bWEB_Ok = false;
                Response = "Error: " + e.Message;
                StatusMessage = e.Message;
            }
            catch (Exception e)
            {
                bWEB_Ok = false;
                Response = "Error";
                Debug.Print("Error: " + e.Message);
                StatusMessage = e.Message;
            }
            finally
            {
                // Cleanup the streams and the response.
                if(reader != null) reader.Close();
                if(dataStream != null) dataStream.Close();
                if(response != null) response.Close();
            }
            return (Response);
	    }
        #endregion

        #region Destructor
        /// <summary>
        /// this is necessary -> don't delete it
        /// </summary>
	    public void delete()
        {
		    //close the HTTP connection
		    try
            {
			    //from mbed.close();
		    }
            catch(IOException e)
            {
			    Debug.Print("IO Exception in delete: "+ e.Message);
		    }
            catch(NullReferenceException e)
            {
                Debug.Print("Null Reference exception in delete: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.Print("Exception in delete: " + e.Message);
            }
        }
        #endregion

        #region HTTPRPC Properties
        /// <summary>
        /// Get the HTTP RPC status
        /// </summary>
        public string StatusMessage { get; private set; }

        /// <summary>
        /// Get the mbed himself 
        /// </summary>
        //public SerialRPC MBED 
        //{ 
        //    get 
        //    { 
        //        return this; 
        //    } 
        //}

        /// <summary>
        /// Is the mbed working and the connection alive?
        /// </summary>
        public bool IsAlive 
        { 
            get 
            { 
                return bWEB_Ok; 
            } 

            set 
            { 
                bWEB_Ok = value; 
            }
        }
        #endregion
    }
}
