using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace dialogowe_pkp
{
    public class PageHandler : System.Windows.Controls.Page
    {
        protected readonly Page previousPage;

        protected readonly Window window;

        public PageHandler() : base()
        {
        }

        public PageHandler(Window window) : this(window, null)
        {
        }

        public PageHandler(Window window, Page previousPage) : this()
        {
            this.window = window;

            this.previousPage = previousPage;
        }

        protected void ChangePage(Page page)
        {
            // TODO do all tasks async

            if (this is ISpeechSynthesis)
            {
                ((ISpeechSynthesis)this).StopSpeak();
            }

            if (this is ISpeechRecognize)
            {
                ((ISpeechRecognize)this).StopSpeechRecognition();
            }

            window.Content = page ?? throw new NullReferenceException("Page cannot be null.");

            if (page is ISpeechRecognize)
            {
                ((ISpeechRecognize)page).EnableSpeechRecognition();
            }
        }

        protected void Close()
        {
            window.Close();
        }

        protected void MoveBack()
        {
            ChangePage(previousPage);
        }
    }

}