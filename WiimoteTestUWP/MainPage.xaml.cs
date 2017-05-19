using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WiiMoteLibUWP;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WiimoteTestUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Dictionary<Guid, SingleWiimote> mWiimoteMap = new Dictionary<Guid, SingleWiimote>();
        WiimoteCollection mWC;
        public MainPage()
        {
            mWC = new WiimoteCollection();
            try
            {
                mWC.FindAllWiimotes();
            }
            catch (WiimoteNotFoundException ex)
            {
                throw new Exception("Wiimote not found", ex);
                //MessageBox.Show(ex.Message, "Wiimote not found error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (WiimoteException ex)
            {
                throw new Exception("Wiimote error", ex);
                //MessageBox.Show(ex.Message, "Wiimote error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                throw new Exception("Unknown error", ex);
                //MessageBox.Show(ex.Message, "Unknown error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.InitializeComponent();
            int index = 1;

            
            if (mWC.Count > 0)
            {
                this.Frame.Navigate(typeof(SingleWiimote), mWC[0]);
            }
            //            foreach (Wiimote wm in mWC)
            //            {
            //                // create a new tab
            //                PivotItem tp = new PivotItem();
            //                tp.Header = "Wiimote " + index;
            //                tabWiimotes.Items.Add(tp);
            //
            //                // create a new user control
            //                SingleWiimote wi = new SingleWiimote(wm);
            //                tp.Content = wi;
            //
            //                // setup the map from this wiimote's ID to that control
            //                mWiimoteMap[wm.ID] = wi;
            //            }
        }
    }
}
