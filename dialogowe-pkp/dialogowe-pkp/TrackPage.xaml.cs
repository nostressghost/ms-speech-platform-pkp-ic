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
    public partial class TrackPage : SpeechHandler
    {

        private List<StationRelation> stationRelation;
        private List<String> endStation;
        private List<String> startStation;
        private Order order;

        public TrackPage(Window window) : base(window)
        {
            InitializeComponent();

            stationRelation = new List<StationRelation>();
            stationRelation.Add(new StationRelation("Warszawa", "Lublin"));
            stationRelation.Add(new StationRelation("Katowice", "Lublin"));
            stationRelation.Add(new StationRelation("Warszawa", "Katowice"));
            stationRelation.Add(new StationRelation("Gdynia", "Lublin"));
            stationRelation.Add(new StationRelation("Lublin", "Sopot"));

            lvStations.ItemsSource = stationRelation;

            startStation = new List<String>();
            startStation.Add("Warszawa");
            startStation.Add("Katowice");
            startStation.Add("Gdynia");
            startStation.Add("Lublin");

            endStation = new List<String>();
            endStation.Add("Lublina");
            endStation.Add("Katowic");
            endStation.Add("Sopotu");
        }

        public override void InitializeSpeech(object sender, DoWorkEventArgs e)
        {
            base.InitializeSpeech(sender, e);

            SpeakHello();
        }

        private void StationListSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lvStations.SelectedItem != null)
            {
                TrackChoosed(lvStations.SelectedItem as StationRelation);
                ChangePage(new HoursPage(this.window, this.order));
            }
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

                DispatchAsync(() =>
                {
                    switch (command.First())
                    {
                        case "help":
                            SpeakHelp();
                            break;
                        case "completeorders":
                            TrackChoosed(stationRelation[int.Parse(command.Skip(1).First())]);
                            itemNameStationStart.Text = "Stacja początkowa to: " + order.From;
                            itemNameStationEnd.Text = "Stacja końcowa to: " + order.To;
                            SpeakConfirmation();

                            break;
                        case "incompleteorders":
                            EndStationChooded(endStation[int.Parse(command.Skip(1).First())]);
                            itemNameStationEnd.Text = "Stacja końcowa to: " + order.To;
                            SpeakQuestion();
                            break;
                        case "confirmations":
                            ChangePage(new HoursPage(window, order));
                            break;
                        case "startstations":
                            Console.WriteLine("asd");
                            if (order != null)
                            {
                                order.From = startStation[int.Parse(command.Skip(1).First())];
                                itemNameStationStart.Text = "Stacja początkowa to: " + order.From;
                                SpeakConfirmation();
                            }
                            break;
                        case "cancel":
                            CloseWindow();
                            break;
                    }
                });
            }
        }

        private void SpeakConfirmation()
        {
            Speak("Stacja początkowa to: " + order.From + " stacja końcowa to: " + order.To + " Czy potwierdzasz swój wybór? Jeżeli tak powiedz POTWIERDZAM WYBÓR");
        }

        private void SpeakQuestion()
        {
            Speak("Podaj nazwę stacji początkowej. Na przykład stacja początkowa to Warszawa.");
        }

        private void EndStationChooded(String endStation)
        {
            order = new Order(null, endStation);
        }

        private void TrackChoosed(StationRelation stationRelation)
        {
            order = new Order(stationRelation.From, stationRelation.To);
        }

        protected override void AddCustomSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
            AddCompleteOrderSpeechGrammarRules(srgsRules);
            AddInCompleteOrderSpeechGrammarRules(srgsRules);
            AddStartStationSpeechGrammarRules(srgsRules);
            AddConfirmationSpeechGrammarRules(srgsRules);
        }

        private void AddStartStationSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
            SrgsRule movieSrgsRule;

            {
                SrgsOneOf startStationSrgsOneOf = new SrgsOneOf();

                int i = 0;
                foreach (String station in startStation)
                {
                    SrgsItem srgsItem = new SrgsItem(station);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"startstations." + i++ + "\";"));

                    startStationSrgsOneOf.Add(srgsItem);
                }

                SrgsItem ticketSrgsItem = new SrgsItem();
                ticketSrgsItem.Add(new SrgsItem(0, 1, "Stacja początkowa to "));

                SrgsItem phraseSrgsItem2 = new SrgsItem();
                phraseSrgsItem2.Add(ticketSrgsItem);

                phraseSrgsItem2.Add(startStationSrgsOneOf);

                movieSrgsRule = new SrgsRule("startstation", phraseSrgsItem2);
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

        private void AddCompleteOrderSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
            SrgsRule movieSrgsRule;

            {
                SrgsOneOf startStationSrgsOneOf = new SrgsOneOf();

                int i = 0;
                foreach (StationRelation relation in stationRelation)
                {
                    SrgsItem srgsItem = new SrgsItem(relation.From + " " + relation.To);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"completeorders." + i++ + "\";"));

                    startStationSrgsOneOf.Add(srgsItem);
                }

                SrgsItem pleaseSrgsItem = new SrgsItem();
                SrgsOneOf pleaseSrgsOneOf = new SrgsOneOf();
                pleaseSrgsOneOf.Add(new SrgsItem("Chcę kupić"));
                pleaseSrgsOneOf.Add(new SrgsItem("Chciałbym kupić"));
                pleaseSrgsItem.Add(pleaseSrgsOneOf);

                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(pleaseSrgsItem);

                SrgsItem ticketSrgsItem = new SrgsItem();
                SrgsOneOf ticketSrgsOneOf = new SrgsOneOf();
                ticketSrgsOneOf.Add(new SrgsItem("bilet na pociąg relacji"));
                ticketSrgsOneOf.Add(new SrgsItem("bilety na pociąg relacji"));
                ticketSrgsItem.Add(ticketSrgsOneOf);

                SrgsItem phraseSrgsItem2 = new SrgsItem();
                phraseSrgsItem2.Add(ticketSrgsItem);

                phraseSrgsItem2.Add(startStationSrgsOneOf);

                movieSrgsRule = new SrgsRule("completeorder", phraseSrgsItem, phraseSrgsItem2);
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

        private void AddInCompleteOrderSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
            SrgsRule movieSrgsRule;

            {
                SrgsOneOf startStationSrgsOneOf = new SrgsOneOf();

                int i = 0;
                foreach (String station in endStation)
                {
                    SrgsItem srgsItem = new SrgsItem(station);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"incompleteorders." + i++ + "\";"));

                    startStationSrgsOneOf.Add(srgsItem);
                }

                SrgsItem pleaseSrgsItem = new SrgsItem();
                SrgsOneOf pleaseSrgsOneOf = new SrgsOneOf();
                pleaseSrgsOneOf.Add(new SrgsItem("Chcę kupić"));
                pleaseSrgsOneOf.Add(new SrgsItem("Chciałbym kupić"));
                pleaseSrgsItem.Add(pleaseSrgsOneOf);

                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(pleaseSrgsItem);

                SrgsItem ticketSrgsItem = new SrgsItem();
                SrgsOneOf ticketSrgsOneOf = new SrgsOneOf();
                ticketSrgsOneOf.Add(new SrgsItem("bilet"));
                ticketSrgsOneOf.Add(new SrgsItem("bilety"));

                ticketSrgsItem.Add(ticketSrgsOneOf);
                ticketSrgsItem.Add(new SrgsItem(0, 1, "na pociąg"));
                ticketSrgsItem.Add(new SrgsItem("do"));

                SrgsItem phraseSrgsItem2 = new SrgsItem();
                phraseSrgsItem2.Add(ticketSrgsItem);

                phraseSrgsItem2.Add(startStationSrgsOneOf);

                movieSrgsRule = new SrgsRule("incompleteorder", phraseSrgsItem, phraseSrgsItem2);
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

        private void AddConfirmationSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
            SrgsRule movieSrgsRule;

            {
                SrgsOneOf startStationSrgsOneOf = new SrgsOneOf();

                SrgsItem asd = new SrgsItem("Potwierdzam wybór");
                asd.Add(new SrgsSemanticInterpretationTag("out=\"confirmations." + 0 + "\";"));

                SrgsItem asd1 = new SrgsItem("Tak potwierdzam");
                asd1.Add(new SrgsSemanticInterpretationTag("out=\"confirmations." + 1 + "\";"));

                SrgsItem asd2 = new SrgsItem(new SrgsItem("Tak"));
                asd2.Add(new SrgsSemanticInterpretationTag("out=\"confirmations." + 2 + "\";"));


                startStationSrgsOneOf.Add(asd);
                startStationSrgsOneOf.Add(asd1);
                startStationSrgsOneOf.Add(asd2);

                SrgsItem phraseSrgsItem2 = new SrgsItem();
              

                phraseSrgsItem2.Add(startStationSrgsOneOf);

                movieSrgsRule = new SrgsRule("confirmation", phraseSrgsItem2);
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

        private void SpeakRepeat()
        {
            Speak("Nie rozumiem powiedz jeszcze raz.");
        }

        private void SpeakHello()
        {
            Speak("Podaj stację początkową oraz końcową. Powiedz POMOC w razie potrzeby. Aby zakończyc powiedz ANULUJ ZAMÓWIENIE.");
        }

        private void SpeakHelp()
        {
            Speak("Aby kupić bilet powiedz na przykład. Chcę kupić bilet na pociąg relacji Warszawa Lublin. Aby wyjść powiedz WYJDŹ.");
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
    }
}
