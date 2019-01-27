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
    public partial class SummaryPage : SpeechHandler
    {
        private Order Order;
        private DbConnector connector;

        public SummaryPage(Window window, Order order) : base(window)
        {
            InitializeComponent();
            Order = order;

            itemNameStationStart.Text = "Stacja początkowa: " + order.From;
            itemNameStationEnd.Text = "Stacja końcowa: " + order.To;
            itemNameHours.Text = "Godzina odjazdu: " + order.Hour;
            itemNameStationSeats.Text = "Liczba miejsc: " + order.Quantity;

            connector = new DbConnector();
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
                    case "last":
                        connector.saveOrderToDatabase(Order);
                        SpeakTYP();
                        break;

                    case "cancel":
                        CloseWindow();
                        break;
                }
            }
        }

        private void SpeakRepeat()
        {
            Speak("Nie rozumiem powiedz jeszcze raz.");
        }

        private void SpeakTYP()
        {
            Speak("Dziękuję za złożenie zamówienia");
            DispatchAsync(System.Windows.Application.Current.Shutdown);
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

        private void SpeakHello()
        {
            Speak("Podsumowanie rezerwacji.");
            Speak("Stacja początkowa to: " + Order.From);
            Speak("Stacja końcowa to: " + Order.To);
            Speak("Godzina odjazdu to: " + Order.Hour);
            Speak("Liczba zarezerwowanych miejsc to: " + Order.Quantity);
            Speak("Jeżeli rezerwacja jest poprawna powiedz POTWIERDZAM REZERWACJĘ, w innym przypadku powiedz ANULUJ");
        }
    }
}
