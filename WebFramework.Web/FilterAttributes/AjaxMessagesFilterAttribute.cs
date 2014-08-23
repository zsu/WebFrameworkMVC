/// Author: Zhicheng Su
using System;
using System.Web.Mvc;
using App.Common.SessionMessage;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

namespace Web.FilterAttributes
{
    /// <summary>
    /// If we're dealing with ajax requests, any message that is in the view data goes to
    /// the http header.
    /// </summary>
    public class MvcAjaxMessagesFilterAttribute : ActionFilterAttribute //TODO: return data only
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var response = filterContext.HttpContext.Response;
                List<SessionMessage> sessionMessages = SessionMessageManager.GetMessage();
                if (sessionMessages != null && sessionMessages.Count > 0)
                {
                    string json = null;
                    var messages = sessionMessages.Where(x => x.Behavior == MessageBehaviors.StatusBar).Select(x => new SessionMessageJsonModal { Message = x.Message, Type = Enum.GetName(typeof(MessageType), x.Type).ToLowerInvariant(),Key=x.Key }).ToList();
                    if (messages != null && messages.Count > 0)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(messages.GetType());
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ser.WriteObject(ms, messages);
                            json = Encoding.Default.GetString(ms.ToArray());
                            ms.Close();
                        }
                        response.Headers.Add("X-Message", json);
                    }
                    messages = sessionMessages.Where(x => x.Behavior == MessageBehaviors.Modal).Select(x => new SessionMessageJsonModal { Message = x.Message, Type = Enum.GetName(typeof(MessageType), x.Type).ToLowerInvariant(), Key=x.Key }).ToList();
                    if (messages != null && messages.Count > 0)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(messages.GetType());
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ser.WriteObject(ms, messages);
                            json = Encoding.Default.GetString(ms.ToArray());
                            ms.Close();
                        }
                        response.Headers.Add("X-ModalMessage", json);
                    }
                    //TagBuilder messageBoxBuilder = null, messageBoxModal = null, messageBoxStatusBar = null;
                    //for (int i = 0; i < sessionMessages.Count; i++)
                    //{
                    //    var sessionMessage = sessionMessages[i];
                    //    switch (sessionMessage.Behavior)
                    //    {
                    //        case MessageBehaviors.Modal:
                    //            if (messageBoxModal == null)
                    //            {
                    //                messageBoxModal = new TagBuilder("div");
                    //                messageBoxModal.Attributes.Add("id", "messageboxModal");
                    //                //messageBoxModal.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                    //            }
                    //            messageBoxBuilder = new TagBuilder("div");
                    //            messageBoxBuilder.Attributes.Add("id", "messagebox" + i);
                    //            messageBoxBuilder.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                    //            messageBoxBuilder.InnerHtml += sessionMessage.Message;
                    //            messageBoxModal.InnerHtml += messageBoxBuilder.ToString();
                    //            break;
                    //        case MessageBehaviors.StatusBar:
                    //        default:
                    //            if (messageBoxStatusBar == null)
                    //            {
                    //                messageBoxStatusBar = new TagBuilder("div");
                    //                messageBoxStatusBar.Attributes.Add("id", "messageboxStatusBar");
                    //                //messageBoxStatusBar.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                    //                messageBoxStatusBar.AddCssClass(String.Format("messagebox {0}", Enum.GetName(typeof(MessageType), sessionMessage.Type).ToLowerInvariant()));
                    //            }
                    //            messageBoxBuilder = new TagBuilder("div");
                    //            messageBoxBuilder.Attributes.Add("id", "messagebox" + i);
                    //            messageBoxBuilder.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                    //            messageBoxBuilder.AddCssClass(String.Format("messagebox {0}", Enum.GetName(typeof(MessageType), sessionMessage.Type).ToLowerInvariant()));
                    //            messageBoxBuilder.InnerHtml += sessionMessage.Message;
                    //            messageBoxStatusBar.InnerHtml += messageBoxBuilder.ToString();
                    //            break;
                    //    }
                    //}
                    //if (messageBoxStatusBar != null && !string.IsNullOrEmpty(messageBoxStatusBar.ToString()))
                    //    response.Headers.Add("X-Message", messageBoxStatusBar.ToString());
                    //if (messageBoxModal != null && !string.IsNullOrEmpty(messageBoxModal.ToString()))
                    //    response.Headers.Add("X-ModalMessage", messageBoxModal.ToString());
                    SessionMessageManager.Clear();
                }
            }
        }
        [DataContract]
        private class SessionMessageJsonModal
        {
            [DataMember]
            public string Message
            {
                get;
                set;
            }
            [DataMember]
            public string Type
            {
                get;
                set;
            }
            [DataMember]
            public string Key
            {
                get;
                set;
            }
        }
    }
    public class AjaxMessagesFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute    //TODO: return data only
    {
        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext filterContext)
        {
            var request = filterContext.Request;
            var headers = request.Headers;
            if (headers.Contains("X-Requested-With") && headers.GetValues("X-Requested-With").FirstOrDefault() == "XMLHttpRequest")
            {
                var response = filterContext.ActionContext.Response;
                if (response != null)
                {
                    List<SessionMessage> sessionMessages = SessionMessageManager.GetMessage();
                    if (sessionMessages != null && sessionMessages.Count > 0)
                    {
                        string json = null;
                        var messages = sessionMessages.Where(x => x.Behavior == MessageBehaviors.StatusBar).Select(x => new SessionMessageJsonModal { Message = x.Message, Type = Enum.GetName(typeof(MessageType), x.Type).ToLowerInvariant(), Key = x.Key }).ToList();
                        if (messages != null && messages.Count > 0)
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(messages.GetType());
                            using (MemoryStream ms = new MemoryStream())
                            {
                                ser.WriteObject(ms, messages);
                                json = Encoding.Default.GetString(ms.ToArray());
                                ms.Close();
                            }
                            response.Headers.Add("X-Message", json);
                        }
                        messages = sessionMessages.Where(x => x.Behavior == MessageBehaviors.Modal).Select(x => new SessionMessageJsonModal { Message = x.Message, Type = Enum.GetName(typeof(MessageType), x.Type).ToLowerInvariant(), Key = x.Key }).ToList();
                        if (messages != null && messages.Count > 0)
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(messages.GetType());
                            using (MemoryStream ms = new MemoryStream())
                            {
                                ser.WriteObject(ms, messages);
                                json = Encoding.Default.GetString(ms.ToArray());
                                ms.Close();
                            }
                            response.Headers.Add("X-ModalMessage", json);
                        }
                        SessionMessageManager.Clear();
                        //TagBuilder messageBoxBuilder = null, messageBoxModal = null, messageBoxStatusBar = null;
                        //for (int i = 0; i < sessionMessages.Count; i++)
                        //{
                        //    var sessionMessage = sessionMessages[i];
                        //    switch (sessionMessage.Behavior)
                        //    {
                        //        case MessageBehaviors.Modal:
                        //            if (messageBoxModal == null)
                        //            {
                        //                messageBoxModal = new TagBuilder("div");
                        //                messageBoxModal.Attributes.Add("id", "messageboxModal");
                        //                messageBoxModal.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                        //            }
                        //            messageBoxBuilder = new TagBuilder("div");
                        //            messageBoxBuilder.Attributes.Add("id", "messagebox" + i);
                        //            messageBoxBuilder.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                        //            messageBoxBuilder.AddCssClass(String.Format("messagebox {0}", Enum.GetName(typeof(MessageType), sessionMessage.Type).ToLowerInvariant()));
                        //            messageBoxBuilder.InnerHtml += sessionMessage.Message;
                        //            messageBoxModal.InnerHtml += messageBoxBuilder.ToString();
                        //            break;
                        //        case MessageBehaviors.StatusBar:
                        //        default:
                        //            if (messageBoxStatusBar == null)
                        //            {
                        //                messageBoxStatusBar = new TagBuilder("div");
                        //                messageBoxStatusBar.Attributes.Add("id", "messageboxStatusBar");
                        //                messageBoxStatusBar.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                        //            }
                        //            messageBoxBuilder = new TagBuilder("div");
                        //            messageBoxBuilder.Attributes.Add("id", "messagebox" + i);
                        //            messageBoxBuilder.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                        //            messageBoxBuilder.AddCssClass(String.Format("messagebox {0}", Enum.GetName(typeof(MessageType), sessionMessage.Type).ToLowerInvariant()));
                        //            messageBoxBuilder.InnerHtml += sessionMessage.Message;
                        //            messageBoxStatusBar.InnerHtml += messageBoxBuilder.ToString();
                        //            break;
                        //    }
                        //}
                        //if (messageBoxStatusBar != null && !string.IsNullOrEmpty(messageBoxStatusBar.ToString()))
                        //    response.Headers.Add("X-Message", messageBoxStatusBar.ToString());
                        //if (messageBoxModal != null && !string.IsNullOrEmpty(messageBoxModal.ToString()))
                        //    response.Headers.Add("X-ModalMessage", messageBoxModal.ToString());
                    }
                }
            }
        }
        [DataContract]
        private class SessionMessageJsonModal
        {
            [DataMember]
            public string Message
            {
                get;
                set;
            }
            [DataMember]
            public string Type
            {
                get;
                set;
            }
            [DataMember]
            public string Key
            {
                get;
                set;
            }
        }
    }
}