using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dialogowe_pkp
{
    public partial class WelcomePage : SpeechHandler
    {
        public WelcomePage(Window window) : base(window)
        {
            InitializeComponent();
        }

        public override void InitializeSpeech(object sender, DoWorkEventArgs e)
        {
            base.InitializeSpeech(sender, e);

            SpeakHello();
        }

        protected override void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            base.SpeechRecognitionEngine_SpeechRecognized(sender, e);

            RecognitionResult result = e.Result;

            if (result.Confidence < 0.6)
            {
                SpeakRepeat();
            }
            else
            {
                string command = result.Semantics.Value.ToString().ToLower();
                switch (command)
                {
                    case "help":
                        SpeakHelp();
                        break;
                    case "order":
                        DispatchAsync(MoveToOrderPage);
                        break;
                    case "quit":
                        CloseWindow();
                        break;
                }
            }
        }

        private void MoveToOrderPage()
        {
            this.ChangePage(new TrackPage(this.window));
        }
    
        private void SpeakRepeat()
        {
            Speak("Nie rozumiem powiedz jeszcze raz.");
        }

        private void SpeakHello()
        {
            Speak("Witaj w biletomacie PKP gdzie możesz kupić bilety. Powiedz POMOC w razie potrzeby. Aby zakończyć powiedz WYJDŹ");
        }


        private void SpeakHelp()
        {
            Speak("Aby kupić bilet powiedz CHCĘ KUPIĆ BILET. Aby zakończyć powiedz WYJDŹ.");
        }

        private void SpeakQuit()
        {
            Speak("Zapraszam ponownie.");
        }

        private void CloseWindow()
        {
            SpeakQuit();
            DispatchAsync(System.Windows.Application.Current.Shutdown);
        }

        private void orderButtonClick(object sender, RoutedEventArgs e)
        {
            DispatchAsync(MoveToOrderPage);
        }

        private void helpButtonClick(object sender, RoutedEventArgs e)
        {
            SpeakHelp();
        }

        private void quitButtonClick(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
    }
}
