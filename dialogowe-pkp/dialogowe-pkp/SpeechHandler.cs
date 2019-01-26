using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace dialogowe_pkp
{
    public class SpeechHandler : PageHandler, ISpeechRecognize, ISpeechSynthesis
    {
        private SpeechRecognitionEngine speechRecognitionEngine;
        private SpeechSynthesizer speechSynthesizer;

        public SpeechHandler() : this(null, null)
        {
        }

        public SpeechHandler(Window window) : this(window, null)
        {
        }

        public SpeechHandler(Window window, Page previousPage) : base(window, previousPage)
        {
            ExecuteBackgroundAction(InitializeSpeech);
        }

        protected void ExecuteBackgroundAction(DoWorkEventHandler action)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += action;
            backgroundWorker.RunWorkerAsync();
        }

        public virtual void InitializeSpeech(object sender, DoWorkEventArgs e)
        {
            InitializeSpeechSynthesis();

            InitializeSpeechRecognition();

            EnableSpeechRecognition();
        }

        public void InitializeSpeechRecognition()
        {
            CultureInfo cultureInfo = new CultureInfo("pl-PL");

            speechRecognitionEngine = new SpeechRecognitionEngine(cultureInfo);
            speechRecognitionEngine.LoadGrammarAsync(GetSpeechGrammar());
            speechRecognitionEngine.SetInputToDefaultAudioDevice();
            speechRecognitionEngine.SpeechRecognized += SpeechRecognitionEngine_SpeechRecognized;
        }

        protected virtual void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            RecognitionResult result = e.Result;

            Console.WriteLine(GetType().Name + "[" + result.Semantics.Value + "] " + result.Text + " (" + result.Confidence + ")");
        }

        public void InitializeSpeechSynthesis()
        {
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SetOutputToDefaultAudioDevice();
        }

        public void EnableSpeechRecognition()
        {
            try
            {
                speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        public Grammar GetSpeechGrammar()
        {
            SrgsDocument srgsDocument = new SrgsDocument("./Resources/" + GetType().Name + ".srgs");

            AddCustomSpeechGrammarRules(srgsDocument.Rules);

            return new Grammar(srgsDocument);
        }

        protected virtual void AddCustomSpeechGrammarRules(SrgsRulesCollection rules)
        {
        }
        public void Speak(string message)
        {
            StopSpeechRecognition();

            try
            {
                speechSynthesizer.Speak(message);

                EnableSpeechRecognition();
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void StopSpeak()
        {
            speechSynthesizer.SpeakAsyncCancelAll();
        }

        public void StopSpeechRecognition()
        {
            try
            {
                speechRecognitionEngine.RecognizeAsyncCancel();
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        public void WaitForSpeechRecognition()
        {
            try
            {
                speechRecognitionEngine.RecognizeAsyncStop();
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        protected void DispatchAsync(Action action)
        {
            Dispatcher.BeginInvoke(action);
        }

        protected void DispatchSync(Action action)
        {
            Dispatcher.Invoke(action);
        }
    }
}
