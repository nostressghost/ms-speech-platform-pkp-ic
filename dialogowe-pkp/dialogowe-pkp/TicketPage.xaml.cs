using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
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
    public partial class TicketPage : SpeechHandler
    {
        List<SeatsQuantity> seats;
        private Order Order;

        public TicketPage(Window window, Order order) : base(window)
        {
            InitializeComponent();
            Order = order;

            seats = new List<SeatsQuantity>();
            seats = new List<SeatsQuantity>();
            seats = new List<SeatsQuantity>();
            seats.Add(new SeatsQuantity(1));
            seats.Add(new SeatsQuantity(2));
            seats.Add(new SeatsQuantity(3));
            seats.Add(new SeatsQuantity(4));

            lbSeatsList.ItemsSource = seats;
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
                string[] command = result.Semantics.Value.ToString().ToLower().Split('.');

                switch (command.First())
                {
                    case "help":
                        SpeakHelp();
                        break;
                    case "ticket":
                        seatsChoosed(seats[int.Parse(command.Skip(1).First())]);
                        break;
                    case "quit":
                        CloseWindow();
                        break;
                }
            }
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

        private void SpeakRepeat()
        {
            Speak("Nie rozumiem powiedz jeszcze raz.");
        }

        private void SpeakHelp()
        {
            Speak("Aby wybrać liczbę miejsc powiedz na przykład CHCĘ JEDEN BILET. Aby anulować zamówienie powiedz WYJDŹ.");
        }

        protected override void AddCustomSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
            AddTicketsSpeechGrammarRules(srgsRules);
        }

        private void AddTicketsSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
                SrgsRule movieSrgsRule;

                {
                    SrgsOneOf quantitySrgsOneOf = new SrgsOneOf();

                    int i = 0;
                    foreach (SeatsQuantity ticketQuantity in seats)
                    {
                        SrgsItem srgsItem = new SrgsItem(ticketQuantity.Quantity.ToString());
                        srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"ticket." + i++ + "\";"));

                        quantitySrgsOneOf.Add(srgsItem);
                    }

                    SrgsItem quantitySrgsItem = new SrgsItem();
                    SrgsItem ticketSrgsItem = new SrgsItem();

                    SrgsOneOf srgsOneOf = new SrgsOneOf();
                    srgsOneOf.Add(new SrgsItem("Chcę"));
                    srgsOneOf.Add(new SrgsItem("Poproszę"));
                    srgsOneOf.Add(new SrgsItem("Chciałbym"));
                    quantitySrgsItem.Add(srgsOneOf);

                    SrgsOneOf ticketSrgsOneOf = new SrgsOneOf();
                    ticketSrgsOneOf.Add(new SrgsItem("bilet"));
                    ticketSrgsOneOf.Add(new SrgsItem("bilety"));
                    ticketSrgsItem.Add(ticketSrgsOneOf);

                    SrgsItem phraseSrgsItem = new SrgsItem();
                    phraseSrgsItem.Add(quantitySrgsItem);
                    phraseSrgsItem.Add(quantitySrgsOneOf);
                    phraseSrgsItem.Add(ticketSrgsItem);

                    movieSrgsRule = new SrgsRule("ticket", phraseSrgsItem);
                }

                srgsRules.Add(movieSrgsRule);

                {
                    SrgsItem srgsItem = new SrgsItem();
                    srgsItem.Add(new SrgsRuleRef(movieSrgsRule));

                    SrgsRule rootSrgsRule = srgsRules.Where(rule => rule.Id == "root").First();
                    SrgsOneOf srgsOneOf = (SrgsOneOf)rootSrgsRule.Elements.Where(element => element is SrgsOneOf).First();
                    srgsOneOf.Add(srgsItem);
                }
        }

        private void seatsChoosed(SeatsQuantity seatsQuantity)
        {
            Order.Quantity = seatsQuantity.Quantity;
            Speak("Wybrałeś " + Order.Quantity + " miejsca");
            DispatchAsync(asd);
            
        }

        private void asd()
        {
            this.ChangePage(new SummaryPage(this.window, Order));
        }

        private void SeatsListSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lbSeatsList.SelectedItem != null)
            {
                seatsChoosed(lbSeatsList.SelectedItem as SeatsQuantity);
            }
        }

        private void SpeakHello()
        {
            Speak("Podaj liczbę miejsc");
        }
    }
}
