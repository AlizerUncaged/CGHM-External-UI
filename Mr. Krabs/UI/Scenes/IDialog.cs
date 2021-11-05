using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mr.Krabs.UI.Scenes {
    public interface IDialog {
        event EventHandler Closing;
    }
}
