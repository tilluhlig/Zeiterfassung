using System;
using System.Collections.Generic;
using System.Text;

namespace Zeiterfassung
{
    public class Messung
    {
        public bool Zustand = false;
        public TimeSpan Heute = new TimeSpan();
        public TimeSpan Monat = new TimeSpan();

        public void Abschalten()
        {
            Zustand = false;
        }

        public void Einschalten()
        {
            Zustand = true;
        }
    }
}
