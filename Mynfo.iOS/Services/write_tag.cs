using CoreFoundation;
using CoreNFC;
using Foundation;
using Mynfo.iOS.Services;
using Mynfo.ViewModels;
using Mynfo.Views;
using Plugin.NFC;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using static Mynfo.Views.TAGPage;

[assembly: Dependency(typeof(write_tag))]
namespace Mynfo.iOS.Services
{
    public class write_tag : NFCNdefReaderSessionDelegate, IBackgroundDependency
    {        
        public static NFCNdefReaderSession _tagSession;        
        public static TaskCompletionSource<string> _tcs;

        public Task ScanWriteAsync()
        {
            if (!NFCNdefReaderSession.ReadingAvailable)
            {
                throw new InvalidOperationException("Reading NDEF is not available");
            }
            _tcs = new TaskCompletionSource<string>();
            var pollingOption = NFCPollingOption.Iso14443;
            _tagSession = new NFCNdefReaderSession(this, DispatchQueue.CurrentQueue, true)
            {
                AlertMessage = "Writing",
            };
            _tagSession.BeginSession();
            return _tcs.Task;
        }

        public override void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
        }
        [Foundation.Export("readerSession:didDetectTags:")]
        [Foundation.Preserve(Conditional = true)]
        public override void DidDetectTags(NFCNdefReaderSession session, INFCNdefTag[] tags)
        {
            try
            {
                var nFCNdefTag = tags[0];
                session.ConnectToTag(nFCNdefTag, CompletionHandler);
                string dominio = "http://boxweb.azurewebsites.net/";
                string user = MainViewModel.GetInstance().User.UserId.ToString();
                string tag_id = "";
                string url = dominio + "index3.aspx?user_id=" + user + "&tag_id=" + tag_id;
                NFCNdefPayload payload = NFCNdefPayload.CreateWellKnownTypePayload(url);
                NFCNdefMessage nFCNdefMessage = new NFCNdefMessage(new NFCNdefPayload[] { payload });
                nFCNdefTag.WriteNdef(nFCNdefMessage, delegate
                {
                    session.InvalidateSession();
                });
                //Task task = App.DisplayAlertAsync(user_id_tag);

                //AppDelegate.user_id_tag = "?";
                //PopupNavigation.Instance.PopAsync(); 
                
                
                //session.InvalidateSession();
                //session.Dispose(); 
            } 
            catch (Exception ex)
            { 
                Console.WriteLine(ex);
                session.Dispose();
                session.InvalidateSession();
            }
            
            MainViewModel.GetInstance().Home = new HomeViewModel();
            Xamarin.Forms.Application.Current.MainPage = new MasterPage(); 
        }

        string GetRecords(NFCNdefPayload[] records)
        {
            string record = null;
            var results = new NFCNdefRecord[records.Length];
            for (var i = 0; i < records.Length; i++)
            {
                record = records[i].Payload.ToString();
            }
            return record;
        }
        public static void CompletionHandler(NSError obj)
        {
            //add code here
        }
        public override void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            
            session.InvalidateSession();
            session.Dispose();
            _tagSession.InvalidateSession();
        }

        public static bool modo_escritura = false;

        public void ExecuteCommand()
        {
            //modo_escritura = true;
            //PopupNavigation.Instance.PushAsync(new ConfigStikerPage());

            //AppDelegate j = new AppDelegate();

            //j.invoke_lector();
            ScanWriteAsync();
        }
    }
}



/*
 public void DidDetectTags(NFCNdefReaderSession session, INFCNdefTag[] tags)
        {
            string user_id_tag = AppDelegate.user_id_tag;
            if (user_id_tag == "?") 
            {
                Task task = App.DisplayAlertAsync(Helpers.Languages.ScanTAGFirst);
                session.InvalidateSession();
                session.Dispose();
                AppDelegate.user_id_tag = "?";
            }
            else
            {
                if ((Convert.ToInt32(user_id_tag) == Convert.ToInt32(MainViewModel.GetInstance().User.UserId.ToString())) || (0 == Convert.ToInt32(user_id_tag)))
                {
                    var nFCNdefTag = tags[0];
                    session.ConnectToTag(nFCNdefTag, CompletionHandler);
                    string dominio = "http://boxweb.azurewebsites.net/";
                    string user = MainViewModel.GetInstance().User.UserId.ToString();
                    string tag_id = "";
                    string url = dominio + "index3.aspx?user_id=" + user + "&tag_id=" + tag_id;
                    NFCNdefPayload payload = NFCNdefPayload.CreateWellKnownTypePayload(url);
                    NFCNdefMessage nFCNdefMessage = new NFCNdefMessage(new NFCNdefPayload[] { payload });
                    nFCNdefTag.WriteNdef(nFCNdefMessage, delegate
                    {
                        session.Dispose();
                        session.InvalidateSession();
                    });
                    //Task task = App.DisplayAlertAsync(user_id_tag);
                }
                else
                {
                    Task task2 = App.DisplayAlertAsync(Helpers.Languages.TAGUsed);                    
                    session.Dispose();
                    session.InvalidateSession();
                    AppDelegate.user_id_tag = "?";
                }
                AppDelegate.user_id_tag = "?";
                PopupNavigation.Instance.PopAsync();
                session.InvalidateSession();
                _tagSession.InvalidateSession();
                //PopupNavigation.Instance.PushAsync(new Stickerconfig());
                //Thread.Sleep(4000);
                //PopupNavigation.Instance.PopAsync();
                // session.Dispose();     
            }            
        }
 */

