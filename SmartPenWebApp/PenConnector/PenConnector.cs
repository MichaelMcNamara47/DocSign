using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Neosmartpen.Net;// | Framework providing interface for communication |
using Neosmartpen.Net.Bluetooth;// | Provide features for controlling Bluetooth communication |
using Neosmartpen.Net.Support;// | Provide features except communication(such as graphics ) |
using Neosmartpen.Net.Protocol.v1;// | Handling data and communication with peer device(protocol version is 1.xx )   |
using System.Diagnostics;
//using Neosmartpen.Net.Protocol.v2 | Handling data and communication with peer device(protocol version is 2.xx ) |

namespace SmartSignWebApp.PenConnector
{
    public class PenConnector : PenCommV1Callbacks
    {
        static PenCommV1 mPenCommV1;

        public void connectPen()
        {
            BluetoothAdapter mBtAdt = new BluetoothAdapter();
            PenDevice[] devices = mBtAdt.FindAllDevices();

            Debug.WriteLine("Pens discovered " + devices.Length);

            int deviceNumber = 0;
            foreach (PenDevice p in devices)
            {
                Debug.WriteLine("[" + deviceNumber + "]"
                    + "\t Name:" + p.Name
                    + "\t Address:" + p.Address
                    + "\t Authenticated:" + p.Authenticated
                    + "\t ClassOfDevice:" + p.ClassOfDevice
                    + "\t LastSeen:" + p.LastSeen
                    + "\t LastUsed:" + p.LastUsed
                    + "\t Remembered:" + p.Remembered
                    + "\t Rssi:" + p.Rssi
                    );
            }

            Debug.WriteLine("Type index of device to connect to: ");
            //int connectToDeviceNumber = (int)Debug.Read();
            Debug.WriteLine("Connecting to device 0");
            mPenCommV1 = new PenCommV1(new PenConnector());


            bool result = mBtAdt.Connect(devices.ElementAt(0).Address, delegate (uint deviceClass)
            {
                if (deviceClass == mPenCommV1.DeviceClass)
                {
                    mBtAdt.Bind(mPenCommV1);

                    // You can set the name of PenComm object in the following ways
                    // If you don't set the name of the PenComm, it is automatically set to the address of a connected pen.
                    mBtAdt.Bind(mPenCommV1, "name of PenComm");

                    // You can get or set a name of PenComm
                    // mBtAdt.Name = "name of PenComm";
                }
            });
        }

        public void onConnected(IPenComm sender, int maxForce, string firmwareVersion)
        {
            Debug.WriteLine("onConnected...\n");
            Debug.WriteLine("Connected...\n"
                + "\t DeviceClass:" + sender.DeviceClass
                + "\t Name:" + sender.Name
                + "\t Parser:" + sender.Parser
                + "\t Version:" + sender.Version
                );
            PenCommV1 pencomm = sender as PenCommV1;
        }


        public void onPenPasswordRequest(IPenComm sender, int retryCount, int resetCount)
        {
            Debug.WriteLine("onPenPasswordRequest...\n");

            Debug.WriteLine("Enter Passcode: 1111");
            //String passowrd = Debug.ReadLine();
            //mPenCommV1.ReqInputPassword(passowrd);
            mPenCommV1.ReqInputPassword("1111");
        }

        public void onPenAuthenticated(IPenComm sender)
        {
            Debug.WriteLine("onPenAuthenticated...\n");

            Debug.WriteLine("Authenticated");
            mPenCommV1.ReqAddUsingNote();
        }

        public void onReceivedPenStatus(IPenComm sender, int timeoffset, long timetick, int maxForce, int battery, int usedmem, int pencolor, bool autopowerMode, bool accelerationMode, bool hoverMode, bool beep, short autoshutdownTime, short penSensitivity, string modelName)
        {
            Debug.WriteLine("onReceivedPenStatus...\n\n\n");

            Debug.WriteLine("Connected...\n"
                + "\t DeviceClass:" + sender.DeviceClass
                + "\t Name:" + sender.Name
                + "\t Parser:" + sender.Parser
                + "\t Version:" + sender.Version

                + "\t timeoffset:" + timeoffset
                + "\t timetick:" + timetick
                + "\t maxForce:" + maxForce
                + "\t battery:" + battery
                + "\t usedmem:" + usedmem
                + "\t pencolor:" + pencolor
                + "\t autopowerMode:" + autopowerMode
                + "\t accelerationMode:" + accelerationMode
                + "\t hoverMode:" + hoverMode
                + "\t beep:" + beep
                + "\t autoshutdownTime:" + autoshutdownTime
                + "\t penSensitivity:" + penSensitivity
                + "\t modelName:" + modelName

                );
        }


        public void onReceiveDot(IPenComm sender, Dot dot)
        {
            Debug.WriteLine("onReceiveDot...\n");
            Debug.WriteLine("Dot...\n"
                + "\t Note:" + dot.Note
                + "\t Owner:" + dot.Owner
                + "\t Page:" + dot.Page
                + "\t Section:" + dot.Section
                + "\t Timestamp:" + dot.Timestamp
                + "\t Color:" + dot.Color
                + "\t DotType:" + dot.DotType
                + "\t Force:" + dot.Force
                + "\t Twist:" + dot.Twist
                + "\t Fx:" + dot.Fx
                + "\t Fy:" + dot.Fy
                + "\t TiltX:" + dot.TiltX
                + "\t TiltY:" + dot.TiltY
                + "\t X:" + dot.X
                + "\t Y:" + dot.Y
                );
            onTarget(dot);
        }

        public void onTarget(Dot dot)
        {
            if (dot.X == 10 && dot.Y == 10)
            {
                Debug.WriteLine("In boundary A\n");
            }

            if (dot.X == 15 && dot.Y == 10)
            {
                Debug.WriteLine("In boundary B\n");
            }
        }

        public void onDisconnected(IPenComm sender)
        {
            Debug.WriteLine("onReceiveDot...\n");
            throw new NotImplementedException();
        }

        public void onFinishedOfflineDownload(IPenComm sender, bool result)
        {
            throw new NotImplementedException();
        }

        public void onOfflineDataList(IPenComm sender, OfflineDataInfo[] offlineNotes)
        {
            throw new NotImplementedException();
        }

        public void onPenAutoPowerOnSetUpResponse(IPenComm sender, bool result)
        {
            throw new NotImplementedException();
        }

        public void onPenAutoShutdownTimeSetUpResponse(IPenComm sender, bool result)
        {
            throw new NotImplementedException();
        }

        public void onPenBeepSetUpResponse(IPenComm sender, bool result)
        {
            throw new NotImplementedException();
        }

        public void onPenColorSetUpResponse(IPenComm sender, bool result)
        {
            throw new NotImplementedException();
        }

        public void onPenHoverSetUpResponse(IPenComm sender, bool result)
        {
            throw new NotImplementedException();
        }

        public void onPenPasswordSetUpResponse(IPenComm sender, bool result)
        {
            throw new NotImplementedException();
        }

        public void onPenSensitivitySetUpResponse(IPenComm sender, bool result)
        {
            throw new NotImplementedException();
        }

        public void onReceivedFirmwareUpdateResult(IPenComm sender, bool result)
        {
            throw new NotImplementedException();
        }

        public void onReceivedFirmwareUpdateStatus(IPenComm sender, int total, int amountDone)
        {
            throw new NotImplementedException();
        }

        public void onReceiveOfflineStrokes(IPenComm sender, Stroke[] strokes)
        {
            throw new NotImplementedException();
        }

        public void onStartOfflineDownload(IPenComm sender)
        {
            throw new NotImplementedException();
        }

        public void onUpdateOfflineDownload(IPenComm sender, int total, int amountDone)
        {
            throw new NotImplementedException();
        }

        public void onUpDown(IPenComm sender, bool isUp)
        {
            throw new NotImplementedException();
        }

}
}
