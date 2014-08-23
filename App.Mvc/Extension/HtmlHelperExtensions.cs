using System;
using System.Web;
using System.Web.Mvc;
using App.Common.SessionMessage;
using System.Collections.Generic;

namespace App.Mvc
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Render all messages that have been set during execution of the controller action.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static HtmlString RenderMessages(this HtmlHelper htmlHelper)
        {
            var messages = string.Empty;
            List<SessionMessage> sessionMessages = SessionMessageManager.GetMessage();
            if (sessionMessages != null && sessionMessages.Count > 0)
            {
                TagBuilder messageBoxBuilder = null, messageBoxStatusBar = null,messageBoxModalBuilder = null, messageBoxModal = null; ;
                for (int i = 0; i < sessionMessages.Count; i++)
                {
                    var sessionMessage = sessionMessages[i];
                    switch (sessionMessage.Behavior)
                    {
                        case MessageBehaviors.Modal:
                            if (messageBoxModal == null)
                            {
                                messageBoxModal = new TagBuilder("div");
                                messageBoxModal.Attributes.Add("id", "messageboxmodal");
                                //messageBoxModal.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                            }
                            messageBoxModalBuilder = new TagBuilder("div");
                            //messageBoxModalBuilder.Attributes.Add("id", "messagebox" + i);
                            //messageBoxModalBuilder.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                            messageBoxModalBuilder.AddCssClass(String.Format("messagebox {0}", Enum.GetName(typeof(MessageType), sessionMessage.Type).ToLowerInvariant()));
                            if(!string.IsNullOrEmpty(sessionMessage.Key))
                                messageBoxBuilder.Attributes.Add("key", sessionMessage.Key);
                            messageBoxModalBuilder.InnerHtml += sessionMessage.Message;
                            messageBoxModal.InnerHtml += messageBoxModalBuilder.ToString();
                            break;
                        case MessageBehaviors.StatusBar:
                        default:
                            if (messageBoxStatusBar == null)
                            {
                                messageBoxStatusBar = new TagBuilder("div");
                                messageBoxStatusBar.Attributes.Add("id", "messageboxstatusbar");
                                //messageBoxStatusBar.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                            }
                            messageBoxBuilder = new TagBuilder("div");
                            //messageBoxBuilder.Attributes.Add("id", "messagebox" + i);
                            //messageBoxBuilder.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                            messageBoxBuilder.AddCssClass(String.Format("messagebox {0}", Enum.GetName(typeof(MessageType), sessionMessage.Type).ToLowerInvariant()));
                            if(!string.IsNullOrEmpty(sessionMessage.Key))
                                messageBoxBuilder.Attributes.Add("key", sessionMessage.Key);
                            messageBoxBuilder.InnerHtml += sessionMessage.Message;
                            messageBoxStatusBar.InnerHtml += messageBoxBuilder.ToString();
                            break;
                    }
                }
                messages = messageBoxStatusBar == null || string.IsNullOrEmpty(messageBoxStatusBar.ToString()) ? null : messageBoxStatusBar.ToString()+
                    (messageBoxModal == null || string.IsNullOrEmpty(messageBoxModal.ToString()) ? null : messageBoxModal.ToString());
                SessionMessageManager.Clear();
            }
            return MvcHtmlString.Create(messages);
        }
        //public static HtmlString RenderModalMessages(this HtmlHelper htmlHelper)    //TODO: Handled in actionfilter
        //{
        //    var messages = string.Empty;
        //    List<SessionMessage> sessionMessages = SessionMessageManager.GetMessage();
        //    if (sessionMessages != null && sessionMessages.Count > 0)
        //    {
        //        TagBuilder messageBoxBuilder = null, messageBoxModal = null;
        //        for (int i = 0; i < sessionMessages.Count; i++)
        //        {
        //            var sessionMessage = sessionMessages[i];
        //            switch (sessionMessage.Behavior)
        //            {
        //                case MessageBehaviors.Modal:
        //                    //messageBoxBuilder.InnerHtml = string.Format("<img src='{0}' />{1}", VirtualPathUtility.ToAbsolute(string.Format("~/Content/Images/{0}.gif", sessionMessage.Type.ToString())), sessionMessage.Message);
        //                    //break;
        //                    if (messageBoxModal == null)
        //                    {
        //                        messageBoxModal = new TagBuilder("div");
        //                        messageBoxModal.Attributes.Add("id", "messageboxmodal");
        //                        messageBoxModal.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
        //                    }
        //                    messageBoxBuilder = new TagBuilder("div");
        //                    messageBoxBuilder.Attributes.Add("id", "messagebox" + i);
        //                    messageBoxBuilder.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
        //                    messageBoxBuilder.AddCssClass(String.Format("messagebox {0}", Enum.GetName(typeof(MessageType), sessionMessage.Type).ToLowerInvariant()));
        //                    messageBoxBuilder.InnerHtml += sessionMessage.Message;
        //                    messageBoxModal.InnerHtml += messageBoxBuilder.ToString();
        //                    break;
        //            }
        //        }
        //        messages += messageBoxModal == null || string.IsNullOrEmpty(messageBoxModal.ToString()) ? null : messageBoxModal.ToString();
        //        SessionMessageManager.Clear();
        //    }
        //    return MvcHtmlString.Create(messages);
        //}

    }
}