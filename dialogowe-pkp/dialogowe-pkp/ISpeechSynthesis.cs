using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dialogowe_pkp
{
    interface ISpeechSynthesis
    {
        void InitializeSpeechSynthesis();
        void Speak(String message);
        void StopSpeak();
    }
}
