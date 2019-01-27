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
    public partial class HoursPage : SpeechHandler
    {
        private DbConnector Connector;
        private List<Hour> hours;
        private Order Order;

        public HoursPage(Window window, Order order, DbConnector connector) : base(window)
        {
            InitializeComponent();

            Order = order;
            Connector = connector;

            hours = connector.getHours();
            lbHourList.ItemsSource = hours;
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

            Console.WriteLine(GetType().Name + "[" + result.Semantics.Value + "] " + result.Text + " (" + result.Confidence + ")");

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
                        case "hours":
                            hourChoosed(hours[int.Parse(command.Skip(1).First())]);
                            break;
                        case "quit":
                            CloseWindow();
                            break;
                    }
                });
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

        private void SpeakHelp()
        {
            Speak("Aby wybrać godzinę odjazdu powiedz. Chcę bilet na godzinę. Aby anulować zamówienie powiedz wyjdź.");
        }

        protected override void AddCustomSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
            AddHoursSpeechGrammarRules(srgsRules);
        }

        private void AddHoursSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {

            if ((hours != null) && (hours.Count > 0))
            {
                SrgsRule hoursSrgsRule;

                {
                    SrgsOneOf hourSrgsOneOf = new SrgsOneOf();

                    int i = 0;
                    foreach (Hour hour in hours)
                    {
                        SrgsItem srgsItem = new SrgsItem(hour.Value);
                        srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"hours." + i++ + "\";"));

                        hourSrgsOneOf.Add(srgsItem);
                    }

                    SrgsItem hourSrgsItem = new SrgsItem();
                    SrgsOneOf srgsOneOf = new SrgsOneOf();
                    srgsOneOf.Add(new SrgsItem("Chcę bilet"));
                    srgsOneOf.Add(new SrgsItem("Poproszę bilet"));
                    hourSrgsItem.Add(srgsOneOf);
                    hourSrgsItem.Add(new SrgsItem(0, 1, "na godzinę"));

                    SrgsItem phraseSrgsItem = new SrgsItem();
                    phraseSrgsItem.Add(hourSrgsItem);
                    phraseSrgsItem.Add(hourSrgsOneOf);

                    hoursSrgsRule = new SrgsRule("hour", phraseSrgsItem);
                }

                srgsRules.Add(hoursSrgsRule);

                {
                    SrgsItem srgsItem = new SrgsItem();
                    srgsItem.Add(new SrgsRuleRef(hoursSrgsRule));

                    SrgsRule rootSrgsRule = srgsRules.Where(rule => rule.Id == "root").First();
                    SrgsOneOf srgsOneOf = (SrgsOneOf)rootSrgsRule.Elements.Where(element => element is SrgsOneOf).First();
                    srgsOneOf.Add(srgsItem);
                }
            }
        }

        private void SpeakHello()
        {
            String toSay = "Godziny odjazdów to: ";

            foreach (Hour hour in hours)
            {
                toSay += hour.Value + " ";
            }

            Speak(toSay);
            Speak("W razie problemów powiedz POMOC.");
        }

        private void SpeakRepeat()
        {
            Speak("Nie rozumiem powiedz jeszcze raz.");
        }

        public void HourListSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            hourChoosed(lbHourList.SelectedItem as Hour);
        }

        private void hourChoosed(Hour hour)
        {
            Order.Hour = hour.Value;
            Speak("Wybrałeś bilet na" + hour.Value);
            ChangePage(new TicketPage(window, Order)); 
        }
    }
}
