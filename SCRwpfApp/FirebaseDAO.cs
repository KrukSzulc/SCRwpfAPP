using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCRwpfApp
{
    interface FirebaseDAO
    {

        void POST(TaskSCR task);
        bool CHECK();
        string READ();
        TaskSCR GET();


    }
}
