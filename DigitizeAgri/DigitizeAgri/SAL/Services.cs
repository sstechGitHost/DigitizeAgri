using DigitizeAgri.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace DigitizeAgri.SAL
{
    public class Services
    {
        //Nahid - Common method to get and post 
        public async Task<string> GetPostDetails(string url,string method,string dataToUpload)
        {
            string returnString = string.Empty;

            try
            {
                Uri uri = new Uri(url);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                //When GET, set the request.Accept to whatever format you want to receive the webResponse
                //When POST/PUT, set the request.ContentType to specify in what format you uploading the data and receive webRespnse in same format

                if (method == "GET")
                {
                    request.Accept = "application/json";
                    request.Method = "GET";
                }

                if (method == "POST")
                {
                    request.ContentType = "application/json; charset=utf-8";
                    request.Method = "POST";
                    Stream requestWriter;
                    requestWriter = await request.GetRequestStreamAsync();
                    byte[] data = Encoding.UTF8.GetBytes(dataToUpload);
                    requestWriter.Write(data, 0, data.Length);
                }

                WebResponse response = await request.GetResponseAsync();
                using (var responseStream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(responseStream))
                    {
                        returnString = await sr.ReadToEndAsync();
                    }
                }


                return returnString;
            }
            catch (WebException wex)
            {
                string fullerror = "";
                // This exception will be raised if the server didn't return 200 - OK  
                // Try to retrieve more information about the network error  
                if (wex.Response != null)
                {
                    using (HttpWebResponse errorResponse = (HttpWebResponse)wex.Response)
                    {
                        string ErrResp = wex.Response.Headers.ToString();

                        fullerror = "ERROR " + errorResponse.StatusDescription + " " + errorResponse.StatusCode + " " + errorResponse.StatusCode;
                        System.Diagnostics.Debug.WriteLine("GetPostDetails ex: " + fullerror);
                    }
                }
                return string.Empty;
            }
        }

        public async Task<string> LoginBtnClicked(string phone,string password)
        {
            string retVal = "failure";
            string url = Consts.BaseURL + "auto.person";
            string _return = await GetPostDetails(url,"GET",string.Empty);

            try
            {
                RootObject personObj = (RootObject)JsonConvert.DeserializeObject(_return, typeof(RootObject));
                Person loginUser = personObj.persons.Find(x => x.phone.Trim() == phone.Trim());
                if (loginUser != null)
                {
                    if (loginUser.password.Trim() == password.Trim())
                    {
                        retVal = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Login ex: "+ ex.Message);
            }

            //PersonDetails personDetails = SetPersonDetails(_return);
            //if (personDetails.phone.Trim() == phone && personDetails.password.Trim() == password)
            //{
            //    retVal = "success";
            //}
            return retVal;
        }

        public async Task<string> CreateAccountClicked(RootObject rootObj)
        {
            if (rootObj.persons[0].income.Trim() == "")
                rootObj.persons[0].income = "0";


            string dataToUpload = JsonConvert.SerializeObject(rootObj, Formatting.Indented);

            string retVal = "failure";
            string url = Consts.BaseURL + "auto.person";
            string _return = await GetPostDetails(url, "POST",dataToUpload);

            RootObject personObj = (RootObject)JsonConvert.DeserializeObject(_return, typeof(RootObject));
            if (personObj.persons[0].phone.Trim() != string.Empty)
            {
                retVal = "success";
            }

            return retVal;
        }

        public Person SetPersonDetails(string personXML)
        {
            Person personDetails = new Person();

            try
            {
                string _return = personXML;

                byte[] byteArray = Encoding.UTF8.GetBytes(_return);

                using (var OutputStream = new MemoryStream(byteArray))
                {
                    var XmlResp = XDocument.Load(OutputStream);

                    var RespElement = GetRootElement(XmlResp.Root, "readResponse");

                    foreach (XElement Response in RespElement.Elements())
                    {
                        string Node = Response.Name.LocalName;

                        if (Node == "person")
                        {
                            foreach (XAttribute item in Response.Attributes())
                            {
                                //if (item.Name == "id")
                                //{
                                //    personDetails.id = item.Value;
                                //}
                                if (item.Name == "login")
                                {
                                    personDetails.login = item.Value;
                                }

                                if (item.Name == "fname")
                                {
                                    personDetails.fname = item.Value;
                                }
                                if (item.Name == "lname")
                                {
                                    personDetails.lname = item.Value;
                                }
                                if (item.Name == "password")
                                {
                                    personDetails.password = item.Value;
                                }
                                if (item.Name == "phone")
                                {
                                    personDetails.phone = item.Value;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SetPersonDetails ex: "+ex.Message);
            }
            return personDetails;
        }

        public static XElement GetRootElement(XElement element, string localName)
        {
            XElement Element = null;

            if (element != null && element.Name.LocalName == localName)
                return element;

            if (element != null && element.HasElements)
            {
                foreach (var E in element.Elements())
                {
                    Element = GetRootElement(E, localName);
                }
            }

            return Element;
        }

        public async Task<string> GetIOTData()
        {
            string retVal = "failure";

            try
            {
                string url = Consts.BaseURL + "auto.sensor_data?_limit=1&_offset=0";
                string _return = await GetPostDetails(url, "GET", string.Empty);

                if (_return != string.Empty)
                    retVal = _return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetIOTData ex: " + ex.Message);
            }

            return retVal;
        }
        public async Task<CurrentWeatherModel> CurrentWeather(string latitude, string longitude)
        {
            string received;
            CurrentWeatherModel context = null;
            try
            {
                string cwurl = "http://api.openweathermap.org/data/2.5/weather?APPID=489cb1eabed24d2d5afe50292d33cb78&lat=" + latitude + "&lon=" + longitude + "&mode=json&units=metric";
                //string cwurl = "http://api.openweathermap.org/data/2.5/weather?APPID=489cb1eabed24d2d5afe50292d33cb78&lat=17.3850&lon=78.4867&mode=json&units=metric"; 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cwurl);
                request.Method = "GET";

                using (var response = (HttpWebResponse)(await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null)))
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(responseStream))
                        {
                            received = await sr.ReadToEndAsync();
                        }
                    }
                }
                context = (CurrentWeatherModel)JsonConvert.DeserializeObject(received, typeof(CurrentWeatherModel));
            }
            catch (Exception ex)
            {

            }
            return context;
        }
        public async Task<ForecastWeatherModel> ForecastWeather(string latitude, string longitude)
        {
            string received;
            ForecastWeatherModel context = null;
            try
            {
                string fcwurl = "http://api.openweathermap.org/data/2.5/forecast?APPID=489cb1eabed24d2d5afe50292d33cb78&lat=" + latitude + "&lon=" + longitude + "&mode=json&units=metric";
                //string fcwurl = "http://api.openweathermap.org/data/2.5/forecast?APPID=489cb1eabed24d2d5afe50292d33cb78&lat=17.3850&lon=78.4867&mode=json&units=metric";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fcwurl);
                request.Method = "GET";

                using (var response = (HttpWebResponse)(await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null)))
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(responseStream))
                        {
                            received = await sr.ReadToEndAsync();
                        }
                    }
                }
                context = (ForecastWeatherModel)JsonConvert.DeserializeObject(received, typeof(ForecastWeatherModel));
            }
            catch (Exception ex)
            {

            }
            return context;
        }
        public async Task<List<Scheme_Category>> GetSchemeCategories()
        {
            List<Scheme_Category> retVal = null;
            string url = Consts.BaseURL + "auto.scheme_category";
            string _return = await GetPostDetails(url, "GET", string.Empty);

            try
            {
                SchemeCategorysRootobject schemeCategoriesObj = (SchemeCategorysRootobject)JsonConvert.DeserializeObject(_return, typeof(SchemeCategorysRootobject));
                retVal = schemeCategoriesObj.scheme_categorys;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("scheme_category ex: " + ex.Message);
            }
            return retVal;
        }
        public async Task<List<Scheme>> GetScheme(string id)
        {
            List<Scheme> retVal = null;
            string url = Consts.BaseURL + "auto.scheme?category=" + id;
            string _return = await GetPostDetails(url, "GET", string.Empty);

            try
            {
                SchemeRootobject schemeObj = (SchemeRootobject)JsonConvert.DeserializeObject(_return, typeof(SchemeRootobject));
                retVal = schemeObj.schemes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("scheme ex: " + ex.Message);
            }
            return retVal;
        }
    }
}
