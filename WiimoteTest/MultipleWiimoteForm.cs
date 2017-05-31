using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WiimoteLib;
using WiimoteLib.DataTypes;
using WiiMoteLib.Exceptions;

namespace WiimoteTest
{
	public partial class MultipleWiimoteForm : Form
	{
		// map a wiimote to a specific state user control dealie
		Dictionary<Guid,WiimoteInfo> mWiimoteMap = new Dictionary<Guid,WiimoteInfo>();
		WiimoteCollection mWC;

		public MultipleWiimoteForm()
		{
			InitializeComponent();
		    var wiimote = new Wiimote();
		    var test1 = wiimote.Load16bitMonoSampleWAV(Path.Combine(Environment.CurrentDirectory, "assets\\catfood.wav"), 4200);
		    var test2 = wiimote.Load16bitMonoSampleWAV(Path.Combine(Environment.CurrentDirectory, "assets\\project.wav"), 4200);
		    var test3 = wiimote.Load16bitMonoSampleWAV(Path.Combine(Environment.CurrentDirectory, "assets\\1kSine16 (3130).wav"));
		    var test4 = wiimote.Load16bitMonoSampleWAV(Path.Combine(Environment.CurrentDirectory, "assets\\Daisy16 (3130).wav"));
		}

		private void MultipleWiimoteForm_Load(object sender, EventArgs e)
		{
			// find all wiimotes connected to the system
			mWC = new WiimoteCollection();
			int index = 1;

			try
			{
				mWC.FindAllWiimotes();
			}
			catch(WiimoteNotFoundException ex)
			{
				MessageBox.Show(ex.Message, "Wiimote not found error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch(WiimoteException ex)
			{
				MessageBox.Show(ex.Message, "Wiimote error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Unknown error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			foreach(Wiimote wm in mWC)
			{
				// create a new tab
				TabPage tp = new TabPage("Wiimote " + index);
				tabWiimotes.TabPages.Add(tp);

				// create a new user control
				WiimoteInfo wi = new WiimoteInfo(wm);
				tp.Controls.Add(wi);

				// setup the map from this wiimote's ID to that control
				mWiimoteMap[wm.ID] = wi;

				// connect it and set it up as always
				wm.WiimoteChanged += wm_WiimoteChanged;
				wm.WiimoteExtensionChanged += wm_WiimoteExtensionChanged;

				wm.Connect();
				if(wm.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
					wm.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
				
				wm.SetLEDs(index++);
			}
		}

		void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs e)
		{
			WiimoteInfo wi = mWiimoteMap[((Wiimote)sender).ID];
			wi.UpdateState(e);
		}

		void wm_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs e)
		{
			// find the control for this Wiimote
			WiimoteInfo wi = mWiimoteMap[((Wiimote)sender).ID];
			wi.UpdateExtension(e);

			if(e.Inserted)
				((Wiimote)sender).SetReportType(InputReport.IRExtensionAccel, true);
			else
				((Wiimote)sender).SetReportType(InputReport.IRAccel, true);
		}

		private void MultipleWiimoteForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			foreach(Wiimote wm in mWC)
				wm.Disconnect();
		}
	}
}
