﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Neosmartpen.Net;// | Framework providing interface for communication |
using Neosmartpen.Net.Bluetooth;// | Provide features for controlling Bluetooth communication |
using Neosmartpen.Net.Support;// | Provide features except communication(such as graphics ) |
using Neosmartpen.Net.Protocol.v1;// | Handling data and communication with peer device(protocol version is 1.xx )   |
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
//using Neosmartpen.Net.Protocol.v2 | Handling data and communication with peer device(protocol version is 2.xx ) |

namespace SmartSignWebApp.PenConnector

{
    public class PenConnector : PenCommV1Callbacks
    {
        
        private PenCommV1 mPenCommV1;
        public BluetoothAdapter mBtAdt;
        public static int numInstances = 0;

        public PressureFilter mFilter;

        //a4 600dpi 4960px x 7016px
        static int w = 800;//4960; //800;
        static int h = 1131;//7016;//1131;
        private Bitmap mBitmap;// = new Bitmap(w, h);

        public Signature mSig { get; set; }

        private Stroke mStroke;
        private IHostingEnvironment _hostingEnvironment;

        public PenConnector(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            initImage();
            mSig = new Signature();
        }

        internal void DrawSignature()
        {
            mSig.ForEach(delegate (Stroke s) {
                DrawStroke(s);
                Debug.WriteLine(s);
            });
            saveImage();
        }

        public void connectPen()
        {
            //mSig = new Signature();
            mBtAdt = new BluetoothAdapter();
            Thread thread = new Thread(unused =>
            {
                PenDevice[] devices = mBtAdt.FindAllDevices();
                Debug.WriteLine("Pens discovered " + devices.Length);

                if (devices.Length > 0)
                {

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

                    Debug.WriteLine("Connecting to pen...");
                    //mPenCommV1 = new PenCommV1(new PenConnector(_hostingEnvironment));
                    mPenCommV1 = new PenCommV1(this);
                    

                    bool result = mBtAdt.Connect(devices.ElementAt(0).Address, delegate (uint deviceClass)
                    {
                        if (deviceClass == mPenCommV1.DeviceClass)
                        {
                            mBtAdt.Bind(mPenCommV1);

                            // You can set the name of PenComm object in the following ways
                            // If you don't set the name of the PenComm, it is automatically set to the address of a connected pen.
                            
                            //mBtAdt.Bind(mPenCommV1, "name of PenComm");
                        
                            // You can get or set a name of PenComm
                            // mBtAdt.Name = "name of PenComm";
                        }
                    });
                }
                else
                {
                    Debug.WriteLine("Pen Not Found...");
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        internal void ClearImage()
        {
            initImage();
            saveImage();
        }
        internal void ClearSignature() {

            if (mSig != null) { mSig.Clear(); }
        }




        private void saveImage()
        {
            mBitmap.Save(System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "img/pen/web.png"));
            //mBitmap.Save(System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "web.png"));
            //mBitmap.Save(@"C:\Users\Uver\Documents\NUIG\Semester 2\Project\Images\web.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private void initImage()
        {
            if (mBitmap != null) {
                mBitmap.Dispose();
            }
            mBitmap = null;

            mBitmap = new Bitmap(w, h);
            
            Graphics g = Graphics.FromImage(mBitmap);
            g.Clear(Color.Transparent);
            g.Dispose();

        }

    public void onConnected(IPenComm sender, int maxForce, string firmwareVersion)
        {
            Debug.WriteLine("Pen Max Force = " + maxForce);
            mFilter = new PressureFilter(maxForce);



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
            //mPenCommV1.ReqOfflineDataList();
            mPenCommV1.ReqPenStatus();

        }

        public void onReceivedPenStatus(IPenComm sender, int timeoffset, long timetick, int maxForce, int battery, int usedmem, int pencolor, bool autopowerMode, bool accelerationMode, bool hoverMode, bool beep, short autoshutdownTime, short penSensitivity, string modelName)
        {
            Debug.WriteLine("\n\n\nonReceivedPenStatus...");

            Debug.WriteLine("Connected....\n"
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
            //Debug.WriteLine("onReceiveDot...\n");
            //Debug.WriteLine("Dot...\n"

            //    + "\t Timestamp:" + dot.Timestamp

            //    + "\t DotType:" + dot.DotType

            //    );
            ////onTarget(dot);
            ProcessDot(dot);
        }

        public void printDot(Dot dot) {
            Debug.WriteLine("Type:"+dot.DotType + "\nX=" + dot.X + "\t Y=" + dot.Y + "\nfx=" + dot.Fx + "\tfy=" + dot.Fy);
        }

        private void ProcessDot(Dot dot)
        {
            
            dot.Force = mFilter.Filter(dot.Force);

            // TODO: Drawing sample code
            if (dot.DotType == DotTypes.PEN_DOWN)
            {
                mStroke = new Stroke(dot.Section, dot.Owner, dot.Note, dot.Page);
                mStroke.Add(dot);
                printDot(dot);
            }
            else if (dot.DotType == DotTypes.PEN_MOVE)
            {
                mStroke.Add(dot);
            }
            else if (dot.DotType == DotTypes.PEN_UP)
            {
                mStroke.Add(dot);
                
                //DrawStroke(mStroke);

                mSig.Add(mStroke);

                ClearImage();

                DrawSignature();

                //Debug.WriteLine("Number of lines: " + mSig.Count);

                mFilter.Reset();

                //saveImage();
            }
        }



        private void DrawStroke(Stroke stroke)
        {

            float qx = 91f; //88.88f;
            float qy = 119.5f;
            int dx = 1;// 56;// (int)((1 * w) / qx);    //offset x
            int dy = -6;// 56;//(int)((1 * h) / qy);   //offset y
            float scalex = (float)(w / qx);
            float scaley = (float)(h / qy);
            /*
             * Dot.X, Dot.Y Coordinates of our NCode cell.( NCode's cell size is 2.371mm )
             * Dot.Fx, Dot.Fy It is fractional part of NCode Coordinates. ( maximum value is 100 )
             * How to get millimeter unit from NCode unit ( Dot.X + Dot.Fx x 0.01) x 2.371 = millimeter unit 
             * 
             * A4 210mm x 297mm
             * 
             * ------------------
             * (210 / 600) * 56 = 19.6
             * (297 / 600) * 56 = 27.72
             * ------------------
             * (83/600
             * 
             * ------------------
             * //  609 Idea Pad ncu sizes: DX:5.52, DY:16.10, Width:88.88 Height:125.65
                    int dx = (int)((5.52 * mWidth) / 88.88);
                    int dy = (int)((16.10 * mHeight) / 125.65);

                        float qx = 88.88f;
            float qy = 125.65f;
            int dx = (int)((5.52 * w) / qx);    //offset x
            int dy = (int)((16.10 * h) / qy);   //offset y
            float scalex = (float)(w / qx);
            float scaley = (float)(h / qy);
             * ------------------
             * 
             * 
             * Scale = 72 / 600 * 56 = 6.72
             * nproj size /scale = ncode size
             * ex) 500 height in nproj file, 500/Scale = 500/6.72 = 74.4 (ncode unit)
             * 
             * Ref:
             * ● 2.37mm = (56 pixels / 600 dpi) * 25.4
             * ● On 1200 DPI, output was designed to maintain the same cell size (2.37 mm)
             * ● Therefore, the calculation of cell coordinates received from a Pen will use the same formula for both 600 DPI and 1200 DPI.
             */
            /*
             * 
             float qx = 88.88f;
             float qy = 125.70f;
             int dx = (int)((5.52 * w) / qx);
             int dy = (int)((5.41 * h) / qy);
                                dx      dy      width   heigth
            601 Pocket Note     5.40    5.45    35.26   62.59 
             */


            Renderer.draw(
                   mBitmap,                        //bitmap
                   stroke,                         //stroke
                   (float)(w / qx),                //scale x
                   (float)(h / qy),                //scale y
                   -dx,                            //offset x
                   -dy,                            //offset y
                   1,                              //width
                   Color.FromArgb(200, Color.Blue) //color
                   );
            

               /*
               mBitmap.Save(System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "web.png"));
               mBitmap.Save(@"C:\Users\Uver\Documents\NUIG\Semester 2\Project\Images\web.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
               */
            //var image = pictureBox1.Image = mBitmap;
            //added
            //pictureBox1.Image.Save(@"C:\Users\Uver\Documents\NUIG\Semester 2\Project\Images\save1.png", System.Drawing.Imaging.ImageFormat.Jpeg);

            /*
            this.Invoke(new MethodInvoker(delegate ()
            {
                lock (mDrawLock)
                {
                    //614 N_A4 210x297
                    // A4 Dimensions in inch w8.27 * h11.69
                    //603 Ring Note Height  5.52  5.41 	63.46 	88.88  (dimensions 150 x 210)
                    float qx = 88.88f;
                    float qy = 125.70f;
                    int dx = (int)((5.52 * mWidth) / qx);
                    int dy = (int)((5.41 * mHeight) / qy);


                    // original
                    //603 Ring Note Height  5.52  5.41 	63.46 	88.88 
                    //int dx = (int)( ( 5.52 * mWidth ) / 63.46 );
                    //int dy = (int)( ( 5.41 * mHeight ) / 88.88 );
                    //
                    //original
                    //Renderer.draw(mBitmap, stroke, (float)(mWidth / 63.46f), (float)(mHeight / 88.88f), -dx, -dy, 1, Color.FromArgb(200, Color.Blue));
                    
                    var image = pictureBox1.Image = mBitmap;
                    //added
                    pictureBox1.Image.Save(@"C:\Users\Uver\Documents\NUIG\Semester 2\Project\Images\save1.png", System.Drawing.Imaging.ImageFormat.Jpeg);
                    //Graphics g = Graphics.FromImage(pictureBox1.Image);
                    //g.Ima.Save(@"C:\Users\Uver\Documents\NUIG\Semester 2\Project\Images\saveGFX.png");
                    PDFOverlay pdf = new PDFOverlay();
                    pdf.combine(image);


                }
            }));
            */
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
            Debug.WriteLine("onDisconnected...\n");
            //throw new NotImplementedException();
        }

        public void onFinishedOfflineDownload(IPenComm sender, bool result)
        {
            //throw new NotImplementedException();
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

        public bool disconnected()
        {
            return false;
        }
    }
}
