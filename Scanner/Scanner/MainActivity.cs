using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;

namespace Scanner
{
    [Activity(Label = "Scanner", MainLauncher = true,  AlwaysRetainTaskState = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@drawable/icon")]
    //[IntentFilter(new string[] { "android.intent.category.DEFAULT" })]
    [IntentFilter(new string[] { Intent.ActionView }, Categories = new string[] { "android.intent.category.DEFAULT" })]	
    public class MainActivity : Activity
    {
        // This intent string contains the source of the data as a string  
        private static string SOURCE_TAG = "com.motorolasolutions.emdk.datawedge.source";
        // This intent string contains the barcode symbology as a string  
        private static string LABEL_TYPE_TAG = "com.motorolasolutions.emdk.datawedge.label_type";
        // This intent string contains the captured data as a string  
        // (in the case of MSR this data string contains a concatenation of the track data)  
        private static string DATA_STRING_TAG = "com.motorolasolutions.emdk.datawedge.data_string";

        private static string ourIntentAction = "Scanner.Scanner.RECVR";


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            EditText et = FindViewById<EditText>(Resource.Id.editbox);
            et.SetSelection(et.Text.Length);
           
            Intent i = Intent;
            handleDecodeData(i);
        }
        
        protected override void OnNewIntent(Intent i)
        {
            handleDecodeData(i);
        }

        private void handleDecodeData(Android.Content.Intent i)
        {
            // check the intent action is for us  
            if (i.Action.Equals(ourIntentAction))
            {
                // define a string that will hold our output  
                String Out = "";
                // get the source of the data  
                String source = i.GetStringExtra(SOURCE_TAG);
                // save it to use later  
                if (source == null) source = "scanner";
                // get the data from the intent  
                String data = i.GetStringExtra(DATA_STRING_TAG);
                // let's define a variable for the data length  


                int data_len = 0;
                // and set it to the length of the data  
                if (data != null) data_len = data.Length;
                // check if the data has come from the barcode scanner  
                if (source.Equals("scanner"))
                {
                    // check if there is anything in the data  
                    if (data != null && data.Length > 0)
                    {
                        // we have some data, so let's get it's symbology  
                        String sLabelType = i.GetStringExtra(LABEL_TYPE_TAG);
                        // check if the string is empty  
                        if (sLabelType != null && sLabelType.Length > 0)
                        {                        // format of the label type string is LABEL-TYPE-SYMBOLOGY  
                            // so let's skip the LABEL-TYPE- portion to get just the symbology  
                            sLabelType = sLabelType.Substring(11);
                        }
                        else
                        {
                            // the string was empty so let's set it to "Unknown"  
                            sLabelType = "Unknown";
                        }


                        // let's construct the beginning of our output string  
                        Out = "Source: Scanner, " + "Symbology: " + sLabelType + ", Length: " + data_len.ToString() + ", Data: ...\r\n";
                    }
                }
                // check if the data has come from the MSR  
                if (source.Equals("msr"))
                {
                    // construct the beginning of our output string  
                    Out = "Source: MSR, Length: " + data_len.ToString() + ", Data: ...\r\n";
                }


                // let's get our edit box view  
                EditText et = (EditText)FindViewById<EditText>(Resource.Id.editbox);
                // and get it's text into an editable string  


                // we need to put the edit box text into a spannable string builder  
                SpannableStringBuilder stringbuilder = new SpannableStringBuilder(et.Text);
                // add the output string we constructed earlier  
                stringbuilder.Append(Out);
                // now let's highlight our output string in bold type  
                //stringbuilder.SetSpan(new StyleSpan(Typeface.DefaultBold), et.Text.Length, stringbuilder.Length, SpannableString.SPAN_EXCLUSIVE_EXCLUSIVE);  
                // then add the barcode or msr data, plus a new line, and add it to the string builder  
                stringbuilder.Append(data + "\r\n");
                // now let's update the text in the edit box  
                et.SetText(stringbuilder.ToString(), null);
                // we want the text cursor to be at the end of the edit box  
                // so let's get the edit box text again  

                // and set the cursor position at the end of the text  
                et.SetSelection(et.Text.Length);
                // and we are done!  
            }
        }
    }
}

